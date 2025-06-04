using NAudio.Midi;

namespace KeytoneThunk;

public class MusicPlayer : IDisposable
{
    const int MidiDeviceId = 0;
    const int Channel = 1;

    public MusicPlayer(int volume = 50, int currentOctave = 4, int bpm = 240)
    {
        DefaultVolume = Volume = volume;
        CurrentOctave = currentOctave;
        CurrentInstrument = Instrument.AcousticGrandPiano;
        CurrentBpm = bpm;
    }

    public int DefaultVolume { get; private set; }
    public int Volume { get; private set; }
    public int CurrentOctave { get; private set; }

    public int CurrentBpm { get; private set; }

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

    public void Play(KeytoneParser keytoneInstructions)
    {
        Task.Run(async () =>
        {
            try
            {
                while (keytoneInstructions.MoveNext())
                {
                    var instruction = keytoneInstructions.Current;
                    await MatchInstruction(keytoneInstructions, instruction);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
    }

    async ValueTask MatchInstruction(KeytoneParser keytoneInstructions, IKeytoneInstruction instruction)
    {
        const byte maxAllowedDigit = 9;
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
            case IKeytoneInstruction.RepeatLastNote { Or: var or }:
                await MatchInstruction(keytoneInstructions, or);
                break;
            case IKeytoneInstruction.Silence:
                await Task.Delay(BeatDelay);
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
            case IKeytoneInstruction.SoundEffect { InstrumentId: var id }:
                await PlayNoteWithInstrument(2*NoteDuration, MidiNote.C, CurrentOctave, id);
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

    static bool LastIsNote(KeytoneParser keytoneInstructions, out IKeytoneInstruction.Note lastNote)
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
        CurrentInstrument = new Instrument((CurrentInstrument.Midi + morphInstrument.MorphDigit) % Instrument.Count);
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
        CurrentBpm = bpm;
    }

    async ValueTask PlayNoteWithInstrument(TimeSpan duration, MidiNote note, int octave, int instrumentId)
    {
        var prev = CurrentInstrument;
        ChangeInstrument(new IKeytoneInstruction.ChangeToInstrument(instrumentId));
        await PlayNote(duration, note, octave);
        ChangeInstrument(new IKeytoneInstruction.ChangeToInstrument(prev.Midi));
    }

    void BpmUp(int bpm)
    {
        CurrentBpm = Math.Clamp(CurrentBpm + bpm, 0, int.MaxValue);
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

    async ValueTask PlayNote(TimeSpan duration, MidiNote note, int octave = 4)
    {
        try
        {
            int midi = MidiConverter.Note(note, octave);
            _midiOut.Send(MidiMessage.StartNote(midi, Volume, Channel));
            _ = Task.Run(async () =>
            {
                await Task.Delay(duration);
                _midiOut.Send(MidiMessage.StopNote(midi, 0, Channel));
            });
            await Task.Delay(BeatDelay);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    static readonly TimeSpan NoteDuration = TimeSpan.FromMilliseconds(50);
    TimeSpan BeatDelay =>  CurrentBpm > 0d ? TimeSpan.FromMinutes(1d/CurrentBpm) : TimeSpan.Zero;
}