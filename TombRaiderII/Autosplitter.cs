using System;
using System.Diagnostics.CodeAnalysis;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR2
{
    /// <summary>
    ///     The game's level and demo values.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum Level : uint
    {
        // Levels
        LarasHome = 00,
        GreatWall = 01,
        Venice = 02,
        BartolisHideout = 03,
        OperaHouse = 04,
        OffshoreRig = 05,
        DivingArea = 06,
        FortyFathoms = 07,
        WreckOfTheMariaDoria = 08,
        LivingQuarters = 09,
        TheDeck = 10,
        TibetanFoothills = 11,
        BarkhangMonastery = 12,
        CatacombsOfTheTalion = 13,
        IcePalace = 14,
        TempleOfXian = 15,
        FloatingIslands = 16,
        DragonsLair = 17,
        HomeSweetHome = 18,
        // Demos
        DemoVenice = 19,
        DemoWreckOfTheMariaDoria = 20,
        DemoTibetanFoothills = 21
    }

    /// <summary>
    ///     Implementation of <see cref="IAutoSplitter"/> for an <see cref="AutoSplitComponent"/>'s use.
    /// </summary>
    internal class Autosplitter : IAutoSplitter
    {
        private Level _fullGameFarthestLevel = Level.LarasHome;

        internal readonly ComponentSettings Settings = new ComponentSettings();
        internal GameMemory GameMemory = new GameMemory();

        /// <summary>
        ///     Determines the IGT.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>IGT as a <see cref="TimeSpan"/> if available, otherwise <see langword="null"/></returns>
        public TimeSpan? GetGameTime(LiveSplitState state)
        {
            const uint igtTicksPerSecond = 30;
            uint currentTime = GameMemory.Data.LevelTime.Current;
            Level currentLevel = GameMemory.Data.Level.Current;
            
            // IL
            if (!Settings.FullGame)
                return TimeSpan.FromSeconds((double)currentTime / igtTicksPerSecond);

            // FG
            int finishedLevelsTime = 0;
            for (int i = 0; i < ((int)currentLevel - 1); ++i)
                finishedLevelsTime += GameMemory.Game.ReadValue<int>((IntPtr)(GameData.FirstLevelTimeAddress + (i * 0x2c)));
            return TimeSpan.FromSeconds((double)finishedLevelsTime / igtTicksPerSecond);
        }

        /// <summary>
        ///     Determines if IGT should be paused when the game is quit or <see cref="GetGameTime"/> returns <see langword="null"/>
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if IGT should be paused, <see langword="false"/> otherwise</returns>
        public bool IsGameTimePaused(LiveSplitState state) => true;

        /// <summary>
        ///     Determines if the timer should split.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should split, <see langword="false"/> otherwise</returns>
        public bool ShouldSplit(LiveSplitState state)
        {
            // Deathrun
            bool laraJustDied = GameMemory.Data.Health.Old > 0 && GameMemory.Data.Health.Current == 0;
            if (Settings.Deathrun && laraJustDied)
                return true;
            
            // IL
            Level currentLevel = GameMemory.Data.Level.Current;
            bool levelJustCompleted = !GameMemory.Data.LevelComplete.Old && GameMemory.Data.LevelComplete.Current;
            if (!Settings.FullGame)
                // Assuming the runner only has one split in the layout.
                // If not, this causes multiple splits on levels ending with an in-game cutscene.
                return levelJustCompleted;
            
            // FG
            if (!levelJustCompleted || _fullGameFarthestLevel != currentLevel - 1)
                return false;
            _fullGameFarthestLevel++;
            return true;
        }

        /// <summary>
        ///     Determines if the timer should reset.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should reset, <see langword="false"/> otherwise</returns>
        public bool ShouldReset(LiveSplitState state)
        {
            /* It is hypothetically reasonable to use _fullGameFarthestLevel to reset
             * if the player loads into a level ahead of their current level.
             * However, considering a case where a runner accidentally loads an incorrect
             * save after dying, it's clear that this should be avoided.
             */
            return GameMemory.Data.PickedPassportFunction.Current == 2;
        }

        /// <summary>
        ///     Determines if the timer should start.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should start, <see langword="false"/> otherwise</returns>
        public bool ShouldStart(LiveSplitState state)
        {
            // First, determine if a new game was started; it applies to FG runs and IL runs of the first level.
            Level currentLevel = GameMemory.Data.Level.Current;
            Level oldLevel = GameMemory.Data.Level.Old;
            uint time = GameMemory.Data.LevelTime.Current;
            // The New Game page of the passport (page 2, index 1) can be accessed from the title screen or Lara's Home.
            uint passportPage = GameMemory.Data.PickedPassportFunction.Current;
            bool cameFromTitleScreenOrLarasHome = GameMemory.Data.TitleScreen.Old && !GameMemory.Data.TitleScreen.Current || oldLevel == Level.LarasHome;
            bool newGameStarted = cameFromTitleScreenOrLarasHome && currentLevel == Level.GreatWall && time == 0 && passportPage == 1;
            if (newGameStarted)
                return true;

            // The remaining logic only applies to IL runs.
            if (Settings.FullGame)  
                return false;
            // Ignore non-levels
            if (currentLevel >= Level.DemoVenice || currentLevel == Level.LarasHome)
                return false;
            // LevelComplete is insufficient because it throws false positives at the start of end-level in-game cutscenes.
            bool leftCompletedLevelOrStartedCutscene = GameMemory.Data.LevelComplete.Old && !GameMemory.Data.LevelComplete.Current;
            bool wentToNextLevel = oldLevel == currentLevel - 1;
            bool startingNextLevel = leftCompletedLevelOrStartedCutscene && wentToNextLevel;
            return startingNextLevel;
        }

        /// <summary>
        ///     Resets values for full game runs.
        /// </summary>
        public void ResetValues()
        {
            _fullGameFarthestLevel = Level.LarasHome;
        }
    }
}
