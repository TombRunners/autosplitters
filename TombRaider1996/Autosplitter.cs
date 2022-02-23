using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LiveSplit.Model;
using TRUtil;

namespace TR1
{
    /// <summary>The game's level, FMV, and cutscene values.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum Level
    {
        // Levels
        LarasHome        = 00,
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
    
    /// <summary>Implementation of <see cref="ClassicAutosplitter"/>.</summary>
    internal sealed class Autosplitter : ClassicAutosplitter
    {
        private bool _newGameSelected;

        /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
        public Autosplitter()
        {
            Settings = new ComponentSettings();

            LevelCount = 15;
            CompletedLevels.Capacity = LevelCount;
            GameData.CompletedLevelTicks.Capacity = LevelCount;

            Data = new GameData();
            Data.OnGameFound += Settings.SetGameVersion;
        }

        public override TimeSpan? GetGameTime(LiveSplitState state)
        {
            // Check that IGT is ticking and not reset.
            uint currentLevelTicks = ClassicGameData.LevelTime.Current;
            uint oldLevelTicks = ClassicGameData.LevelTime.Old;
            bool igtNotTicking = currentLevelTicks - oldLevelTicks == 0;
            // IGT reset: the moment when the game is restarted after a quit-out,
            // or when you transition from a level to the next,
            // or when you start a new game.
            bool igtGotReset = oldLevelTicks != 0 && currentLevelTicks == 0;
            if (igtNotTicking || igtGotReset)
                return null;

            // Check the demo isn't running.
            uint currentDemoTimer = GameData.DemoTimer.Current;
            const uint demoStartThreshold = 480;
            if (currentDemoTimer > demoStartThreshold)
                return null;

            uint currentLevel = ClassicGameData.Level.Current;
            Level? lastRealLevel = GetLastRealLevel((Level)currentLevel);
            if (lastRealLevel is null)
                return null;

            // Sum the current and completed levels' IGT.
            double currentLevelTime = ClassicGameData.LevelTimeAsDouble(currentLevelTicks);
            double finishedLevelsTime = Data.SumCompletedLevelTimes(CompletedLevels, currentLevel);
            return TimeSpan.FromSeconds(currentLevelTime + finishedLevelsTime);
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

        public override bool ShouldStart(LiveSplitState state)
        {
            // Determine if a new game was started; it applies to FG runs and IL runs of the first level.
            uint currentLevel = ClassicGameData.Level.Current;
            uint oldLevel = ClassicGameData.Level.Old;
            uint oldPassportPage = ClassicGameData.PickedPassportFunction.Old;
            uint currentPassportPage = ClassicGameData.PickedPassportFunction.Current;
            if (oldPassportPage == 0 && currentPassportPage == 1)
                _newGameSelected = true;
            bool cameFromTitleScreenOrLarasHome = ClassicGameData.TitleScreen.Old && !ClassicGameData.TitleScreen.Current || oldLevel == (uint)Level.LarasHome;
            bool justStartedCaves = currentLevel == (uint)Level.Caves;
            bool newGameStarted = cameFromTitleScreenOrLarasHome && justStartedCaves && _newGameSelected;
            if (newGameStarted)
                return true;

            // The remaining logic only applies to non-FG runs starting on a level besides the first.
            uint oldTime = ClassicGameData.LevelTime.Old;
            uint currentTime = ClassicGameData.LevelTime.Current;
            bool wentToNextLevel = oldLevel == currentLevel - 1;
            return !Settings.FullGame && wentToNextLevel && oldTime > currentTime;
        }

        public override void OnStart()
        {
            GameData.CompletedLevelTicks.Clear();
            _newGameSelected = false;
            base.OnStart();
        }

        public override void OnSplit(uint completedLevel)
        {
            GameData.CompletedLevelTicks.Add(ClassicGameData.LevelTime.Current);
            base.OnSplit(completedLevel);
        }

        public override void OnUndoSplit()
        {
            GameData.CompletedLevelTicks.RemoveAt(GameData.CompletedLevelTicks.Count - 1);
            base.OnUndoSplit();
        }
    }
}
