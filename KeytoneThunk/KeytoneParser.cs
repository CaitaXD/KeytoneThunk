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
                'A' =>                                    new IKeytoneInstruction.Note(MidiNote.A),
                'B' =>                                    new IKeytoneInstruction.Note(MidiNote.B),
                'C' =>                                    new IKeytoneInstruction.Note(MidiNote.C),
                'D' =>                                    new IKeytoneInstruction.Note(MidiNote.D),
                'E' =>                                    new IKeytoneInstruction.Note(MidiNote.E),
                'F' =>                                    new IKeytoneInstruction.Note(MidiNote.F),
                'G' =>                                    new IKeytoneInstruction.Note(MidiNote.G),
                >= 'a' and <= 'g' =>                      new IKeytoneInstruction.Silence(),
                ' ' =>                                    new IKeytoneInstruction.VolumeUp(),
                '!' =>                                    new IKeytoneInstruction.ChangeToInstrument(24 - 1),
                'o' or 'i' or 'u' or 'O' or 'I' or 'U' => new IKeytoneInstruction.ChangeToInstrument(110 - 1),
                >= '0' and <= '9' when CharIsEven(ch) =>  new IKeytoneInstruction.MorphInstrument(CharToByte(ch)),
                ';' =>                                    new IKeytoneInstruction.ChangeToInstrument(15 - 1),
                >= '0' and <= '9' when CharIsOdd(ch) =>   new IKeytoneInstruction.ChangeToInstrument(15 - 1),
                '?' or '.' =>                             new IKeytoneInstruction.OctaveUp(),
                ',' =>                                    new IKeytoneInstruction.ChangeToInstrument(114 - 1),
                '\n' =>                                   new IKeytoneInstruction.ChangeToInstrument(123 - 1),
                _ =>                                      new IKeytoneInstruction.RepeatLastNote(),
            };
        }
    }
    static byte CharToByte(char ch) => (byte)(ch - 48);
    static bool CharIsEven(char ch) => CharToByte(ch) % 2 == 0;
    static bool CharIsOdd(char ch) => !CharIsEven(ch);
}