using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using LiveSplit.ComponentUtil;

namespace TR123;

public static partial class GameData
{
    private static partial class GameMemory;

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
                    throw new ArgumentOutOfRangeException(nameof(GameMemory.ActiveGame.Current), "Unknown Game value read from CurrentActiveBaseGame.");
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
                _ => throw new ArgumentOutOfRangeException(nameof(GameMemory.ActiveGame.Current), "Unknown Game value read from CurrentActiveBaseGame."),
            };
        }
    }

    /// <summary>Identifies the game without NG+ identification.</summary>
    private static Game CurrentActiveBaseGame => (Game)(GameMemory.ActiveGame.Current * 3);

    public static MemoryWatcher<short> Health => GameMemory.HealthWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<short> InventoryChosen => GameMemory.InventoryChosenWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<InventoryMode> InventoryMode => GameMemory.InventoryModeWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<bool> LevelComplete => GameMemory.LevelCompleteWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<uint> LevelIgt => GameMemory.LevelIgtWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<uint> LoadFade => GameMemory.LoadFadeWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<bool> TitleLoaded => GameMemory.TitleLoadedWatchers[CurrentActiveBaseGame];

    private static MemoryWatcher<bool> BonusFlag => GameMemory.BonusFlagWatchers[CurrentActiveBaseGame];
    private static MemoryWatcher<byte> Level => GameMemory.LevelWatchers[CurrentActiveBaseGame];

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the current level.</summary>
    /// <returns>Correct current level of the game</returns>
    public static uint CurrentLevel()
    {
        if (CurrentActiveBaseGame is not Game.Tr1)
            return Level.Current;

        uint levelCutsceneValue = GameMemory.Tr1LevelCutscene.Current;
        bool levelCutsceneIsAfterStatsScreen = (Tr1Level)levelCutsceneValue is Tr1Level.AfterNatlasMines;
        if (levelCutsceneIsAfterStatsScreen) // A cutscene after a stats screen increments the value to the level which hasn't started yet.
            return Level.Current - 1U;

        // First level check is necessary because Level value is based on save-game info, so it is not updated until the first level ends.
        // Otherwise, we use the Level value because we don't want to use cutscene values.
        bool levelCutsceneIsFirstLevel = (Tr1Level)levelCutsceneValue is Tr1Level.Caves or Tr1Level.AtlanteanStronghold;
        return levelCutsceneIsFirstLevel ? levelCutsceneValue : Level.Current;
    }

    /// <summary>Test that the game has fully initialized based on expected memory readings.</summary>
    private static bool GameIsInitialized => GameMemory.ActiveGame.Old is >= 0 and <= 2;

    /// <summary>Contains the sizes of the save game structs for each base <see cref="Game" />.</summary>
    private static readonly ImmutableDictionary<Game, uint> GameSaveStructSizes = new Dictionary<Game, uint>(3)
    {
        { Game.Tr1, 0x30 },
        { Game.Tr2, 0x30 },
        { Game.Tr3, 0x40 },
    }.ToImmutableDictionary();

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    private static Process _gameProcess;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    private static GameVersion _version;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The new <see cref="GameVersion" /></param>
    /// <param name="hash">MD5 hash of the game EXE</param>
    public delegate void GameVersionChangedDelegate(GameVersion version, string hash);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public static GameVersionChangedDelegate OnGameVersionChanged;

    /// <summary>Updates <see cref="GameData" /> implementation and its addresses' values.</summary>
    /// <returns><see langword="true" /> if game data was updated, <see langword="false" /> otherwise</returns>
    public static bool Update()
    {
        try
        {
            if (_gameProcess is null || _gameProcess.HasExited)
            {
                if (!FindSupportedGame())
                    return false;

                GameMemory.InitializeMemoryWatchers(_version);
                return GameIsInitialized;
            }

            GameMemory.Watchers.UpdateAll(_gameProcess);
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
    private static bool FindSupportedGame()
    {
        var detectedVersion = VersionDetector.DetectVersion(out var gameProcess, out string hash);
        if (_version != detectedVersion)
        {
            _version = detectedVersion;
            OnGameVersionChanged.Invoke(_version, hash);
        }

        if (gameProcess is null || _version is GameVersion.EgsDebug) // EGS Debug is not supported.
            return false;

        SetGameProcess(gameProcess);
        return true;
    }

    /// <summary>Sets <see cref="_gameProcess" /> and performs additional work to ensure the process's termination is handled.</summary>
    /// <param name="gameProcess">Game process</param>
    private static void SetGameProcess(Process gameProcess)
    {
        _gameProcess = gameProcess;
        _gameProcess.EnableRaisingEvents = true;
        _gameProcess.Exited += static (_, _) => OnGameVersionChanged.Invoke(GameVersion.None, string.Empty);
    }

    /// <summary>Converts IGT ticks to a double representing time elapsed in decimal seconds.</summary>
    public static double LevelTimeAsDouble(ulong ticks) => (double)ticks / IgtTicksPerSecond;

    /// <summary>Sums completed levels' times.</summary>
    /// <returns>The sum of completed levels' times</returns>
    public static ulong SumCompletedLevelTimesInMemory(IEnumerable<uint> completedLevels, uint? currentLevel)
    {
        var activeBaseGame = CurrentActiveBaseGame;
        string activeModuleName = GameMemory.GameModules[activeBaseGame];
        var activeModule = _gameProcess.ModulesWow64Safe().FirstOrDefault(module => module.ModuleName == activeModuleName);
        if (activeModule is null)
            return 0;

        uint levelSaveStructSize = GameSaveStructSizes[activeBaseGame];

        int firstLevelTimeAddress = GameMemory.GameVersionAddresses[_version][activeBaseGame].FirstLevelTime;
        var moduleBaseAddress = activeModule.BaseAddress;
        uint finishedLevelsTicks = completedLevels
            .TakeWhile(completedLevel => completedLevel != currentLevel)
            .Select(completedLevel => (completedLevel - 1) * levelSaveStructSize)
            .Select(levelOffset => (IntPtr)((long)moduleBaseAddress + firstLevelTimeAddress + levelOffset))
            .Aggregate<IntPtr, uint>(0, static (ticks, levelAddress) => ticks + _gameProcess.ReadValue<uint>(levelAddress));

        return finishedLevelsTicks;
    }
}