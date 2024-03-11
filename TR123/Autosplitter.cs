using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR123;

public class Autosplitter : IAutoSplitter, IDisposable
{
    /// <summary>Used to size new entries to AllGameStats.</summary>
    private static readonly ImmutableDictionary<Game, int> LevelCount = new Dictionary<Game, int>(9)
    {
        { Game.Tr1,                   15 },
        { Game.Tr1NgPlus,             15 },
        { Game.Tr1UnfinishedBusiness, 04 },
        { Game.Tr2,                   18 },
        { Game.Tr2NgPlus,             18 },
        { Game.Tr2GoldenMask,         05 },
        { Game.Tr3,                   20 },
        { Game.Tr3NgPlus,             20 },
        { Game.Tr3TheLostArtifact,    06 },
    }.ToImmutableDictionary();

    /// <summary>Used to decide when to split and which level time addresses should be read from memory.</summary>
    private static readonly ImmutableDictionary<Game, GameStats> AllGameStats = new Dictionary<Game, GameStats>(9)
    {
        { Game.Tr1,                   new GameStats(LevelCount[Game.Tr1]) },
        { Game.Tr1NgPlus,             new GameStats(LevelCount[Game.Tr1NgPlus]) },
        { Game.Tr1UnfinishedBusiness, new GameStats(LevelCount[Game.Tr1UnfinishedBusiness]) },
        { Game.Tr2,                   new GameStats(LevelCount[Game.Tr2]) },
        { Game.Tr2NgPlus,             new GameStats(LevelCount[Game.Tr2NgPlus]) },
        { Game.Tr2GoldenMask,         new GameStats(LevelCount[Game.Tr2GoldenMask]) },
        { Game.Tr3,                   new GameStats(LevelCount[Game.Tr3]) },
        { Game.Tr3NgPlus,             new GameStats(LevelCount[Game.Tr3NgPlus]) },
        { Game.Tr3TheLostArtifact,    new GameStats(LevelCount[Game.Tr3TheLostArtifact]) },
    }.ToImmutableDictionary();

    /// <summary>Sums completed levels' times.</summary>
    /// <returns>The sum of completed levels' times</returns>
    private static double SumCompletedLevelTimes(uint? currentLevel)
    {
        // Sum IGT from other games' completed levels from splitter memory.
        ulong finishedLevelsTicks = AllGameStats
            .Where(static entry => entry.Key != CurrentActiveGame)
            .Select(static entry => entry.Value)
            .Aggregate<GameStats, ulong>(
                0, static (current1, gameStats) =>
                gameStats
                    .LevelStats
                    .Aggregate(current1, static (current, levelStats) => current + levelStats.Igt)
            );

        // Sum the current game's completed levels up to the current level from game memory if the game is not already complete.
        var gameLevelStats = AllGameStats[CurrentActiveGame];
        if (gameLevelStats.GameComplete)
            finishedLevelsTicks = gameLevelStats
                .LevelStats
                .Aggregate(finishedLevelsTicks, static (current, levelStat) => current + levelStat.Igt);
        else
            finishedLevelsTicks +=
                GameData.SumCompletedLevelTimesInMemory(gameLevelStats.LevelStats.Select(static stats => stats.LevelNumber), currentLevel);

        return GameData.LevelTimeAsDouble(finishedLevelsTicks);
    }

    /// <summary>Shorthand for accessing <see cref="GameData.CurrentActiveGame" />.</summary>
    private static Game CurrentActiveGame => GameData.CurrentActiveGame;

    internal readonly ComponentSettings Settings = new();

    public GameData Data = new();

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter()
    {
        Data.OnAslComponentChanged += Settings.SetAslWarningLabelVisibility;
        Data.OnGameFound += Settings.SetGameVersion;
        Data.OnGameFound += UpdateWatchers;
    }

    /// <summary>
    ///     This method should be called by GameData's OnGameFound to ensure that LiveSplit MemoryWatchers do not have
    ///     zeroed values on initialization, which ruins some of our logic.
    /// </summary>
    private void UpdateWatchers(GameVersion version, string _)
    {
        if (version is GameVersion.None or GameVersion.Unknown or GameVersion.EgsDebug)
            return;

        Data.Update();
    }

