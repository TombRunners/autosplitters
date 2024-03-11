using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExtensionMethods;
using LiveSplit.ComponentUtil;

namespace TRUtil;

public abstract class BaseGameData
{
    /// <summary>Used to calculate <see cref="TimeSpan"/>s from IGT ticks.</summary>
    private const int IGTTicksPerSecond = 30;

    /// <summary>Strings used when searching for a running game <see cref="Process"/>.</summary>
    protected static readonly List<string> ProcessSearchNames = [];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>Ideally, this will be converted from some <see cref="Enum"/> for clarity.</remarks>
    protected static readonly Dictionary<string, uint> VersionHashes = [];

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
    protected static readonly MemoryWatcherList Watchers = [];

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    protected static Process Game;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    public static uint Version;

    /// <summary>Allows creation of an event regarding when an ASL Component was found in the LiveSplit layout.</summary>
    public delegate void AslComponentChangedDelegate(bool aslComponentIsPresent);

    /// <summary>Allows subscribers to know when an ASL Component was found in the LiveSplit layout.</summary>
    public AslComponentChangedDelegate OnAslComponentChanged;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The version found; ideally, this will be converted from some <see cref="Enum"/> for clarity.</param>
    /// <param name="hash">The MD5 hash of the game process EXE.</param>
    public delegate void GameFoundDelegate(uint version, string hash);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public GameFoundDelegate OnGameFound;

    /// <summary>A constructor existing primarily to clear static fields expected to be managed only in derived classes' constructors.</summary>
    protected BaseGameData()
    {
        VersionHashes.Clear();
        ProcessSearchNames.Clear();
    }

    #region MemoryWatcherList Items

    /// <summary>Gives the value of the active level; for TR1, also matches an active cutscene, FMV, or demo.</summary>
    /// <remarks>
    ///     Usually matches chronological number (TR3 can have exceptions due to level order choice).
    ///     Lara's Home (even if not present in the game) usually counts as 0.
    ///         One exception is in the ATI version of Tomb Raider Unfinished Business, where Lara's Home is not present,
    ///         the first level's value is 0, and the main menu is level 4.
    /// </remarks>
    public static MemoryWatcher<uint> Level => (MemoryWatcher<uint>)Watchers?["Level"];

    /// <summary>Lara's current HP.</summary>
    /// <remarks>Max HP is 1000. When it hits 0, Lara dies.</remarks>
    public static MemoryWatcher<short> Health => (MemoryWatcher<short>)Watchers?["Health"];

    #endregion

    /// <summary>Sets addresses based on <paramref name="version"/>.</summary>
    /// <param name="version">Version to base addresses on; ideally, this will be converted from some <see cref="Enum"/> for clarity.</param>
    protected abstract void SetAddresses(uint version);

    /// <summary>Updates <see cref="ClassicGameData"/> implementation and its addresses' values.</summary>
    /// <returns><see langword="true"/> if game data was updated, <see langword="false"/> otherwise</returns>
    public bool Update()
    {
        try
        {
            if (Game is null || Game.HasExited)
                return TrySetGameProcessAndVersion();

            Watchers.UpdateAll(Game);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>If applicable, finds a <see cref="Process"/> running an expected version of the game.</summary>
    /// <returns><see langword="true"/> if <see cref="Game"/> and <see cref="Version"/> were meaningfully set, <see langword="false"/> otherwise</returns>
    private bool TrySetGameProcessAndVersion()
    {
        const uint noneOrUndetectedValue = 0;

        // Find game Processes.
        var processes = ProcessSearchNames.SelectMany(Process.GetProcessesByName).ToList();
        if (processes.Count == 0)
        {
            // Set Version to a value indicating no game was found.
            if (Version != noneOrUndetectedValue)
                OnGameFound.Invoke(noneOrUndetectedValue, string.Empty);

            Version = noneOrUndetectedValue;
            return false;
        }

        // Try finding a match from known version hashes.
        var hash = string.Empty;
        var gameProcess = processes.FirstOrDefault(p =>
            {
                hash = p.GetMd5Hash();
                return VersionHashes.TryGetValue(hash, out Version);
            }
        );
        if (gameProcess is null)
        {
            // Set Version to a value indicating the game version is unknown.
            const uint unknownValue = 0xDEADBEEF;
            if (Version != unknownValue)
                OnGameFound.Invoke(unknownValue, hash);

            Version = unknownValue;
            return false;
        }

        // Set Game and do some event management.
        Game = gameProcess;
        Game.EnableRaisingEvents = true;
        Game.Exited += (_, _) => OnGameFound.Invoke(noneOrUndetectedValue, string.Empty);
        SetAddresses(Version);
        OnGameFound.Invoke(Version, hash);
        return true;
    }

    /// <summary>Converts IGT ticks to a double representing time elapsed in decimal seconds.</summary>
    public static double LevelTimeAsDouble(ulong ticks) => (double)ticks / IGTTicksPerSecond;
}