using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using LiveSplit.ComponentUtil;

namespace TR123;

public partial class GameData
{
    private static partial class Memory;

    /// <summary>Used to calculate <see cref="TimeSpan" />s from IGT ticks.</summary>
    private const int IgtTicksPerSecond = 30;

    /// <summary>Reads the current active game or expansion, accounting for NG+ variations for base games.</summary>
    public static Game CurrentActiveGame
    {
        get
        {
            uint currentLevel = CurrentLevel();
            var baseGame = CurrentActiveBaseGame;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (baseGame)
            {
                case Game.Tr1:
                    if ((Tr1Level)currentLevel >= Tr1Level.ReturnToEgypt)
                        return Game.Tr1UnfinishedBusiness;
                    break;

                case Game.Tr2:
                    if ((Tr2Level)currentLevel >= Tr2Level.TheColdWar)
                        return Game.Tr2GoldenMask;
                    break;

                case Game.Tr3:
                    if ((Tr3Level)currentLevel >= Tr3Level.HighlandFling)
                        return Game.Tr3TheLostArtifact;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Memory.ActiveGame.Current), "Unknown Game value read from CurrentActiveBaseGame.");
            }

            bool bonusFlag = BonusFlag.Current;
            if (!bonusFlag)
                return baseGame;

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return baseGame switch
            {
                Game.Tr1 => (Tr1Level)currentLevel < Tr1Level.ReturnToEgypt ? Game.Tr1NgPlus : Game.Tr1,
                Game.Tr2 => (Tr2Level)currentLevel < Tr2Level.TheColdWar ? Game.Tr2NgPlus : Game.Tr2,
                Game.Tr3 => (Tr3Level)currentLevel < Tr3Level.HighlandFling ? Game.Tr3NgPlus : Game.Tr3,
                _ => throw new ArgumentOutOfRangeException(nameof(Memory.ActiveGame.Current), "Unknown Game value read from CurrentActiveBaseGame."),
            };
        }
    }

    /// <summary>Identifies the game without NG+ identification.</summary>
    private static Game CurrentActiveBaseGame => (Game)(Memory.ActiveGame.Current * 3);

    public static MemoryWatcher<short> Health => Memory.HealthWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<short> InventoryChosen => Memory.InventoryChosenWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<InventoryMode> InventoryMode => Memory.InventoryModeWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<bool> LevelComplete => Memory.LevelCompleteWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<uint> LevelIgt => Memory.LevelIgtWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<uint> LoadFade => Memory.LoadFadeWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<bool> TitleLoaded => Memory.TitleLoadedWatchers[CurrentActiveBaseGame];

    private static MemoryWatcher<bool> BonusFlag => Memory.BonusFlagWatchers[CurrentActiveBaseGame];
    private static MemoryWatcher<byte> Level => Memory.LevelWatchers[CurrentActiveBaseGame];

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the current level.</summary>
    /// <returns>Correct current level of the game</returns>
    public static uint CurrentLevel()
    {
        if (CurrentActiveBaseGame is not Game.Tr1)
            return Level.Current;

        uint levelCutsceneValue = Memory.Tr1LevelCutscene.Current;
        bool levelCutsceneIsAfterStatsScreen = (Tr1Level)levelCutsceneValue is Tr1Level.AfterNatlasMines;
        if (levelCutsceneIsAfterStatsScreen) // A cutscene after a stats screen increments the value to the level which hasn't started yet.
            return Level.Current - 1U;

        // First level check is necessary because Level value is based on save-game info, so it is not updated until the first level ends.
        // Otherwise, we use the Level value because we don't want to use cutscene values.
        bool levelCutsceneIsFirstLevel = (Tr1Level)levelCutsceneValue is Tr1Level.Caves or Tr1Level.AtlanteanStronghold;
        return levelCutsceneIsFirstLevel ? levelCutsceneValue : Level.Current;
    }

    /// <summary>Contains the sizes of the save game structs for each base <see cref="Game" />.</summary>
    private static readonly ImmutableDictionary<Game, uint> GameSaveStructSizes = new Dictionary<Game, uint>(3)
    {
        { Game.Tr1, 0x30 },
        { Game.Tr2, 0x30 },
        { Game.Tr3, 0x40 },
    }.ToImmutableDictionary();

    /// <summary>Strings used when searching for a running game <see cref="Process" />.</summary>
    private static readonly ImmutableList<string> ProcessSearchNames = ["tomb123"];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>The uint will be converted from <see cref="GameVersion" />.</remarks>
    private static readonly ImmutableDictionary<string, GameVersion> VersionHashes = new Dictionary<string, GameVersion>
    {
        { "0C0C1C466DAE013ABBB976F11B52C726".ToLowerInvariant(), GameVersion.EgsDebug },
        { "0A937857C0AF755AEEAA98F4520CA0D2".ToLowerInvariant(), GameVersion.PublicV10 },
        { "769B1016F945167C48C6837505E37748".ToLowerInvariant(), GameVersion.PublicV101 },
        { "5B1644AFFD7BAD65B2AC5D76F15139C6".ToLowerInvariant(), GameVersion.PublicV102 },
    }.ToImmutableDictionary();

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    private static Process _gameProcess;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    private static GameVersion _version;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The version found; the uint will be converted to <see cref="GameVersion" />.</param>
    /// <param name="hash">The MD5 hash of the game process EXE.</param>
    public delegate void GameFoundDelegate(GameVersion version, string hash);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public GameFoundDelegate OnGameFound;

    /// <summary>Updates <see cref="GameData" /> implementation and its addresses' values.</summary>
    /// <returns><see langword="true" /> if game data was updated, <see langword="false" /> otherwise</returns>
    public bool Update()
    {
        try
        {
            if (_gameProcess is null || _gameProcess.HasExited)
                return TrySetGameProcessAndVersion() && GameIsInitialized;

            Memory.Watchers.UpdateAll(_gameProcess);
            return GameIsInitialized;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>If applicable, finds a <see cref="Process" /> running an expected version of the game.</summary>
    /// <returns>
    ///     <see langword="true" /> if <see cref="_gameProcess" /> and <see cref="_version" /> were meaningfully set,
    ///     <see langword="false" /> otherwise
    /// </returns>
    private bool TrySetGameProcessAndVersion()
    {
        // Find game Processes.
        var processes = ProcessSearchNames.SelectMany(Process.GetProcessesByName).ToList();
        if (processes.Count == 0)
        {
            if (_version != GameVersion.None)
                OnGameFound.Invoke(GameVersion.None, string.Empty);

            _version = GameVersion.None;
            return false;
        }

        // Try finding a match from known version hashes.
        var hash = string.Empty;
        var gameProcess = processes.FirstOrDefault(p =>
            {
                hash = p.GetMd5Hash();
                return VersionHashes.TryGetValue(hash, out _version);
            }
        );
        if (gameProcess is null)
        {
            if (_version != GameVersion.Unknown)
                OnGameFound.Invoke(GameVersion.Unknown, hash);

            _version = GameVersion.Unknown;
            return false;
        }

        if (_version is GameVersion.EgsDebug)
        {
            OnGameFound.Invoke(GameVersion.EgsDebug, hash);
            return false;
        }

        // Set GameProcess and Watcher addresses, and do some event management.
        _gameProcess = gameProcess;
        _gameProcess.EnableRaisingEvents = true;
        _gameProcess.Exited += (_, _) => OnGameFound.Invoke(GameVersion.None, string.Empty);
        Memory.InitializeMemoryWatchers(_version);
        OnGameFound.Invoke(_version, hash);
        return true;
    }

    private static bool GameIsInitialized => Memory.ActiveGame.Old is >= 0 and <= 2;

    /// <summary>Converts IGT ticks to a double representing time elapsed in decimal seconds.</summary>
    public static double LevelTimeAsDouble(ulong ticks) => (double)ticks / IgtTicksPerSecond;

    /// <summary>Sums completed levels' times.</summary>
    /// <returns>The sum of completed levels' times</returns>
    public static ulong SumCompletedLevelTimesInMemory(IEnumerable<uint> completedLevels, uint? currentLevel)
    {
        var activeBaseGame = CurrentActiveBaseGame;
        string activeModuleName = Memory.GameModules[activeBaseGame];
        var activeModule = _gameProcess.ModulesWow64Safe().FirstOrDefault(module => module.ModuleName == activeModuleName);
        if (activeModule is null)
            return 0;

        uint levelSaveStructSize = GameSaveStructSizes[activeBaseGame];

        int firstLevelTimeAddress = Memory.GameVersionAddresses[_version][activeBaseGame].FirstLevelTime;
        var moduleBaseAddress = activeModule.BaseAddress;
        uint finishedLevelsTicks = completedLevels
            .TakeWhile(completedLevel => completedLevel != currentLevel)
            .Select(completedLevel => (completedLevel - 1) * levelSaveStructSize)
            .Select(levelOffset => (IntPtr)((long)moduleBaseAddress + firstLevelTimeAddress + levelOffset))
            .Aggregate<IntPtr, uint>(0, static (ticks, levelAddress) => ticks + _gameProcess.ReadValue<uint>(levelAddress));

        return finishedLevelsTicks;
    }
}