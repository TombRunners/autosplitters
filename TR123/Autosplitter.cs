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
    public double SumCompletedLevelTimes(uint? currentLevel)
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
        var gameLevelStats = AllGameStats.Single(static list => list.Key == CurrentActiveGame).Value;
        if (gameLevelStats.GameComplete)
            finishedLevelsTicks = gameLevelStats
                .LevelStats
                .Aggregate(finishedLevelsTicks, static (current, levelStat) => current + levelStat.Igt);
        else
            finishedLevelsTicks +=
                Data.SumCompletedLevelTimesInMemory(gameLevelStats.LevelStats.Select(static stats => stats.LevelNumber), currentLevel);

        return GameData.LevelTimeAsDouble(finishedLevelsTicks);
    }

    /// <summary>Shorthand for accessing <see cref="GameData.CurrentActiveGame" />.</summary>
    private static Game CurrentActiveGame => GameData.CurrentActiveGame;

    internal ComponentSettings Settings = new();

    public GameData Data = new();

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter() => Data.OnGameFound += Settings.SetGameVersion;

    /// <summary>
    ///     Determines if IGT pauses when the game is quit or <see cref="GetGameTime" /> returns <see langword="null" />
    /// </summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> when IGT should be paused during the conditions, <see langword="false" /> otherwise</returns>
    public bool IsGameTimePaused(LiveSplitState state) => true;

    /// <summary>Determines the IGT.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns>IGT as a <see cref="TimeSpan" /> if available, otherwise <see langword="null" /></returns>
    public TimeSpan? GetGameTime(LiveSplitState state)
    {
        // Check that the title screen is not active.
        var title = GameData.TitleLoaded;
        if (title.Current)
            return null;

        // Check that IGT is ticking.
        var levelIgt = GameData.LevelIgt;
        if (!levelIgt.Changed)
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

        var titleLoaded = GameData.TitleLoaded;
        bool justEnteredTitleScreen = !titleLoaded.Old && titleLoaded.Current;
        if (!justEnteredTitleScreen)
            return false;

        // Checking LevelComplete is inaccurate; depending on exactly when LiveSplit polls, the Old and Current values may differ.
        // Instead, perform a simple check to see if the runner was likely to have come from a credits / last level.
        uint level = GameData.CurrentLevel();
        if (CurrentActiveGame > Game.Tr1UnfinishedBusiness)
            return !IsLastLevel(level);

        // TR1 logic.
        bool statsLevelNumberWasOnCreditsValue = level == 1;
        var levelCutscene = GameData.Tr1LevelCutscene;
        bool wasPossiblyOnLastLevel = IsLastLevel(levelCutscene.Old);
        return !wasPossiblyOnLastLevel || !statsLevelNumberWasOnCreditsValue;
    }

    /// <summary>Determines if the timer should start.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should start, <see langword="false" /> otherwise</returns>
    public bool ShouldStart(LiveSplitState state)
    {
        uint oldLevelTime = GameData.LevelIgt.Old;
        uint currentLevelTime = GameData.LevelIgt.Current;

        // Perform new game logic first, since it is the only place where FG should start.
        uint currentLevel = GameData.CurrentLevel();
        bool levelTimeJustStarted = oldLevelTime > 0 && currentLevelTime == 0;
        bool newGameStarted = levelTimeJustStarted && IsFirstLevel(currentLevel);
        if (newGameStarted)
            return true;

        return !Settings.FullGame && levelTimeJustStarted;
    }

    /// <summary>Determines if <paramref name="level" /> is the first of the game or expansion.</summary>
    /// <param name="level">Level to check</param>
    /// <returns><see langword="true" /> if <paramref name="level" /> is the first; <see langword="false" /> otherwise.</returns>
    public bool IsFirstLevel(uint level) =>
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

    public bool IsLastLevel(uint level) =>
        CurrentActiveGame switch
        {
            Game.Tr1 or Game.Tr1NgPlus => (Tr1Level)level is Tr1Level.TheGreatPyramid,
            Game.Tr1UnfinishedBusiness => (Tr1Level)level is Tr1Level.TempleOfTheCat,
            Game.Tr2 or Game.Tr2NgPlus => (Tr2Level)level is Tr2Level.HomeSweetHome,
            Game.Tr2GoldenMask => (Tr2Level)level is Tr2Level.Kingdom or Tr2Level.NightmareInVegas,
            Game.Tr3 or Game.Tr3NgPlus => (Tr3Level)level is Tr3Level.MeteoriteCavern or Tr3Level.AllHallows,
            Game.Tr3TheLostArtifact => (Tr3Level)level is Tr3Level.Reunion,
            _ => throw new ArgumentOutOfRangeException(nameof(CurrentActiveGame), CurrentActiveGame, "Unknown Game"),
        };

    /// <summary>On <see cref="LiveSplitState.OnStart" />, updates values.</summary>
    public void OnStart()
    {
        foreach (var gameStats in AllGameStats.Values)
            gameStats.Clear();
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
        Data.OnGameFound -= Settings.SetGameVersion;
        Data = null;
        Settings?.Dispose();
    }
}