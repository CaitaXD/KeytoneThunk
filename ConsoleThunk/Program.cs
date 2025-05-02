// See https://aka.ms/new-console-template for more information

using KeytoneThunk;

Console.WriteLine("Hello, World!");

var _player = new MusicPlayer();
_player.Volume = 50;
_player.CurrentOctave = 4;
_player.CurrentInstrument = Instrument.AcousticGrandPiano;
var parser = new Parser();
var tokens = new TokenStream(parser.Parse(
    "CDEFGAB ?CDEFGAB ?CDEFGAB CDEFGAB ?CDEFGAB ?CDEFGAB"
));
_player.Play(tokens);
Console.ReadLine();