namespace KeytoneThunk;

public class MusicPlayer
{
    public Instrument CurrentInstrument { get; private set; }
    public int Volume { get; }
    
    public void Play(TokenStream tokens)
    {
        throw new NotImplementedException();
    }
}