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