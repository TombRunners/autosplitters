using System;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR456;

public class Autosplitter : IAutoSplitter, IDisposable
{
    internal readonly ComponentSettings Settings = new();

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter() => GameData.OnGameVersionChanged += Settings.SetGameVersion;

    /// <summary>
    ///     Determines if LiveSplit's "Game Time" pauses when the game is quit or <see cref="GetGameTime" /> returns <see langword="null" />
    /// </summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> when "Game Time" should pause during the conditions, otherwise <see langword="false" /></returns>
    public bool IsGameTimePaused(LiveSplitState state) => throw new NotImplementedException();

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

    private static TimeSpan? IgtGameTime(bool deathrun) => throw new NotImplementedException();

    /// <summary>Determines if the timer should split.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should split, <see langword="false" /> otherwise</returns>
    public bool ShouldSplit(LiveSplitState state) => throw new NotImplementedException();

    /// <summary>Determines if <paramref name="level" /> is the first of the game or expansion.</summary>
    /// <param name="level">Level to check</param>
    /// <returns><see langword="true" /> if <paramref name="level" /> is the first; <see langword="false" /> otherwise.</returns>
    private static bool IsFirstLevel(uint level)  => throw new NotImplementedException();

    /// <summary>Determines if the timer should reset.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should reset, <see langword="false" /> otherwise</returns>
    public bool ShouldReset(LiveSplitState state) => throw new NotImplementedException();

    /// <summary>Determines if the timer should start.</summary>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <returns><see langword="true" /> if the timer should start, <see langword="false" /> otherwise</returns>
    public bool ShouldStart(LiveSplitState state) => throw new NotImplementedException();

    /// <summary>On <see cref="LiveSplitState.OnStart" />, updates values.</summary>
    public void OnStart(LiveSplitState state) => throw new NotImplementedException();

    /// <summary>On <see cref="LiveSplitState.OnSplit" />, updates values.</summary>
    /// <param name="completedLevel">Level completed for the split /></param>
    public void OnSplit(uint completedLevel) => throw new NotImplementedException();

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit" />, updates values.</summary>
    public void OnUndoSplit() => throw new NotImplementedException();

    /// <inheritdoc />
    public void Dispose()
    {
        GameData.OnGameVersionChanged -= Settings.SetGameVersion;
        Settings?.Dispose();
    }
}