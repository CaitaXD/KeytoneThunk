using KeytoneThunk.Interpreter;
using Note = KeytoneThunk.Midi.Note;

namespace KeytoneThunk.Player.Strategy;
public interface IMusicPlayerStrategy : IDisposable
{
    // TODO: This is suspicious, alas im not in the mood
    int Bpm { get; set; }
    int Volume { get; set; }
    //
    
    ValueTask PlayNoteAsync(TimeSpan duration, Note note, int octave);
    ValueTask Silence(TimeSpan duration);
    void ChangeInstrument(ChangeToInstrument changeToInstrument);
    void MorphInstrument(MorphInstrument morphInstrument);
    ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, Note note, int octave, int instrumentId);
    public static IMusicPlayerStrategy Null => new NullMusicPlayerStrategy();
    class NullMusicPlayerStrategy : IMusicPlayerStrategy
    {
        public int Bpm { get; set; }
        public int Volume { get; set; }
        public int Octave { get; set; }

        public ValueTask PlayNoteAsync(TimeSpan duration, Note note, int octave)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask Silence(TimeSpan duration)
        {
            return ValueTask.CompletedTask;
        }

        public void ChangeInstrument(ChangeToInstrument changeToInstrument)
        {
        }

        public void MorphInstrument(MorphInstrument morphInstrument)
        {
        }

        public ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, Note note, int octave, int instrumentId)
        {
            return ValueTask.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}