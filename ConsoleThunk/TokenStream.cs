using System.Collections;

namespace KeytoneThunk;

public sealed class TokenStream(IEnumerable<IToken> tokens) : IEnumerator<IToken>, IEnumerable<IToken>
{
    readonly IEnumerator<IToken> _enumerator = tokens.GetEnumerator();

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

    public bool TryPeekBack(out IToken token)
    {
        if (_last == null)
        {
            token = new IToken.Silence();
            return false;
        }

        token = _last;
        return true;
    }

    public void Reset() => _enumerator.Reset();
    public IToken Current { get; private set; } = new IToken.Silence();
    IToken? _last = null;

    // C# -------------------------------------------------------------------------------------

    public void Dispose() => _enumerator.Dispose();

    object? IEnumerator.Current => Current;

    public IEnumerator<IToken> GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => this;
}