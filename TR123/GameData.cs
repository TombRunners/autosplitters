using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using LiveSplit.ComponentUtil;

namespace TR123;

public enum GameVersion
{
    InitialPublicRelease = 1,
}

public enum Game
{
    Tr1 = 0,
    Tr2 = 1,
    Tr3 = 2,
}

public struct GameAddresses
{
    public int FirstLevelTimeAddress { get; set; }
    public int Health { get; set; }
    public int Level { get; set; }
    public int LevelComplete { get; set; }
    public int LevelIgt { get; set; }
}

public class GameData
{
    /// <summary>Used to calculate <see cref="TimeSpan" />s from IGT ticks.</summary>
    public const int IGTTicksPerSecond = 30;

    /// <summary>Quick accessor for ActiveGame.Current.</summary>
    public static Game CurrentActiveGame => (Game)ActiveGame.Current;

    /// <summary>Games included within the remastered EXE.</summary>
    public static readonly ImmutableList<Game> Games = [Game.Tr1, Game.Tr2, Game.Tr3];

    /// <summary>Strings used when searching for a running game <see cref="Process" />.</summary>
    public static readonly ImmutableList<string> ProcessSearchNames = ["tomb123"];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>The uint will be converted from <see cref="GameVersion" />.</remarks>
    public static readonly ImmutableDictionary<string, uint> VersionHashes = new Dictionary<string, uint>
    {
        { "769B1016F945167C48C6837505E37748".ToLowerInvariant(), (uint)GameVersion.InitialPublicRelease },
    }.ToImmutableDictionary();

    /// <summary>Contains the names of the modules (DLLs) for each <see cref="Game" />.</summary>
    public static readonly ImmutableDictionary<Game, string> GameModules = new Dictionary<Game, string>(3)
    {
        { Game.Tr1, "tomb1.dll" },
        { Game.Tr2, "tomb2.dll" },
        { Game.Tr3, "tomb3.dll" },
    }.ToImmutableDictionary();

    /// <summary>For each released remastered game version, contains each game's address offsets.</summary>
    public static readonly ImmutableDictionary<GameVersion, Dictionary<Game, GameAddresses>> GameVersionAddresses = new Dictionary<GameVersion, Dictionary<Game, GameAddresses>>
    {
        {
            GameVersion.InitialPublicRelease,
            new Dictionary<Game, GameAddresses>
            {
                {
                    Game.Tr1,
                    new GameAddresses
                    {
                        FirstLevelTimeAddress = 0x371E610,
                        Health = 0xEFA18,
                        Level = 0x371EBE8,
                        LevelComplete = 0xEF340,
                        LevelIgt = 0x371EBD0,
                    }
                },
                {
                    Game.Tr2,
                    new GameAddresses
                    {
                        FirstLevelTimeAddress = 0x3753850,
                        Health = 0x122780,
                        Level = 0x1247C8,
                        LevelComplete = 0x124CE4,
                        LevelIgt = 0x3753E0C,
                    }
                },
                {
                    Game.Tr3,
                    new GameAddresses
                    {
                        FirstLevelTimeAddress = 0x37B7164,
                        Health = 0x17EC28,
                        Level = 0x180ECC,
                        LevelComplete = 0x1813AC,
                        LevelIgt = 0x37B7948,
                    }
                },
            }
        },
    }.ToImmutableDictionary();

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
    public static readonly MemoryWatcherList Watchers = [];

    #region MemoryWatcherList Items

    /// <summary>Gives the value of the active game, where TR1 is 0, TR2 is 1, TR3 is 2.</summary>
    /// <remarks>The uint should be converted to <see cref="GameVersion" />.</remarks>
    public static MemoryWatcher<uint> ActiveGame => (MemoryWatcher<uint>)Watchers?["ActiveGame"];

    /// <summary>Gives the value of the active level.</summary>
    /// <remarks>
    ///     Usually matches chronological number. Some exceptions are TR3 due to level order choice and TR1's Unfinished
    ///     Business.
    /// </remarks>
    public static ImmutableDictionary<Game, MemoryWatcher<byte>> Level => new Dictionary<Game, MemoryWatcher<byte>>(3)
    {
        { Game.Tr1, (MemoryWatcher<byte>)Watchers?["Tr1Level"] },
        { Game.Tr2, (MemoryWatcher<byte>)Watchers?["Tr2Level"] },
        { Game.Tr3, (MemoryWatcher<byte>)Watchers?["Tr3Level"] },
    }.ToImmutableDictionary();

