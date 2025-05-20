using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiveSplit.ComponentUtil;

namespace Util;

public abstract class BaseGameData
{
    private readonly VersionDetector _versionDetector = new ();

    /// <summary>Used to calculate <see cref="TimeSpan"/>s from IGT ticks.</summary>
    private const int IGTTicksPerSecond = 30;

    /// <summary>Strings used when searching for a running game <see cref="Process"/>.</summary>
    protected List<string> ProcessSearchNames => _versionDetector.ProcessSearchNames;

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>Ideally, this will be converted from some <see cref="Enum"/> for clarity.</remarks>
    protected Dictionary<string, uint> VersionHashes => _versionDetector.VersionHashes;

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
    protected readonly MemoryWatcherList Watchers = [];

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    protected Process GameProcess { get; private set; }

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    public uint GameVersion { get; private set; }

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The version found; ideally, this will be converted from some <see cref="Enum"/> for clarity.</param>
    /// <param name="hash">The MD5 hash of the game process EXE.</param>
    public delegate void GameFoundDelegate(uint version, string hash);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public GameFoundDelegate OnGameVersionChanged;

    #region MemoryWatcherList Items

    /// <summary>Gives the value of the active level; for TR1, also matches an active cutscene, FMV, or demo.</summary>
    /// <remarks>
    ///     Usually matches chronological number (TR3 can have exceptions due to level order choice).
    ///     Lara's Home (even if not present in the game) usually counts as 0.
    ///         One exception is in the ATI version of Tomb Raider Unfinished Business, where Lara's Home is not present,
    ///         the first level's value is 0, and the main menu is level 4.
    /// </remarks>
    public MemoryWatcher<uint> Level => (MemoryWatcher<uint>)Watchers?["Level"];

    /// <summary>Lara's current HP.</summary>
    /// <remarks>Max HP is 1000. When it hits 0, Lara dies.</remarks>
    public MemoryWatcher<short> Health => (MemoryWatcher<short>)Watchers?["Health"];

    #endregion


    /// <summary>Sets addresses for <see cref="Watchers" /> based on <paramref name="version" />.</summary>
    /// <param name="version">Version to base addresses on; the uint will be converted to <see cref="GameVersion" />.</param>
    protected abstract void SetMemoryAddresses(uint version);

    /// <summary>Tests that the game has fully initialized based on expected memory readings.</summary>
    /// <returns><see langword="true" /> if game is fully initialized, <see langword="false" /> otherwise</returns>
    protected abstract bool IsGameInitialized();

    /// <summary>
    ///     This method should be called when initializing MemoryWatchers to ensure that they do not have
    ///     default / zeroed values on initialization, which complicates or ruins autosplitter logic.
    /// </summary>
    private void PreLoadWatchers()
    {
        Watchers.UpdateAll(GameProcess); // Loads Current values.
        Watchers.UpdateAll(GameProcess); // Moves Current to Old and loads new Current values.
    }

    /// <summary>Updates <see cref="BaseGameData" /> implementation and its addresses' values.</summary>
    /// <returns><see langword="true" /> if game data was updated, <see langword="false" /> otherwise</returns>
    public bool Update()
    {
        try
        {
            if (GameProcess is null || GameProcess.HasExited)
            {
                if (!FindSupportedGame())
                    return false;

                SetMemoryAddresses(GameVersion);
                PreLoadWatchers();
                return IsGameInitialized();
            }

            Watchers.UpdateAll(GameProcess);
            return IsGameInitialized();
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
    private bool FindSupportedGame()
    {
        uint detectedVersion = _versionDetector.DetectVersion(out Process gameProcess, out string hash);
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
    private void SetGameProcess(Process gameProcess)
    {
        GameProcess = gameProcess;
        GameProcess.EnableRaisingEvents = true;
        GameProcess.Exited += (_, _) => OnGameVersionChanged.Invoke(VersionDetector.NoneOrUndetectedValue, string.Empty);
    }

    /// <summary>Converts IGT ticks to a double representing time elapsed in decimal seconds.</summary>
    public static double LevelTimeAsDouble(ulong ticks) => (double)ticks / IGTTicksPerSecond;
}