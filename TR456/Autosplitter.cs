using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.Options;
using Util;

namespace TR456;

public class Autosplitter : BaseAutosplitter
{
    internal readonly ComponentSettings Settings = new();

    private bool _tr6NewGameStartedFromMenu;
    private long _ticksAtStartOfRun;
    private ulong _latestSplitId;
    private TransitionDirection _latestSplitDirection;

    private readonly Dictionary<Game, short> _pickups = new(6)
    {
        {Game.Tr4, 0},
        {Game.Tr4NgPlus, 0},
        {Game.Tr4TheTimesExclusive, 0},
        {Game.Tr5, 0},
        {Game.Tr5NgPlus, 0},
        {Game.Tr6, 0},
        {Game.Tr6NgPlus, 0},
    };

    private readonly Dictionary<Game, short> _secrets = new(6)
    {
        {Game.Tr4, 0},
        {Game.Tr4NgPlus, 0},
        {Game.Tr4TheTimesExclusive, 0},
        {Game.Tr5, 0},
        {Game.Tr5NgPlus, 0},
        {Game.Tr6, 0},
        {Game.Tr6NgPlus, 0},
    };

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter()
    {
        GameData.OnGameVersionChanged += Settings.SetGameVersion;
        GameData.OnSignatureScanStatusChanged += Settings.SetSignatureScanStatusLabel;
    }

    public override bool IsGameTimePaused(LiveSplitState state)
    {
        // Whenever using actual IGT for LiveSplit's Game Time, we want game pauses / crashes to pause the timer.
        if (Settings.GameTimeMethod == GameTimeMethod.Igt)
            return true;

        // This is the load removal logic for RTA without Loads.
        if (!GameData.GameIsInitialized)
            return false;

        if (GameData.CurrentActiveBaseGame is Game.Tr6)
            return GameData.IsLoading.Current;

        // RTA w/o Loads should tick whenever a loading screen is not active.
        const int loadFadeFullAmount = 255;
        return GameData.LoadFade.Current == loadFadeFullAmount;
    }

    public override TimeSpan? GetGameTime(LiveSplitState state)
        => Settings.GameTimeMethod switch
        {
            GameTimeMethod.Igt => IgtGameTime(Settings.Deathrun),
            GameTimeMethod.RtaNoLoads => null,
            _ => throw new ArgumentOutOfRangeException(nameof(Settings.GameTimeMethod), "Unknown GameTimeMethod"),
        };

    private TimeSpan? IgtGameTime(bool deathrun)
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

        // Prevent underflow issues after loading into a different "ticks" timeline.
        if (_ticksAtStartOfRun > GameData.Igt.Current)
            return null;

        long totalTicks = RunStats.GetTotalIgtIn60FpsTicks(GameData.CurrentActiveGame);
        totalTicks -= _ticksAtStartOfRun;

