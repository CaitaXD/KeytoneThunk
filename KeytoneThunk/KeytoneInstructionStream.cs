using System.Collections;

namespace KeytoneThunk;

public sealed class KeytoneInstructionStream(IEnumerable<IKeytoneInstruction> tokens) 
    : IEnumerator<IKeytoneInstruction>, IEnumerable<IKeytoneInstruction>
{
    readonly IEnumerator<IKeytoneInstruction> _enumerator = tokens.GetEnumerator();

    public bool MoveNext()
    {
        _last = Current;
        if (_enumerator.MoveNext())
        {
            Current = _enumerator.Current;
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

    public void Reset() => _enumerator.Reset();
    public IKeytoneInstruction Current { get; private set; } = new IKeytoneInstruction.Silence();
    IKeytoneInstruction? _last = null;

    // C# -------------------------------------------------------------------------------------

    public void Dispose() => _enumerator.Dispose();

    object? IEnumerator.Current => Current;

    public IEnumerator<IKeytoneInstruction> GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;
}