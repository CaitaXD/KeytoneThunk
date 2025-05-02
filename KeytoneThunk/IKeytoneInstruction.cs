namespace KeytoneThunk;

public interface IKeytoneInstruction
{
    public readonly record struct Note(MidiNote MidiNote) : IKeytoneInstruction;
    public readonly record struct ChangeToInstrument(int Midi) : IKeytoneInstruction;
    public readonly record struct MorphInstrument(byte MorphDigit) : IKeytoneInstruction;
    public struct Silence : IKeytoneInstruction;
    public struct VolumeUp : IKeytoneInstruction;
    public struct RepeatLastNote : IKeytoneInstruction;
    public struct OctaveUp : IKeytoneInstruction;
    
    // void Command(MusicPlayer musicPlayer);
}