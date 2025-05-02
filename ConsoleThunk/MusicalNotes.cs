public static class MusicalNotes
{
    public static int MidiFromHz(double frequency)
    {
        // m  =  12*log2(fm/440 Hz) + 69
        return (int)Math.Round(12 * Math.Log2(frequency/440d) + 69);
    }

    public static double FrequencyFromMidi(int midi)
    {
        // fm  =  2^((m−69)/12) * (440 Hz)
        return Math.Round(Math.Pow(2, (midi - 69d) / 12d) * 440d, 2);
    }

    public static int MidiFromNote(MidiNote midiNote, int octave = 4)
    {
        int midi = (int)midiNote + 12 * (octave - 4);
        return midi;
    }
    
    public static double FrequencyFromNote(MidiNote midiNote, int octave = 4)
    {
        return FrequencyFromMidi(MidiFromNote(midiNote, octave));
    }
}

public enum MidiNote
{
    A = 69, B = 71, C = 60, D = 62, E = 64, F = 65, G = 67,
    Do = C, Re = D, Mi = E, Fa = F, Sol = G, La = A, Si = B,
}