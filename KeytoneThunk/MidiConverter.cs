namespace KeytoneThunk;

public static class MidiConverter
{
    // m  =  12*log2(fm/440 Hz) + 69
    public static int FromHertz(double frequency) => (int)Math.Round(12*Math.Log2(frequency/440d) + 69);

    // fm  =  2^((m−69)/12) * (440 Hz)
    public static double ToHertz(int midi) => Math.Round(Math.Pow(2, (midi - 69d)/12d)*440d, 2);
    public static int Note(MidiNote midiNote, int octave = 4) => (int)midiNote + 12*(octave - 4);
    public static double ToHertz(MidiNote midiNote, int octave = 4) => ToHertz(Note(midiNote, octave));
}

public enum MidiNote
{
    A = 69,
    B = 71,
    C = 60,
    D = 62,
    E = 64,
    F = 65,
    G = 67,
    Do = C,
    Re = D,
    Mi = E,
    Fa = F,
    Sol = G,
    La = A,
    Si = B,
}

public readonly record struct MidiMessage(int RawData)
{
    public static implicit operator int(MidiMessage midiMessage) => midiMessage.RawData;
    public static implicit operator MidiMessage(int rawData) => new(rawData);

    public MidiMessage(int status, int data1, int data2) : this(status + (data1 << 8) + (data2 << 16))
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