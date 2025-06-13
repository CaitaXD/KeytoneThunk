namespace KeytoneThunk.Midi;

public static class MidiConverter
{
    // m  =  12*log2(fm/440 Hz) + 69
    public static int FromHertz(double frequency) => (int)Math.Round(12*Math.Log2(frequency/440d) + 69);

    // fm  =  2^((m−69)/12) * (440 Hz)
    public static double ToHertz(int midi) => Math.Round(Math.Pow(2, (midi - 69d)/12d)*440d, 2);
    public static int Note(Note note, int octave = 4) => (int)note + 12*(octave - 4);
    public static double ToHertz(Note note, int octave = 4) => ToHertz(Note(note, octave));
    public static TimeSpan QuarterNoteDuration(int bpm) => TimeSpan.FromMinutes(1)/bpm;
    public static TimeSpan NoteDuration(int bpm) => 4*QuarterNoteDuration(bpm);
    public static double MicrosecondsPerQuarterNote(int bpm) => (int)Math.Round(60_000_000d/bpm);
    public static double MillisecondsPerQuarterNote(int bpm) => (int)Math.Round(60_000d/bpm);
    public static int RoundToInt(this double value) => (int)Math.Round(value);
}