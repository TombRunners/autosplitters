using System;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR456;

public class Autosplitter : IAutoSplitter, IDisposable
{
    internal readonly ComponentSettings Settings = new();

    private bool _tr6NewGameStartedFromMenu;

    private string _latestSplitId;

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter()
    {
        GameData.OnGameVersionChanged += Settings.SetGameVersion;
        GameData.OnSignatureScanStatusChanged += Settings.SetSignatureScanStatusLabelVisibility;
    }

    /// <summary>
    ///     Determines if LiveSplit's "Game Time" pauses when the game is quit or <see cref="GetGameTime" /> returns <see langword="null" />
    /// </summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> when "Game Time" should pause during the conditions, otherwise <see langword="false" /></returns>
    public bool IsGameTimePaused(LiveSplitState state)
    {
        // Whenever using actual IGT for LiveSplit's Game Time, we want game pauses / crashes to pause the timer.
        if (Settings.GameTimeMethod == GameTimeMethod.Igt)
            return true;

        // This is the load removal logic for RTA without Loads.
        return GameData.IsLoading.Current;
    }

    /// <summary>Determines LiveSplit's "Game Time", which can be either IGT or RTA w/o Loads.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns>"Game Time" as a <see cref="TimeSpan" /> if available, otherwise <see langword="null" /></returns>
    public TimeSpan? GetGameTime(LiveSplitState state)
        => Settings.GameTimeMethod switch
        {
            GameTimeMethod.Igt => IgtGameTime(Settings.Deathrun),
            GameTimeMethod.RtaNoLoads => null,
            _ => throw new ArgumentOutOfRangeException(nameof(Settings.GameTimeMethod), "Unknown GameTimeMethod"),
        };

    private static TimeSpan? IgtGameTime(bool deathrun)
    {
        Game baseGame = GameData.CurrentActiveBaseGame;

        // Stop IGT when a deathrun is complete.
        if (deathrun)
        {
            if (baseGame is Game.Tr6 && GameData.Tr6Health.Current <= 0)
                return null;
            if (baseGame is not Game.Tr6 && GameData.Tr45Health.Current <= 0)
                return null;
        }

        // Check IGT is ticking.
        if (!GameData.Igt.Changed)
            return null;

        long totalTicks = RunStats.GetCompletedLevelIgtIn60FpsTicks(GameData.CurrentActiveGame);
        double totalSeconds = (double)totalTicks / 60;
        return TimeSpan.FromSeconds(totalSeconds);
    }

    #region ShouldSplit

    /// <summary>Determines if the timer should split.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should split, <see langword="false" /> otherwise</returns>
    public bool ShouldSplit(LiveSplitState state)
    {
        if (Settings.Deathrun)
            return DeathrunShouldSplit();

        return GameData.CurrentActiveBaseGame == Game.Tr6 ? ShouldSplitTr6() : ShouldSplitTr4Tr5();
    }

    private static bool DeathrunShouldSplit()
    {
        bool laraJustDied;
        if (GameData.CurrentActiveBaseGame is Game.Tr6)
            laraJustDied = GameData.Tr6Health.Old > 0 && GameData.Tr6Health.Current <= 0;
        else
            laraJustDied = GameData.Tr45Health.Old > 0 && GameData.Tr45Health.Current <= 0;

        return laraJustDied;
    }

    private static bool PickupShouldSplit(PickupSplitSetting setting)
    {
        if (GameData.IsLoading.Current || !GameData.Igt.Changed)
            return false;

        return setting switch
        {
            PickupSplitSetting.None => false,
            PickupSplitSetting.All => GameData.Pickups.Current > GameData.Pickups.Old,
            _ => GameData.Secrets.Current > GameData.Secrets.Old,
        };
    }