        double totalSeconds = (double)totalTicks / 60;
        return TimeSpan.FromSeconds(totalSeconds);
    }

    #region ShouldSplit

    public override bool ShouldSplit(LiveSplitState state)
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

    [SuppressMessage("ReSharper", "InvertIf")]
    private bool PickupShouldSplit(PickupSplitSetting setting)
    {
        if (setting is PickupSplitSetting.None)
            return false;

        Game currentGame = GameData.CurrentActiveGame;
        var shouldSplit = false;

        if (GameData.Pickups.Current > _pickups[currentGame])
        {
            _pickups[currentGame] = GameData.Pickups.Current;
            if (setting is PickupSplitSetting.All)
                shouldSplit = true;
        }

        if (GameData.Secrets.Current > _secrets[currentGame])
        {
            _secrets[currentGame] = GameData.Secrets.Current;
            if (setting is PickupSplitSetting.SecretsOnly)
                shouldSplit = true;
        }

        return shouldSplit;
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
        uint oldLevel = GameData.Level.Old;
        if (nextLevel == oldLevel || nextLevel == 0 || oldLevel is 0 or (uint)Tr4Level.Office) // Ignore the menu and opening cutscene of TTE.
            return false;

        // Handle when credits are triggered.
        if (nextLevel == hardcodedCreditsTrigger)
        {
            ulong oldLevelId = GameData.OldLevelId;
            if (Settings.FullGame && RunStats.LevelWasSplit(GameData.CurrentActiveGame, oldLevelId))
                return false;

            _latestSplitId = oldLevelId;
            _latestSplitDirection = TransitionDirection.OneWayFromLower;
            return true;
        }

        // Handle IL / Area% runs, where all level transitions are desirable splits.
        if (Settings.IlOrArea)
            return true;

        // Handle FG.
        byte triggerTimer = GameData.GfRequiredStartPosition.Current;
        bool laraIsInLowerLevel = nextLevel >= oldLevel;
        Tr4Level lowerLevel = laraIsInLowerLevel ? (Tr4Level)oldLevel : (Tr4Level)nextLevel;
        Tr4Level higherLevel = laraIsInLowerLevel ? (Tr4Level)nextLevel : (Tr4Level)oldLevel;
        TransitionDirection direction = laraIsInLowerLevel ? TransitionDirection.OneWayFromLower : TransitionDirection.OneWayFromHigher;

        var activeMatches = Settings
            .Tr4LevelTransitions
            .Where(t =>
                t.Active is not ActiveSetting.IgnoreAll &&
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
            Log.Warning($"No active transition match found!\nTransition: {lowerLevel} | {direction} | {higherLevel} | room: {GameData.Room.Current} | tt: {triggerTimer}");
#endif
            return false;
        }

        if (activeMatches.Count > 1) // Should be impossible if hardcoded default transitions are set up correctly.
            Log.Error($"TR4R Level Transition Settings improperly coded, found multiple matches!\n"
                      + $"Transition: {lowerLevel} | {direction} | {higherLevel} | room: {GameData.Room.Current} | tt: {triggerTimer}\n"
                      + $"Matches: {string.Join(", ", activeMatches.Select(static s => s.DisplayName()))}"
            );

        Tr4LevelTransitionSetting match = activeMatches[0];
        Game game = GameData.CurrentActiveGame;
        if (!match.ComplexIgnore)
        {
            if (RunStats.LevelWasSplit(game, match.Id, direction))
                return false;
        }
        else
        {
            bool levelHasBeenSplit = RunStats.LevelWasSplit(game, match.Id, direction);
            bool levelHasBeenIgnored = RunStats.LevelWasIgnored(game, match.Id, direction);
            switch (match.Active)
            {
                case ActiveSetting.Active:
                {
                    if (RunStats.LevelSplitCount(game, match.Id, direction) > 1)
                        return false;

                    break;
                }

                case ActiveSetting.IgnoreSecond:
                {
                    if (levelHasBeenSplit)
                        return false;

                    break;
                }

                case ActiveSetting.IgnoreFirst:
                {
                    if (levelHasBeenSplit)
                        return false;

                    if (levelHasBeenIgnored)
                            break;

                    // Compose and store level stats.
                    uint igtTicks = game is Game.Tr6 or Game.Tr6NgPlus
                        ? GameData.LevelIgt.Current
                        : GameData.Igt.Current;

                    var stats = new LevelStats
                    {
                        LevelId = match.Id,
                        Igt = igtTicks,
                        Ignored = true,
                        Direction = direction,
                    };
                    RunStats.AddLevelStats(game, stats);

                    return false;
                }

                case ActiveSetting.IgnoreAll:
                default:
                    throw new ArgumentOutOfRangeException(nameof(match.Active), match.Active, "Improper ActiveSetting");
            }
        }

        _latestSplitId = match.Id;
        _latestSplitDirection = direction;
        return true;
    }

    private bool ShouldSplitTr5()
    {
        ulong oldLevelId = GameData.OldLevelId;
        if (RunStats.LevelWasSplit(GameData.CurrentActiveGame, oldLevelId))
            return false;

        uint nextLevel = GameData.NextLevel.Current;
        if (nextLevel <= 1) // Use 1 to prevent a main menu or first level split, helpful for multi-game runs.
            return false;

        bool loadingDesiredLevel = Settings.SplitSecurityBreach || nextLevel != (uint)Tr5Level.CutsceneSecurityBreach;
        if (!loadingDesiredLevel)
            return false;

        _latestSplitId = oldLevelId;
        _latestSplitDirection = TransitionDirection.OneWayFromLower;
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
            return false; // Guard against unnecessary checks when the inventory or main menu is accessed.

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
            Log.Warning($"No active transition match found!\n" +
                        $"{oldLevel} -> {currentLevel}");
#endif
            return false;
        }

        if (activeMatches.Count > 1)
        {
            // Should be impossible if hardcoded default transitions are set up correctly.
            Log.Error($"TR6R Level Transition Settings improperly coded, found multiple matches!\n"
                      + $"Transition: {oldLevel} -> {currentLevel} \n"
                      + $"Matches: {string.Join(", ", activeMatches.Select(static s => s.Name))}"
            );

            return false;
        }

        Tr6LevelTransitionSetting match = activeMatches[0];
        if (RunStats.LevelWasSplit(GameData.CurrentActiveGame, match.Id))
            return false;

        _latestSplitId = match.Id;
        _latestSplitDirection = TransitionDirection.OneWayFromLower; // This is wrong, but it doesn't matter with current logic and features.
        return true;
    }

    #endregion

    #region ShouldReset

    public override bool ShouldReset(LiveSplitState state)
    {
        if (!Settings.EnableAutoReset)
            return false;

        return GameData.CurrentActiveBaseGame == Game.Tr6 ? ShouldResetTr6() : ShouldResetTr4Tr5();
    }

    private bool ShouldResetTr4Tr5()
    {
        bool enteringCreditsOrMainMenu = GameData.GfInitializeGame.Current && !GameData.GfInitializeGame.Old;
        if (!enteringCreditsOrMainMenu)
            return false;

        bool comingFromEndOfGame = GameData.NextLevel.Current != 0;
        return Settings.EnableAutoReset && !comingFromEndOfGame;
    }

    private bool ShouldResetTr6()
    {
        if (!GameData.Tr6LevelName.Changed)
            return false;

        string nextLevel = GameData.Tr6LevelName.Current.Trim();
        return Settings.EnableAutoReset && nextLevel.Equals("FRONTEND.GMX");
    }

    #endregion

    #region ShouldStart

    public override bool ShouldStart(LiveSplitState state)
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
        if (GameData.Tr6LevelName.Old is null)
            return false;

        // ReSharper disable once StringLiteralTypo
        if (GameData.Fmv.Old.Trim().Equals("CRDIT")) // This is the second FMV that plays after "New Game" has been confirmed.
            _tr6NewGameStartedFromMenu = true;

        if (Settings.RunType is RunType.FullGame && !_tr6NewGameStartedFromMenu)
            return false;

        // The timer starts when IGT first ticks.
        bool igtJustStarted = GameData.LevelIgt.Old == 0 && GameData.LevelIgt.Changed;
        bool loadedAfterLaunchingGame = GameData.Tr6LevelName.Old.Contains("FRONTEND");
        if (!igtJustStarted || loadedAfterLaunchingGame)
            return false;

        _tr6NewGameStartedFromMenu = false;
        return true;
    }

    #endregion

    /// <summary>On <see cref="LiveSplitState.OnStart" />, updates values.</summary>
    public void OnStart(LiveSplitState state)
    {
        RunStats.Clear();
        foreach (Game game in _pickups.Keys.ToList())
            _pickups[game] = 0;
        foreach (Game game in _secrets.Keys.ToList())
            _secrets[game] = 0;

        // Ensure LiveSplit's GameTime initializes, matching Real Time if it has already increased.
        if (!state.IsGameTimeInitialized)
            state.SetGameTime(state.CurrentTime.RealTime ?? TimeSpan.Zero);
        state.IsGameTimePaused = false;

        try
        {
            Game currentGame = GameData.CurrentActiveGame;

            _ticksAtStartOfRun = currentGame is Game.Tr6 or Game.Tr6NgPlus
                ? GameData.Igt.Old
                : GameData.Level.Current is 1 or 39 ? 0 : GameData.Igt.Old * 2;

            _pickups[currentGame] = GameData.Pickups.Current;
            _secrets[currentGame] = GameData.Secrets.Current;
        }
        catch // GameData is unpopulated when no game is running.
        {
            _ticksAtStartOfRun = 0;
        }
    }

    /// <summary>On <see cref="LiveSplitState.OnSplit" />, updates values.</summary>
    /// <param name="game">Game whose split was completed</param>
    public void OnSplit(Game game)
    {
        if (Settings.RunType is not RunType.FullGame)
            return;

        // Compose and store level stats.
        uint igtTicks = game is Game.Tr6 or Game.Tr6NgPlus ? GameData.LevelIgt.Current : GameData.Igt.Current;

        var stats = new LevelStats
        {
            LevelId = _latestSplitId,
            Igt = igtTicks,
            Direction = _latestSplitDirection,
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

    public override void Dispose()
    {
        GameData.OnGameVersionChanged -= Settings.SetGameVersion;
        Settings?.Dispose();
    }
}