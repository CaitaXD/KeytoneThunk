namespace KeytoneThunk;

public interface IToken
{
    public readonly record struct Note(MidiNote MidiNote) : IToken;
    public readonly record struct ChangeToInstrument(int Midi) : IToken;
    public readonly record struct MorphInstrument(byte MorphDigit) : IToken;
    public struct Silence : IToken;
    public struct VolumeUp : IToken;
    public struct RepeatLastNote : IToken;
    public struct OctaveUp : IToken;
}