    /// <summary>
    ///     Determines if IGT pauses when the game is quit or <see cref="GetGameTime" /> returns <see langword="null" />
    /// </summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> when IGT should be paused during the conditions, <see langword="false" /> otherwise</returns>
    public bool IsGameTimePaused(LiveSplitState state)
    {
        if (Settings.GameTimeMethod == GameTimeMethod.Igt)
            return true;

        // RTA w/o Loads should tick at the title screen once a run has started.
        var title = GameData.TitleLoaded;
        if (title.Current)
            return false;

        // RTA w/o Loads should tick whenever a loading screen is not active.
        var loadFade = GameData.LoadFade;
        if (loadFade.Current <= 0)
            return false;

        // A loading screen is active; check if loadFade is decreasing.
        bool fadeDecreasing = loadFade.Old > loadFade.Current; // Decreasing => loading screen is fading out, level is starting.
        return !fadeDecreasing;
    }

    /// <summary>Determines the IGT.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns>IGT as a <see cref="TimeSpan" /> if available, otherwise <see langword="null" /></returns>
    public TimeSpan? GetGameTime(LiveSplitState state) =>
        Settings.GameTimeMethod switch
        {
            GameTimeMethod.Igt => IgtGameTime(Settings.Deathrun),
            GameTimeMethod.RtaNoLoads => null,
            _ => throw new ArgumentOutOfRangeException(nameof(Settings.GameTimeMethod), "Unknown GameTimeMethod"),
        };

    private static TimeSpan? IgtGameTime(bool deathrun)
    {
        // Stop IGT when a deathrun is complete.
        if (deathrun && GameData.Health.Current <= 0)
            return null;

        // Check that the title screen is not active.
        var title = GameData.TitleLoaded;
        if (title.Current)
            return null;

        // Check that IGT is ticking.
        var levelIgt = GameData.LevelIgt;
        bool increasing = levelIgt.Current > levelIgt.Old;
        if (!increasing)
            return null;

        // TR3's IGT ticks during globe level selection; the saved end-level IGT is unaffected, thus the overall FG IGT is also unaffected.
        // If a runner is watching LiveSplit's IGT, this may confuse them, despite it being a non-issue for the level/FG IGT.
        // To prevent the ticks from showing in LS, we use the fact that LevelComplete isn't reset to 0 until the next level is loaded.
        var levelComplete = GameData.LevelComplete;
        bool levelCompleteStillActive = levelComplete.Old && levelComplete.Current;
        uint currentLevel = GameData.CurrentLevel();
        bool currentLevelWasAlreadyCompleted = AllGameStats[CurrentActiveGame].LevelStats.Any(stats => stats.LevelNumber == currentLevel);
        if (currentLevelWasAlreadyCompleted && levelCompleteStillActive)
            return null;

        // Sum the current and completed levels' IGT.
        uint currentLevelTicks = levelIgt.Current;
        double currentLevelTime = GameData.LevelTimeAsDouble(currentLevelTicks);
        double finishedLevelsTime = SumCompletedLevelTimes(currentLevel);
        return TimeSpan.FromSeconds(currentLevelTime + finishedLevelsTime);
    }

    /// <summary>Determines if the timer should split.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should split, <see langword="false" /> otherwise</returns>
    public bool ShouldSplit(LiveSplitState state)
    {
        // Determine if the player is in a level we have not already split.
        uint currentLevel = GameData.CurrentLevel();
        bool onValidLevelToSplit = AllGameStats[CurrentActiveGame].LevelStats.All(stats => stats.LevelNumber != currentLevel);
        if (!onValidLevelToSplit)
            return false;

        // Deathrun
        if (Settings.Deathrun)
        {
            var health = GameData.Health;
            bool laraJustDied = health.Old > 0 && health.Current <= 0;
            return laraJustDied;
        }

        // FG & IL/Section
        var levelComplete = GameData.LevelComplete;
        bool levelJustCompleted = !levelComplete.Old && levelComplete.Current;
        return levelJustCompleted;
    }

