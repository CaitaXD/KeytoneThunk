using NAudio.Midi;

namespace KeytoneThunk;

public class MidiExportFileMusicPlayerStrategy : IMusicPlayerStrategy
{
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

    // Dumb ass attempt to make the timestamp of the midi file match the timing of the player
    // This is dumb ahh a guess
    const int DeltaTicksPerQuarterNote = 25;

    public MidiExportFileMusicPlayerStrategy(string filePath, int volume = 50, int currentOctave = 4, int bpm = 240)
    {
        CurrentBpm = bpm;
        CurrentVolume = volume;
        CurrentOctave = currentOctave;
        _currentInstrument = Instrument.AcousticGrandPiano;
        _filePath = filePath;
        _midiEvents = new MidiEventCollection(0, DeltaTicksPerQuarterNote);
        _midiEvents.AddTrack();
    }

    public ValueTask PlayNoteAsync(TimeSpan duration, MidiNote note, int octave)
    {
        int number = MidiConverter.Note(note, octave);
        var startNodeEvent = new NoteOnEvent(DeltaTicks(_timeStamp), Channel, number, Velocity, DeltaTicks(duration));
        var stopNodeEvent = new NoteOnEvent(DeltaTicks(_timeStamp + duration), Channel, number, 0, 0);
        _midiEvents.AddEvent(startNodeEvent, TrackNumber);
        _midiEvents.AddEvent(stopNodeEvent, TrackNumber);
        _timeStamp += ((IMusicPlayerStrategy)this).BeatDelay;
        return ValueTask.CompletedTask;
    }

    public ValueTask Silence(TimeSpan duration)
    {
        _timeStamp += ((IMusicPlayerStrategy)this).BeatDelay;
        return ValueTask.CompletedTask;
    }

    public void ChangeInstrument(ChangeToInstrument changeToInstrument)
    {
        var changeInstrumentEvent = new PatchChangeEvent(DeltaTicks(_timeStamp), Channel, changeToInstrument.Midi);
        _midiEvents.AddEvent(changeInstrumentEvent, TrackNumber);
        _currentInstrument = new Instrument(changeToInstrument.Midi);
    }

    public void MorphInstrument(MorphInstrument morphInstrument)
    {
        int id = (_currentInstrument.Midi + morphInstrument.MorphDigit)%Instrument.Count;
        ChangeInstrument(new ChangeToInstrument(id));
    }

    public ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, MidiNote note, int octave, int instrumentId)
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

    int DeltaTicks(TimeSpan timeSpan)
    {
        // Dumb ass attempt to make the timestamp of the midi file match the timing of the player
        // This is dumb ass a guess
        return (int)Math.Round(timeSpan.TotalMilliseconds*.05d);
    }

    public void Dispose()
    {
        _midiEvents.PrepareForExport();
        MidiFile.Export(_filePath, _midiEvents);
    }
}