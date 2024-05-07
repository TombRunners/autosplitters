using System;
using System.Collections.Generic;
using LiveSplit.Model;

namespace TRUtil;

public abstract class ClassicAutosplitter<TData>(Version version, TData data) : BaseAutosplitter
    where TData : ClassicGameData
{
    protected internal ClassicComponentSettings Settings = new(version);
    public TData Data = data;

    /// <summary>Used to size CompletedLevels.</summary>
    protected int LevelCount = 0;

    /// <summary>Used to decide when to split and which level time addresses should be read from memory.</summary>
    protected readonly List<uint> CompletedLevels = [];

    public override TimeSpan? GetGameTime(LiveSplitState state)
    {
        // Stop IGT when a deathrun is complete.
        if (Settings.Deathrun && Data.Health.Current <= 0)
            return null;

        // Check that IGT is ticking.
        uint currentLevelTicks = Data.LevelTime.Current;
        uint oldLevelTicks = Data.LevelTime.Old;
        if (currentLevelTicks - oldLevelTicks == 0)
            return null;

        // TR3's IGT ticks during globe level selection; the saved end-level IGT is unaffected, thus the overall FG IGT is also unaffected.
        // If a runner is watching LiveSplit's IGT, this may confuse them, despite it being a non-issue for the level/FG IGT.
        // To prevent the ticks from showing in LS, we use the fact that LevelComplete isn't reset to 0 until the next level is loaded.
        uint currentLevel = Data.Level.Current;
        bool oldLevelComplete = Data.LevelComplete.Old;
        bool currentLevelComplete = Data.LevelComplete.Current;
        bool stillOnCompletedLevel = oldLevelComplete && currentLevelComplete;
        if (CompletedLevels.Contains(currentLevel) && stillOnCompletedLevel)
            return null;

        // Sum the current and completed levels' IGT.
        ulong ticks = currentLevelTicks + Data.SumLevelTimes(CompletedLevels, currentLevel);
        return TimeSpan.FromSeconds(BaseGameData.LevelTimeAsDouble(ticks));
    }

    public override bool ShouldSplit(LiveSplitState state)
    {
        // Determine if the player is on the correct level to split; if not, we stop.
        uint currentLevel = Data.Level.Current;
        bool onCorrectLevelToSplit = !CompletedLevels.Contains(currentLevel);
        if (!onCorrectLevelToSplit)
            return false;

        // Deathrun
        if (Settings.Deathrun)
        {
            bool laraJustDied = Data.Health.Old > 0 && Data.Health.Current == 0;
            return laraJustDied;
        }

        // FG & IL/Section
        bool levelJustCompleted = !Data.LevelComplete.Old && Data.LevelComplete.Current;
        return levelJustCompleted;
    }

    public override bool ShouldReset(LiveSplitState state)
    {
        if (!Settings.EnableAutoReset)
            return false;

        /* It is hypothetically reasonable to use CompletedLevels to reset
         * if the player loads into a level ahead of their current level.
         * However, considering a case where a runner accidentally loads an incorrect
         * save after dying, it's clear that this should be avoided.
         */
        return Data.PickedPassportFunction.Current == 2;
    }

    public override bool ShouldStart(LiveSplitState state)
    {
        uint oldLevelTime = Data.LevelTime.Old;
        uint currentLevelTime = Data.LevelTime.Current;
        uint currentPickedPassportFunction = Data.PickedPassportFunction.Current;
        bool oldTitleScreen = Data.TitleScreen.Old;

        // Perform new game logic first, since it is the only place where FG should start.
        bool levelTimeJustStarted = oldLevelTime == 0 && currentLevelTime != 0 && currentLevelTime < 50;
        bool newGameStarted = levelTimeJustStarted && currentPickedPassportFunction == 1;
        if (newGameStarted)
            return true;

        return !Settings.FullGame && levelTimeJustStarted && !oldTitleScreen;
    }

    /// <summary>On <see cref="LiveSplitState.OnStart"/>, updates values.</summary>
    public virtual void OnStart() => CompletedLevels.Clear();

    /// <summary>On <see cref="LiveSplitState.OnSplit"/>, updates values.</summary>
    /// <param name="completedLevel">What to add to <see cref="CompletedLevels"/></param>
    public virtual void OnSplit(uint completedLevel) => CompletedLevels.Add(completedLevel);

    /// <summary>On <see cref="LiveSplitState.OnUndoSplit"/>, updates values.</summary>
    public virtual void OnUndoSplit() => CompletedLevels.RemoveAt(CompletedLevels.Count - 1);

    public override void Dispose()
    {
        Data.OnGameVersionChanged -= Settings.SetGameVersion;
        Data = null;
    }
}