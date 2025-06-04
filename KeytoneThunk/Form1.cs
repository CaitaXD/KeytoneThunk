namespace KeytoneThunk;

public partial class Form1 : Form
{
    readonly MusicPlayer _player = new(volume: 50, currentOctave: 4);

    const string Test = "???BPM+???;;;aaaa";
    public Form1()
    {
        InitializeComponent();
        rtxtboxUserInput.Text = Test;
    }

    void button1_Click(object sender, EventArgs e)
    {
        var tokens = new KeytoneParser(
            rtxtboxUserInput.Text
        );
        _player.Play(tokens);
    }
}