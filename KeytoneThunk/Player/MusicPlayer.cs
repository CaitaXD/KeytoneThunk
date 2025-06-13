using KeytoneThunk.Interpreter;
using KeytoneThunk.Midi;
using KeytoneThunk.Player.Strategy;
using Note = KeytoneThunk.Midi.Note;

namespace KeytoneThunk.Player;

public sealed class MusicPlayer(
    IMusicPlayerStrategy musicStrategy,
    int defaultVolume = 50,
    int defaultBpm = 240,
    int defaultOctave = 4)
    : IDisposable
{
    bool _stopRequested;
    int _startedPlaying;

    public readonly int DefaultOctave = defaultOctave;
    public readonly int DefaultVolume = defaultVolume;
    public readonly int DefaultBpm = defaultBpm;

    public event Action<int>? VolumeChanged;
    public event Action<int>? BpmChanged;

    public int Octave { get; set; } = defaultOctave;
    public bool IsPaused { get; private set; }

    public int Volume
    {
        get => musicStrategy.Volume;
        set => musicStrategy.Volume = value;
    }

    public int Bpm
    {
        get => musicStrategy.Bpm;
        set => musicStrategy.Bpm = value;
    }

    readonly Instrument _defaultInstrument = Instrument.AcousticGrandPiano;

    public void Play(KeytoneParser keytoneInstructions, CancellationToken cancellationToken = default)
    {
        _ = PlayAsync(keytoneInstructions, cancellationToken).AsTask();
    }

    public async ValueTask PlayAsync(KeytoneParser parser, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _startedPlaying);
        ResetDefaults();
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
            ResetDefaults();
            Interlocked.Decrement(ref _startedPlaying);
            if (_startedPlaying == 0) _stopRequested = false;
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
                await musicStrategy.PlayNoteAsync(MidiConverter.NoteDuration(Bpm), lastNote.MidiNote, Octave);
                break;
            case RepeatLastNote { Or: var or }:
                await MatchInstruction(parser, or);
                break;
            case Silence:
                await musicStrategy.Silence(MidiConverter.NoteDuration(10*Bpm));
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
            case Interpreter.Note note:
                await musicStrategy.PlayNoteAsync(MidiConverter.NoteDuration(Bpm), note.MidiNote, Octave);
                break;
            case SoundEffect { InstrumentId: var id }:
                await musicStrategy.PlayNoteWithInstrumentAsync(MidiConverter.NoteDuration(10*Bpm), Note.C,
                    Octave,
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
        if (_startedPlaying > 0) _stopRequested = true;
    }

    static bool LastIsNote(KeytoneParser keytoneInstructions, out Interpreter.Note lastNote)
    {
        if (keytoneInstructions.TryGetPreviousInstruction(out var last) && last is Interpreter.Note note)
        {
            lastNote = note;
            return true;
        }

        lastNote = default;
        return false;
    }

    void BpmUp(int bpm)
    {
        Bpm = Math.Clamp(Bpm + bpm, 0, int.MaxValue);
        BpmChanged?.Invoke(Bpm);
    }

    void SetBpm(int bpm)
    {
        Bpm = bpm;
        BpmChanged?.Invoke(Bpm);
    }

    void DoVolumeUp()
    {
        Volume = Math.Clamp(Volume*2, 0, sbyte.MaxValue);
        VolumeChanged?.Invoke(Volume);
    }

    void ResetDefaults()
    {
        try
        {
            DoResetBpm();
            DoResetOctave();
            DoResetVolume();
            musicStrategy.ChangeInstrument(new ChangeToInstrument(_defaultInstrument.Midi));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    void DoResetVolume()
    {
        Volume = DefaultVolume;
        VolumeChanged?.Invoke(Volume);
    }

    void DoResetBpm()
    {
        Bpm = DefaultBpm;
        BpmChanged?.Invoke(Bpm);
    }

    void DoResetOctave()
    {
        Octave = Math.Clamp(DefaultOctave, 1, 8);
    }

    void OctaveUp(int octave)
    {
        if (Octave < 8)
            Octave += octave;
        else
            Octave = 4;
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