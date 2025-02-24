using System.ComponentModel;

namespace TR456;

/// <summary>The game's level values.</summary>
public enum Tr5Level
{
    [Description("Main Menu / Credits")]
    MainMenu = 00,

    // Rome
    [Description("Streets of Rome")]
    StreetsOfRome  = 01,

    [Description("Trajan's Markets")]
    TrajansMarkets = 02,

    [Description("The Colosseum")]
    TheColosseum   = 03,

    // Russia
    [Description("The Base")]
    TheBase        = 04,

    [Description("The Submarine")]
    TheSubmarine   = 05,

    [Description("Deep Sea Dive")]
    DeepseaDive    = 06,

    [Description("Sinking Sub")]
    SinkingSub = 07,

    // Ireland
    [Description("Gallows Tree")]
    GallowsTree = 08,

    [Description("Labyrinth")]
    Labyrinth   = 09,

    [Description("Old Mill")]
    OldMill     = 10,

    // VCI Headquarters
    [Description("13th Floor")]
    ThirteenthFloor         = 11,

    [Description("Escape with the Iris")]
    EscapeWithTheIris       = 12,

    [Description("Security Breach [Cutscene]")]
    CutsceneSecurityBreach  = 13,

    [Description("Red Alert!")]
    RedAlert                = 14,
}