    private bool ShouldSplitTr4Tr5()
    {
        if (PickupShouldSplit(Settings.PickupSplitSetting))
            return true;

        // Prevent double-splits; applies to ILs and FG for both glitched and glitchless.
        if (!GameData.NextLevel.Changed)
            return false;

        return GameData.CurrentActiveBaseGame == Game.Tr4 ? ShouldSplitTr4() : ShouldSplitTr5();
    }

    private bool ShouldSplitTr4()
    {
        if (GameData.CurrentActiveGame == Game.Tr4TheTimesExclusive)
        {
            // There are only 2 non-menu levels; 39 is the cutscene and 40 is the playable level.
            // The playable level is hardcoded to trigger credits.
            string oldLevelId = GameData.OldLevelId;
            if (RunStats.LevelHasBeenSplit(GameData.CurrentActiveGame, oldLevelId))
                return false;

            uint tteNextLevel = GameData.NextLevel.Current;
            if (tteNextLevel == 0 || GameData.Level.Current is 0 or (uint)Tr4Level.Office)
                return false;

            _latestSplitId = oldLevelId;
            return true;
        }

        const uint hardcodedCreditsTrigger = 39;

        uint nextLevel = GameData.NextLevel.Current;
        uint currentLevel = GameData.Level.Current;
        if (nextLevel == currentLevel || nextLevel == 0)
            return false;

        // Handle IL / Area% runs, where all level transitions are desirable splits.
        if (Settings.IlOrArea)
            return true;

        // Handle when credits are triggered.
        if (nextLevel == hardcodedCreditsTrigger && currentLevel != 0)
            return true;

        byte triggerTimer = GameData.GfRequiredStartPosition.Current;
        bool laraIsInLowerLevel = nextLevel >= currentLevel;
        Tr4Level lowerLevel = laraIsInLowerLevel ? (Tr4Level)currentLevel : (Tr4Level)nextLevel;
        Tr4Level higherLevel = laraIsInLowerLevel ? (Tr4Level)nextLevel : (Tr4Level)currentLevel;
        TransitionDirection direction = laraIsInLowerLevel ? TransitionDirection.OneWayFromLower : TransitionDirection.OneWayFromHigher;

        var activeMatches = Settings
            .Tr4LevelTransitions
            .Where(t =>
                t.Active &&
                t.LowerLevel == lowerLevel &&
                (t.HigherLevel == higherLevel || nextLevel == t.UnusedLevelNumber) &&
                (t.SelectedDirectionality == TransitionDirection.TwoWay || t.SelectedDirectionality == direction) &&
                t.TriggerMatchedOrNotRequired(triggerTimer, laraIsInLowerLevel)
            )
            .ToList();

        if (!activeMatches.Any())
        {
#if DEBUG
            // Warn is the minimum threshold when using LiveSplit's Event Viewer logging.
            LiveSplit.Options.Log.Warning($"No active transition match found!\nTransition: {lowerLevel} | {direction} | {higherLevel} | room: {GameData.Room.Current} | tt: {triggerTimer}");
#endif
            return false;
        }

        if (activeMatches.Count > 1) // Should be impossible if hardcoded default transitions are set up correctly.
            LiveSplit.Options.Log.Error($"TR4R Level Transition Settings improperly coded, found multiple matches!\n"
                                        + $"Transition: {lowerLevel} | {direction} | {higherLevel} | room: {GameData.Room.Current} | tt: {triggerTimer}\n"
                                        + $"Matches: {string.Join(", ", activeMatches.Select(static s => s.DisplayName()))}"
            );

        Tr4LevelTransitionSetting match = activeMatches[0];
        if (RunStats.LevelHasBeenSplit(GameData.CurrentActiveGame, match.Id))
            return false;

        _latestSplitId = match.Id;
        return true;
    }

