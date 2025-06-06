using System.Diagnostics;

namespace KeytoneThunk;

public sealed class MusicPlayer(IMusicPlayerStrategy musicStrategy) : IDisposable
{
    TimeSpan NoteDuration => TimeSpan.FromMilliseconds(50);

    public event Action<int>? VolumeChanged;
    public event Action<int>? BpmChanged;

    public int CurrentVolume => musicStrategy.CurrentVolume;
    public int CurrentBpm => musicStrategy.CurrentBpm;


    readonly Instrument _defaultInstrument = Instrument.AcousticGrandPiano;

    public void Play(KeytoneParser keytoneInstructions)
    {
        _ = PlayAsync(keytoneInstructions).AsTask();
    }

    public async ValueTask PlayAsync(KeytoneParser keytoneInstructions)
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
        finally
        {
            ResetVolume();
            ResetBpm();
            try
            {
                musicStrategy.ChangeInstrument(new IKeytoneInstruction.ChangeToInstrument(_defaultInstrument.Midi));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    async ValueTask MatchInstruction(KeytoneParser keytoneInstructions, IKeytoneInstruction instruction)
    {
        const byte maxAllowedDigit = 9;
        switch (instruction)
        {
            case IKeytoneInstruction.MorphInstrument { MorphDigit: > maxAllowedDigit }:
                throw new ArgumentOutOfRangeException(nameof(IKeytoneInstruction.MorphInstrument.MorphDigit));
            case IKeytoneInstruction.MorphInstrument morphInstrument:
                musicStrategy.MorphInstrument(morphInstrument);
                break;
            case IKeytoneInstruction.ChangeToInstrument changeToInstrument:
                musicStrategy.ChangeInstrument(changeToInstrument);
                break;
            case IKeytoneInstruction.RepeatLastNote when LastIsNote(keytoneInstructions, out var lastNote):
                await musicStrategy.PlayNoteAsync(NoteDuration, lastNote.MidiNote, musicStrategy.CurrentOctave);
                break;
            case IKeytoneInstruction.RepeatLastNote { Or: var or }:
                await MatchInstruction(keytoneInstructions, or);
                break;
            case IKeytoneInstruction.Silence:
                await musicStrategy.Silence(musicStrategy.BeatDelay);
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
                await musicStrategy.PlayNoteAsync(NoteDuration, note.MidiNote, musicStrategy.CurrentOctave);
                break;
            case IKeytoneInstruction.SoundEffect { InstrumentId: var id }:
                await musicStrategy.PlayNoteWithInstrumentAsync(2*NoteDuration, MidiNote.C, musicStrategy.CurrentOctave,
                    id);
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

    void BpmUp(int bpm)
    {
        musicStrategy.CurrentBpm = Math.Clamp(musicStrategy.CurrentBpm + bpm, 0, int.MaxValue);
        BpmChanged?.Invoke(musicStrategy.CurrentBpm);
    }

    void SetBpm(int bpm)
    {
        musicStrategy.CurrentBpm = bpm;
        BpmChanged?.Invoke(musicStrategy.CurrentBpm);
    }

    void VolumeUp()
    {
        musicStrategy.CurrentVolume = Math.Clamp(musicStrategy.CurrentVolume*2, 0, sbyte.MaxValue);
        VolumeChanged?.Invoke(musicStrategy.CurrentVolume);
    }

    void ResetVolume()
    {
        musicStrategy.CurrentVolume = musicStrategy.DefaultVolume;
        VolumeChanged?.Invoke(musicStrategy.CurrentVolume);
    }

    void ResetBpm()
    {
        musicStrategy.CurrentBpm = musicStrategy.DefaultBpm;
        BpmChanged?.Invoke(musicStrategy.CurrentBpm);
    }

    void OctaveUp(int octave)
    {
        if (musicStrategy.CurrentOctave < 8)
            musicStrategy.CurrentOctave += octave;
        else
            musicStrategy.CurrentOctave = 4;
    }

    public void Dispose()
    {
        try
        {
            musicStrategy.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}