    /// <summary>Indicates if the current level is finished.</summary>
    /// <remarks>
    ///     1 while an end-level stats screen is active.
    ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
    ///     Before most end-level in-game cutscenes, the value changes from 0 to 1 then back to 0 immediately.
    ///     Otherwise, the value is 0.
    /// </remarks>
    public static ImmutableDictionary<Game, MemoryWatcher<bool>> LevelComplete => new Dictionary<Game, MemoryWatcher<bool>>(3)
    {
        { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1LevelComplete"] },
        { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2LevelComplete"] },
        { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3LevelComplete"] },
    }.ToImmutableDictionary();

    /// <summary>Gives the running IGT of the current level.</summary>
    public static ImmutableDictionary<Game, MemoryWatcher<uint>> LevelIgt => new Dictionary<Game, MemoryWatcher<uint>>(3)
    {
        { Game.Tr1, (MemoryWatcher<uint>)Watchers?["Tr1LevelIgt"] },
        { Game.Tr2, (MemoryWatcher<uint>)Watchers?["Tr2LevelIgt"] },
        { Game.Tr3, (MemoryWatcher<uint>)Watchers?["Tr3LevelIgt"] },
    }.ToImmutableDictionary();

    /// <summary>Lara's current HP.</summary>
    /// <remarks>Max HP is 1000. When less than or equal to 0, Lara dies.</remarks>
    public static ImmutableDictionary<Game, MemoryWatcher<short>> Health => new Dictionary<Game, MemoryWatcher<short>>(3)
    {
        { Game.Tr1, (MemoryWatcher<short>)Watchers?["Tr1Health"] },
        { Game.Tr2, (MemoryWatcher<short>)Watchers?["Tr2Health"] },
        { Game.Tr3, (MemoryWatcher<short>)Watchers?["Tr3Health"] },
    }.ToImmutableDictionary();

    public static MemoryWatcher<uint> Tr1LevelCutscene => (MemoryWatcher<uint>)Watchers?["Tr1LevelCutscene"];

    #endregion

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    public static Process GameProcess;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    public static uint Version;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The version found; the uint will be converted to <see cref="GameVersion" />.</param>
    public delegate void GameFoundDelegate(uint version);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public GameFoundDelegate OnGameFound;

    /// <summary>Sets addresses based on <paramref name="version" />.</summary>
    /// <param name="version">Version to base addresses on; the uint will be converted to <see cref="GameVersion" />.</param>
    private static void SetAddresses(uint version)
    {
        Watchers.Clear();

        switch ((GameVersion)version)
        {
            case GameVersion.InitialPublicRelease:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD6B68)) { Name = "ActiveGame" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xD4A54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddWatchersForAllGames(GameVersion.InitialPublicRelease);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }

    /// <summary>Adds MemoryWatchers which are common across all games.</summary>
    /// <param name="version">Game version</param>
    private static void AddWatchersForAllGames(GameVersion version)
    {
        foreach (var game in Games)
        {
            string moduleName = GameModules[game];
            var addresses = GameVersionAddresses[version][game];

            int healthOffset = addresses.Health;
            Watchers.Add(new MemoryWatcher<short>(new DeepPointer(moduleName, healthOffset)) { Name = $"{game}Health" });

            int levelOffset = addresses.Level;
            Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(moduleName, levelOffset)) { Name = $"{game}Level" });

            int levelCompleteOffset = addresses.LevelComplete;
            Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, levelCompleteOffset)) { Name = $"{game}LevelComplete" });

            int levelIgtOffset = addresses.LevelIgt;
            Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(moduleName, levelIgtOffset)) { Name = $"{game}LevelIgt" });
        }
    }

    /// <summary>Updates <see cref="GameData" /> implementation and its addresses' values.</summary>
    /// <returns><see langword="true" /> if game data was updated, <see langword="false" /> otherwise</returns>
    public bool Update()
    {
        try
        {
            if (GameProcess is null || GameProcess.HasExited)
            {
                if (!SetGameProcessAndVersion())
                    return false;

                SetAddresses(Version);
                OnGameFound.Invoke(Version);
                return true;
            }

            Watchers.UpdateAll(GameProcess);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>If applicable, finds a <see cref="Process" /> running an expected version of the game.</summary>
    /// <returns>
    ///     <see langword="true" /> if <see cref="GameProcess" /> and <see cref="Version" /> were meaningfully set,
    ///     <see langword="false" /> otherwise
    /// </returns>
    private bool SetGameProcessAndVersion()
    {
        // Find game Process, if any, and set Version member accordingly.
        var processes = ProcessSearchNames.SelectMany(Process.GetProcessesByName);
        var gameProcess = processes.First(static p => VersionHashes.TryGetValue(p.GetMd5Hash(), out Version));
        if (gameProcess is null)
        {
            // Leave game unset and ensure Version is at its default value.
            Version = 0;
            return false;
        }

        // Set GameProcess and do some event management.
        GameProcess = gameProcess;
        GameProcess.EnableRaisingEvents = true;
        GameProcess.Exited += (_, _) => OnGameFound.Invoke(0);
        return true;
    }

    /// <summary>Converts IGT ticks to a double representing time elapsed in decimal seconds.</summary>
    public static double LevelTimeAsDouble(ulong ticks) => (double)ticks / IGTTicksPerSecond;

    /// <summary>Sums completed levels' times.</summary>
    /// <returns>The sum of completed levels' times</returns>
    public double SumCompletedLevelTimes(IEnumerable<uint> completedLevels, uint? currentLevel)
    {
        var activeGame = (Game)ActiveGame.Current;
        int firstLevelTimeAddress = GameVersionAddresses[(GameVersion)Version][activeGame].FirstLevelTimeAddress;

        const int tr3SaveStructSize = 0x40;
        const int tr1AndTr2SaveStructSize = 0x30;
        int levelSaveStructSize = activeGame == Game.Tr3 ? tr3SaveStructSize : tr1AndTr2SaveStructSize;
        uint finishedLevelsTicks = completedLevels
            .TakeWhile(completedLevel => completedLevel != currentLevel)
            .Select(completedLevel => (completedLevel - 1) * levelSaveStructSize)
            .Select(levelOffset => (IntPtr)(firstLevelTimeAddress + levelOffset))
            .Aggregate<IntPtr, uint>(0, static (ticks, levelAddress) => ticks + GameProcess.ReadValue<uint>(levelAddress));

        return LevelTimeAsDouble(finishedLevelsTicks);
    }
}