using KeytoneThunk.Interpreter;
using KeytoneThunk.Midi;
using NAudio.Midi;
using Note = KeytoneThunk.Midi.Note;

namespace KeytoneThunk.Player.Strategy;

public class MidiExportFileMusicPlayerStrategy : IMusicPlayerStrategy
{
    public int Bpm
    {
        get => _bpm;
        set
        {
            _bpm = value;
            var tempoEvent = new TempoEvent(MicroSecondsPerQuarterNote.RoundToInt(), _timeStamp);
            _midiEvents.AddEvent(tempoEvent, TrackNumber);
        }
    }

    public int Volume { get; set; }

    readonly string _filePath;
    const int TrackNumber = 0;
    const int Channel = 1;
    Instrument _currentInstrument;
    readonly MidiEventCollection _midiEvents;
    int _timeStamp;
    int _bpm;

    const int DeltaTicksPerQuarterNote = 960;
    double TicksPerMicrosecond => DeltaTicksPerQuarterNote/MidiConverter.MicrosecondsPerQuarterNote(_bpm);
    double MicroSecondsPerQuarterNote => MidiConverter.MicrosecondsPerQuarterNote(_bpm);

    public MidiExportFileMusicPlayerStrategy(string filePath)
    {
        _currentInstrument = Instrument.AcousticGrandPiano;
        _filePath = filePath;
        _midiEvents = new MidiEventCollection(0, DeltaTicksPerQuarterNote);
        _midiEvents.AddTrack();
    }

    public ValueTask PlayNoteAsync(TimeSpan duration, Note note, int octave)
    {
        int ticksDuration = Ticks(duration)/4;
        int noteNumber = MidiConverter.Note(note, octave);
        var startNodeEvent = new NoteOnEvent(_timeStamp, Channel, noteNumber, Volume, ticksDuration);
        var stopNodeEvent = new NoteOnEvent(_timeStamp + ticksDuration, Channel, noteNumber, 0, 0);
        _midiEvents.AddEvent(startNodeEvent, TrackNumber);
        _midiEvents.AddEvent(stopNodeEvent, TrackNumber);
        _timeStamp += ticksDuration;
        return ValueTask.CompletedTask;
    }

    public ValueTask Silence(TimeSpan duration)
    {
        _timeStamp += DeltaTicksPerQuarterNote;
        return ValueTask.CompletedTask;
    }

    public void ChangeInstrument(ChangeToInstrument changeToInstrument)
    {
        var changeInstrumentEvent = new PatchChangeEvent(_timeStamp, Channel, changeToInstrument.Midi);
        _midiEvents.AddEvent(changeInstrumentEvent, TrackNumber);
        _currentInstrument = new Instrument(changeToInstrument.Midi);
    }

    public void MorphInstrument(MorphInstrument morphInstrument)
    {
        int id = (_currentInstrument.Midi + morphInstrument.MorphDigit)%Instrument.Count;
        ChangeInstrument(new ChangeToInstrument(id));
    }

    public ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, Note note, int octave, int instrumentId)
    {
        if (instrumentId == _currentInstrument.Midi)
        {
            PlayNoteAsync(duration, note, octave).AsTask().GetAwaiter().GetResult();
            return ValueTask.CompletedTask;
        }

        var prev = _currentInstrument;
        ChangeInstrument(new ChangeToInstrument(instrumentId));
        PlayNoteAsync(duration, note, octave).AsTask().GetAwaiter().GetResult();
        ChangeInstrument(new ChangeToInstrument(prev.Midi));
        return ValueTask.CompletedTask;
    }

    int Ticks(TimeSpan timeSpan)
    {
        return (TicksPerMicrosecond*timeSpan.TotalMicroseconds).RoundToInt();
    }

    public void Dispose()
    {
        _midiEvents.PrepareForExport();
        MidiFile.Export(_filePath, _midiEvents);
    }
}