using System.Collections;

namespace KeytoneThunk;

public class Parser
{
    public IEnumerable<IToken> Parse(string input)
    {
        foreach (var ch in input)
        {
            yield return ch switch
            {
                'A' =>                                    new IToken.Note(MidiNote.A),
                'B' =>                                    new IToken.Note(MidiNote.B),
                'C' =>                                    new IToken.Note(MidiNote.C),
                'D' =>                                    new IToken.Note(MidiNote.D),
                'E' =>                                    new IToken.Note(MidiNote.E),
                'F' =>                                    new IToken.Note(MidiNote.F),
                'G' =>                                    new IToken.Note(MidiNote.G),
                >= 'a' and <= 'g' =>                      new IToken.Silence(),
                ' ' =>                                    new IToken.VolumeUp(),
                '!' =>                                    new IToken.ChangeToInstrument(24 - 1),
                'o' or 'i' or 'u' or 'O' or 'I' or 'U' => new IToken.ChangeToInstrument(110 - 1),
                >= '0' and <= '9' when CharIsEven(ch) =>  new IToken.MorphInstrument(CharToByte(ch)),
                ';' =>                                    new IToken.ChangeToInstrument(15 - 1),
                >= '0' and <= '9' when CharIsOdd(ch) =>   new IToken.ChangeToInstrument(15 - 1),
                '?' or '.' =>                             new IToken.OctaveUp(),
                ',' =>                                    new IToken.ChangeToInstrument(114 - 1),
                '\n' =>                                   new IToken.ChangeToInstrument(123 - 1),
                _ =>                                      new IToken.RepeatLastNote(),
            };
        }
    }

    static byte CharToByte(char ch) => (byte)(ch - 48);
    static bool CharIsEven(char ch) => CharToByte(ch) % 2 == 0;
    static bool CharIsOdd(char ch) => !CharIsEven(ch);
}