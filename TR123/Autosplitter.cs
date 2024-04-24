using System;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR123;

public class Autosplitter : IAutoSplitter, IDisposable
{
    /// <summary>Shorthand for accessing <see cref="GameData.CurrentActiveGame" />.</summary>
    private static Game CurrentActiveGame => GameData.CurrentActiveGame;

    internal readonly ComponentSettings Settings = new();

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter() => GameData.OnGameVersionChanged += Settings.SetGameVersion;

    private static bool _cinematicValueUpdatedLastFrame;
    private static bool _loadingScreenFadedIn;

    /// <summary>
    ///     Determines if IGT pauses when the game is quit or <see cref="GetGameTime" /> returns <see langword="null" />
    /// </summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> when IGT should be paused during the conditions, <see langword="false" /> otherwise</returns>
    public bool IsGameTimePaused(LiveSplitState state)
    {
        if (Settings.GameTimeMethod == GameTimeMethod.Igt)
            return true;

        // RTA w/o Loads should not tick if a "freeze-frame load" is occuring.
        // These can occur on slower drives after a level's gameplay has ended, when a cutscene is loading.
        if (_cinematicValueUpdatedLastFrame)
        {
            if (!GameData.GlobalFrameIndex.Changed)
                return true;
            _cinematicValueUpdatedLastFrame = false;
        }
        else
        {
            _cinematicValueUpdatedLastFrame = GameData.Cinematic.Changed;
        }

        // RTA w/o Loads should tick at the title screen once a run has started.
        var title = GameData.TitleLoaded;
        if (title.Current)
            return false;

        // RTA w/o Loads should tick whenever a loading screen is not active.
        var loadFade = GameData.LoadFade;
        if (loadFade.Current <= 0)
        {
            _loadingScreenFadedIn = false;
            return false;
        }

        // A loading screen is active; check if loadFade is decreasing.
        const int loadFadeFullAmount = 255;
        if (!_loadingScreenFadedIn)
        {
            _loadingScreenFadedIn = loadFade.Current == loadFadeFullAmount;
            return true;
        }

        // Decreasing => loading screen is fading out, and the level has started.
        bool fadeDecreasing = loadFade.Current < loadFadeFullAmount;
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
        bool currentLevelWasAlreadyCompleted = RunStats.LevelHasBeenSplit(CurrentActiveGame, currentLevel);
        if (currentLevelWasAlreadyCompleted && levelCompleteStillActive)
            return null;

        // Sum the current and completed levels' IGT.
        uint currentLevelTicks = levelIgt.Current;
        ulong otherLevelTicks = RunStats.GetCompletedLevelIgtTicks(CurrentActiveGame, currentLevel);
        double totalTicks = GameData.LevelTimeAsDouble(otherLevelTicks + currentLevelTicks);
        return TimeSpan.FromSeconds(totalTicks);
    }

    /// <summary>Determines if the timer should split.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should split, <see langword="false" /> otherwise</returns>
    public bool ShouldSplit(LiveSplitState state)
    {
        // Handle Lara's Home special case.
        uint oldLevel = GameData.OldLevel();
        if (oldLevel == 0 && !GameData.TitleLoaded.Old) // Title Screen disambiguation
        {
            // The runner must be using the passport for this special case.
            if (!GameData.PassportWasChosen(GameData.InventoryChosen.Current))
                return false;

            // Prevent re-splits in this special case.
            bool homeAlreadySplit = RunStats.LevelHasBeenSplit(CurrentActiveGame, oldLevel);
            if (homeAlreadySplit)
                return false;

            // Split if OverlayFlag has changed to represent the title (Exit to Title), an FMV (New Game), or a loading screen (Load Game).
            bool overlayFlagChangedFromInventory =
                GameData.OverlayFlag.Old == OverlayFlag.Inventory && GameData.OverlayFlag.Current == OverlayFlag.Other;
            return overlayFlagChangedFromInventory;
        }

        // Prevent title screen splits.
        if (GameData.TitleLoaded.Current)
            return false;

        // Prevent re-splits.
        uint currentLevel = GameData.CurrentLevel();
        bool levelAlreadySplit = RunStats.LevelHasBeenSplit(CurrentActiveGame, currentLevel);
        if (levelAlreadySplit)
            return false;

        // Handle deathruns.
        if (Settings.Deathrun)
        {
            var health = GameData.Health;
            bool laraJustDied = health.Old > 0 && health.Current <= 0;
            return laraJustDied;
        }

        // Handle any level.
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
        bool passportWasChosen = GameData.PassportWasChosen(GameData.InventoryChosen.Old);
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
        // Clear tracked progress.
        RunStats.Clear();
        _loadingScreenFadedIn = true; // For a level to have started, a loading screen must have been active.

        // Ensure LiveSplit's GameTime initializes, matching Real Time if it has already increased.
        if (!state.IsGameTimeInitialized)
            state.SetGameTime(state.CurrentTime.RealTime ?? TimeSpan.Zero);
        state.IsGameTimePaused = false;
    }

    /// <summary>On <see cref="LiveSplitState.OnSplit" />, updates values.</summary>
    /// <param name="completedLevel">Level to add to <see cref="RunStats" /></param>
    public void OnSplit(uint completedLevel)
    {
        // Store stats for tracking and to prevent re-splits.
        var stats = new LevelStats
        {
            LevelNumber = completedLevel,
            Igt = GameData.LevelIgt.Current,
        };

        var activeGame = CurrentActiveGame;
        RunStats.AddLevelStats(activeGame, stats);
    }

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit" />, updates values.</summary>
    public void OnUndoSplit() => RunStats.UndoLevelStats();

    /// <inheritdoc />
    public void Dispose()
    {
        GameData.OnGameVersionChanged -= Settings.SetGameVersion;
        Settings?.Dispose();
    }
}