using System.Collections;
using System.Diagnostics.Metrics;
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
            _midiOut.Send(MidiMessage.ChangePatch(value.Midi, 1).RawData);
        }
    }

    public void Play(TokenStream tokens)
    {
        _ = PlayAsync(tokens);
    }

    public async ValueTask PlayAsync(TokenStream tokens)
    {
        foreach (var token in tokens)
        {
            switch (token)
            {
                case IToken.MorphInstrument { MorphDigit: > 9 }:
                    throw new ArgumentOutOfRangeException(nameof(IToken.MorphInstrument.MorphDigit));
                case IToken.MorphInstrument morphInstrument:
                    CurrentInstrument = new Instrument(CurrentInstrument.Midi + morphInstrument.MorphDigit);
                    break;
                case IToken.ChangeToInstrument changeToInstrument:
                    CurrentInstrument = new Instrument(changeToInstrument.Midi);
                    break;
                case IToken.RepeatLastNote when tokens.TryPeekBack(out var last) && last is IToken.Note lastNote:
                    await PlayNote(NoteDuration, lastNote.MidiNote, CurrentOctave);
                    break;
                case IToken.RepeatLastNote or IToken.Silence:
                    await Task.Delay(NoteDuration);
                    break;
                case IToken.OctaveUp:
                    CurrentOctave = OctaveChecked(CurrentOctave + 1);
                    break;
                case IToken.VolumeUp:
                    Volume = VolumeChecked(Volume*2);
                    break;
                case IToken.Note note:
                    await PlayNote(NoteDuration, note.MidiNote, CurrentOctave);
                    break;
            }
        }
    }

    public void Dispose() => _midiOut.Dispose();

    Instrument _currentInstrument;

    readonly MidiOut _midiOut = new(MidiDeviceId);

    async Task PlayNote(TimeSpan duration, MidiNote note, int octave = 4)
    {
        int midi = MusicalNotes.MidiFromNote(note, octave);
        _midiOut.Send(MidiMessage.StartNote(midi, Volume, Channel).RawData);
        await Task.Delay(duration);
        _midiOut.Send(MidiMessage.StopNote(midi, 0, Channel).RawData);
    }

    static readonly TimeSpan NoteDuration = TimeSpan.FromMilliseconds(50);
    static int VolumeChecked(int volume) => Math.Clamp(volume, 0, 127);
    static int OctaveChecked(int octave) => Math.Clamp(octave, 0, 8);
}