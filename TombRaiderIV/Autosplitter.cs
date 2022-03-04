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
                (currentKarnakProgress == 0 && currentLevel == 7 && currentGfLevelComplete == 8) || // Karnak to Hypostyle
                (currentKarnakProgress == 1 && currentLevel == 8 && currentGfLevelComplete == 9) || // Hypostyle to Sacred Lake
                (currentKarnakProgress == 2 && currentLevel == 9 && currentGfLevelComplete == 7) || // Sacred Lake to Karnak
                (currentKarnakProgress == 3 && currentLevel == 7 && currentGfLevelComplete == 8) || // Karnak to Hypostyle
                (currentKarnakProgress == 4 && currentLevel == 8 && currentGfLevelComplete == 9)    // Hypostyle to Sacred Lake
            )
            {
                // Update CurrentProgressEntry.
                CurrentProgressEntry.Clear();
                TrackSectionProgress(LevelSection.Karnak, currentKarnakProgress);

                // Update split logic helpers.
                if (currentKarnakProgress == 4)
                    NextLevel = 11;
                GlitchlessAreaLevelTracker[LevelSection.Karnak] = currentKarnakProgress + 1;
                
                return currentKarnakProgress != 1; // 1st Karnak to Hypostyle "undesired"
            }

            // Alexandria
            var currentAlexandriaProgress = GlitchlessAreaLevelTracker[LevelSection.Alexandria];
            if (
                (currentAlexandriaProgress == 0 && currentLevel == 14 && currentGfLevelComplete == 15) || // Alexandria to Coastal
                (currentAlexandriaProgress == 1 && currentLevel == 15 && currentGfLevelComplete == 18) || // Coastal to Catacombs
                (currentAlexandriaProgress == 2 && currentLevel == 18 && currentGfLevelComplete == 15) || // Catacombs to Coastal
                (currentAlexandriaProgress == 3 && currentLevel == 15 && currentGfLevelComplete == 18) || // Coastal to Catacombs
                (currentAlexandriaProgress == 4 && currentLevel == 18 && currentGfLevelComplete == 15) || // Catacombs to Coastal
                (currentAlexandriaProgress == 5 && currentLevel == 15 && currentGfLevelComplete == 18) || // Coastal to Catacombs
                (currentAlexandriaProgress == 6 && currentLevel == 18 && currentGfLevelComplete == 19)    // Catacombs to Poseidon
            )
            {
                // Update CurrentProgressEntry.
                CurrentProgressEntry.Clear();
                TrackSectionProgress(LevelSection.Alexandria, currentAlexandriaProgress);

                // Update split logic helpers.
                if (currentAlexandriaProgress == 5)
                    NextLevel = 19;
                GlitchlessAreaLevelTracker[LevelSection.Alexandria] = currentAlexandriaProgress + 1;

                // All but 3rd Coastal to Catacombs and Catacombs to Posideon "undesired"
                return currentAlexandriaProgress == 6 || currentAlexandriaProgress == 7;
            }

            // Temple of Isis
            var currentTempleOfIsisProgress = GlitchlessAreaLevelTracker[LevelSection.TempleOfIsis];
            if (
                (currentTempleOfIsisProgress == 0 && currentLevel == 16 && currentGfLevelComplete == 17) || // Isis to Cleopatra
                (currentTempleOfIsisProgress == 1 && currentLevel == 17 && currentGfLevelComplete == 16) || // Cleopatra to Isis
                (currentTempleOfIsisProgress == 2 && currentLevel == 16 && currentGfLevelComplete == 17)    // Isis to Cleopatra
            )
            {
                bool split = currentTempleOfIsisProgress == 2;

                // Update CurrentProgressEntry.
                CurrentProgressEntry.Clear();
                TrackSectionProgress(LevelSection.TempleOfIsis, currentTempleOfIsisProgress);

                // Update split logic helpers.
                if (split)
                    NextLevel = 22;
                GlitchlessAreaLevelTracker[LevelSection.TempleOfIsis] = currentTempleOfIsisProgress + 1;

                // All but 2nd Isis to Cleopatra "undesired"
                return split;
            }

            // Lost Library
            var currentLostLibraryProgress = GlitchlessAreaLevelTracker[LevelSection.LostLibrary];
            if (
                (currentLostLibraryProgress == 0 && currentLevel == 19 && currentGfLevelComplete == 20) || // Poseidon to Lost Library
                (currentLostLibraryProgress == 1 && currentLevel == 20 && currentGfLevelComplete == 19)    // Lost Library to Poseidon
            )
            {
                // Update CurrentProgressEntry.
                CurrentProgressEntry.Clear();
                TrackSectionProgress(LevelSection.LostLibrary, currentLostLibraryProgress);

                // Update split logic helpers.
                GlitchlessAreaLevelTracker[LevelSection.LostLibrary] = currentLostLibraryProgress + 1;
                
                return true;
            }

            // Cairo
            var currentCairoProgress = GlitchlessAreaLevelTracker[LevelSection.Cairo];
            if (
                (currentCairoProgress == 0 && currentLevel == 22 && currentGfLevelComplete == 24) || // City to Tulun
                (currentCairoProgress == 1 && currentLevel == 24 && currentGfLevelComplete == 23) || // Tulun to Trenches
                (currentCairoProgress == 2 && currentLevel == 23 && currentGfLevelComplete == 25) || // Trenches to Street Bazaar
                (currentCairoProgress == 3 && currentLevel == 25 && currentGfLevelComplete == 23) || // Street Bazaar to Trenches 
                (currentCairoProgress == 4 && currentLevel == 23 && currentGfLevelComplete == 25) || // Trenches to Street Bazaar
                (currentCairoProgress == 5 && currentLevel == 25 && currentGfLevelComplete == 23) || // Street Bazaar to Trenches
                (currentCairoProgress == 6 && currentLevel == 23 && currentGfLevelComplete == 26) || // Trenches to Citadel Gate
                (currentCairoProgress == 7 && currentLevel == 26 && currentGfLevelComplete == 27)    // Citadel Gate to Citadel
            )
            {
                // Update CurrentProgressEntry.
                CurrentProgressEntry.Clear();
                TrackSectionProgress(LevelSection.Cairo, currentCairoProgress);
                
                // Update split logic helpers.
                if (currentCairoProgress == 7)
                    NextLevel = 28;
                GlitchlessAreaLevelTracker[LevelSection.Cairo] = currentCairoProgress + 1;

                // Trenches "undesired" except first entering and last leaving Street Bazaar
                return currentCairoProgress == 0 || currentCairoProgress == 2 || currentCairoProgress == 5 || currentCairoProgress == 7;
            }   

            // Giza
            var currentGizaProgress = GlitchlessAreaLevelTracker[LevelSection.Giza];
            if (
                (currentGizaProgress == 0 && currentLevel == 27 && currentGfLevelComplete == 28) || // Citadel to Sphinx Complex
                (currentGizaProgress == 1 && currentLevel == 28 && currentGfLevelComplete == 30) || // Sphinx Complex to Underneath the Sphinx
                (currentGizaProgress == 2 && currentLevel == 30 && currentGfLevelComplete == 31) || // Underneath the Sphinx to Menkaure's Pyramid
                (currentGizaProgress == 3 && currentLevel == 31 && currentGfLevelComplete == 32) || // Menkaure's Pyramid to Inside Menkaure's Pyramid
                (currentGizaProgress == 4 && currentLevel == 32 && currentGfLevelComplete == 28)    // Inside Menkaure's Pyramid to Sphinx Complex
            )
            {
                // Update CurrentProgressEntry.
                CurrentProgressEntry.Clear();
                TrackSectionProgress(LevelSection.Giza, currentGizaProgress);

                // Update split logic helpers.
                if (currentGizaProgress == 4)
                    NextLevel = 323;
                GlitchlessAreaLevelTracker[LevelSection.Giza] = currentGizaProgress + 1;

                // Inside Menkaure's Pyramid to Sphinx Complex "undesired"
                return currentGizaProgress != 4;;
            }

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
