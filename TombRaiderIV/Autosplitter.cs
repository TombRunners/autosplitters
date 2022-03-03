using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TRUtil;

namespace TR4
{
    /// <summary>The game's level and demo values.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum Level
    {
        MainMenu                = 0,
        // Cambodia
        AngkorWat               = 1,
        RacefortheIris          = 2,
        // Valley of the Kings
        TheTombofSeth           = 3,
        BurialChambers          = 4,
        ValleyoftheKings        = 5,
        KV5                     = 6,
        // Karnak
        TempleofKarnak          = 7,
        GreatHypostyleHall      = 8,
        SacredLake              = 9,
        TombofSemerkhet         = 11, // 10 is unused.
        GuardianofSemerkhet     = 12,
        // Eastern Desert
        DesertRailroad          = 13,
        // Alexandria
        Alexandria              = 14,
        CoastalRuins            = 15,
        // Temple of Isis
        PharosTempleOfIsis      = 16,
        CleopatrasPalaces       = 17,
        Catacombs               = 18,
        TempleOfPoseidon        = 19,
        // Lost Library
        TheLostLibrary          = 20,
        HallOfDemetrius         = 21,
        // Cairo
        CityOfTheDead           = 22,
        Trenches                = 23,
        ChambersOfTulun         = 24,
        StreetBazaar            = 25,
        CitadelGate             = 26,
        Citadel                 = 27,
        // Giza
        SphinxComplex           = 28,
        UnderneathTheSphinx     = 30, // 29 is unused.
        MenkauresPyramid        = 31,
        InsideMenkauresPyramid  = 32,
        TheMastabas             = 33,
        TheGreatPyramid         = 34,
        KhufusQueensPyramid     = 35,
        InsideTheGreatPyramid   = 36,
        // Temple of Horus
        TempleOfHorus           = 37,
        HorusBoss               = 38
    }

    /// <summary>The "areas" of the game.</summary>
    internal enum LevelSection
    {
        All,
        Cambodia,
        ValleyOfTheKings,
        Karnak,
        EasternDesert,
        Alexandria,
        TempleOfIsis,
        LostLibrary,
        Cairo,
        Giza,
        TempleOfHorus
    }

    /// <summary>Implementation of <see cref="LaterClassicAutosplitter"/>.</summary>
    internal sealed class Autosplitter : LaterClassicAutosplitter
    {
        private static readonly HashSet<Level> GlitchedNextSplitLevels = new HashSet<Level>()
        {
            Level.TheTombofSeth,
            Level.ValleyoftheKings,
            Level.TempleofKarnak,
            Level.DesertRailroad,
            Level.Alexandria,
            Level.CityOfTheDead,
            Level.Citadel,
            Level.SphinxComplex,
            Level.TempleOfHorus
        };

        private static readonly Dictionary<LevelSection, uint> GlitchlessAreaLevelTracker = new Dictionary<LevelSection, uint>()
        {
            {LevelSection.Cambodia, 0},
            {LevelSection.ValleyOfTheKings, 0},
            {LevelSection.Karnak, 0},
            {LevelSection.EasternDesert, 0},
            {LevelSection.Alexandria, 0},
            {LevelSection.TempleOfIsis, 0},
            {LevelSection.LostLibrary, 0},
            {LevelSection.Cairo, 0},
            {LevelSection.Giza, 0},
            {LevelSection.TempleOfHorus, 0}
        };

        private static void TrackGeneralProgress()
            => CurrentProgressEntry.Add((uint)LevelSection.All, BaseGameData.Level.Current);

        private static void TrackSectionProgress(LevelSection section, uint value)
            => CurrentProgressEntry.Add((uint)section, value);

        private static uint NextLevel = 2;

        /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
        public Autosplitter()
        {
            Settings = new ComponentSettings();

            Data = new GameData();
            Data.OnGameFound += Settings.SetGameVersion;
        }

        public override bool ShouldSplit(LiveSplitState state)
        {
            // Deathrun
            if (Settings.Deathrun)
            {
                bool laraJustDied = BaseGameData.Health.Old > 0 && BaseGameData.Health.Current <= 0;
                if (laraJustDied)
                {
                    CurrentProgressEntry.Clear();
                    CurrentProgressEntry.Add((uint)LevelSection.All, BaseGameData.Level.Current);
                }
            }

            if (Settings.Glitchless)
                return GlitchlessShouldSplit();

            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;
            bool enteringNextSplitLevel = GlitchedNextSplitLevels.Contains((Level)currentGfLevelComplete);
            bool finishedGame = currentGfLevelComplete == 39; // 39 is hardcoded to trigger credits.

            bool shouldSplit = enteringNextSplitLevel || finishedGame;
            if (shouldSplit)
            {
                CurrentProgressEntry.Clear();
                TrackGeneralProgress();
                return true;
            }
            return false;
        }

