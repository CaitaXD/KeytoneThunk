namespace KeytoneThunk;

public readonly record struct MidiMessage(int RawData)
{
    public static implicit operator int(MidiMessage midiMessage) => midiMessage.RawData;
    public static implicit operator MidiMessage(int rawData) => new(rawData);

    public MidiMessage(int status, int data1, int data2)
        // Byte magic from the midi spec
        : this(status + (data1 << 8) + (data2 << 16))
    {
    }

    public static MidiMessage StartNote(int note, int volume, int channel)
    {
        ValidateChannel(channel);
        ValidateMidiByte(note);
        ValidateMidiByte(volume);
        return new MidiMessage(144 + channel - 1, note, volume);
    }

    public static MidiMessage StopNote(int note, int volume, int channel)
    {
        ValidateChannel(channel);
        ValidateMidiByte(note);
        ValidateMidiByte(volume);
        return new MidiMessage(128 + channel - 1, note, volume);
    }

    public static MidiMessage ChangePatch(int patch, int channel)
    {
        ValidateChannel(channel);
        return new MidiMessage(192 + channel - 1, patch, 0);
    }
    
    static void ValidateChannel(int channel)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(channel, 16);
        ArgumentOutOfRangeException.ThrowIfLessThan(channel, 1);
    }

    static void ValidateMidiByte(int midi)
    {
        // Casting to unsigned for free check against negative values (blazing fast 🔥)
        ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)midi, (uint)sbyte.MaxValue);
    }
}