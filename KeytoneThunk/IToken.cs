namespace KeytoneThunk;
public interface IToken;
public struct Note : IToken
{
    public int Pitch;
    public int Hertz;
    public int Octave;
}
public struct ChangeToInstrument : IToken
{
    public int Midi;
}
public struct MorphInstrument : IToken
{
    public byte MorphDigit;
}
public struct Silence : IToken;
public struct VolumeUp : IToken;
public struct RepeatLastNote : IToken;
public struct OctaveUp : IToken;