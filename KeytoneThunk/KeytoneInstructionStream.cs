using System.Collections;

namespace KeytoneThunk;

public sealed class KeytoneInstructionStream(IEnumerator<IKeytoneInstruction> enumerator)
    : IEnumerator<IKeytoneInstruction?>, IEnumerable<IKeytoneInstruction?>
{
    public KeytoneInstructionStream(IEnumerable<IKeytoneInstruction> tokens) : this(tokens.GetEnumerator())
    {
    }

    public bool MoveNext()
    {
        _last = Current;
        if (enumerator.MoveNext())
        {
            Current = enumerator.Current;
            return true;
        }
        return false;
    }

    public bool TryGetPreviousInstruction(out IKeytoneInstruction keytoneInstruction)
    {
        if (_last == null)
        {
            keytoneInstruction = new IKeytoneInstruction.Silence();
            return false;
        }

        keytoneInstruction = _last;
        return true;
    }

    public void Reset() => enumerator.Reset();
    public IKeytoneInstruction? Current { get; private set; }
    IKeytoneInstruction? _last = null;

    // C# -------------------------------------------------------------------------------------

    public void Dispose() => enumerator.Dispose();

    object? IEnumerator.Current => Current;

    public IEnumerator<IKeytoneInstruction?> GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;
}