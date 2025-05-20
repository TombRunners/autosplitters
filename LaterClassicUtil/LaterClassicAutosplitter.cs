using System;
using LiveSplit.Model;
using Util;

namespace LaterClassicUtil;

public abstract class LaterClassicAutosplitter<TData, TSettings>(TData data, TSettings settings) : BaseAutosplitter
    where TData : LaterClassicGameData
    where TSettings : LaterClassicComponentSettings
{
    public readonly TSettings Settings = settings;
    protected internal readonly TData Data = data;

    /// <summary>Populated by the default implementation of <see cref="OnStart"/>.</summary>
    /// <remarks>
    ///     <see cref="_ticksAtStartOfRun"/> is used in <see cref="GetGameTime(LiveSplitState)"/> to subtract from <see cref="LaterClassicGameData.GameTimer"/>.
    ///     The subtraction is necessary for ILs/section runs starting on any level besides the first to get an accurate IGT from the start of the run.
    /// </remarks>
    private ulong _ticksAtStartOfRun;

    public override TimeSpan? GetGameTime(LiveSplitState state)
    {
        // Stop IGT when a deathrun is complete.
        if (Settings.Deathrun && Data.Health.Current <= 0)
            return null;

        if (!Data.GameTimer.Changed)
            return null;

        // Prevent underflow issues after loading into a different "ticks" timeline.
        if (_ticksAtStartOfRun > Data.GameTimer.Current)
            return null;

        return TimeSpan.FromSeconds(BaseGameData.LevelTimeAsDouble(Data.GameTimer.Current - _ticksAtStartOfRun));
    }

    public override bool IsGameTimePaused(LiveSplitState state) => true;

    public override bool ShouldReset(LiveSplitState state)
    {
        if (!Settings.EnableAutoReset)
            return false;

        bool loadingIntoMainMenu = Data.GfLevelComplete.Current == 0 && Data.Level.Current == 0 && Data.Loading.Current;
        // Checking the old level number ensures that someone re-opening the game (perhaps after a crash or Alt+F4) does not Reset.
        // This works because when loading non-test/demo versions of the games, the level variable initializes as 0 before the main menu load is called.
        bool comingFromALevel = Data.Level.Old != 0;
        return loadingIntoMainMenu && comingFromALevel;
    }

    protected bool DeathrunShouldSplit()
    {
        bool laraJustDied = Data.Health.Old > 0 && Data.Health.Current <= 0;
        return laraJustDied;
    }

    protected bool SecretShouldSplit()
    {
        if (!Data.Secrets.Changed || Data.GfInitializeGame.Current || Data.InventoryActive.Current != 0)
            return false;

        bool secretWasTriggered = Data.Secrets.Current > Data.Secrets.Old;
        return secretWasTriggered;
    }

    public override bool ShouldStart(LiveSplitState state)
    {
        uint currentGfLevelComplete = Data.GfLevelComplete.Current;
        uint oldGfLevelComplete = Data.GfLevelComplete.Old;

        bool justFinishedLoading = currentGfLevelComplete == 0 && oldGfLevelComplete != 0;
        if (!justFinishedLoading)
            return false;

        // GfLevelComplete will only be 1 if New Game is clicked; loading a save does not set GfLevelComplete to the loaded level's value.
        return Settings.FullGame ? oldGfLevelComplete == 1 : oldGfLevelComplete != 0;
    }

    // ReSharper disable VirtualMemberNeverOverridden.Global
    /// <summary>On <see cref="LiveSplitState.OnStart"/>, updates values.</summary>
    public virtual void OnStart()
    {
        try
        {
            _ticksAtStartOfRun = Data.Level.Current == 1 ? 0 : Data.GameTimer.Old;
        }
        catch // Data is unpopulated when no game is running.
        {
            _ticksAtStartOfRun = 0;
        }
    }

    /// <summary>On <see cref="LiveSplitState.OnSplit"/>, updates values.</summary>
    public virtual void OnSplit() { }

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit"/>, updates values.</summary>
    public virtual void OnUndoSplit() { }
    // ReSharper restore VirtualMemberNeverOverridden.Global

    public override void Dispose()
    {
        Data.OnGameVersionChanged -= Settings.SetGameVersion;
    }
}