using System;
using System.Diagnostics.CodeAnalysis;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;
using TRUtil;

namespace TR2Gold
{
    /// <summary>The game's level and demo values.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum Level : uint
    {
        // Levels
        LarasHome = 00,
        TheColdWar = 01,
        FoolsGold = 02,
        FurnaceOfTheGods = 03,
        Kingdom = 04,
        // Bonus
        NightmareInVegas = 05
    }

    /// <summary>Implementation of <see cref="ClassicAutosplitter"/>.</summary>
    internal sealed class Autosplitter : ClassicAutosplitter
    {
        private bool _newGameSelected;

        /// <summary>A constructor that primarily exists to handle events/delegations.</summary>
        public Autosplitter()
        {
            Settings = new ComponentSettings();

            LevelCount = 5;
            CompletedLevels.Capacity = LevelCount;

            Data = new GameData();
            Data.OnGameFound += Settings.SetGameVersion;
        }

        public override bool ShouldStart(LiveSplitState state)
        {
            uint currentLevel = ClassicGameData.Level.Current;
            uint oldLevel = ClassicGameData.Level.Old;

            // Determine if a new game was started; it applies to FG runs and IL runs of the first level.
            uint oldPassportPage = ClassicGameData.PickedPassportFunction.Old;
            uint currentPassportPage = ClassicGameData.PickedPassportFunction.Current;
            if (oldPassportPage == 0 && currentPassportPage == 1)
                _newGameSelected = true;
            bool cameFromTitleScreenOrLarasHome = ClassicGameData.TitleScreen.Old && !ClassicGameData.TitleScreen.Current || oldLevel == (uint)Level.LarasHome;
            bool justStartedColdWar = currentLevel == (uint)Level.TheColdWar;
            bool newGameStarted = cameFromTitleScreenOrLarasHome && justStartedColdWar && _newGameSelected;
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
            _newGameSelected = false;
            base.OnStart();
        }
    }
}
