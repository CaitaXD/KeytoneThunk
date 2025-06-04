using System.Collections;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace KeytoneThunk;

public struct KeytoneParserEnumerator(string input) : IEnumerator<IKeytoneInstruction>
{
    const int BpmUpAmount = 80;
    const int RingSoundEffectId = 125;
    static readonly int AlternativeInstrumentId = Instrument.AcousticBass.Midi;
    
    ReadOnlyMemory<char> _state = input.AsMemory();
    readonly ReadOnlyMemory<char> _input = input.AsMemory();

    public bool MoveNext()
    {
        if (_state.IsEmpty) return false;
        Current = Match();
        return true;
    }
    
    IKeytoneInstruction Match()
    {
        char? result = Shift(ref _state);
        if (!result.HasValue) return ElseCase;
        
        if (result.Value == 'R')
        {
            result = Shift(ref _state);
            if (!result.HasValue) return ElseCase;
            
            return result.Value switch
            {
                '+' => new IKeytoneInstruction.OctaveUp(1),
                '-' => new IKeytoneInstruction.OctaveUp(-1),
                _ => ElseCase,
            };
        }

        if (result.Value == 'B')
        {
            var p = Peek(ref _state);
            if (p is not 'P') return MatchSingleChar(result.Value);
            
            var m = Peek(ref _state, 2);
            if (m is not 'M') return MatchSingleChar(result.Value);
            
            var plus = Peek(ref _state, 3);
            if (plus is not '+') return MatchSingleChar(result.Value);
            
            return new IKeytoneInstruction.BpmUp(BpmUpAmount);
        }
        
        return MatchSingleChar(result.Value);
    }

    static IKeytoneInstruction MatchSingleChar(char ch)
    {
        return ch switch
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
            ' ' =>                                    new IKeytoneInstruction.Silence(),
            'o' or 'i' or 'u' or 'O' or 'I' or 'U' => new IKeytoneInstruction.RepeatLastNote(or: new IKeytoneInstruction.SoundEffect(RingSoundEffectId)),
            ';' =>                                    new IKeytoneInstruction.SetBpm(RandomBpm()),
            '?' =>                                    new IKeytoneInstruction.Note(RandomNote()),
            '\n' =>                                   new IKeytoneInstruction.ChangeToInstrument(AlternativeInstrumentId),
            _ =>                                      ElseCase,
        };
    }
    
    static int RandomBpm()
    {
        var r = Random.Shared.Next(100, 200);
        return r;
    }
    
    static readonly ImmutableArray<MidiNote> MidiNotes = ImmutableCollectionsMarshal.AsImmutableArray(Enum.GetValues<MidiNote>());
    static MidiNote RandomNote()
    {
        var r = Random.Shared.Next(MidiNotes.Length);
        return MidiNotes[r];
    }

    static readonly IKeytoneInstruction ElseCase = new IKeytoneInstruction.Nop();
    static byte CharToByte(char ch) => (byte)(ch - 48);
    static bool CharIsEven(char ch) => CharToByte(ch) % 2 == 0;
    static bool CharIsOdd(char ch) => !CharIsEven(ch);
    static char? Shift(ref ReadOnlyMemory<char> state, int count = 1)
    {
        if (state.Length < count) return null;
        var result = state.Span[0];
        state = state[1..];
        return result;
    }
    static char? Peek(ref ReadOnlyMemory<char> state, int count = 1)
    {
        if (state.Length < count) return null;
        var result = state.Span[0];
        return result;
    }
    public KeytoneParserEnumerator GetEnumerator() => this;
    public void Dispose()
    {
        _state = Memory<char>.Empty;
    }
    public void Reset()
    {
        _state = _input;
    }
    object? IEnumerator.Current => Current;
    public IKeytoneInstruction Current { get; private set; }
}