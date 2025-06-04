using System.Collections;

namespace KeytoneThunk;

public static class KeytoneParser
{
    public static IEnumerable<IKeytoneInstruction> Parse(string input)
    {
        foreach (var ch in input)
        {
            yield return ch switch
            {
                'A' or 'a' =>                             new IKeytoneInstruction.Note(MidiNote.A),
                'B' or 'b' =>                             new IKeytoneInstruction.Note(MidiNote.B),
                'C' or 'c' =>                             new IKeytoneInstruction.Note(MidiNote.C),
                'D' or 'd' =>                             new IKeytoneInstruction.Note(MidiNote.D),
                'E' or 'e' =>                             new IKeytoneInstruction.Note(MidiNote.E),
                'F' or 'f' =>                             new IKeytoneInstruction.Note(MidiNote.F),
                'G' or 'g' =>                             new IKeytoneInstruction.Note(MidiNote.G),
                '+' =>                                    new IKeytoneInstruction.VolumeUp(),
                '-' =>                                    new IKeytoneInstruction.ResetVolume(),
                '!' =>                                    new IKeytoneInstruction.ChangeToInstrument(24 - 1), // 1 Based Indexing 
                'o' or 'i' or 'u' or 'O' or 'I' or 'U' => new IKeytoneInstruction.ChangeToInstrument(110 - 1), // 1 Based Indexing
                >= '0' and <= '9' when CharIsEven(ch) =>  new IKeytoneInstruction.MorphInstrument(CharToByte(ch)),
                ';' =>                                    new IKeytoneInstruction.ChangeToInstrument(15 - 1), // 1 Based Indexing
                >= '0' and <= '9' when CharIsOdd(ch) =>   new IKeytoneInstruction.ChangeToInstrument(15 - 1), // 1 Based Indexing
                '?' or '.' =>                             new IKeytoneInstruction.OctaveUp(),
                ',' =>                                    new IKeytoneInstruction.ChangeToInstrument(114 - 1), // 1 Based Indexing
                '\n' =>                                   new IKeytoneInstruction.ChangeToInstrument(123 - 1), // 1 Based Indexing
                _ =>                                      new IKeytoneInstruction.RepeatLastNote(),
            };
        }
    }
    static byte CharToByte(char ch) => (byte)(ch - 48);
    static bool CharIsEven(char ch) => CharToByte(ch) % 2 == 0;
    static bool CharIsOdd(char ch) => !CharIsEven(ch);
}