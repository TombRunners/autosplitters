using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using LiveSplit.ComponentUtil;
using Util;

namespace TR123;

public static class GameData
{
    private static readonly VersionDetector VersionDetector =
        new(
            ["tomb123"],
            new Dictionary<string, uint>
            {
                { "0C0C1C466DAE013ABBB976F11B52C726".ToLowerInvariant(), (uint)GameVersion.EgsDebug },
                { "0A937857C0AF755AEEAA98F4520CA0D2".ToLowerInvariant(), (uint)GameVersion.PublicV10 },
                { "769B1016F945167C48C6837505E37748".ToLowerInvariant(), (uint)GameVersion.PublicV101 },
                { "5B1644AFFD7BAD65B2AC5D76F15139C6".ToLowerInvariant(), (uint)GameVersion.PublicV101Patch1 },
                { "224D11BEBEC79A0B579C0001C66E64CF".ToLowerInvariant(), (uint)GameVersion.PublicV101Patch2 },
                { "02D456CC7FEAAC61819BE9A05228D2B3".ToLowerInvariant(), (uint)GameVersion.PublicV101Patch3 },
                { "1930B6B2167805C890B293FEB0B640B3".ToLowerInvariant(), (uint)GameVersion.PublicV101Patch4 },
            }
        );

    private static readonly GameMemory GameMemory = new ();

    /// <summary>Tests if the passes <paramref name="inventoryChosen" /> value matches a game's passport.</summary>
    /// <param name="inventoryChosen">Value of chosen inventory item</param>
    /// <returns><see langword="true" /> if a game's passport was chosen, <see langword="false" /> otherwise</returns>
    public static bool PassportWasChosen(short inventoryChosen)
    {
        const short tr1PassportChosen = 71, tr2PassportChosen = 120, tr3PassportChosen = 145;
        bool passportWasChosen = inventoryChosen is tr1PassportChosen or tr2PassportChosen or tr3PassportChosen;
        return passportWasChosen;
    }

