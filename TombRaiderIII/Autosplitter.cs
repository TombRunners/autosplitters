using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI.Components.AutoSplit;

namespace TR3
{
    /// <summary>
    ///     The game's level and demo values.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum Level : uint
    {
        LarasHome = 00,
        // India
        Jungle = 01,
        TempleRuins = 02,
        TheRiverGanges = 03,
        CavesOfKaliya = 04,
        // South Pacific
        CoastalVillage = 05,
        CrashSite = 06,
        MadubuGorge = 07,
        TempleOfPuna = 08,
        // London
        ThamesWharf = 09,
        Aldwych = 10,
        LudsGate = 11,
        City = 12,
        // Nevada
        NevadaDesert = 13,
        HighSecurityCompound = 14,
        Area51 = 15,
        // Antarctica
        Antarctica = 16,
        RxTechMines = 17,
        LostCityOfTinnos = 18,
        MeteoriteCavern = 19,
        // Bonus (All Secrets)
        AllHallows = 20
    }

    /// <summary>
    ///     Implementation of <see cref="IAutoSplitter"/> for an <see cref="AutoSplitComponent"/>'s use.
    /// </summary>
    internal class Autosplitter : IAutoSplitter, IDisposable
    {
        private const int IgtTicksPerSecond = 30;
        private const int LevelCount = 20; // This considers Lara's Home as 0 and includes the bonus level.

        private readonly List<Level> _completedLevels = new List<Level>(LevelCount);

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
            // Check that IGT is ticking.
            uint currentLevelTicks = GameMemory.Data.LevelTime.Current;
            uint oldLevelTicks = GameMemory.Data.LevelTime.Old;
            if (currentLevelTicks - oldLevelTicks == 0)
                return null;

            // TR3's IGT ticks during globe level selection; the saved end-level IGT is unaffected, thus the overall FG IGT is also unaffected.
            // If a runner is watching LS's IGT, this may confuse them, despite it being a non-issue for the level/FG IGT.
            // To prevent the ticks from showing in LS, we use the fact that LevelComplete isn't reset to 0 until the next level is loaded.
            Level currentLevel = GameMemory.Data.Level.Current;
            bool oldLevelComplete = GameMemory.Data.LevelComplete.Old;
            bool currentLevelComplete = GameMemory.Data.LevelComplete.Current;
            bool levelStillComplete = oldLevelComplete == currentLevelComplete == true;
            if (_completedLevels.Contains(currentLevel) && levelStillComplete)
                return null;

            // Sum the current and completed levels' IGT.
            double currentLevelTime = (double)currentLevelTicks / IgtTicksPerSecond;
            return TimeSpan.FromSeconds(currentLevelTime + SumCompletedLevelTimes());
        }

        /// <summary>
        ///     Sums completed levels' times.
        /// </summary>
        /// <returns>The sum of completed levels' times as a double.</returns>
        /// <remarks>
        ///     Because TR3's level order is variable and we must maintain compatibility with Section and NG+ runs,
        ///     we read addresses based on <c>_completedLevels</c>.
        /// </remarks>
        private double SumCompletedLevelTimes()
        {
            const int levelSaveStructSize = 0x33;

            // Add up the level times of interest from the game's memory.
            var finishedLevelsTicks = 0;
            foreach (Level completedLevel in _completedLevels)
            {
                if (completedLevel == GameMemory.Data.Level.Current)
                    break;
                int levelOffset = ((int)completedLevel - 1) * levelSaveStructSize; // - 1 because FirstLevelTimeAddress is already on the first address
                var levelAddress = (IntPtr)(GameData.FirstLevelTimeAddress + levelOffset);
                finishedLevelsTicks += GameMemory.Game.ReadValue<int>(levelAddress);
            }
            return (double)finishedLevelsTicks / IgtTicksPerSecond;
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
            // Determine if the player is on the correct level to split; if not, we stop.
            Level currentLevel = GameMemory.Data.Level.Current;
            bool onCorrectLevelToSplit = !_completedLevels.Contains(currentLevel);
            if (!onCorrectLevelToSplit)
                return false;

            // Deathrun
            bool laraJustDied = GameMemory.Data.Health.Old > 0 && GameMemory.Data.Health.Current == 0;
            if (Settings.Deathrun && laraJustDied)
                return true;
            
            // FG & IL/Section
            bool levelJustCompleted = !GameMemory.Data.LevelComplete.Old && GameMemory.Data.LevelComplete.Current;
            return levelJustCompleted;
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
            uint oldLevelTime = GameMemory.Data.LevelTime.Old;
            uint currentLevelTime = GameMemory.Data.LevelTime.Current;
            uint currentPickedPassportFunction = GameMemory.Data.PickedPassportFunction.Current;
            bool oldTitleScreen = GameMemory.Data.TitleScreen.Old;

            // Perform new game logic first, since it is the only place where FG should start.
            bool levelTimeJustStarted = oldLevelTime == 0 && currentLevelTime != 0 && currentLevelTime < 50;
            bool newGameStarted = levelTimeJustStarted && currentPickedPassportFunction == 1;
            if (newGameStarted)
                return true;

            return !Settings.FullGame && levelTimeJustStarted && !oldTitleScreen;
        }

        /// <summary>
        ///     On <see cref="LiveSplitState.OnStart"/>, updates values.
        /// </summary>
        public void OnStart() => _completedLevels.Clear();

        /// <summary>
        ///     On <see cref="LiveSplitState.OnSplit"/>, updates values.
        /// </summary>
        public void OnSplit() => _completedLevels.Add(GameMemory.Data.Level.Current);

        /// <summary>
        ///     On <see cref="LiveSplitState.OnUndoSplit"/>, updates values.
        /// </summary>
        public void OnUndoSplit() => _completedLevels.RemoveAt(_completedLevels.Count - 1);
        
        public void Dispose() 
        {
            GameMemory.OnGameFound -= Settings.SetGameVersion;
            GameMemory = null;
        }
    }
}
