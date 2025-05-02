namespace KeytoneThunk;

public partial class Form1 : Form
{
    readonly MusicPlayer _player = new();

    public Form1()
    {
        InitializeComponent();
    }

    void button1_Click(object sender, EventArgs e)
    {
        _player.Volume = 50;
        _player.CurrentOctave = 4;
        _player.CurrentInstrument = Instrument.AcousticGrandPiano;
        var tokens = new KeytoneInstructionStream(KeytoneParser.Parse(
            rtxtboxUserInput.Text
        ));
        _player.Play(tokens);
    }
}