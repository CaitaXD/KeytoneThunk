using System.Collections;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace KeytoneThunk;

public sealed class KeytoneParser(string input, int? randomSeed = null) : IEnumerator<IKeytoneInstruction>
{
    static KeytoneParser()
    {
        MidiNotes = ImmutableCollectionsMarshal.AsImmutableArray(Enum.GetValues<MidiNote>());
    }
    
    readonly Random _random = randomSeed is null ? Random.Shared : new Random(randomSeed.Value);
    const int BpmUpAmount = 80;
    const int RingSoundEffectId = 125 - 1; // 1 Based Indexing
    
    ReadOnlyMemory<char> _state = input.AsMemory();
    readonly ReadOnlyMemory<char> _input = input.AsMemory();

    public bool MoveNext()
    {
        _last = Current;
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
            var p = Peek(_state);
            if (p is not 'P') return MatchSingleChar(result.Value);
            
            var m = Peek(_state, 2);
            if (m is not 'M') return MatchSingleChar(result.Value);
            
            var plus = Peek(_state, 3);
            if (plus is not '+') return MatchSingleChar(result.Value);
            
            Shift(ref _state, 3);
            
            return new IKeytoneInstruction.BpmUp(BpmUpAmount);
        }
        
        return MatchSingleChar(result.Value);
    }

    IKeytoneInstruction MatchSingleChar(char ch)
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
            '\n' =>                                   new IKeytoneInstruction.ChangeToInstrument(RandomInstrument()),
            _ =>                                      ElseCase,
        };
    }
    
    int RandomBpm()
    {
        var r = _random.Next(60, 1200);
        return r;
    }
    
    int RandomInstrument()
    {
        var r = _random.Next(Instrument.Count);
        return r;
    }
    
    static readonly ImmutableArray<MidiNote> MidiNotes;
    MidiNote RandomNote()
    {
        var r = _random.Next(MidiNotes.Length);
        return MidiNotes[r];
    }

    static readonly IKeytoneInstruction ElseCase = new IKeytoneInstruction.Nop();
    static char? Shift(ref ReadOnlyMemory<char> state, int count = 1)
    {
        if (state.Length < count) return null;
        var result = state.Span[count - 1];
        state = state[count..];
        return result;
    }
    static char? Peek(ReadOnlyMemory<char> state, int count = 1)
    {
        if (state.Length < count) return null;
        var result = state.Span[count - 1];
        return result;
    }
    public KeytoneParser GetEnumerator() => this;
    public void Dispose()
    {
        _state = Memory<char>.Empty;
        _last = null;
    }
    public void Reset()
    {
        _state = _input;
        _last = null;
    }
    IKeytoneInstruction? _last;
    object IEnumerator.Current => Current;
    public IKeytoneInstruction Current { get; private set; } = new IKeytoneInstruction.Nop();
    
    public bool TryGetPreviousInstruction(out IKeytoneInstruction keytoneInstruction)
    {
        if (_last == null)
        {
            keytoneInstruction = new IKeytoneInstruction.Nop();
            return false;
        }

        keytoneInstruction = _last;
        return true;
    }
}