        private bool GlitchlessShouldSplit()
        {
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;

            // Normally split levels: always non-backtracked levels; sometimes "undesired" splits abitrarily skipped
            if (
                currentGfLevelComplete == NextLevel &&                            // Loading into supposed next level
                currentGfLevelComplete != 2 &&                                    // Not loading into last level of Cambodia (undesired)
                currentGfLevelComplete != 8 && currentGfLevelComplete != 9 &&     // Not loading into backtracked levels of Karnak
                (currentGfLevelComplete <= 15 || currentGfLevelComplete >= 20) && // Not loading into backtracked levels of Alexandria or Lost Library
                (currentGfLevelComplete <= 23 || currentGfLevelComplete >= 26) && // Not loading into backtracked levels of Cairo
                (currentGfLevelComplete <= 28 || currentGfLevelComplete >= 32) && // Not loading into backtracked levels of Giza
                currentGfLevelComplete != 38                                      // Not loading into boss battle (undesired)
            )
            {
                CurrentProgressEntry.Clear();
                TrackGeneralProgress();
                return true;
            }

            // Cambodia
            var currentCambodiaProgress = GlitchlessAreaLevelTracker[LevelSection.Cambodia];
            if (currentCambodiaProgress == 0 && currentLevel == 1 && currentGfLevelComplete == 2) // Angkor Wat to Race for the Iris
            {
                // Update CurrentProgressEntry.
                CurrentProgressEntry.Clear();
                TrackGeneralProgress();
                TrackSectionProgress(LevelSection.Cambodia, currentCambodiaProgress);

                // Update split logic helpers.
                GlitchlessAreaLevelTracker[LevelSection.Cambodia] = currentCambodiaProgress + 1;
                NextLevel++;

                return false; // "Undesired"
            }

            // Karnak
            var currentKarnakProgress = GlitchlessAreaLevelTracker[LevelSection.Karnak];
            if (
                (currentKarnakProgress == 1 && currentLevel == 7 && currentGfLevelComplete == 8) || // Karnak to Hypostyle
                (currentKarnakProgress == 2 && currentLevel == 8 && currentGfLevelComplete == 9) || // Hypostyle to Sacred Lake
                (currentKarnakProgress == 3 && currentLevel == 9 && currentGfLevelComplete == 7) || // Sacred Lake to Karnak
                (currentKarnakProgress == 4 && currentLevel == 7 && currentGfLevelComplete == 8) || // Karnak to Hypostyle
                (currentKarnakProgress == 5 && currentLevel == 8 && currentGfLevelComplete == 9)    // Hypostyle to Sacred Lake
            )
            {
                // Update CurrentProgressEntry.
                CurrentProgressEntry.Clear();
                TrackSectionProgress(LevelSection.Karnak, currentKarnakProgress);

                // Update split logic helpers.
                if (currentKarnakProgress == 5)
                    NextLevel = 11;
                GlitchlessAreaLevelTracker[LevelSection.Karnak] = currentKarnakProgress + 1;
                
                return currentKarnakProgress != 1; // 1st Karnak to Hypostyle "undesired"
            }

            // TODO:
            // Alexandria
            // Temple of Isis
            // LostLibrary
            // Cairo
            // Giza

            bool finishedGame = currentGfLevelComplete == 39; // 39 is hardcoded to trigger credits.
            if (finishedGame)
            {
                CurrentProgressEntry.Clear();
                TrackGeneralProgress();
                return true;
            }
            return false;
        }

        public override void OnStart()
        {
            base.OnStart();
            NextLevel = 2;
        }

        public override void OnUndoSplit()
        {
            if (ProgressTracker.Count <= 0)
                return;

            var undoneProgress = ProgressTracker.Pop(); // Glitched runs can disregard this.
            if (Settings.Glitchless)
                foreach (var item in undoneProgress)
                    GlitchlessAreaLevelTracker[(LevelSection)item.Section] = item.Value;
        }
    }
}
