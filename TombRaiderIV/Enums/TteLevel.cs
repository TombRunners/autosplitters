namespace TR4;

/// <summary>The game's level and demo values.</summary>
internal enum TteLevel
{
    MainMenu = 0,

    // Cutscene
    Office = 1,

    // Playable Level
    TheTimesExclusive = 2, // At the end of the level, gfLevelComplete is set to 39 to trigger credits.
}