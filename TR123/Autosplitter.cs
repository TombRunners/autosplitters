using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR123;

public class Autosplitter : IAutoSplitter, IDisposable
{
    /// <summary>Used to size CompletedLevels.</summary>
    private static readonly ImmutableDictionary<Game, int> LevelCount = new Dictionary<Game, int>(3)
    {
        { Game.Tr1, 19 },
        { Game.Tr2, 23 },
        { Game.Tr3, 26 },
    }.ToImmutableDictionary();

    /// <summary>Used to decide when to split and which level time addresses should be read from memory.</summary>
    private static readonly ImmutableDictionary<Game, List<uint>> CompletedLevels = new Dictionary<Game, List<uint>>(3)
    {
        { Game.Tr1, new List<uint>(LevelCount[Game.Tr1]) },
        { Game.Tr2, new List<uint>(LevelCount[Game.Tr2]) },
        { Game.Tr3, new List<uint>(LevelCount[Game.Tr3]) },
    }.ToImmutableDictionary();

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
        var activeGame = CurrentActiveGame;

        // Check that IGT is ticking.
        var levelIgt = GameData.LevelIgt[activeGame];
        uint currentLevelTicks = levelIgt.Current;
        uint oldLevelTicks = levelIgt.Old;
        if (currentLevelTicks - oldLevelTicks == 0)
            return null;

        // TR3's IGT ticks during globe level selection; the saved end-level IGT is unaffected, thus the overall FG IGT is also unaffected.
        // If a runner is watching LiveSplit's IGT, this may confuse them, despite it being a non-issue for the level/FG IGT.
        // To prevent the ticks from showing in LS, we use the fact that LevelComplete isn't reset to 0 until the next level is loaded.
        var level = GameData.Level[activeGame];
        uint currentLevel = activeGame == Game.Tr1 && GameData.Tr1LevelCutscene.Current == 1U ? 1 : (uint)level.Current;
        var levelComplete = GameData.LevelComplete[activeGame];
        bool oldLevelComplete = levelComplete.Old;
        bool currentLevelComplete = levelComplete.Current;
        bool stillOnCompletedLevel = oldLevelComplete && currentLevelComplete;
        if (CompletedLevels[activeGame].Contains(currentLevel) && stillOnCompletedLevel)
            return null;

        // Sum the current and completed levels' IGT.
        double currentLevelTime = GameData.LevelTimeAsDouble(currentLevelTicks);
        double finishedLevelsTime = Data.SumCompletedLevelTimes(CompletedLevels[activeGame], currentLevel);
        return TimeSpan.FromSeconds(currentLevelTime + finishedLevelsTime);
    }

    /// <summary>Determines if the timer should split.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should split, <see langword="false" /> otherwise</returns>
    public bool ShouldSplit(LiveSplitState state)
    {
        var activeGame = CurrentActiveGame;

        // Determine if the player is in a level we have not already split.
        var level = GameData.Level[activeGame];
        uint currentLevel = activeGame == Game.Tr1 && GameData.Tr1LevelCutscene.Current == 1U ? 1 : (uint)level.Current;
        bool onCorrectLevelToSplit = !CompletedLevels[activeGame].Contains(currentLevel);
        if (!onCorrectLevelToSplit)
            return false;

        // Deathrun
        var health = GameData.Health[activeGame];
        if (Settings.Deathrun)
        {
            bool laraJustDied = health.Old > 0 && health.Current <= 0;
            return laraJustDied;
        }

        // FG & IL/Section
        var levelComplete = GameData.LevelComplete[activeGame];
        bool levelJustCompleted = !levelComplete.Old && levelComplete.Current;
        return levelJustCompleted;
    }

    /// <summary>Determines if the timer should reset.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should reset, <see langword="false" /> otherwise</returns>
    public bool ShouldReset(LiveSplitState state)
    {
        if (Settings.DisableAutoReset)
            return false;

        // TODO: Actual reset logic here.
        return false;
    }

    /// <summary>Determines if the timer should start.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should start, <see langword="false" /> otherwise</returns>
    public bool ShouldStart(LiveSplitState state)
    {
        var activeGame = CurrentActiveGame;
        var level = GameData.Level[activeGame];
        var levelIgt = GameData.LevelIgt[activeGame];

        uint oldLevelTime = levelIgt.Old;
        uint currentLevelTime = levelIgt.Current;

        // Perform new game logic first, since it is the only place where FG should start.
        uint currentLevel = activeGame == Game.Tr1 && GameData.Tr1LevelCutscene.Current == 1U ? 1 : (uint)level.Current;
        bool levelTimeJustStarted = oldLevelTime > 0 && currentLevelTime == 0;
        bool newGameStarted = levelTimeJustStarted && IsFirstLevel(activeGame, currentLevel);
        if (newGameStarted)
            return true;

        return !Settings.FullGame && levelTimeJustStarted;
    }

    public bool IsFirstLevel(Game activeGame, uint level)
    {
        switch (activeGame)
        {
            case Game.Tr1:
                var tr1Level = (Tr1Level)level;
                return tr1Level is Tr1Level.Caves or Tr1Level.AtlanteanStronghold;

            case Game.Tr2:
                var tr2Level = (Tr2Level)level;
                return tr2Level is Tr2Level.GreatWall or Tr2Level.TheColdWar;

            case Game.Tr3:
                var tr3Level = (Tr3Level)level;
                return tr3Level is Tr3Level.Jungle or Tr3Level.HighlandFling;

            default:
                throw new ArgumentOutOfRangeException(nameof(activeGame), activeGame, "Unknown Game");
        }
    }

    /// <summary>On <see cref="LiveSplitState.OnStart" />, updates values.</summary>
    public void OnStart()
    {
        var activeGame = CurrentActiveGame;
        CompletedLevels[activeGame].Clear();
    }

    /// <summary>On <see cref="LiveSplitState.OnSplit" />, updates values.</summary>
    /// <param name="completedLevel">What to add to <see cref="CompletedLevels" /></param>
    public void OnSplit(uint completedLevel)
    {
        var activeGame = CurrentActiveGame;
        CompletedLevels[activeGame].Add(completedLevel);
    }

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit" />, updates values.</summary>
    public void OnUndoSplit()
    {
        var activeGame = CurrentActiveGame;
        CompletedLevels[activeGame].RemoveAt(CompletedLevels[activeGame].Count - 1);
    }

    public void Dispose()
    {
        Data.OnGameFound -= Settings.SetGameVersion;
        Data = null;
        Settings?.Dispose();
    }
}