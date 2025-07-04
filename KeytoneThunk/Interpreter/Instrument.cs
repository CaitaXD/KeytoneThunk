﻿namespace KeytoneThunk.Interpreter;

/// <summary>
///     This structer models the instrument from the MiDi specification
///     A lista de intrumentos foi retirada de : https://fmslogo.sourceforge.io/manual/midi-instrument.html
/// </summary>

public readonly struct Instrument
{
    public readonly int Midi;

    public Instrument(int midi)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(midi);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(midi, 127);
        Midi = midi;
    }

    public static Instrument AcousticGrandPiano = new(0);
    public static Instrument BrightAcousticPiano = new(1);
    public static Instrument ElectricGrandPiano = new(2);
    public static Instrument HonkyTonkPiano = new(3);
    public static Instrument RhodesPiano = new(4);
    public static Instrument ChorusedPiano = new(5);
    public static Instrument Harpsichord = new(6);
    public static Instrument Clavinet = new(7);
    public static Instrument Celesta = new(8);
    public static Instrument Glockenspiel = new(9);
    public static Instrument MusicBox = new(10);
    public static Instrument Vibraphone = new(11);
    public static Instrument Marimba = new(12);
    public static Instrument Xylophone = new(13);
    public static Instrument TubularBells = new(14);
    public static Instrument Dulcimer = new(15);
    public static Instrument DrawbarOrgan = new(16);
    public static Instrument PercussiveOrgan = new(17);
    public static Instrument RockOrgan = new(18);
    public static Instrument ChurchOrgan = new(19);
    public static Instrument ReedOrgan = new(20);
    public static Instrument Accordion = new(21);
    public static Instrument Harmonica = new(22);
    public static Instrument TangoAccordion = new(23);
    public static Instrument AcousticGuitarNylon = new(24);
    public static Instrument AcousticGuitarSteel = new(25);
    public static Instrument ElectricGuitarJazz = new(26);
    public static Instrument ElectricGuitarClean = new(27);
    public static Instrument ElectricGuitarMuted = new(28);
    public static Instrument OverdrivenGuitar = new(29);
    public static Instrument DistortionGuitar = new(30);
    public static Instrument GuitarHarmonics = new(31);
    public static Instrument AcousticBass = new(32);
    public static Instrument ElectricBassFinger = new(33);
    public static Instrument ElectricBassPick = new(34);
    public static Instrument FretlessBass = new(35);
    public static Instrument SlapBass1 = new(36);
    public static Instrument SlapBass2 = new(37);
    public static Instrument SynthBass1 = new(38);
    public static Instrument SynthBass2 = new(39);
    public static Instrument Violin = new(40);
    public static Instrument Viola = new(41);
    public static Instrument Cello = new(42);
    public static Instrument Contrabass = new(43);
    public static Instrument TremoloStrings = new(44);
    public static Instrument PizzicatoStrings = new(45);
    public static Instrument OrchestralHarp = new(46);
    public static Instrument Timpani = new(47);
    public static Instrument StringEnsemble1 = new(48);
    public static Instrument StringEnsemble2 = new(49);
    public static Instrument SynthStrings1 = new(50);
    public static Instrument SynthStrings2 = new(51);
    public static Instrument ChoirAahs = new(52);
    public static Instrument VoiceOohs = new(53);
    public static Instrument SynthVoice = new(54);
    public static Instrument OrchestraHit = new(55);
    public static Instrument Trumpet = new(56);
    public static Instrument Trombone = new(57);
    public static Instrument Tuba = new(58);
    public static Instrument MutedTrumpet = new(59);
    public static Instrument FrenchHorn = new(60);
    public static Instrument BrassSection = new(61);
    public static Instrument SynthBrass1 = new(62);
    public static Instrument SynthBrass2 = new(63);
    public static Instrument SopranoSax = new(64);
    public static Instrument AltoSax = new(65);
    public static Instrument TenorSax = new(66);
    public static Instrument BaritoneSax = new(67);
    public static Instrument Oboe = new(68);
    public static Instrument EnglishHorn = new(69);
    public static Instrument Bassoon = new(70);
    public static Instrument Clarinet = new(71);
    public static Instrument Piccolo = new(72);
    public static Instrument Flute = new(73);
    public static Instrument Recorder = new(74);
    public static Instrument PanFlute = new(75);
    public static Instrument BlownBottle = new(76);
    public static Instrument Shakuhachi = new(77);
    public static Instrument Whistle = new(78);
    public static Instrument Ocarina = new(79);
    public static Instrument Lead1Square = new(80);
    public static Instrument Lead2Sawtooth = new(81);
    public static Instrument Lead3Calliope = new(82);
    public static Instrument Lead4Chiff = new(83);
    public static Instrument Lead5Charang = new(84);
    public static Instrument Lead6Voice = new(85);
    public static Instrument Lead7Fifths = new(86);
    public static Instrument Lead8BassLead = new(87);
    public static Instrument Pad1NewAge = new(88);
    public static Instrument Pad2Warm = new(89);
    public static Instrument Pad3Polysynth = new(90);
    public static Instrument Pad4Choir = new(91);
    public static Instrument Pad5Bowed = new(92);
    public static Instrument Pad6Metallic = new(93);
    public static Instrument Pad7Halo = new(94);
    public static Instrument Pad8Sweep = new(95);
    public static Instrument Fx1Rain = new(96);
    public static Instrument Fx2Soundtrack = new(97);
    public static Instrument Fx3Crystal = new(98);
    public static Instrument Fx4Atmosphere = new(99);
    public static Instrument Fx5Brightness = new(100);
    public static Instrument Fx6Goblins = new(101);
    public static Instrument Fx7Echoes = new(102);
    public static Instrument Fx8SciFi = new(103);
    public static Instrument Sitar = new(104);
    public static Instrument Banjo = new(105);
    public static Instrument Shamisen = new(106);
    public static Instrument Koto = new(107);
    public static Instrument Kalimba = new(108);
    public static Instrument Bagpipe = new(109);
    public static Instrument Fiddle = new(110);
    public static Instrument Shanai = new(111);
    public static Instrument TinkleBell = new(112);
    public static Instrument Agogo = new(113);
    public static Instrument SteelDrums = new(114);
    public static Instrument Woodblock = new(115);
    public static Instrument TaikoDrum = new(116);
    public static Instrument MelodicTom = new(117);
    public static Instrument SynthDrum = new(118);
    public static Instrument ReverseCymbal = new(119);
    public static Instrument GuitarFretNoise = new(120);
    public static Instrument BreathNoise = new(121);
    public static Instrument Seashore = new(122);
    public static Instrument BirdTweet = new(123);
    public static Instrument TelephoneRing = new(124);
    public static Instrument Helicopter = new(125);
    public static Instrument Applause = new(126);
    public static Instrument Gunshot = new(127);

    public const int Count = 128;
}