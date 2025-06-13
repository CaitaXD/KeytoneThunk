using KeytoneThunk.Midi;

namespace KeytoneThunk.unit.tst;
using KeytoneThunk;

[TestClass]
public sealed class MidiConverter
{
    const double FrequencyA4 = 440;
    const int MidiA4 = 69;
    
    const double FrequencyC4 = 261.63;
    const int MidiC4 = 60;
    
    const int MidiA0 = 21;
    const int MidiC8 = 108;
    
    [TestMethod]
    [DataRow(MidiA4, FrequencyA4)]
    [DataRow(MidiC4, FrequencyC4)]
    public void Correct_Midi_To_Frequency_Conversion(int expectedMidi, double givenFrequency)
    {
        // arrange
        // act
        int actualMidi = Midi.MidiConverter.FromHertz(givenFrequency);
        // assert
        Assert.AreEqual(expectedMidi, actualMidi);
    }
    
    [TestMethod]
    [DataRow(MidiA4, FrequencyA4)]
    [DataRow(MidiC4, FrequencyC4)]
    public void Correct_Frequency_To_Midi_Conversion(int givenMidi, double expectedFrequency)
    {
        // arrange
        // act
        double actualFrequency = Midi.MidiConverter.ToHertz(givenMidi);
        // assert
        Assert.AreEqual(expectedFrequency, actualFrequency, double.Epsilon);
    }
    
    [TestMethod]
    [DataRow(MidiA4, Note.A)]
    [DataRow(MidiA0, Note.A, 0)]
    [DataRow(MidiC4, Note.C)]
    [DataRow(MidiC8, Note.C, 8)]
    public void Correct_Midi_From_Note_Conversion(int expectedMidi, Note note, int octave = 4)
    {
        // arrange
        // act
        int actualMidi = Midi.MidiConverter.Note(note, octave);
        // assert
        Assert.AreEqual(expectedMidi, actualMidi);
    }
}