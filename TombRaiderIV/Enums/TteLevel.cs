using System.ComponentModel;

namespace TR4;

/// <summary>The game's level and demo values.</summary>
public enum TteLevel
{
    [Description("Main Menu / Credits")]
    MainMenu = 0,

    // Cutscene
    [Description("Office")]
    Office = 1,

    // Playable Level
    [Description("The Times Exclusive")]
    TheTimesExclusive = 2, // At the end of the level, gfLevelComplete is set to 39 to trigger credits.
}