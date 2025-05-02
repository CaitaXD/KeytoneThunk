using NAudio.Midi;

namespace KeytoneThunk;

public class MusicPlayer : IDisposable
{
    const int MidiDeviceId = 0;
    const int Channel = 1;
    public int Volume { get; set; } = 50;
    public int CurrentOctave { get; set; } = 4;

    public Instrument CurrentInstrument
    {
        get => _currentInstrument;
        set
        {
            if (_currentInstrument.Midi == value.Midi) return;
            _currentInstrument = value;
            _midiOut.Send(MidiMessage.ChangePatch(value.Midi, 1));
        }
    }

    public void Play(KeytoneInstructionStream keytoneInstructions)
    {
        _ = PlayAsync(keytoneInstructions);
    }

    public async ValueTask PlayAsync(KeytoneInstructionStream keytoneInstructions)
    {
        foreach (var token in keytoneInstructions)
        {
            switch (token)
            {
                case IKeytoneInstruction.MorphInstrument { MorphDigit: > 9 }:
                    throw new ArgumentOutOfRangeException(nameof(IKeytoneInstruction.MorphInstrument.MorphDigit));
                case IKeytoneInstruction.MorphInstrument morphInstrument:
                    MorphInstrument(morphInstrument);
                    break;
                case IKeytoneInstruction.ChangeToInstrument changeToInstrument:
                    ChangeInstrument(changeToInstrument);
                    break;
                case IKeytoneInstruction.RepeatLastNote when keytoneInstructions.TryGetPreviousInstruction(out var last) && last is IKeytoneInstruction.Note lastNote:
                    await PlayNote(NoteDuration, lastNote.MidiNote, CurrentOctave);
                    break;
                case IKeytoneInstruction.RepeatLastNote or IKeytoneInstruction.Silence:
                    await Task.Delay(NoteDuration);
                    break;
                case IKeytoneInstruction.OctaveUp: 
                    OctaveUp();
                    break;
                case IKeytoneInstruction.VolumeUp:
                    VolumeUp();
                    break;
                case IKeytoneInstruction.Note note:
                    await PlayNote(NoteDuration, note.MidiNote, CurrentOctave);
                    break;
            }
        }
    }

    void ChangeInstrument(IKeytoneInstruction.ChangeToInstrument changeToInstrument)
    {
        CurrentInstrument = new Instrument(changeToInstrument.Midi);
    }

    void MorphInstrument(IKeytoneInstruction.MorphInstrument morphInstrument)
    {
        CurrentInstrument = new Instrument(CurrentInstrument.Midi + morphInstrument.MorphDigit);
    }

    void VolumeUp()
    {
        Volume = Math.Clamp(Volume*2, 0, sbyte.MaxValue);
    }

    void OctaveUp()
    {
        if (CurrentOctave < 8)
            CurrentOctave += 1;
        else
            CurrentOctave = 4;
    }

    public void Dispose() => _midiOut.Dispose();

    Instrument _currentInstrument;

    readonly MidiOut _midiOut = new(MidiDeviceId);

    async Task PlayNote(TimeSpan duration, MidiNote note, int octave = 4)
    {
        int midi = MidiConverter.Note(note, octave);
        _midiOut.Send(MidiMessage.StartNote(midi, Volume, Channel));
        await Task.Delay(duration);
        _midiOut.Send(MidiMessage.StopNote(midi, 0, Channel));
    }

    static readonly TimeSpan NoteDuration = TimeSpan.FromMilliseconds(50);
}