    private bool ShouldSplitTr5()
    {
        string oldLevelId = GameData.OldLevelId;
        if (RunStats.LevelHasBeenSplit(GameData.CurrentActiveGame, oldLevelId))
            return false;

        uint nextLevel = GameData.NextLevel.Current;
        if (nextLevel <= 1) // Use 1 to prevent a main menu or first level split, helpful for multi-game runs.
            return false;

        bool loadingDesiredLevel = Settings.SplitSecurityBreach || nextLevel != (uint)Tr5Level.CutsceneSecurityBreach;
        if (!loadingDesiredLevel)
            return false;

        _latestSplitId = oldLevelId;
        return true;
    }

    private bool ShouldSplitTr6()
    {
        // Ignore when a menu is open.
        if (GameData.Tr6MenuTicker.Changed)
            return false;

        // Handle Pickups + Secrets splits.
        if (PickupShouldSplit(Settings.PickupSplitSetting))
            return true;

        // End of game split based on FMV, used in all non-deathrun run types.
        if (GameData.Fmv.Changed && GameData.Fmv.Current.Trim().Equals("END"))
            return true;

        // Handle IL / Area% transitions.
        if (Settings.RunType is RunType.IndividualLevelOrArea)
        {
            // LevelIgt is only set to 0 after end-level triggers leading to a loading screen.
            bool levelIgtWasReset = GameData.LevelIgt.Changed && GameData.LevelIgt.Current == 0;
            return levelIgtWasReset;
        }

        // Handle FG level transitions.
        if (!GameData.Tr6LevelName.Changed)
            return false;

        string oldLevel = GameData.Tr6LevelName.Old.Trim();
        string currentLevel = GameData.Tr6LevelName.Current.Trim();

        const string inventory = "INVENT";
        const string frontend = "FRONTEND";
        if (oldLevel.StartsWith(inventory, StringComparison.OrdinalIgnoreCase) ||
            oldLevel.StartsWith(frontend, StringComparison.OrdinalIgnoreCase) ||
            currentLevel.StartsWith(inventory, StringComparison.OrdinalIgnoreCase) ||
            currentLevel.StartsWith(frontend, StringComparison.OrdinalIgnoreCase))
        {
            // Guard against unnecessary checks when the inventory or main menu is accessed.
            return false;
        }

        var activeMatches = Settings
            .Tr6LevelTransitions
            .Where(t =>
                t.Active &&
                t.OldLevel == oldLevel &&
                t.NextLevel == currentLevel
            )
            .ToList();

        if (!activeMatches.Any())
        {
#if DEBUG
            // Warn is the minimum threshold when using LiveSplit's Event Viewer logging.
            LiveSplit.Options.Log.Warning($"No active transition match found!\n" +
                                          $"{oldLevel} -> {currentLevel}");
#endif
            return false;
        }

        if (activeMatches.Count > 1)
        {
            // Should be impossible if hardcoded default transitions are set up correctly.
            LiveSplit.Options.Log.Error($"TR6R Level Transition Settings improperly coded, found multiple matches!\n"
                                        + $"Transition: {oldLevel} -> {currentLevel} \n"
                                        + $"Matches: {string.Join(", ", activeMatches.Select(static s => s.Name))}"
            );

            return false;
        }

        Tr6LevelTransitionSetting match = activeMatches[0];
        if (RunStats.LevelSplitCount(GameData.CurrentActiveGame, match.Id) >= match.SelectedCount)
            return false;

        _latestSplitId = match.Id;
        return true;
    }

    #endregion

    #region ShouldReset

    /// <summary>Determines if the timer should reset.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should reset, <see langword="false" /> otherwise</returns>
    public bool ShouldReset(LiveSplitState state)
    {
        if (!Settings.EnableAutoReset)
            return false;

        return GameData.CurrentActiveBaseGame == Game.Tr6 ? ShouldResetTr6() : ShouldResetTr4Tr5();
    }

    private static bool ShouldResetTr4Tr5()
    {
        bool enteringCreditsOrMainMenu = GameData.GfInitializeGame.Current && !GameData.GfInitializeGame.Old;
        if (!enteringCreditsOrMainMenu)
            return false;

        bool comingFromEndOfGame = GameData.NextLevel.Current != 0;
        return !comingFromEndOfGame;
    }

