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
    protected const int IGTTicksPerSecond = 30;

    /// <summary>Strings used when searching for a running game <see cref="Process"/>.</summary>
    protected static readonly List<string> ProcessSearchNames = [];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>Ideally, this will be converted from some <see cref="Enum"/> for clarity.</remarks>
    protected static readonly Dictionary<string, uint> VersionHashes = new();

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
    protected static readonly MemoryWatcherList Watchers = [];

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    protected static Process Game;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    public static uint Version;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The version found; ideally, this will be converted from some <see cref="Enum"/> for clarity.</param>
    public delegate void GameFoundDelegate(uint version);

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
            {
                if (!SetGameProcessAndVersion())
                    return false;

                SetAddresses(Version);
                OnGameFound.Invoke(Version);
                return true;
            }

            Watchers.UpdateAll(Game);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>If applicable, finds a <see cref="Process"/> running an expected version of the game.</summary>
    /// <returns><see langword="true"/> if <see cref="Game"/> and <see cref="Version"/> were meaningfully set, <see langword="false"/> otherwise</returns>
    private bool SetGameProcessAndVersion()
    {
        // Find game Process, if any, and set Version member accordingly.
        var gameProcess = ProcessSearchNames.SelectMany(Process.GetProcessesByName)
            .First(static p => VersionHashes.TryGetValue(p.GetMd5Hash(), out Version));
        if (gameProcess is null)
        {
            // Leave game unset and ensure Version is at its default value.
            Version = 0;
            return false;
        }

        // Set Game and do some event management.
        Game = gameProcess;
        Game.EnableRaisingEvents = true;
        Game.Exited += (_, _) => OnGameFound.Invoke(0);
        return true;
    }

    /// <summary>Converts IGT ticks to a double representing time elapsed in decimal seconds.</summary>
    public static double LevelTimeAsDouble(ulong ticks) => (double)ticks / IGTTicksPerSecond;
}