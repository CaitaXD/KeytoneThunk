using NAudio.Midi;

namespace KeytoneThunk;

public class MidiDeviceMusicPlayerStrategy : IMusicPlayerStrategy
{
    public int DefaultVolume { get; }
    public int DefaultBpm { get; }
    public int CurrentBpm { get; set; }
    public int CurrentVolume { get; set; }
    public int CurrentOctave { get; set; }

    Instrument _currentInstrument;
    readonly MidiOut _midiOut;
    readonly int _channel;

    public MidiDeviceMusicPlayerStrategy(int deviceId = 0, int channel = 1, int octave = 4, int bpm = 240, int volume = 50)
    {
        _midiOut = new MidiOut(deviceId);
        _channel = channel;
        CurrentBpm = bpm;
        CurrentOctave = octave;
        DefaultVolume = CurrentVolume = volume;
        DefaultBpm = CurrentBpm = bpm;
    }

    public void ChangeInstrument(IKeytoneInstruction.ChangeToInstrument changeToInstrument)
    {
        _midiOut.Send(MidiMessage.ChangePatch(changeToInstrument.Midi, 1));
        _currentInstrument = new Instrument(changeToInstrument.Midi);
    }

    public void MorphInstrument(IKeytoneInstruction.MorphInstrument morphInstrument)
    {
        int id = (_currentInstrument.Midi + morphInstrument.MorphDigit)%Instrument.Count;
        _midiOut.Send(MidiMessage.ChangePatch(id, 1));
        _currentInstrument = new Instrument(id);
    }

    public async ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, MidiNote note, int octave, int instrumentId)
    {
        var prev = _currentInstrument;
        ChangeInstrument(new IKeytoneInstruction.ChangeToInstrument(instrumentId));
        await PlayNoteAsync(duration, note, octave);
        ChangeInstrument(new IKeytoneInstruction.ChangeToInstrument(prev.Midi));
    }

    public async ValueTask PlayNoteAsync(TimeSpan duration, MidiNote note, int octave = 4)
    {
        try
        {
            int midi = MidiConverter.Note(note, octave);
            _midiOut.Send(MidiMessage.StartNote(midi, CurrentVolume, _channel));
            _ = Task.Run(async () =>
            {
                await Task.Delay(duration);
                _midiOut.Send(MidiMessage.StopNote(midi, 0, _channel));
            });
            await Task.Delay(((IMusicPlayerStrategy)this).BeatDelay);
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