    /// <summary>Determines if the timer should reset.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should reset, <see langword="false" /> otherwise</returns>
    public bool ShouldReset(LiveSplitState state)
    {
        if (!Settings.EnableAutoReset)
            return false;

        // Reset is only applied when the player uses the passport; this avoids resets after credits, for example.
        const short tr1PassportChosen = 71, tr2PassportChosen = 120, tr3PassportChosen = 145;
        bool passportWasChosen = GameData.InventoryChosen.Old is tr1PassportChosen or tr2PassportChosen or tr3PassportChosen;
        if (!passportWasChosen)
            return false;

        // Deathruns should reset in the death menu regardless of if "Load Game", "Restart Level", or "Exit to Title" is selected.
        if (Settings.Deathrun && GameData.InventoryMode.Current == InventoryMode.Death)
            return true;

        // Non-deathruns reset when the title is loaded; the player did not load into a level using the passport.
        var titleLoaded = GameData.TitleLoaded;
        bool justEnteredTitleScreen = !titleLoaded.Old && titleLoaded.Current;
        return justEnteredTitleScreen;
    }

    /// <summary>Determines if the timer should start.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should start, <see langword="false" /> otherwise</returns>
    public bool ShouldStart(LiveSplitState state)
    {
        // Do not start when on a title screen.
        var title = GameData.TitleLoaded;
        if (title.Current)
            return false;

        // Start when IGT ticks up from 0.
        var igt = GameData.LevelIgt;
        bool levelTimeJustStarted = igt.Old == 0 && igt.Current > 0;
        return Settings.FullGame
            ? levelTimeJustStarted && IsFirstLevel(GameData.CurrentLevel())
            : levelTimeJustStarted;
    }

    /// <summary>Determines if <paramref name="level" /> is the first of the game or expansion.</summary>
    /// <param name="level">Level to check</param>
    /// <returns><see langword="true" /> if <paramref name="level" /> is the first; <see langword="false" /> otherwise.</returns>
    private static bool IsFirstLevel(uint level) =>
        CurrentActiveGame switch
        {
            Game.Tr1 or Game.Tr1NgPlus => (Tr1Level)level is Tr1Level.Caves,
            Game.Tr1UnfinishedBusiness => (Tr1Level)level is Tr1Level.AtlanteanStronghold,
            Game.Tr2 or Game.Tr2NgPlus => (Tr2Level)level is Tr2Level.GreatWall,
            Game.Tr2GoldenMask         => (Tr2Level)level is Tr2Level.TheColdWar,
            Game.Tr3 or Game.Tr3NgPlus => (Tr3Level)level is Tr3Level.Jungle,
            Game.Tr3TheLostArtifact    => (Tr3Level)level is Tr3Level.HighlandFling,
            _ => throw new ArgumentOutOfRangeException(nameof(CurrentActiveGame), CurrentActiveGame, "Unknown Game"),
        };

    /// <summary>On <see cref="LiveSplitState.OnStart" />, updates values.</summary>
    public void OnStart(LiveSplitState state)
    {
        foreach (var gameStats in AllGameStats.Values)
            gameStats.Clear();

        if (!state.IsGameTimeInitialized)
            state.SetGameTime(new TimeSpan(0));
        state.IsGameTimePaused = false;
    }

    /// <summary>On <see cref="LiveSplitState.OnSplit" />, updates values.</summary>
    /// <param name="completedLevel">Level to add to <see cref="AllGameStats" /></param>
    public void OnSplit(uint completedLevel)
    {
        var stats = new LevelStats
        {
            LevelNumber = completedLevel,
            Igt = GameData.LevelIgt.Current,
        };

        var activeGame = CurrentActiveGame;
        AllGameStats[activeGame].AddLevelStats(stats);
    }

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit" />, updates values.</summary>
    public void OnUndoSplit()
    {
        var activeGame = CurrentActiveGame;
        AllGameStats[activeGame].RemoveLevelStats();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Data.OnAslComponentChanged -= Settings.SetAslWarningLabelVisibility;
        Data.OnGameFound -= Settings.SetGameVersion;
        Data.OnGameFound -= UpdateWatchers;
        Data = null;
        Settings?.Dispose();
    }
}