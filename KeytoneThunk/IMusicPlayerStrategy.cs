namespace KeytoneThunk;

public interface IMusicPlayerStrategy : IDisposable
{
    int DefaultVolume { get; }
    int DefaultBpm { get; }
    int CurrentBpm { get; set; }
    int CurrentVolume { get; set; }
    int CurrentOctave { get; set; }
    ValueTask PlayNoteAsync(TimeSpan duration, MidiNote note, int octave);
    void ChangeInstrument(IKeytoneInstruction.ChangeToInstrument changeToInstrument);
    void MorphInstrument(IKeytoneInstruction.MorphInstrument morphInstrument);
    ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, MidiNote note, int octave, int instrumentId);
    TimeSpan BeatDelay => CurrentBpm > 0d ? TimeSpan.FromMinutes(1d/CurrentBpm) : TimeSpan.Zero;
}