    private static bool ShouldResetTr6()
    {
        if (!GameData.Tr6LevelName.Changed)
            return false;

        string nextLevel = GameData.Tr6LevelName.Current.Trim();
        return nextLevel.Equals("FRONTEND.GMX");
    }

    #endregion

    #region ShouldStart

    /// <summary>Determines if the timer should start.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should start, <see langword="false" /> otherwise</returns>
    public bool ShouldStart(LiveSplitState state)
        => GameData.CurrentActiveBaseGame == Game.Tr6 ? ShouldStartTr6() : ShouldStartTr4Tr5();

    private bool ShouldStartTr4Tr5()
    {
        uint currentNextLevel = GameData.NextLevel.Current;
        uint oldNextLevel = GameData.NextLevel.Old;

        bool justFinishedLoading = currentNextLevel == 0 && oldNextLevel != 0;
        if (!justFinishedLoading)
            return false;

        // NextLevel will only be set for New Game and triggered level ends; loading a save does not set NextLevel.
        bool oldNextLevelTargetedFirstLevel = oldNextLevel
            is 1 // Covers TR4's Angkor Wat and TR5's Streets of Rome.
            or (uint)Tr4Level.Office; // Covers TTE cutscene level.

        return Settings.FullGame ? oldNextLevelTargetedFirstLevel : oldNextLevel != 0;
    }

    private bool ShouldStartTr6()
    {
        // ReSharper disable once StringLiteralTypo
        if (GameData.Fmv.Old.Trim().Equals("CRDIT")) // This is the second FMV that plays after "New Game" has been confirmed.
            _tr6NewGameStartedFromMenu = true;

        if (Settings.RunType is RunType.FullGame && !_tr6NewGameStartedFromMenu)
            return false;

        // The timer starts when IGT first ticks.
        bool igtJustStarted = GameData.LevelIgt.Old == 0 && GameData.LevelIgt.Changed;
        if (!igtJustStarted)
            return false;

        _tr6NewGameStartedFromMenu = false;
        return true;
    }

    #endregion

    /// <summary>On <see cref="LiveSplitState.OnStart" />, updates values.</summary>
    public void OnStart(LiveSplitState state)
    {
        RunStats.Clear();

        // Ensure LiveSplit's GameTime initializes, matching Real Time if it has already increased.
        if (!state.IsGameTimeInitialized)
            state.SetGameTime(state.CurrentTime.RealTime ?? TimeSpan.Zero);
        state.IsGameTimePaused = false;
    }

    /// <summary>On <see cref="LiveSplitState.OnSplit" />, updates values.</summary>
    /// <param name="game">Game whose split was completed</param>
    public void OnSplit(Game game)
    {
        if (Settings.RunType is not RunType.FullGame)
            return;

        // Compose and store level stats.
        uint igtTicks = game is Game.Tr6 or Game.Tr6NgPlus ? GameData.LevelIgt.Current : GameData.Igt.Current;

        uint maxCompletions = game switch
        {
            Game.Tr4 or Game.Tr4NgPlus or Game.Tr4TheTimesExclusive or Game.Tr5 or Game.Tr5NgPlus => 1,
            Game.Tr6 or Game.Tr6NgPlus => (uint)Settings.Tr6LevelTransitions.Single(t => t.Id == _latestSplitId).SelectedCount,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "Unknown game"),
        };

        var stats = new LevelStats
        {
            LevelId = _latestSplitId,
            Igt = igtTicks,
            MaxCompletions = maxCompletions,
        };

        RunStats.AddLevelStats(game, stats);
    }

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit" />, updates values.</summary>
    public void OnUndoSplit()
    {
        if (Settings.RunType is not RunType.FullGame)
            return;

        RunStats.UndoLevelStats();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GameData.OnGameVersionChanged -= Settings.SetGameVersion;
        Settings?.Dispose();
    }
}