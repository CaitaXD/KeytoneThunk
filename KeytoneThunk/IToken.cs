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

public enum CommandType
{
    Silence,
    VolumeUp,
    RepeatLastNote,
    OctaveUp,
}

public struct Command : IToken
{
    CommandType CommandType;
}