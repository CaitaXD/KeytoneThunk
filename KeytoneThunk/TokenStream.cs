using System.Collections;

namespace KeytoneThunk;

public class TokenStream : IEnumerator<IToken>, IEnumerable<IToken>
{
    public TokenStream(IEnumerator<IToken> tokens)
    {
        
    }

    public bool MoveNext()
    {
        throw new NotImplementedException();
    }

    public bool TryPeekBack(out IToken token)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
    
    public IToken Current => throw new NotImplementedException();

    // C# -------------------------------------------------------------------------------------
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
    
    object? IEnumerator.Current => Current;
    public IEnumerator<IToken> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}