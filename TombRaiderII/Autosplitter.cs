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
    internal class Autosplitter : IAutoSplitter, IDisposable
    {
        private Level _farthestLevelCompleted = Level.LarasHome;

        internal readonly ComponentSettings Settings = new ComponentSettings();
        internal GameMemory GameMemory = new GameMemory();

        /// <summary>A constructor that primarily exists to handle events/delegations.</summary>
        public Autosplitter() => GameMemory.OnGameFound += Settings.SetGameVersion;

        /// <summary>
        ///     Determines the IGT.
        /// </summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>IGT as a <see cref="TimeSpan"/> if available, otherwise <see langword="null"/></returns>
        public TimeSpan? GetGameTime(LiveSplitState state)
        {
            const uint igtTicksPerSecond = 30;
            uint currentLevelTicks = GameMemory.Data.LevelTime.Current;
            var currentLevelTime = (double)currentLevelTicks / igtTicksPerSecond;

            // IL
            if (!Settings.FullGame)
                return TimeSpan.FromSeconds(currentLevelTime);

            // FG
            Level currentLevel = GameMemory.Data.Level.Current;
            int finishedLevelsTicks = 0;
            // Add up the level times stored in the game's memory.
            for (int i = 0; i < ((int)currentLevel - 1); i++)
            {
                var levelAddress = (IntPtr)(GameData.FirstLevelTimeAddress + (i * 0x2c));
                finishedLevelsTicks += GameMemory.Game.ReadValue<int>(levelAddress);
            }
            var finishedLevelsTime = (double)finishedLevelsTicks / igtTicksPerSecond;
            return TimeSpan.FromSeconds(currentLevelTime + finishedLevelsTime);
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
            Level currentLevel = GameMemory.Data.Level.Current;
            bool onCorrectLevel = _farthestLevelCompleted == currentLevel - 1;

            // Deathrun
            bool laraJustDied = GameMemory.Data.Health.Old > 0 && GameMemory.Data.Health.Current == 0;
            if (Settings.Deathrun && onCorrectLevel && laraJustDied)
                return true;
            
            // FG & IL/Section
            bool levelJustCompleted = !GameMemory.Data.LevelComplete.Old && GameMemory.Data.LevelComplete.Current;
            if (levelJustCompleted && onCorrectLevel)
            {
                _farthestLevelCompleted++;
                return true;
            }
            return false;
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
            // Ignore non-levels.
            Level currentLevel = GameMemory.Data.Level.Current;
            Level oldLevel = GameMemory.Data.Level.Old;
            if (currentLevel >= Level.DemoVenice || currentLevel == Level.LarasHome)
                return false;

            // Determine if a new game was started; it applies to FG runs and IL runs of the first level.
            uint time = GameMemory.Data.LevelTime.Current;
            bool cameFromTitleScreenOrLarasHome = GameMemory.Data.TitleScreen.Old && !GameMemory.Data.TitleScreen.Current || oldLevel == Level.LarasHome;
            bool justStartedGreatWall = currentLevel == Level.GreatWall && time == 0;
            bool newGameStarted = cameFromTitleScreenOrLarasHome && justStartedGreatWall;
            if (newGameStarted)
                return true;

            // The remaining logic only applies to non-FG runs starting on a level besides the first.
            if (Settings.FullGame)
                return false;

            bool wentToNextLevel = oldLevel == currentLevel - 1;
            if (wentToNextLevel && time == 0)
            {
                _farthestLevelCompleted = oldLevel;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Resets values for full game runs.
        /// </summary>
        public void ResetValues() => _farthestLevelCompleted = Level.LarasHome;

        public void Dispose() 
        {
            GameMemory.OnGameFound -= Settings.SetGameVersion;
            GameMemory = null;
        }
    }
}
