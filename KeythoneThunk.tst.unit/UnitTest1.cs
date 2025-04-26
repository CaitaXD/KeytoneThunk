namespace KeythoneThunk.tst.unit;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    const double FrequencyA4 = 440;
    const int MidiA4 = 69;
    
    const double FrequencyC4 = 261.63;
    const int MidiC4 = 60;
    
    [TestCase(MidiA4, FrequencyA4)]
    [TestCase(MidiC4, FrequencyC4)]
    public void Correct_Midi_To_Frequency_Conversion(int midi, double frequency)
    {
        // arrange
        int midiFromHz = KeytoneThunk.MidiFromHz(frequency);
        // act
        
        // assert
    }
}