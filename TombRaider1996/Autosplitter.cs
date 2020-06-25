using System;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR1
{
    /// <summary>
    ///     The game's level and cutscene values.
    /// </summary>
    internal enum Level
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
        private const uint NumberOfLevels = 15;
        
        private uint _fullGameFarthestLevel = 1;
        private uint[] _fullGameLevelTimes = new uint[NumberOfLevels];

        internal readonly ComponentSettings Settings = new ComponentSettings();
        internal GameMemory GameMemory = new GameMemory();

        /// <summary>
        ///     Determines the IGT.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>The IGT</returns>
        public TimeSpan? GetGameTime(LiveSplitState state)
        {
            const uint igtTicksPerSecond = 30;
            // We can't track time to the millisecond very accurately, which is acceptable
            // given that the stats screen reports time to the whole second anyway.
            uint levelTime = GameMemory.Data.LevelTime.Current;

            Level? lastRealLevel = GetLastRealLevel((Level) GameMemory.Data.Level.Current);
            if (lastRealLevel is null)
                return null;

            if (Settings.FullGame)
            {
                // The array is 0-indexed.
                _fullGameLevelTimes[(uint) lastRealLevel - 1] = levelTime;
                // But Take's argument is the number of elements, not an index.
                uint sumOfLevelTimes = (uint) _fullGameLevelTimes
                    .Take((int) lastRealLevel)
                    .Sum();

                return TimeSpan.FromSeconds(sumOfLevelTimes / igtTicksPerSecond);
            }
            
            return TimeSpan.FromSeconds(levelTime / igtTicksPerSecond);
        }

        /// <summary>
        ///     Gets the last non-cutscene level the runner was on.
        /// </summary>
        /// <param name="level">Level</param>
        /// <returns>The last non-cutscene level.</returns>
        private static Level? GetLastRealLevel(Level level)
        {
            if (level <= Level.TheGreatPyramid)
                return level;
            if (level == Level.QualopecCutscene)
                return Level.Qualopec;
            if (level == Level.TihocanCutscene)
                return Level.Tihocan;
            if (level == Level.MinesToAtlantis)
                return Level.NatlasMines;
            if (level == Level.AfterAtlantisFMV)
                return Level.Atlantis;
             
            return null;
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

            // Handle IL RTA-specific splitting logic first.
            if (!Settings.FullGame) 
            {
                if (oldLevel <= (uint) Levels.TheGreatPyramid && currentLevel > (uint) Levels.TheGreatPyramid)
                    return true;
            }

            // We explicitly do not split on any of these levels, because cutscenes or FMVs around them cause issues.
            if (currentLevel == (uint) Levels.Qualopec || 
                currentLevel == (uint) Levels.Tihocan  || 
                currentLevel == (uint) Levels.Atlantis || 
                currentLevel == (uint) Levels.MinesToAtlantis))
                return false;

            if (GameMemory.Data.StatsScreenIsActive.Old == 0 && GameMemory.Data.StatsScreenIsActive.Current == 1 &&
                _fullGameFarthestLevel < currentLevel)
            {
                _fullGameFarthestLevel++;
                return true;
            }
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
            return GameMemory.Data.PickedPassportPage.Current == 2;
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
            uint currentLevelTime = GameMemory.Data.LevelTime.Current;

            if (Settings.FullGame)
                return (currentLevel == Level.Caves && currentLevelTime == 0);
            else
                return (currentLevel != Level.Manor && currentLevel != TitleAndFirstFMV && currentLevelTime == 0);
        }

        /// <summary>
        ///     Resets values for full game runs.
        /// </summary>
        public void ResetValues()
        {
            _fullGameFarthestLevel = 1;
            _fullGameLevelTimes = new uint[NumberOfLevels];
        }
    }
}
