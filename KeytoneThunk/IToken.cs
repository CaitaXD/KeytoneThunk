namespace KeytoneThunk;
public interface IToken;
public readonly struct Note : IToken
{
    public readonly int Pitch;
    public readonly int Hertz;
    public readonly int Octave;
}
public readonly struct ChangeToInstrument : IToken
{
    public readonly int Midi;
}
public readonly struct MorphInstrument : IToken
{
    public readonly byte MorphDigit;
}
public struct Silence : IToken;
public struct VolumeUp : IToken;
public struct RepeatLastNote : IToken;
public struct OctaveUp : IToken;