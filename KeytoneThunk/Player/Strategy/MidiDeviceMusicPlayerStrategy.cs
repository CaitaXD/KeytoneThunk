using KeytoneThunk.Interpreter;
using KeytoneThunk.Midi;
using NAudio.Midi;
using MidiMessage = KeytoneThunk.Midi.MidiMessage;
using Note = KeytoneThunk.Midi.Note;

namespace KeytoneThunk.Player.Strategy;

public class MidiDeviceMusicPlayerStrategy : IMusicPlayerStrategy
{
    public int Bpm { get; set; }
    public int Volume { get; set; }
    public int Octave { get; set; }

    Instrument _currentInstrument;
    readonly MidiOut _midiOut;
    readonly int _channel;

    public MidiDeviceMusicPlayerStrategy(int deviceId = 0, int channel = 1, int octave = 4, int bpm = 240, int volume = 50)
    {
        _midiOut = new MidiOut(deviceId);
        _channel = channel;
        Bpm = bpm;
        Octave = octave;
        Volume = volume;
        Bpm = bpm;
    }

    public ValueTask Silence(TimeSpan duration)
    {
        return  new ValueTask(Task.Delay(duration));
    }

    public void ChangeInstrument(ChangeToInstrument changeToInstrument)
    {
        _midiOut.Send(MidiMessage.ChangePatch(changeToInstrument.Midi, 1));
        _currentInstrument = new Instrument(changeToInstrument.Midi);
    }

    public void MorphInstrument(MorphInstrument morphInstrument)
    {
        int id = (_currentInstrument.Midi + morphInstrument.MorphDigit)%Instrument.Count;
        _midiOut.Send(MidiMessage.ChangePatch(id, 1));
        _currentInstrument = new Instrument(id);
    }

    public async ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, Note note, int octave, int instrumentId)
    {
        var prev = _currentInstrument;
        ChangeInstrument(new ChangeToInstrument(instrumentId));
        await PlayNoteAsync(duration, note, octave);
        ChangeInstrument(new ChangeToInstrument(prev.Midi));
    }

    public async ValueTask PlayNoteAsync(TimeSpan duration, Note note, int octave = 4)
    {
        try
        {
            int midi = MidiConverter.Note(note, octave);
            _midiOut.Send(MidiMessage.StartNote(midi, Volume, _channel));
            _ = Task.Run(async () =>
            {
                await Task.Delay(duration);
                _midiOut.Send(MidiMessage.StopNote(midi, 0, _channel));
            });
            await Task.Delay(MidiConverter.QuarterNoteDuration(Bpm));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void Dispose()
    {
        try
        {
            _midiOut.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}