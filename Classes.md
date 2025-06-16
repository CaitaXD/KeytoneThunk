### IKeytoneInstruction (Interface)
- Implementations:
  - ### Note (Structured Data)
    - Fields:
      - readonly Note MidiNote
  - ### ChangeToInstrument (Structured Data)
    - Fields:
      - readonly int Midi
  - ### MorphInstrument (Structured Data)
    - Fields:
      - readonly byte MorphDigit
  - ### Silence (Structured Data)
  - ### VolumeUp (Structured Data)
  - ### RepeatLastNote (Structured Data)
    - Fields:
      - IKeytoneInstruction Or 
  - ### OctaveUp (Structured Data)
  - ### Noop (Structured Data)
  - ### SoundEffect
    - Fields:
      - int InstrumentId 

### Instrument (Structured Data)
- Fields:
  - readonly int Midi

### MusicPlayer (Class)
- Properties:
  - Instrument CurrentInstrument { get; }
  - int Volume { get; set; }
  - int Bpm { get; set; }
  - int Octave { get; set; }
- Methods:
  - void Play(KeytoneParser tokens, CancellationToken cancellationToken = default)
  - ValueTask PlayAsync(KeytoneParser parser, CancellationToken cancellationToken = default)

### KeytoneParser (Class) implements IEnumerator\<IKeytoneInstruction\>
- Methods:
  - bool MoveNext()
  - IKeytoneInstruction Current { get; }
  - TryGetPreviousInstruction(out IKeytoneInstruction);
  - KeytoneParser GetEnumerator();

### IMusicPlayerStrategy (Interface)
- Propeties:
  - int Bpm { get; set; }
  - int Volume { get; set; }
- Methods:
  -  ValueTask PlayNoteAsync(TimeSpan duration, Note note, int octave)
  -  ValueTask Silence(TimeSpan duration)
  -  void ChangeInstrument(ChangeToInstrument changeToInstrument)
  -  void MorphInstrument(MorphInstrument morphInstrument)
  -  ValueTask PlayNoteWithInstrumentAsync(TimeSpan duration, Note note, int octave, int instrumentId)
  -  static IMusicPlayerStrategy Null
- Implementations:
  - ### MidiDeviceMusicPlayerStrategy (Class)
  - ### MidiExportFileMusicPlayerStrategy (Class)
  - ### NullMusicPlayerStrategy (Class)

 ### Note (Enumeration)
   - A, B, C, D, E, F, G, H

### MidiConverter (Static Class)
- Methods:
- static int Note(Note note, int octave = 4)
