using KeytoneThunk.Midi;

namespace KeytoneThunk.Interpreter;

public interface IKeytoneInstruction;

public readonly record struct Note(Midi.Note MidiNote) : IKeytoneInstruction;

public readonly record struct ChangeToInstrument(int Midi) : IKeytoneInstruction;

public readonly record struct MorphInstrument(byte MorphDigit) : IKeytoneInstruction;

public readonly record struct Silence : IKeytoneInstruction;

public readonly record struct VolumeUp : IKeytoneInstruction;

public readonly record struct ResetVolume : IKeytoneInstruction;

public readonly record struct RepeatLastNote(IKeytoneInstruction Or) : IKeytoneInstruction
{
    public RepeatLastNote() : this(new Silence())
    {
    }
}

public readonly record struct Nop : IKeytoneInstruction;

public readonly record struct OctaveUp(int Octaves) : IKeytoneInstruction;

public readonly record struct BpmUp(int Bpm) : IKeytoneInstruction;

public readonly record struct SetBpm(int Bpm) : IKeytoneInstruction;

public readonly record struct SoundEffect(int InstrumentId) : IKeytoneInstruction;