using System;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR456;

public class Autosplitter : IAutoSplitter, IDisposable
{
    internal readonly ComponentSettings Settings = new();

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
    public bool ShouldSplit(LiveSplitState state) => GameData.CurrentActiveBaseGame == Game.Tr6 ? ShouldSplitTr6() : ShouldSplitTr4Tr5();

    private bool ShouldSplitTr4Tr5() => false;

    private bool ShouldSplitTr6() => false;

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
        bool loadingIntoMainMenu = GameData.NextLevel.Current == 0 && GameData.CurrentLevel == 0 && GameData.IsLoading.Current;

        // Checking the old level number ensures that someone re-opening the game (perhaps after a crash or Alt+F4) does not Reset.
        // This works because the level variable initializes as 0 before the main menu load is called.
        bool comingFromALevel = GameData.OldLevel != 0;

        return loadingIntoMainMenu && comingFromALevel;
    }

    private bool ShouldResetTr6() => false;

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

    private bool ShouldStartTr6() => false;

    #endregion

    /// <summary>On <see cref="LiveSplitState.OnStart" />, updates values.</summary>
    public void OnStart(LiveSplitState state)
    {
    }

    /// <summary>On <see cref="LiveSplitState.OnSplit" />, updates values.</summary>
    /// <param name="completedLevel">Level completed for the split /></param>
    public void OnSplit(uint completedLevel)
    {
    }

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit" />, updates values.</summary>
    public void OnUndoSplit()
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
        GameData.OnGameVersionChanged -= Settings.SetGameVersion;
        Settings?.Dispose();
    }
}