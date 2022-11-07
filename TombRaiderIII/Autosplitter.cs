using TRUtil;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace TR3;

/// <summary>The game's level and demo values.</summary>
internal enum Tr3Level : uint
{
    LarasHome = 00,
    // India
    Jungle = 01,
    TempleRuins = 02,
    TheRiverGanges = 03,
    CavesOfKaliya = 04,
    // South Pacific
    CoastalVillage = 05,
    CrashSite = 06,
    MadubuGorge = 07,
    TempleOfPuna = 08,
    // London
    ThamesWharf = 09,
    Aldwych = 10,
    LudsGate = 11,
    City = 12,
    // Nevada
    NevadaDesert = 13,
    HighSecurityCompound = 14,
    Area51 = 15,
    // Antarctica
    Antarctica = 16,
    RxTechMines = 17,
    LostCityOfTinnos = 18,
    MeteoriteCavern = 19,
    // Bonus (All Secrets)
    AllHallows = 20
}

/// <summary>The game's level and demo values.</summary>
internal enum TlaLevel : uint
{
    LarasHome = 00,
    // Scotland
    HighlandFling = 01,
    WillardsLair = 02,
    // Channel Tunnel
    ShakespeareCliff = 03,
    SleepingWithTheFishes = 04,
    // Paris
    ItsAMadhouse= 05,
    Reunion = 06
}

/// <summary>Implementation of <see cref="ClassicAutosplitter"/>.</summary>
internal sealed class Autosplitter : ClassicAutosplitter
{
    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter()
    {
        Settings = new ComponentSettings();

        LevelCount = 20; // This is the highest between TR3 at 20 and TLA at 6.
        CompletedLevels.Capacity = LevelCount;

        Data = new GameData();
        Data.OnGameFound += Settings.SetGameVersion;
    }
}