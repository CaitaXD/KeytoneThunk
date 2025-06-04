using NAudio.Midi;

namespace KeytoneThunk;

public class MusicPlayer : IDisposable
{
    const int MidiDeviceId = 0;
    const int Channel = 1;

    public MusicPlayer(int volume = 50, int currentOctave = 4)
    {
        DefaultVolume = Volume = volume;
        CurrentOctave = currentOctave;
        CurrentInstrument = Instrument.AcousticGrandPiano;
    }

    public int DefaultVolume { get; private set; }
    public int Volume { get; private set; }
    public int CurrentOctave { get; private set; }

    public Instrument CurrentInstrument
    {
        get => _currentInstrument;
        private set
        {
            if (_currentInstrument.Midi == value.Midi) return;
            try
            {
                _currentInstrument = value;
                _midiOut.Send(MidiMessage.ChangePatch(value.Midi, 1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public void Play(KeytoneInstructionStream keytoneInstructions)
    {
        _ = PlayAsync(keytoneInstructions);
    }

    public async ValueTask PlayAsync(KeytoneInstructionStream keytoneInstructions)
    {
        const byte maxAllowedDigit = 9;
        try
        {
            foreach (var instruction in keytoneInstructions)
            {
                switch (instruction)
                {
                    case IKeytoneInstruction.MorphInstrument { MorphDigit: > maxAllowedDigit }:
                        throw new ArgumentOutOfRangeException(nameof(IKeytoneInstruction.MorphInstrument.MorphDigit));
                    case IKeytoneInstruction.MorphInstrument morphInstrument:
                        MorphInstrument(morphInstrument);
                        break;
                    case IKeytoneInstruction.ChangeToInstrument changeToInstrument:
                        ChangeInstrument(changeToInstrument);
                        break;
                    case IKeytoneInstruction.RepeatLastNote when LastIsNote(keytoneInstructions, out var lastNote):
                        await PlayNote(NoteDuration, lastNote.MidiNote, CurrentOctave);
                        break;
                    case IKeytoneInstruction.RepeatLastNote or IKeytoneInstruction.Silence:
                        await Task.Delay(NoteDuration);
                        break;
                    case IKeytoneInstruction.OctaveUp { Octaves: var octaves }:
                        OctaveUp(octaves);
                        break;
                    case IKeytoneInstruction.VolumeUp:
                        VolumeUp();
                        break;
                    case IKeytoneInstruction.ResetVolume:
                        ResetVolume();
                        break;
                    case IKeytoneInstruction.Note note:
                        await PlayNote(NoteDuration, note.MidiNote, CurrentOctave);
                        break;
                    case IKeytoneInstruction.SoundEffect soundEffect:
                        await PlaySoundEffect(soundEffect.SoundEffectId);
                        break;
                    case IKeytoneInstruction.SetBpm setBpm:
                        SetBpm(setBpm.Bpm);
                        break;
                    case IKeytoneInstruction.BpmUp bpmUp:
                        BpmUp(bpmUp.Bpm);
                        break;
                    case IKeytoneInstruction.Nop:
                        // I'm confused
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    static bool LastIsNote(KeytoneInstructionStream keytoneInstructions, out IKeytoneInstruction.Note lastNote)
    {
        if (keytoneInstructions.TryGetPreviousInstruction(out var last) && last is IKeytoneInstruction.Note note)
        {
            lastNote = note;
            return true;
        }

        lastNote = default;
        return false;
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
    
    void ResetVolume()
    {
        Volume = DefaultVolume;
    }

    void OctaveUp(int octave)
    {
        if (CurrentOctave < 8)
            CurrentOctave += octave;
        else
            CurrentOctave = 4;
    }
    
    void SetBpm(int bpm)
    {
        throw new NotImplementedException(nameof(SetBpm) + " not implemented");
    }
    
    ValueTask PlaySoundEffect(int soundEffectId)
    {
        throw new NotImplementedException(nameof(PlaySoundEffect) + " not implemented");
    }
    
    void BpmUp(int bpm)
    {
        throw new NotImplementedException(nameof(BpmUp) + " not implemented");
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

    Instrument _currentInstrument;

    readonly MidiOut _midiOut = new(MidiDeviceId);

    async Task PlayNote(TimeSpan duration, MidiNote note, int octave = 4)
    {
        try
        {
            int midi = MidiConverter.Note(note, octave);
            _midiOut.Send(MidiMessage.StartNote(midi, Volume, Channel));
            await Task.Delay(duration);
            _midiOut.Send(MidiMessage.StopNote(midi, 0, Channel));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    static readonly TimeSpan NoteDuration = TimeSpan.FromMilliseconds(50);
}