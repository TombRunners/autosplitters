using System;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR1
{
    /// <summary>
    ///     Implementation of <see cref="IAutoSplitter"/> for the component to use.
    /// </summary>
    internal class Autosplitter : IAutoSplitter
    {
        private uint _fullGameFarthestLevel = 1;
        internal readonly ComponentSettings Settings = new ComponentSettings();
        internal GameMemory GameMemory = new GameMemory();

        /// <summary>
        ///     Determines the IGT.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>The IGT</returns>
        public TimeSpan? GetGameTime(LiveSplitState state)
        {
            if (state.CurrentPhase != TimerPhase.Running)
                return null;
            
            // TODO Finish this so it doesn't only work in Caves (if it even does that).
            // We can't track time to the millisecond very accurately, which is acceptable
            // given that the stats screen reports time to the whole second anyway.
            uint levelTime = GameMemory.Data.LevelTime.Current / 30;
            return TimeSpan.FromSeconds(levelTime);
        }

        /// <summary>
        ///     Determines whether IGT is paused.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <remarks>
        ///     <c>true</c> indicates a pause, <c>false</c> indicates no pause.
        /// </remarks>
        /// <returns>If IGT should be paused</returns>
        public bool IsGameTimePaused(LiveSplitState state)
        {
            return false;
        }

        /// <summary>
        ///     Determines if the timer should split.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <remarks>
        ///     <c>true</c> splits, <c>false</c> does nothing.
        /// </remarks>
        /// <returns>If the timer should split</returns>
        public bool ShouldSplit(LiveSplitState state)
        {
            uint currentLevel = GameMemory.Data.Level.Current;
            uint oldLevel = GameMemory.Data.Level.Old;

            if (Settings.FullGame)
            {
                // Prevents a resplit at an already-complete level in case the player must load back into it.
                if (currentLevel < _fullGameFarthestLevel)
                    return false;

                // The following checks are needed because the level value switches for cutscenes and FMVs.
                if (_fullGameFarthestLevel == 15)
                {
                    if (currentLevel >= 16) 
                        return false;
                }
                else if (_fullGameFarthestLevel > 9)
                {
                    if (currentLevel >= 16 && currentLevel < 19)
                        return false;
                }
                else if (_fullGameFarthestLevel > 4)
                {
                    if (currentLevel == 16)
                        return false;
                }
            }

            bool shouldSplit;
            // Deal with levels that end in a cutscene.
            // 16 is the cutscene number occurring at the end of level 4 (Qualopec).
            // 17 is the cutscene number occurring at the end of level 9 (Tihocan).
            if (currentLevel == 16 && oldLevel == 4 ||
                currentLevel == 17 && oldLevel == 9)
            {
                if (GameMemory.Data.StatsScreenIsActive.Current)
                {
                    if (GameMemory.GameVersion == GameVersion.ATI)
                        shouldSplit = GameMemory.Data.CutsceneFlag.Current == 0;
                    else
                        shouldSplit = GameMemory.Data.CutsceneFlag.Current == 1;
                }
                else
                {
                    shouldSplit = false;
                }
            }
            // 18 is the cutscene after the stats screen in Natla's Mines;
            // we always return false, because at some points during said scene,
            // the stats value fluctuates and would return false positives.
            // Level 14 is Atlantis, and the stats screen does not occur until the
            // end of 19, the cutscene after the FMV at the end of Atlantis.
            // So we ignore the fluctuating stats value while level 14 is active.
            else if (currentLevel == 18 || currentLevel == 14)
                shouldSplit = false;
            // If not on the Qualopec or Tihocan levels, inspect the normal case.
            else if (currentLevel != 4 && currentLevel != 9)
                shouldSplit = GameMemory.Data.StatsScreenIsActive.Current && !GameMemory.Data.StatsScreenIsActive.Old;
            else
                shouldSplit = false;

            if (shouldSplit && Settings.FullGame)
                _fullGameFarthestLevel++;

            return shouldSplit;
        }

        /// <summary>
        ///     Determines if the timer should reset.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <remarks>
        ///     <c>true</c> resets, <c>false</c> does nothing.
        /// </remarks>
        /// <returns>If the timer should reset</returns>
        public bool ShouldReset(LiveSplitState state)
        {
            /* It is hypothetically reasonable to use _fullGameFarthestLevel to reset
             * if the player loads into a level ahead of their current level.
             * However, considering a case where a runner accidentally loads an incorrect
             * save after dying, it's clear that this should be avoided.
             */
            return GameMemory.Data.StartGameFlag.Current == 2;
        }

        /// <summary>
        ///     Determines if the timer should start.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <remarks>
        ///     <c>true</c> starts, <c>false</c> does nothing.
        /// </remarks>
        /// <returns>If the timer should start</returns>
        public bool ShouldStart(LiveSplitState state)
        {
            _fullGameFarthestLevel = 1;

            uint currentLevel = GameMemory.Data.Level.Current;
            uint oldLevel = GameMemory.Data.Level.Old;

            // Check if the player may have used `New Game` from the main menu.
            bool loadedCaves = currentLevel == 1 && GameMemory.Data.StartGameFlag.Current == 1;
            // Avoid false positives caused by saving and loading within Caves.
            bool notAlreadyInCaves = oldLevel == 0 || oldLevel == 20;
            if (loadedCaves && notAlreadyInCaves)
                return true;

            return !Settings.FullGame && ILTimerShouldStart(currentLevel, oldLevel);
        }

        /// <summary>
        ///     Used to determine if the timer should start for IL runs.
        /// </summary>
        /// <param name="currentLevel">Current level</param>
        /// <param name="oldLevel">Old level</param>
        /// <returns>If the timer should start for IL runs</returns>
        private bool ILTimerShouldStart(uint currentLevel, uint oldLevel)
        {
            // Don't start in non-levels.
            if (currentLevel > 15)
                return false;
            // Check for a level that follows a cutscene.
            if (oldLevel > 15 && oldLevel < 20)
                return currentLevel == 05 || currentLevel == 10 ||
                       currentLevel == 14 || currentLevel == 15;
            // For normal cases, check if the stats screen was closed.
            return GameMemory.Data.StatsScreenIsActive.Old && !GameMemory.Data.StatsScreenIsActive.Current;
        }
    }
}
