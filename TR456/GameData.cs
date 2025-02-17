using System;
using System.Diagnostics;

namespace TR456;

public static class GameData
{
    private static readonly GameMemory GameMemory = new ();

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    internal static Process GameProcess;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    internal static GameVersion GameVersion;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The new <see cref="GameVersion" /></param>
    /// <param name="hash">MD5 hash of the game EXE</param>
    public delegate void GameVersionChangedDelegate(GameVersion version, string hash);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public static GameVersionChangedDelegate OnGameVersionChanged;

    /// <summary>Reads the current active game or expansion, accounting for NG+ variations for base games.</summary>
    public static Game CurrentActiveGame => throw new NotImplementedException();

    /// <summary>Identifies the game without NG+ identification.</summary>
    public static Game CurrentActiveBaseGame => throw new NotImplementedException();

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the current level.</summary>
    /// <returns>Current level of the game</returns>
    public static uint CurrentLevel() => throw new NotImplementedException();

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the old level.</summary>
    /// <returns>Old level of the game</returns>
    public static uint OldLevel() => throw new NotImplementedException();

    /// <summary>Test that the game has fully initialized based on expected memory readings.</summary>
    private static bool GameIsInitialized => GameMemory.ActiveGame.Old is >= 0 and <= 2;

    /// <summary>Updates <see cref="GameData" /> implementation and its addresses' values.</summary>
    /// <returns><see langword="true" /> if game data was updated, <see langword="false" /> otherwise</returns>
    public static bool Update()
    {
        try
        {
            if (GameProcess is null || GameProcess.HasExited)
            {
                if (!FindSupportedGame())
                    return false;

                try
                {
                    GameMemory.InitializeMemoryWatchers(GameVersion, GameProcess);
                }
                catch (Exception e)
                {
                    LiveSplit.Options.Log.Error(e);
                    GameProcess = null;
                    return false;
                }

                return GameIsInitialized;
            }

            GameMemory.UpdateMemoryWatchers(GameProcess);
            return GameIsInitialized;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>If applicable, finds a <see cref="Process" /> running an expected version of the game.</summary>
    /// <returns>
    ///     <see langword="true" /> if <see cref="GameProcess" /> and <see cref="GameVersion" /> were meaningfully set,
    ///     <see langword="false" /> otherwise
    /// </returns>
    private static bool FindSupportedGame()
    {
        GameVersion detectedVersion = VersionDetector.DetectVersion(out Process gameProcess, out string hash);
        if (GameVersion != detectedVersion)
        {
            GameVersion = detectedVersion;
            OnGameVersionChanged.Invoke(GameVersion, hash);
        }

        if (gameProcess is null)
            return false;

        SetGameProcess(gameProcess);
        return true;
    }

    /// <summary>Sets <see cref="GameProcess" /> and performs additional work to ensure the process's termination is handled.</summary>
    /// <param name="gameProcess">Game process</param>
    private static void SetGameProcess(Process gameProcess)
    {
        GameProcess = gameProcess;
        GameProcess.EnableRaisingEvents = true;
        GameProcess.Exited += static (_, _) => OnGameVersionChanged.Invoke(GameVersion.None, string.Empty);
    }
}