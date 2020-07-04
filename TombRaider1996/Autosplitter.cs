using System;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR1
{
    /// <summary>
    ///     The game's level and cutscene values.
    /// </summary>
    internal enum Level : uint
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
        // Cutscenes and title screen
        QualopecCutscene = 16,
        TihocanCutscene  = 17,
        MinesToAtlantis  = 18,
        AfterAtlantisFMV = 19,
        TitleAndFirstFMV = 20
    }
    
    /// <summary>
    ///     Implementation of <see cref="IAutoSplitter"/> for an <see cref="AutoSplitComponent"/>'s use.
    /// </summary>
    internal class Autosplitter : IAutoSplitter
    {
        private const uint NumberOfLevels = 15;
        
        private Level _fullGameFarthestLevel = Level.Manor;
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
            uint oldTime = GameMemory.Data.LevelTime.Old;
            uint currentTime = GameMemory.Data.LevelTime.Current;
            uint currentDemoTimer = GameMemory.Data.DemoTimer.Current;

            bool igtIsNotTicking = oldTime == currentTime;
            // IGT reset: the moment when the game is restarted after a quitout,
            // or when you transition from a level to the next,
            // or when you start new game.
            bool igtGotReset = oldTime != 0 && currentTime == 0;
            if (igtIsNotTicking || igtGotReset || currentDemoTimer > 480)
                return null;

            Level? lastRealLevel = GetLastRealLevel(GameMemory.Data.Level.Current);
            if (lastRealLevel is null)
                return null;

            const uint igtTicksPerSecond = 30;

            if (Settings.FullGame)
            {
                // The array is 0-indexed.
                _fullGameLevelTimes[(uint) lastRealLevel - 1] = currentTime;
                // But Take's argument is the number of elements, not an index.
                uint sumOfLevelTimes = (uint) _fullGameLevelTimes
                    .Take((int) lastRealLevel)
                    .Sum(x => x);

                return TimeSpan.FromSeconds((double) sumOfLevelTimes / igtTicksPerSecond);
            }
            
            return TimeSpan.FromSeconds((double) currentTime / igtTicksPerSecond);
        }

        /// <summary>
        ///     Gets the last non-cutscene <see cref="Level"/> the runner was on.
        /// </summary>
        /// <param name="level"><see cref="Level"/></param>
        /// <returns>The last non-cutscene <see cref="Level"/>.</returns>
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
        ///     Used to control whether Game Time is paused if the game is quit, or if GetGameTime returned null.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>If true is returned, Game Time is paused when the game is quit (or if GGT returned null), return false and GT will keep running after quitting (or when GGT returned null).</returns>
        public bool IsGameTimePaused(LiveSplitState state) => true;

        /// <summary>
        ///     Determines if the timer should split.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should split, <see langword="false"/> otherwise.</returns>
        public bool ShouldSplit(LiveSplitState state)
        {
            Level currentLevel = GameMemory.Data.Level.Current;
            Level oldLevel = GameMemory.Data.Level.Old;
            bool levelJustCompleted = !GameMemory.Data.LevelComplete.Old && GameMemory.Data.LevelComplete.Current;

            // Handle IL RTA-specific splitting logic first.
            if (!Settings.FullGame)
            {
                // Assuming the runner only has one split in the layout.
                // If not, this causes multiple splits on levels ending with a cutscene.
                return levelJustCompleted;
            }

            // Do not split on these levels because cutscenes/FMVs around them cause issues.
            if (currentLevel == Level.Qualopec || 
                currentLevel == Level.Tihocan  || 
                currentLevel == Level.Atlantis || 
                currentLevel == Level.MinesToAtlantis)
                return false;

            if (levelJustCompleted && _fullGameFarthestLevel < currentLevel)
            {
                _fullGameFarthestLevel++;
                return true;
            }
            
            return false;
        }

        /// <summary>
        ///     Determines if the timer should reset.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should reset, <see langword="false"/> otherwise.</returns>
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
        /// <returns><see langword="true"/> if the timer should start, <see langword="false"/> otherwise.</returns>
        public bool ShouldStart(LiveSplitState state)
        {
            Level currentLevel = GameMemory.Data.Level.Current;
            Level oldLevel = GameMemory.Data.Level.Old;
            uint currentLevelTime = GameMemory.Data.LevelTime.Current;
            uint passportPage = GameMemory.Data.PickedPassportPage.Current;
            bool oldLevelComplete = GameMemory.Data.LevelComplete.Old;
            bool currentLevelComplete = GameMemory.Data.LevelComplete.Current;

            // When the game process starts, currentLevel is initialized as Caves and IGT as 0.
            // Thus the code also checks if the user picked New Game from the passport.
            // A new game starts from the New Game page of the passport (page 2, index 1).
            bool goingToFirstLevel = oldLevel != Level.Caves && currentLevel == Level.Caves && passportPage == 1;
            if (goingToFirstLevel)
                return true;

            if (!Settings.FullGame)
            {
                Level? oldRealLevel = GetLastRealLevel(oldLevel);
                Level? currentRealLevel = GetLastRealLevel(currentLevel);
                if ((oldRealLevel is null) || (currentRealLevel is null))
                    return false;

                bool goingToNextRealLevel = oldRealLevel == currentRealLevel - 1 && oldLevelComplete && !currentLevelComplete;

                if (goingToNextRealLevel)
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Resets values for full game runs.
        /// </summary>
        public void ResetValues()
        {
            _fullGameFarthestLevel = Level.Manor;
            _fullGameLevelTimes = new uint[NumberOfLevels];
        }
    }
}
