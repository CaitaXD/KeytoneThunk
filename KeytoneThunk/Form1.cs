using System.Diagnostics;

namespace KeytoneThunk;

public partial class Form1 : Form
{
    static int _seed = Environment.TickCount;

    readonly MusicPlayer _player = new(new MidiDeviceMusicPlayerStrategy());
    //readonly MusicPlayer _player = new(IMusicPlayerStrategy.Null);

    const string Test = """
                        ???
                        ???
                        ???
                        """;
    public Form1()
    {
        InitializeComponent();
        _player.VolumeChanged += VolumeChanged;
        _player.BpmChanged += BpmChanged;
        rtxtboxUserInput.Text = Test;
        VolumeChanged(_player.CurrentVolume);
        BpmChanged(_player.CurrentBpm);
        txtboxSeed.Text = _seed.ToString();
        txtboxSeed.TextChanged += TxtboxSeed_TextChanged;
    }

    static void TxtboxSeed_TextChanged(object? sender, EventArgs e)
    {
        MaskedTextBox txtboxSeed = (MaskedTextBox)sender!;
        if (int.TryParse(txtboxSeed.Text, out var seed))
        {
            _seed = seed;
        }
    }
    
    void BpmChanged(int obj)
    {
        txtboxBpm.Text = obj.ToString();
    }

    void VolumeChanged(int obj)
    {
        txtboxVolume.Text = obj.ToString();
    }

    void button1_Click(object sender, EventArgs e)
    {
        var tokens = new KeytoneParser(
            rtxtboxUserInput.Text,
            _seed
        );
        _player.Play(tokens);
    }

    void btnSaveToMidiFile_Click(object sender, EventArgs e)
    {
        var fd = new SaveFileDialog();
        fd.Filter = "MIDI Files|*.mid;*.midi";
        if (fd.ShowDialog() != DialogResult.OK) return;
        var filePath = fd.FileName;
        using var p = new MusicPlayer(new MidiExportFileMusicPlayerStrategy(filePath));
        p.Play(new KeytoneParser(rtxtboxUserInput.Text, _seed));
    }

    void btnRandSeed_Click(object sender, EventArgs e)
    {
        _seed = HashCode.Combine(_seed, Environment.TickCount);
        txtboxSeed.Text = _seed.ToString();
    }

    void btnLoadText_Click(object sender, EventArgs e)
    {
        var fd = new OpenFileDialog();
        fd.Filter = "Text Files|*.txt";
        if (fd.ShowDialog() != DialogResult.OK) return;
        var filePath = fd.FileName;
        rtxtboxUserInput.Text = File.ReadAllText(filePath);
    }

    void btnStop_Click(object sender, EventArgs e)
    {
        _player.Stop();
    }
}