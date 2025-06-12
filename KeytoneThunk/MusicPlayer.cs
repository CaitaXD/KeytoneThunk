using System.Diagnostics;

namespace KeytoneThunk;

public sealed class MusicPlayer(IMusicPlayerStrategy musicStrategy, int defaultVolume = 50, int defaultBpm = 240) : IDisposable
{
    TimeSpan NoteDuration => TimeSpan.FromMilliseconds(50);
    
    bool _stopRequested;
    int _startedPlaying;
    
    public event Action<int>? VolumeChanged;
    public event Action<int>? BpmChanged;

    public int DefaultVolume { get; set; } = defaultVolume;
    public int DefaultBpm { get; set; } = defaultBpm;
    public bool IsPaused { get; private set; }
    public int CurrentVolume => musicStrategy.CurrentVolume;
    public int CurrentBpm => musicStrategy.CurrentBpm;


    readonly Instrument _defaultInstrument = Instrument.AcousticGrandPiano;

    public void Play(KeytoneParser keytoneInstructions, CancellationToken cancellationToken = default)
    {
        _ = PlayAsync(keytoneInstructions, cancellationToken).AsTask();
    }

    public async ValueTask PlayAsync(KeytoneParser parser, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _startedPlaying);
        try
        {
            while (!IsPaused && parser.MoveNext())
            {
                if (_stopRequested || cancellationToken.IsCancellationRequested) return;
                var instruction = parser.Current;
                await MatchInstruction(parser, instruction);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            DoResetVolume();
            ResetBpm();
            _stopRequested = false;
            Interlocked.Decrement(ref _startedPlaying);
            try
            {
                musicStrategy.ChangeInstrument(new ChangeToInstrument(_defaultInstrument.Midi));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    async ValueTask MatchInstruction(KeytoneParser parser, IKeytoneInstruction instruction)
    {
        const byte maxAllowedDigit = 9;
        switch (instruction)
        {
            case MorphInstrument { MorphDigit: > maxAllowedDigit }:
                throw new ArgumentOutOfRangeException(nameof(MorphInstrument.MorphDigit));
            case MorphInstrument morphInstrument:
                musicStrategy.MorphInstrument(morphInstrument);
                break;
            case ChangeToInstrument changeToInstrument:
                musicStrategy.ChangeInstrument(changeToInstrument);
                break;
            case RepeatLastNote when LastIsNote(parser, out var lastNote):
                await musicStrategy.PlayNoteAsync(NoteDuration, lastNote.MidiNote, musicStrategy.CurrentOctave);
                break;
            case RepeatLastNote { Or: var or }:
                await MatchInstruction(parser, or);
                break;
            case Silence:
                await musicStrategy.Silence(musicStrategy.BeatDelay);
                break;
            case OctaveUp { Octaves: var octaves }:
                OctaveUp(octaves);
                break;
            case VolumeUp:
                DoVolumeUp();
                break;
            case ResetVolume:
                DoResetVolume();
                break;
            case Note note:
                await musicStrategy.PlayNoteAsync(NoteDuration, note.MidiNote, musicStrategy.CurrentOctave);
                break;
            case SoundEffect { InstrumentId: var id }:
                await musicStrategy.PlayNoteWithInstrumentAsync(2*NoteDuration, MidiNote.C, musicStrategy.CurrentOctave,
                    id);
                break;
            case SetBpm setBpm:
                SetBpm(setBpm.Bpm);
                break;
            case BpmUp bpmUp:
                BpmUp(bpmUp.Bpm);
                break;
            case Nop:
                // Do nothing
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
        }
    }
    
    public void Pause()
    {
        IsPaused = true;
    }
    
    public void Resume()
    {
        IsPaused = false;
    }
    
    public void Stop()
    {
        if(_startedPlaying > 0) _stopRequested = true;
        DoResetVolume();
        ResetBpm();
    }

    static bool LastIsNote(KeytoneParser keytoneInstructions, out Note lastNote)
    {
        if (keytoneInstructions.TryGetPreviousInstruction(out var last) && last is Note note)
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

    void DoVolumeUp()
    {
        musicStrategy.CurrentVolume = Math.Clamp(musicStrategy.CurrentVolume*2, 0, sbyte.MaxValue);
        VolumeChanged?.Invoke(musicStrategy.CurrentVolume);
    }

    void DoResetVolume()
    {
        musicStrategy.CurrentVolume = DefaultVolume;
        VolumeChanged?.Invoke(musicStrategy.CurrentVolume);
    }

    void ResetBpm()
    {
        musicStrategy.CurrentBpm = DefaultBpm;
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