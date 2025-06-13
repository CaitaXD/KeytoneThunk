using System.Collections;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using KeytoneThunk.Midi;

namespace KeytoneThunk.Interpreter;

public sealed class KeytoneParser(string input, int? randomSeed = null) : IEnumerator<IKeytoneInstruction>
{
    static KeytoneParser()
    {
        MidiNotes = ImmutableCollectionsMarshal.AsImmutableArray(Enum.GetValues<Midi.Note>());
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
        if (!result.HasValue) return LastOrNop;

        //Searches for R+ OR R-

        if (result.Value == 'R')
        {
            result = Shift(ref _state);
            if (!result.HasValue) return LastOrNop;

            return result.Value switch
            {
                '+' => new OctaveUp(1),
                '-' => new OctaveUp(-1),
                _ => LastOrNop,
            };
        }

        //Searches for Bpm+

        if (result.Value == 'B')
        {
            var p = Peek(_state);
            if (p is not 'P') return MatchSingleChar(result.Value);

            var m = Peek(_state, 2);
            if (m is not 'M') return MatchSingleChar(result.Value);

            var plus = Peek(_state, 3);
            if (plus is not '+') return MatchSingleChar(result.Value);

            Shift(ref _state, 3);

            return new BpmUp(BpmUpAmount);
        }

        return MatchSingleChar(result.Value);
    }

    IKeytoneInstruction MatchSingleChar(char ch)
    {
        return ch switch
        {
            'A' or 'a' => new Note(Midi.Note.A),
            'B' or 'b' => new Note(Midi.Note.B),
            'C' or 'c' => new Note(Midi.Note.C),
            'D' or 'd' => new Note(Midi.Note.D),
            'E' or 'e' => new Note(Midi.Note.E),
            'F' or 'f' => new Note(Midi.Note.F),
            'G' or 'g' => new Note(Midi.Note.G),
            '+' => new VolumeUp(),
            '-' => new ResetVolume(),
            ' ' => new Silence(),
            'o' or 'i' or 'u' or 'O' or 'I' or 'U' => new RepeatLastNote(
                Or: new SoundEffect(RingSoundEffectId)
            ),
            ';' => new SetBpm(RandomBpm()),
            '?' => new Note(RandomNote()),
            '\n' => new ChangeToInstrument(RandomInstrument()),
            _ => LastOrNop,
        };
    }

    int RandomBpm()
    {
        // Completely arbitrary range here, I have no idea what would be a good range
        return _random.Next(60, 1200);
    }

    int RandomInstrument()
    {
        return _random.Next(Instrument.Count);
    }

    static readonly ImmutableArray<Midi.Note> MidiNotes;

    Midi.Note RandomNote()
    {
        var index = _random.Next(MidiNotes.Length);
        return MidiNotes[index];
    }

    IKeytoneInstruction LastOrNop => TryGetPreviousInstruction(out var last)
        ? last
        : new Nop();

    /// <summary>
    ///     Consumes chars from the parser.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    
    static char? Shift(ref ReadOnlyMemory<char> state, int count = 1)
    {
        if (state.Length < count) return null;
        var result = state.Span[count - 1];
        state = state[count..];
        return result;
    }

    /// <summary>
    ///     Advances the parser without consuming .
    /// </summary>
    /// <param name="state"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    
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
    public IKeytoneInstruction Current { get; private set; } = new Nop();

    public bool TryGetPreviousInstruction(out IKeytoneInstruction keytoneInstruction)
    {
        if (_last == null)
        {
            keytoneInstruction = new Nop();
            return false;
        }

        keytoneInstruction = _last;
        _last = new Nop();
        return true;
    }
}