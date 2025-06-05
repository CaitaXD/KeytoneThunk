using NAudio.Midi;

namespace KeytoneThunk;

public class MidiExportFileMusicPlayerStrategy : IMusicPlayerStrategy
{
    public int DefaultVolume { get; }
    public int DefaultBpm { get; }
    public int CurrentBpm { get; set; }
    public int CurrentVolume { get; set; }
    public int CurrentOctave { get; set; }

    readonly string _filePath;
    const int TrackNumber = 0;
    const int Channel = 1;
    const int Velocity = 100;
    Instrument _currentInstrument;
    readonly MidiEventCollection _midiEvents;
    TimeSpan _timeStamp = TimeSpan.Zero;

    public MidiExportFileMusicPlayerStrategy(string filePath, int volume = 50, int currentOctave = 4, int bpm = 240)
    {
        DefaultBpm = CurrentBpm = bpm;
        DefaultVolume = CurrentVolume = volume;
        CurrentOctave = currentOctave;
        _currentInstrument = Instrument.AcousticGrandPiano;
        _filePath = filePath;
        _midiEvents = new MidiEventCollection(0, 100);
        _midiEvents.AddTrack();
    }

    public ValueTask PlayNoteAsync(TimeSpan duration, MidiNote note, int octave)
    {
        int number = MidiConverter.Note(note, octave);
        var startNodeEvent = new NoteOnEvent(_timeStamp.Milliseconds, Channel, number, Velocity, duration.Milliseconds);
        var stopNodeEvent = new NoteOnEvent(_timeStamp.Milliseconds + duration.Milliseconds, Channel, number, 0, 0);
        _midiEvents.AddEvent(startNodeEvent, TrackNumber);
        _midiEvents.AddEvent(stopNodeEvent, TrackNumber);
        // Because of the quarter Tick think???
        // This feels close to the player tempo so ill just use it but im not quite sure about this
        // Feel free to change this
        _timeStamp += ((IMusicPlayerStrategy)this).BeatDelay/4; 
        return ValueTask.CompletedTask;
    }

    public void ChangeInstrument(IKeytoneInstruction.ChangeToInstrument changeToInstrument)
    {
        var changeInstrumentEvent = new PatchChangeEvent(_timeStamp.Seconds, Channel, changeToInstrument.Midi);
        _midiEvents.AddEvent(changeInstrumentEvent, TrackNumber);
        _currentInstrument = new Instrument(changeToInstrument.Midi);
    }

    public void MorphInstrument(IKeytoneInstruction.MorphInstrument morphInstrument)
    {
        int id = (_currentInstrument.Midi + morphInstrument.MorphDigit)%Instrument.Count;
        ChangeInstrument(new IKeytoneInstruction.ChangeToInstrument(id));
    }

    public ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, MidiNote note, int octave, int instrumentId)
    {
        var prev = _currentInstrument;
        ChangeInstrument(new IKeytoneInstruction.ChangeToInstrument(instrumentId));
        _ = PlayNoteAsync(duration, note, octave).AsTask();
        ChangeInstrument(new IKeytoneInstruction.ChangeToInstrument(prev.Midi));
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        _midiEvents.PrepareForExport();
        MidiFile.Export(_filePath, _midiEvents);
    }
}