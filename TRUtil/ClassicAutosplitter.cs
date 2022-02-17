using System;
using System.Collections.Generic;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TRUtil
{
    public abstract class ClassicAutosplitter : IAutoSplitter, IDisposable
    {
        /// <summary>Used to size CompletedLevels.</summary>
        protected int LevelCount = 0;
        
        /// <summary>Used to decide when to split and which level time addresses should be read from memory.</summary>
        protected readonly List<uint> CompletedLevels = new List<uint>();

        /// <summary>The GUI for the user to select timing options.</summary>
        protected internal ClassicComponentSettings Settings = new ClassicComponentSettings();

        /// <summary>Contains data needed for start, split, reset, and IGT logic.</summary>
        public ClassicGameData Data;

        /// <summary>Determines the IGT.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>IGT as a <see cref="TimeSpan"/> if available, otherwise <see langword="null"/></returns>
        public virtual TimeSpan? GetGameTime(LiveSplitState state)
        {
            // Check that IGT is ticking.
            uint currentLevelTicks = ClassicGameData.LevelTime.Current;
            uint oldLevelTicks = ClassicGameData.LevelTime.Old;
            if (currentLevelTicks - oldLevelTicks == 0)
                return null;

            // TR3's IGT ticks during globe level selection; the saved end-level IGT is unaffected, thus the overall FG IGT is also unaffected.
            // If a runner is watching LiveSplit's IGT, this may confuse them, despite it being a non-issue for the level/FG IGT.
            // To prevent the ticks from showing in LS, we use the fact that LevelComplete isn't reset to 0 until the next level is loaded.
            uint currentLevel = ClassicGameData.Level.Current;
            bool oldLevelComplete = ClassicGameData.LevelComplete.Old;
            bool currentLevelComplete = ClassicGameData.LevelComplete.Current;
            bool levelStillComplete = oldLevelComplete && currentLevelComplete;
            if (CompletedLevels.Contains(currentLevel) && levelStillComplete)
                return null;

            // Sum the current and completed levels' IGT.
            double currentLevelTime = ClassicGameData.LevelTimeAsDouble(currentLevelTicks);
            double finishedLevelsTime = ClassicGameData.SumCompletedLevelTimes(CompletedLevels, currentLevel);
            return TimeSpan.FromSeconds(currentLevelTime + finishedLevelsTime);
        }

        /// <summary>Determines if IGT pauses when the game is quit or <see cref="GetGameTime"/> returns <see langword="null"/></summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> when IGT should be paused during the conditions, <see langword="false"/> otherwise</returns>
        public bool IsGameTimePaused(LiveSplitState state) => true;

        /// <summary>Determines if the timer should split.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should split, <see langword="false"/> otherwise</returns>
        public virtual bool ShouldSplit(LiveSplitState state)
        {
            // Determine if the player is on the correct level to split; if not, we stop.
            uint currentLevel = ClassicGameData.Level.Current;
            bool onCorrectLevelToSplit = !CompletedLevels.Contains(currentLevel);
            if (!onCorrectLevelToSplit)
                return false;

            // Deathrun
            if (Settings.Deathrun)
            {
                bool laraJustDied = ClassicGameData.Health.Old > 0 && ClassicGameData.Health.Current == 0;
                return laraJustDied;
            }
            
            // FG & IL/Section
            bool levelJustCompleted = !ClassicGameData.LevelComplete.Old && ClassicGameData.LevelComplete.Current;
            return levelJustCompleted;
        }

        /// <summary>Determines if the timer should reset.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should reset, <see langword="false"/> otherwise</returns>
        public virtual bool ShouldReset(LiveSplitState state)
        {
            /* It is hypothetically reasonable to use CompletedLevels to reset
             * if the player loads into a level ahead of their current level.
             * However, considering a case where a runner accidentally loads an incorrect
             * save after dying, it's clear that this should be avoided.
             */
            return ClassicGameData.PickedPassportFunction.Current == 2;
        }

        /// <summary>Determines if the timer should start.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should start, <see langword="false"/> otherwise</returns>
        public virtual bool ShouldStart(LiveSplitState state)
        {
            uint oldLevelTime = ClassicGameData.LevelTime.Old;
            uint currentLevelTime = ClassicGameData.LevelTime.Current;
            uint currentPickedPassportFunction = ClassicGameData.PickedPassportFunction.Current;
            bool oldTitleScreen = ClassicGameData.TitleScreen.Old;

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

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            Data.OnGameFound -= Settings.SetGameVersion;
            Data = null;
        }
    }
}
