namespace KeytoneThunk;

public interface IMusicPlayerStrategy : IDisposable
{
    // TODO: This is suspicious, alas im not in the mood
    int CurrentBpm { get; set; }
    int CurrentVolume { get; set; }
    int CurrentOctave { get; set; }
    //
    
    ValueTask PlayNoteAsync(TimeSpan duration, MidiNote note, int octave);
    ValueTask Silence(TimeSpan duration);
    void ChangeInstrument(IKeytoneInstruction.ChangeToInstrument changeToInstrument);
    void MorphInstrument(IKeytoneInstruction.MorphInstrument morphInstrument);
    ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, MidiNote note, int octave, int instrumentId);
    TimeSpan BeatDelay => CurrentBpm > 0d ? TimeSpan.FromMinutes(1d/CurrentBpm) : TimeSpan.Zero;
    
    public static IMusicPlayerStrategy Null => new NullMusicPlayerStrategy();
    class NullMusicPlayerStrategy : IMusicPlayerStrategy
    {
        public int CurrentBpm { get; set; }
        public int CurrentVolume { get; set; }
        public int CurrentOctave { get; set; }

        public ValueTask PlayNoteAsync(TimeSpan duration, MidiNote note, int octave)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask Silence(TimeSpan duration)
        {
            return ValueTask.CompletedTask;
        }

        public void ChangeInstrument(IKeytoneInstruction.ChangeToInstrument changeToInstrument)
        {
        }

        public void MorphInstrument(IKeytoneInstruction.MorphInstrument morphInstrument)
        {
        }

        public ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, MidiNote note, int octave, int instrumentId)
        {
            return ValueTask.CompletedTask;
        }

        public TimeSpan BeatDelay => TimeSpan.Zero;

        public void Dispose()
        {
        }
    }
}