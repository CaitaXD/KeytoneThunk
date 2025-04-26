namespace KeytoneThunk;

public partial class Form1 : Form
{
    readonly MusicPlayer _player = new MusicPlayer();

    public Form1()
    {
        InitializeComponent();
    }

    void button1_Click(object sender, EventArgs e)
    {
        _player.Volume = 50;
        _player.CurrentOctave = 4;
        var parser = new Parser();
        var tokens = new TokenStream(parser.Parse(
            "CDEFGAB ?CDEFGAB ?CDEFGAB ?CDEFGAB"
        ));
        _player.Play(tokens);
    }
}