using System;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR456;

public class Autosplitter : IAutoSplitter, IDisposable
{
    internal readonly ComponentSettings Settings = new();

    private bool _tr6NewGameStartedFromMenu;

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
    public bool IsGameTimePaused(LiveSplitState state) => false;

    /// <summary>Determines LiveSplit's "Game Time", which can be either IGT or RTA w/o Loads.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns>"Game Time" as a <see cref="TimeSpan" /> if available, otherwise <see langword="null" /></returns>
    public TimeSpan? GetGameTime(LiveSplitState state) =>
        Settings.GameTimeMethod switch
        {
            GameTimeMethod.Igt => IgtGameTime(Settings.Deathrun),
            GameTimeMethod.RtaNoLoads => null,
            _ => throw new ArgumentOutOfRangeException(nameof(Settings.GameTimeMethod), "Unknown GameTimeMethod"),
        };

    private static TimeSpan? IgtGameTime(bool deathrun) => null;

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
        if (GameData.IsLoading.Current)
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
        const uint hardcodedCreditsTrigger = 39;

        uint nextLevel = GameData.NextLevel.Current;
        uint currentLevel = GameData.Level.Current;
        if (nextLevel == currentLevel || nextLevel == 0)
            return false;

        // Handle IL / Area% runs, where all level transitions are desirable splits.
        if (Settings.IlOrArea)
            return true;

        // Handle when credits are triggered.
        if (nextLevel == hardcodedCreditsTrigger)
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

        return true;
    }

    private bool ShouldSplitTr5()
    {
        uint currentNextLevel = GameData.NextLevel.Current;

        // Handle ILs and FG for both rulesets.
        bool loadingAnotherLevel = currentNextLevel != 0;
        if (!Settings.SplitSecurityBreach)
            loadingAnotherLevel &= currentNextLevel != (uint)Tr5Level.CutsceneSecurityBreach;
        return loadingAnotherLevel;
    }

    private bool ShouldSplitTr6()
    {
        if (GameData.Tr6MenuTicker.Changed)
            return false;

        const string inventory = "INVENT.GMX";
        string oldLevel = GameData.Tr6LevelName.Old.Trim();
        string nextLevel = GameData.Tr6LevelName.Current.Trim();
        if (oldLevel.Equals(inventory) || nextLevel.Equals(inventory))
            return false;

        if (PickupShouldSplit(Settings.PickupSplitSetting))
            return true;

        // End of game split, based on FMV.
        if (GameData.Fmv.Changed && GameData.Fmv.Current.Trim().Equals("END"))
            return true;

        if (!GameData.Tr6LevelName.Changed)
            return false;

        var activeMatches = Settings
            .Tr6LevelTransitions
            .Where(t =>
                t.Active &&
                t.OldLevel == oldLevel &&
                t.NextLevel == nextLevel
            )
            .ToList();

        if (!activeMatches.Any())
        {
#if DEBUG
            // Warn is the minimum threshold when using LiveSplit's Event Viewer logging.
            LiveSplit.Options.Log.Warning($"No active transition match found!\n" +
                                          $"{oldLevel} -> {nextLevel}");
#endif
            return false;
        }

        if (activeMatches.Count > 1)
        {
            // Should be impossible if hardcoded default transitions are set up correctly.
            LiveSplit.Options.Log.Error($"TR6R Level Transition Settings improperly coded, found multiple matches!\n"
                                        + $"Transition: {oldLevel} -> {nextLevel} \n"
                                        + $"Matches: {string.Join(", ", activeMatches.Select(static s => s.Name))}"
            );

            return false;
        }

        Tr6LevelTransitionSetting match = activeMatches[0];
        // TODO: reference game stats split count instead of 0.
        return 0 < match.SelectedCount;
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
        if (GameData.Fmv.Old.Trim().Equals("CRDIT")) // This is the second FMV that plays after "New Game" has been confirmed. This is a unique condition.
            _tr6NewGameStartedFromMenu = true;

        // The timer starts after the first loading screen.
        bool oldLoading = GameData.IsLoading.Old;
        bool currentLoading = GameData.IsLoading.Current;
        if (!_tr6NewGameStartedFromMenu || !oldLoading || currentLoading)
            return false;

        _tr6NewGameStartedFromMenu = false;
        return true;
    }

    #endregion

    /// <summary>On <see cref="LiveSplitState.OnStart" />, updates values.</summary>
    public void OnStart(LiveSplitState state)
    {
        // TODO: Game + level stats tracking

        // Ensure LiveSplit's GameTime initializes, matching Real Time if it has already increased.
        if (!state.IsGameTimeInitialized)
            state.SetGameTime(state.CurrentTime.RealTime ?? TimeSpan.Zero);
        state.IsGameTimePaused = false;
    }

    /// <summary>On <see cref="LiveSplitState.OnSplit" />, updates values.</summary>
    /// <param name="activeGame">Game whose split was completed</param>
    public void OnSplit(Game activeGame)
    {
        // TODO: Game + level stats tracking
    }

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit" />, updates values.</summary>
    public void OnUndoSplit()
    {
        // TODO: Game + level stats tracking
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GameData.OnGameVersionChanged -= Settings.SetGameVersion;
        Settings?.Dispose();
    }
}