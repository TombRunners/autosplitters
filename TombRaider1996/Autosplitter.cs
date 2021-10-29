using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TR1
{
    /// <summary>
    ///     The game's level and cutscene values.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum Level : uint
    {
        // Levels
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
    internal class Autosplitter : IAutoSplitter, IDisposable
    {
        private const uint NumberOfLevels = 15;
        
        private Level _farthestLevelCompleted = Level.Manor;
        private uint[] _fullGameLevelTimes = new uint[NumberOfLevels];
        private bool newGameSelected = false;

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
            uint oldTime = GameMemory.Data.LevelTime.Old;
            uint currentTime = GameMemory.Data.LevelTime.Current;
            uint currentDemoTimer = GameMemory.Data.DemoTimer.Current;
            const uint demoStartThreshold = 480;

            bool igtIsNotTicking = oldTime == currentTime;
            // IGT reset: the moment when the game is restarted after a quit-out,
            // or when you transition from a level to the next,
            // or when you start new game.
            bool igtGotReset = oldTime != 0 && currentTime == 0;
            Level? lastRealLevel = GetLastRealLevel(GameMemory.Data.Level.Current);
            if (igtIsNotTicking || igtGotReset || currentDemoTimer > demoStartThreshold || lastRealLevel is null)
                return null;
            
            const uint igtTicksPerSecond = 30;
            if (!Settings.FullGame) 
                return TimeSpan.FromSeconds((double) currentTime / igtTicksPerSecond);

            // The array is 0-indexed.
            _fullGameLevelTimes[(uint) lastRealLevel - 1] = currentTime;
            // But Take's argument is the number of elements, not an index.
            uint sumOfLevelTimes = (uint) _fullGameLevelTimes
                .Take((int) lastRealLevel)
                .Sum(x => x);
            return TimeSpan.FromSeconds((double) sumOfLevelTimes / igtTicksPerSecond);
        }

        /// <summary>
        ///     Gets the last non-cutscene <see cref="Level"/> the runner was on.
        /// </summary>
        /// <param name="level"><see cref="Level"/></param>
        /// <returns>The last non-cutscene <see cref="Level"/></returns>
        private static Level? GetLastRealLevel(Level level)
        {
            if (level <= Level.TheGreatPyramid)
                return level;
            
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (level)
            {
                case Level.QualopecCutscene:
                    return Level.Qualopec;
                case Level.TihocanCutscene:
                    return Level.Tihocan;
                case Level.MinesToAtlantis:
                    return Level.NatlasMines;
                case Level.AfterAtlantisFMV:
                    return Level.Atlantis;
                default:
                    return null;
            }
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
            /* It is hypothetically reasonable to use _farthestLevelCompleted to reset
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
            Level currentLevel = GameMemory.Data.Level.Current;
            Level oldLevel = GameMemory.Data.Level.Old;
            uint oldPassportPage = GameMemory.Data.PickedPassportFunction.Old;
            uint currentPassportPage = GameMemory.Data.PickedPassportFunction.Current;
            bool oldLevelComplete = GameMemory.Data.LevelComplete.Old;
            bool currentLevelComplete = GameMemory.Data.LevelComplete.Current;
            bool titleScreen = GameMemory.Data.TitleScreen.Current;

            if (oldPassportPage == 0 && currentPassportPage == 1 && (titleScreen || currentLevel == Level.Manor))
                newGameSelected = true;

            // When the game process starts, currentLevel is initialized as Caves and IGT as 0.
            // Thus the code also checks if the user picked New Game from the passport.
            // A new game starts from the New Game page of the passport (page 2, index 1).
            bool goingToFirstLevel = oldLevel != Level.Caves && currentLevel == Level.Caves && newGameSelected;
            if (goingToFirstLevel)
                return true;

            if (Settings.FullGame) 
                return false;

            // IL RTA logic
            Level? oldRealLevel = GetLastRealLevel(oldLevel);
            Level? currentRealLevel = GetLastRealLevel(currentLevel);
            if (oldRealLevel is null || currentRealLevel is null)
                return false;

            bool goingToNextRealLevel = oldRealLevel == currentRealLevel - 1 && oldLevelComplete && !currentLevelComplete;
            if (goingToNextRealLevel)
            {
                _farthestLevelCompleted = (Level) oldRealLevel;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Resets values for full game runs.
        /// </summary>
        public void ResetValues()
        {
            _farthestLevelCompleted = Level.Manor;
            _fullGameLevelTimes = new uint[NumberOfLevels];
            newGameSelected = false;
        }

        public void Dispose()
        {
            GameMemory.OnGameFound -= Settings.SetGameVersion;
            GameMemory = null;
        }
    }
}
