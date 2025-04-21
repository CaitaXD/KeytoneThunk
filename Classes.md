### IToken
- ### Note
  - Fields:
    - readonly int Pitch
    - readonly int Hertz
    - readonly int Octave
- ### ChangeToInstrument
  - Fields:
    - readonly int Midi
- ### MorphInstrument
  - Fields:
    - readonly byte MorphDigit
- ### Silence
- ### VolumeUp
- ### RepeatLastNote
- ### OctaveUp

### Instrument
- Properties:
  - int Midi { get; }

### MusicPlayer
- Properties:
  - Instrument CurrentInstrument { get; }
  - int Volume { get; }
- Methods:
  - void Play(TokenStream tokens)

### TokenStream
- Methods:
  - bool MoveNext()
  - bool TryPeekBack(out IToken token)
  - void Reset()
- Properties:
  - IToken Current { get; }

### Parser
- Methods:
  - IEnumerator\<IToken> Parse(string input)