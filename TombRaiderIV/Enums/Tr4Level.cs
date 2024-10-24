using System.ComponentModel;

namespace TR4;

/// <summary>The game's level and demo values.</summary>
public enum Tr4Level
{
    [Description("Main Menu / Credits")]
    MainMenu = 0,

    // Cambodia
    [Description("Angkor Wat")]
    AngkorWat = 1,

    [Description("Race for the Iris")]
    RaceForTheIris = 2,

    [Description("The Tomb of Seth")]
    TheTombOfSeth = 3,

    [Description("Burial Chambers")]
    BurialChambers = 4,

    // Valley of the Kings
    [Description("Valley of the Kings")]
    ValleyOfTheKings = 5,

    [Description("KV5")]
    Kv5 = 6,

    // Karnak
    [Description("Temple of Karnak")]
    TempleOfKarnak = 7,

    [Description("Great Hypostyle Hall")]
    GreatHypostyleHall = 8,

    [Description("Sacred Lake")]
    SacredLake = 9,

    [Description("Tomb of Semerkhet")]
    TombOfSemerkhet = 11,

    [Description("Guardian of Semerkhet")]
    GuardianOfSemerkhet = 12,

    // Eastern Desert
    [Description("Desert Railroad")]
    DesertRailroad = 13,

    // Alexandria
    [Description("Alexandria")]
    Alexandria = 14,

    [Description("Coastal Ruins")]
    CoastalRuins = 15,

    [Description("Pharos, Temple of Isis")]
    PharosTempleOfIsis = 16,

    [Description("Cleopatra's Palaces")]
    CleopatrasPalaces = 17,

    [Description("Catacombs")]
    Catacombs = 18,

    [Description("Temple of Poseidon")]
    TempleOfPoseidon = 19,

    [Description("The Lost Library")]
    TheLostLibrary = 20,

    [Description("Hall of Demetrius")]
    HallOfDemetrius = 21,

    // Cairo
    [Description("City of the Dead")]
    CityOfTheDead = 22,

    [Description("Trenches")]
    Trenches = 23,

    [Description("Chambers of Tulun")]
    ChambersOfTulun = 24,

    [Description("Street Bazaar")]
    StreetBazaar = 25,

    [Description("Citadel Gate")]
    CitadelGate = 26,

    [Description("Citadel")]
    Citadel = 27,

    // Giza
    [Description("Sphinx Complex")]
    SphinxComplex = 28,

    [Description("Underneath the Sphinx")]
    UnderneathTheSphinx = 30,

    [Description("Menkaure's Pyramid")]
    MenkauresPyramid = 31,

    [Description("Inside Menkaure's Pyramid")]
    InsideMenkauresPyramid = 32,

    [Description("The Mastabas")]
    TheMastabas = 33,

    [Description("The Great Pyramid")]
    TheGreatPyramid = 34,

    [Description("Khufu's Queen's Pyramids")]
    KhufusQueensPyramid = 35,

    [Description("Inside the Great Pyramid")]
    InsideTheGreatPyramid = 36,

    // Temple of Horus
    [Description("Temple of Horus")]
    TempleOfHorus = 37,

    [Description("Horus Boss")]
    HorusBoss = 38,
}