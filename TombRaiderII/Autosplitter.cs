using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;
using TRUtil;

namespace TR2
{
    /// <summary>
    ///     The game's level and demo values.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum Level
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
    internal sealed class Autosplitter : ClassicAutosplitter
    {
        private bool _newGameSelected = false;

        /// <summary>
        ///     A constructor that primarily exists to handle events/delegations and set static values.
        /// </summary>
        public Autosplitter()
        {
            Settings = new ComponentSettings();
            
            LevelCount = 18;
            CompletedLevels.Capacity = LevelCount;

            Data = new GameData();
            Data.OnGameFound += Settings.SetGameVersion;
        }

        public override bool ShouldStart(LiveSplitState state)
        {
            // Ignore demos.
            uint currentLevel = ClassicGameData.Level.Current;
            uint oldLevel = ClassicGameData.Level.Old;
            if (currentLevel >= (uint)Level.DemoVenice)
                return false;

            // Determine if a new game was started; it applies to FG runs and IL runs of the first level.
            uint oldPassportPage = ClassicGameData.PickedPassportFunction.Old;
            uint currentPassportPage = ClassicGameData.PickedPassportFunction.Current;
            if (oldPassportPage == 0 && currentPassportPage == 1)
                _newGameSelected = true;
            bool cameFromTitleScreenOrLarasHome = ClassicGameData.TitleScreen.Old && !ClassicGameData.TitleScreen.Current || oldLevel == (uint)Level.LarasHome;
            bool justStartedGreatWall = currentLevel == (uint)Level.GreatWall;
            bool newGameStarted = cameFromTitleScreenOrLarasHome && justStartedGreatWall && _newGameSelected;
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