    /// <summary>Reads the current active game or expansion, accounting for NG+ variations for base games.</summary>
    public static Game CurrentActiveGame
    {
        get
        {
            uint currentLevel = CurrentLevel;
            Game baseGame = CurrentActiveBaseGame;
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
    public static Game CurrentActiveBaseGame => (Game)(GameMemory.ActiveGame.Current * 3);

    #region EXE Watcher Accessors

    /// <inheritdoc cref="GameMemory.GFrameIndex" />
    public static MemoryWatcher<int> GlobalFrameIndex => GameMemory.GFrameIndex;

    #endregion

    #region DLL Watcher Accessors

    /// <inheritdoc cref="GameMemory.CineWatchers"/>
    public static MemoryWatcher<short> Cinematic => GameMemory.CineWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.HealthWatchers" />
    public static MemoryWatcher<short> Health => GameMemory.HealthWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.InventoryChosenWatchers" />
    public static MemoryWatcher<short> InventoryChosen => GameMemory.InventoryChosenWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.InventoryModeWatchers" />
    public static MemoryWatcher<InventoryMode> InventoryMode => GameMemory.InventoryModeWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.LevelCompleteWatchers" />
    public static MemoryWatcher<bool> LevelComplete => GameMemory.LevelCompleteWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.LevelIgtWatchers" />
    public static MemoryWatcher<uint> LevelIgt => GameMemory.LevelIgtWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.LoadFadeWatchers" />
    public static MemoryWatcher<uint> LoadFade => GameMemory.LoadFadeWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.OverlayFlagWatchers" />
    public static MemoryWatcher<OverlayFlag> OverlayFlag => GameMemory.OverlayFlagWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.TitleLoadedWatchers" />
    public static MemoryWatcher<bool> TitleLoaded => GameMemory.TitleLoadedWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.BonusFlagWatchers" />
    private static MemoryWatcher<bool> BonusFlag => GameMemory.BonusFlagWatchers[CurrentActiveBaseGame];

    /// <inheritdoc cref="GameMemory.LevelWatchers" />
    private static MemoryWatcher<byte> Level => GameMemory.LevelWatchers[CurrentActiveBaseGame];

    #endregion

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the current level.</summary>
    /// <returns>Current level of the game</returns>
    public static uint CurrentLevel => RealGameLevel(true);

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the old level.</summary>
    /// <returns>Old level of the game</returns>
    public static uint OldLevel => RealGameLevel(false);

    /// <summary>Accounts for special cases of Level readings to match the expected values at those points.</summary>
    /// <param name="current">Whether to use Level.Current; false means to use Level.Old.</param>
    /// <returns>Real game level, anomalies normalized.</returns>
    private static uint RealGameLevel(bool current)
    {
        byte level = current ? Level.Current : Level.Old;
        if (CurrentActiveBaseGame is not Game.Tr1)
            return level;

        // Level value is based on save-game info and only updates after a level is completed; it also doesn't reflect cutscenes.
        var levelCutsceneValue = (Tr1Level)GameMemory.Tr1LevelCutscene.Current;

        // Save-game Level will never purposely show Lara's Home or Title screen values.
        // The anomaly is before any level is completed after game launch, it stays 0 and matches Lara's Home, which is unreliable.
        bool levelCutsceneIsLarasHome = levelCutsceneValue is Tr1Level.LarasHome;
        bool levelCutsceneIsTitleScreen = levelCutsceneValue is Tr1Level.Title;
        if (levelCutsceneIsLarasHome || levelCutsceneIsTitleScreen)
            return (uint)levelCutsceneValue;

        bool levelCutsceneIsAfterStatsScreen = levelCutsceneValue is Tr1Level.AfterNatlasMines;
        if (levelCutsceneIsAfterStatsScreen) // A cutscene after a stats screen increments the value to the level which hasn't started yet.
            return level - 1U;

        // First level check is necessary because Level value is based on save-game info, so it is not updated until the first level ends.
        // Otherwise, we use the Level value because we don't want to use cutscene values.
        bool levelCutsceneIsFirstLevel = levelCutsceneValue is Tr1Level.Caves or Tr1Level.AtlanteanStronghold;
        return levelCutsceneIsFirstLevel ? (uint)levelCutsceneValue : level;
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
    internal static Process GameProcess;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    internal static uint CurrentGameVersion;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    public delegate void GameVersionChangedDelegate(VersionDetectionResult result);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public static GameVersionChangedDelegate OnGameVersionChanged;

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

                GameMemory.InitializeMemoryWatchers(CurrentGameVersion, GameProcess);
                return GameIsInitialized;
            }

            GameMemory.Watchers.UpdateAll(GameProcess);
            return GameIsInitialized;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>If applicable, finds a <see cref="Process" /> running an expected version of the game.</summary>
    /// <returns>
    ///     <see langword="true" /> if <see cref="GameProcess" /> and <see cref="CurrentGameVersion" /> were meaningfully set,
    ///     <see langword="false" /> otherwise
    /// </returns>
    private static bool FindSupportedGame()
    {
        uint previousVersion = CurrentGameVersion;

        VersionDetectionResult result = VersionDetector.DetectVersion();
        switch (result)
        {
            case VersionDetectionResult.Found found:
                CurrentGameVersion = found.Version;
                SetGameProcess(found.Process);
                break;

            case VersionDetectionResult.Unknown unknown:
                CurrentGameVersion = VersionDetector.Unknown;
                SetGameProcess(unknown.Process);
                break;

            case VersionDetectionResult.None:
                CurrentGameVersion = VersionDetector.None;
                GameProcess = null;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }

        if (previousVersion != CurrentGameVersion) // This protects against spamming the event in the repeated None case.
            OnGameVersionChanged.Invoke(result);

        return result is VersionDetectionResult.Found;
    }

    /// <summary>Sets <see cref="GameProcess" /> and performs additional work to ensure the process's termination is handled.</summary>
    /// <param name="gameProcess">Game process</param>
    private static void SetGameProcess(Process gameProcess)
    {
        GameProcess = gameProcess;
        GameProcess.EnableRaisingEvents = true;
        GameProcess.Exited += static (_, _) => OnGameVersionChanged.Invoke(new VersionDetectionResult.None());
    }

    /// <summary>Used to calculate <see cref="TimeSpan" />s from IGT ticks.</summary>
    private const int IgtTicksPerSecond = 30;

    /// <summary>Converts IGT ticks to a double representing time elapsed in decimal seconds.</summary>
    public static double LevelTimeAsDouble(ulong ticks) => (double)ticks / IgtTicksPerSecond;

    /// <summary>Sums completed levels' times.</summary>
    /// <returns>The sum of completed levels' times</returns>
    public static ulong SumCompletedLevelTimesInMemory(IEnumerable<uint> completedLevels, uint currentLevel)
    {
        Game activeBaseGame = CurrentActiveBaseGame;
        string activeModuleName = GameMemory.GameModules[activeBaseGame];
        ProcessModuleWow64Safe activeModule = GameProcess.ModulesWow64Safe().FirstOrDefault(module => module.ModuleName == activeModuleName);
        if (activeModule is null)
            return 0;

        uint levelSaveStructSize = GameSaveStructSizes[activeBaseGame];

        int firstLevelTimeAddress = GameMemory.GameVersionAddresses[(GameVersion)CurrentGameVersion][activeBaseGame].FirstLevelTime;
        IntPtr moduleBaseAddress = activeModule.BaseAddress;
        uint finishedLevelsTicks = completedLevels
            .TakeWhile(completedLevel => completedLevel != currentLevel)
            .Select(completedLevel => (completedLevel - 1) * levelSaveStructSize)
            .Select(levelOffset => (IntPtr)((long)moduleBaseAddress + firstLevelTimeAddress + levelOffset))
            .Aggregate<IntPtr, uint>(0, static (ticks, levelAddress) => ticks + GameProcess.ReadValue<uint>(levelAddress));

        return finishedLevelsTicks;
    }
}