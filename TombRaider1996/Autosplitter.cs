using System;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR1
{
    /// <summary>
    ///     The game's level and cutscene values.
    /// </summary>
    internal enum Levels
    {
        Manor            = 00,
        Caves            = 01,
        Vilcabamba       = 02,
        LostValley       = 03,
        Qualopec         = 04,
        StFrancisFolly   = 05,
        Colosseum        = 06,
        PalaceMidas      = 07,
        Cistern          = 08,
        Tihocan          = 09,
        CityOfKhamoon    = 10,
        ObeliskOfKhamoon = 11,
        SanctuaryScion   = 12,
        NatlasMines      = 13,
        Atlantis         = 14,
        TheGreatPyramid  = 15,
        QualopecCutscene = 16,
        TihocanCutscene  = 17,
        MinesToAtlantis  = 18,
        AfterAtlantisFMV = 19,
        TitleAndFirstFMV = 20
    }
    
    /// <summary>
    ///     Implementation of <see cref="IAutoSplitter"/> for the component to use.
    /// </summary>
    internal class Autosplitter : IAutoSplitter
    {
        private const uint IGTTicksPerSecond = 30;
        private const uint NumberOfLevels = 15;
        
        private uint _fullGameFarthestLevel = 1;
        private uint[] _fullGameLevelTimes = new uint[NumberOfLevels];
        private bool _fmvSkipAlreadySplit;

        internal readonly ComponentSettings Settings = new ComponentSettings();
        internal GameMemory GameMemory = new GameMemory();

        /// <summary>
        ///     Determines the IGT.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>The IGT</returns>
        public TimeSpan? GetGameTime(LiveSplitState state)
        {
            // TODO Finish this so it doesn't only work in Caves (if it even does that).
            // We can't track time to the millisecond very accurately, which is acceptable
            // given that the stats screen reports time to the whole second anyway.
            uint levelTime = GameMemory.Data.LevelTime.Current / IGTTicksPerSecond;
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
        /// <returns><c>true</c> if the timer should split</returns>
        public bool ShouldSplit(LiveSplitState state)
        {
            uint currentLevel = GameMemory.Data.Level.Current;
            uint oldLevel = GameMemory.Data.Level.Old;

            // In MinesToAtlantis, the stats value sometimes gives false positives, and
            // the NatlasMines stats screen appears before the MinesToAtlantis scene anyway.
            if (currentLevel == (int) Levels.MinesToAtlantis)
                return false;
            // If the runner loads into a previously-completed level and re-completes it, do not resplit.
            if (Settings.FullGame && RunnerAlreadyFinishedLevel(currentLevel))
                return false;

            bool shouldSplit = RunnerJustFinishedALevel(currentLevel, oldLevel) || RunnerJustDidTheFMVSkip(currentLevel);
            if (Settings.FullGame && shouldSplit)
                ++_fullGameFarthestLevel;

            return shouldSplit;
        }

        /// <summary>
        ///     Determines if the runner has progressed past the given level.
        /// </summary>
        /// <param name="currentLevel">Current level</param>
        /// <remarks>
        ///     Additional checks are needed because the level value switches for cutscenes and FMVs.
        ///     These checks start from the farthest level and work down to earlier levels.
        /// </remarks>
        /// <returns>If the runner progressed past the given level.</returns>
        private bool RunnerAlreadyFinishedLevel(uint currentLevel)
        {
            // If the runner progresses to the last level, do not split at any cutscenes again.
            if (_fullGameFarthestLevel == (int) Levels.TheGreatPyramid && 
                currentLevel >= (int) Levels.QualopecCutscene)
                return true;
            
            // If the runner progresses past Tihocan, do not split at the previous cutscenes again.
            if (_fullGameFarthestLevel > (int) Levels.Tihocan  &&
                (currentLevel == (int) Levels.QualopecCutscene ||
                 currentLevel == (int) Levels.TihocanCutscene))
            return true;

            // If the runner progresses past Qualopec, do not split at the Qualopec cutscene again.
            if (_fullGameFarthestLevel > (int) Levels.Qualopec &&
                currentLevel == (int) Levels.QualopecCutscene)
                return true;
            
            // Inspecting the normal case, this prevents a resplit at an already-complete level.
            return currentLevel < _fullGameFarthestLevel;
        }

        /// <summary>
        ///     Determines if the runner just finish a level.
        /// </summary>
        /// <param name="currentLevel">Current level</param>
        /// <param name="oldLevel">Old level</param>
        /// <returns><c>true</c> if the runner just finished a level</returns>
        private bool RunnerJustFinishedALevel(uint currentLevel, uint oldLevel)
        {
            // Deal with the levels that end in an in-game cutscene before the stats screen.
            // Splitting at the cutscene's start is desirable for both Full Game and IL RTA runs.
            if (oldLevel == (int) Levels.Qualopec && currentLevel == (int) Levels.QualopecCutscene ||
                oldLevel == (int) Levels.Tihocan  && currentLevel == (int) Levels.TihocanCutscene)
                return true;

            // Since we split at the start of these cutscenes, don't split at the ends of them.
            if (currentLevel == (int) Levels.QualopecCutscene || currentLevel == (int) Levels.TihocanCutscene)
                return false;

            // Inspect the normal case.
            return GameMemory.Data.StatsScreenIsActive.Current && !GameMemory.Data.StatsScreenIsActive.Old;
        }

        /// <summary>
        ///     Determines if the runner just did the ALT + F4 FMV skip.
        /// </summary>
        /// <param name="currentLevel">Current level</param>
        /// <returns><c>true</c> if the runner just did the FMV skip</returns>
        private bool RunnerJustDidTheFMVSkip(uint currentLevel)
        {
            // If this function were not helped by shouldIgnore, LiveSplit
            // would constantly split while the runner was restarting the game.
            // For IL RTA runs, the runner should not perform the ALT + F4 trick.
            bool isILRun = !Settings.FullGame;
            bool shouldIgnore = Settings.FullGame && _fmvSkipAlreadySplit;
            bool skipWasPerformed = currentLevel == (int) Levels.Atlantis && GameMemory.Game.HasExited;
            if (isILRun || shouldIgnore || !skipWasPerformed)
                return false;

            _fmvSkipAlreadySplit = true;
            return true;
        }

        /// <summary>
        ///     Determines if the timer should reset.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <remarks>
        ///     <c>true</c> resets, <c>false</c> does nothing.
        /// </remarks>
        /// <returns><c>true</c> if the timer should reset</returns>
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
        /// <returns><c>true</c> if the timer should start</returns>
        public bool ShouldStart(LiveSplitState state)
        {
            uint currentLevel = GameMemory.Data.Level.Current;
            uint oldLevel = GameMemory.Data.Level.Old;

            // Check if the player may have used `New Game` from the main menu.
            bool loadedCaves = currentLevel == (int) Levels.Caves && GameMemory.Data.StartGameFlag.Current == 1;
            // Avoid false positives caused by saving and loading within Caves.
            bool notAlreadyInCaves = oldLevel == (int) Levels.Manor || oldLevel == (int) Levels.TitleAndFirstFMV;
            if (loadedCaves && notAlreadyInCaves)
                return true;

            return !Settings.FullGame && ILTimerShouldStart(currentLevel, oldLevel);
        }

        /// <summary>
        ///     Determines if the timer should start for IL runs.
        /// </summary>
        /// <param name="currentLevel">Current level</param>
        /// <param name="oldLevel">Old level</param>
        /// <returns><c>true</c> if the timer should start</returns>
        private bool ILTimerShouldStart(uint currentLevel, uint oldLevel)
        {
            // Don't start in non-levels.
            if (currentLevel > (int) Levels.TheGreatPyramid)
                return false;
            // Check for a level that follows a cutscene.
            if (oldLevel > (int) Levels.TheGreatPyramid && oldLevel < (int) Levels.TitleAndFirstFMV)
                return currentLevel == (int) Levels.StFrancisFolly || 
                       currentLevel == (int) Levels.CityOfKhamoon  ||
                       currentLevel == (int) Levels.Atlantis       || 
                       currentLevel == (int) Levels.TheGreatPyramid;
            // For normal cases, check if the stats screen was closed.
            return GameMemory.Data.StatsScreenIsActive.Old && !GameMemory.Data.StatsScreenIsActive.Current;
        }

        /// <summary>
        ///     Resets values for full game runs.
        /// </summary>
        public void ResetValues()
        {
            _fullGameFarthestLevel = 1;
            _fullGameLevelTimes = new uint[NumberOfLevels];
            _fmvSkipAlreadySplit = false;
        }
    }
}
