### IToken (Interface)
- Implementations:
  - ### Note (Structured Data)
    - Fields:
      - readonly int Pitch
      - readonly int Hertz
      - readonly int Octave
  - ### ChangeToInstrument (Structured Data)
    - Fields:
      - readonly int Midi
  - ### MorphInstrument (Structured Data)
    - Fields:
      - readonly byte MorphDigit
  - ### Silence (Structured Data)
  - ### VolumeUp (Structured Data)
  - ### RepeatLastNote (Structured Data)
  - ### OctaveUp (Structured Data)

### Instrument (Structured Data)
- Fields:
  - readonly int Midi

### MusicPlayer (Class)
- Properties:
  - Instrument CurrentInstrument { get; }
  - int Volume { get; }
- Methods:
  - void Play(TokenStream tokens)

### TokenStream (Class)
- Methods:
  - bool MoveNext()
  - bool TryPeekBack(out IToken token)
  - void Reset()
- Properties:
  - IToken Current { get; }

### Parser (Class)
- Methods:
  - IEnumerator\<IToken> Parse(string input)
