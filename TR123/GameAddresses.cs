namespace TR123;

public readonly record struct GameAddresses
{
    /// <summary>Used to distinguish between NG and NG+ playthroughs of non-expansions.</summary>
    public int BonusFlag { get; init; }

    /// <summary>The address of the first member of the IGT in-memory array which stores the IGT of completed levels.</summary>
    public int FirstLevelTime { get; init; }

    /// <summary>Lara's Health.</summary>
    public int Health { get; init; }

    /// <summary>The current level of the game.</summary>
    public int Level { get; init; }

    /// <summary>A flag that dictates the level has been completed and should split.</summary>
    public int LevelComplete { get; init; }

    /// <summary>The running IGT of the current level.</summary>
    public int LevelIgt { get; init; }

    /// <summary>Indicates if the game is in the title screen (main menu).</summary>
    public int TitleLoaded { get; init; }
}