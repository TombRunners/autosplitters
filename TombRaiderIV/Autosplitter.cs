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
        All                 = Level.MainMenu,
        Cambodia            = Level.AngkorWat,
        ValleyOfTheKings    = Level.TheTombofSeth,
        Karnak              = Level.TempleofKarnak,
        EasternDesert       = Level.DesertRailroad,
        Alexandria          = Level.Alexandria,
        Cairo               = Level.CityOfTheDead,
        Giza                = Level.SphinxComplex
    }

    /// <summary>Implementation of <see cref="LaterClassicAutosplitter"/>.</summary>
    internal sealed class Autosplitter : LaterClassicAutosplitter
    {
        private static readonly HashSet<Level> GlitchedNextSplitLevels = new HashSet<Level>()
        {
            Level.TheTombofSeth,
            Level.ValleyoftheKings,
            Level.TempleofKarnak,
            Level.TombofSemerkhet,
            Level.DesertRailroad,
            Level.Alexandria,
            Level.CityOfTheDead,
            Level.Citadel,
            Level.SphinxComplex,
            Level.TempleOfHorus
        };

        private static readonly Dictionary<LevelSection, uint> SectionProgressEntries = new Dictionary<LevelSection, uint>()
        {
            {LevelSection.All,              0},
            {LevelSection.Cambodia,         0},
            {LevelSection.ValleyOfTheKings, 0},
            {LevelSection.Karnak,           0},
            {LevelSection.EasternDesert,    0},
            {LevelSection.Alexandria,       0},
            {LevelSection.Cairo,            0},
            {LevelSection.Giza,             0}
        };

        private static void TrackGeneralProgress() => CurrentProgressEntry.Add((uint)LevelSection.All, BaseGameData.Level.Current);

        private static void TrackSectionProgress(LevelSection section, uint value) => CurrentProgressEntry.Add((uint)section, value);

        private static void TrackGeneralAndSectionProgress(LevelSection section, uint value)
        {
            TrackGeneralProgress();
            TrackSectionProgress(section, value);
        }

        private static uint PossiblyTheNextLevel => SectionProgressEntries[LevelSection.All] + 1;

        /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
        public Autosplitter()
        {
            Settings = new ComponentSettings();

            Data = new GameData();
            Data.OnGameFound += Settings.SetGameVersion;
        }

        public override bool ShouldSplit(LiveSplitState state)
        {
            // Handle deathruns (for both rulesets).
            if (Settings.Deathrun)
            {
                bool laraJustDied = BaseGameData.Health.Old > 0 && BaseGameData.Health.Current <= 0;
                if (laraJustDied)
                {
                    TrackGeneralProgress();
                    return true;
                }
                return false;
            }

            // Prevent double-splits; applies to ILs and FG for both glitched and glitchless.
            uint oldGfLevelComplete = LaterClassicGameData.GfLevelComplete.Old;
            if (oldGfLevelComplete != 0)
                return false; 

            // Handle ILs (for both rulesets).
            if (!Settings.FullGame)
                return LaterClassicGameData.GfLevelComplete.Current != 0;

            // Handle FG for glitchless.
            if (Settings.Glitchless)
                return GlitchlessShouldSplit();

            // Handle FG for glitched.
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;
            bool enteringNextSplitLevel = GlitchedNextSplitLevels.Contains((Level)currentGfLevelComplete);
            bool finishedGame = currentGfLevelComplete == 39; // 39 is hardcoded to trigger credits.

            bool shouldSplit = enteringNextSplitLevel || finishedGame;
            if (shouldSplit)
                TrackGeneralProgress();
            return shouldSplit;
        }

        private bool GlitchlessShouldSplit()
        {
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;

            // Normally split levels: always non-backtracked levels; sometimes "undesired" splits abitrarily skipped
            if (
                currentGfLevelComplete == PossiblyTheNextLevel &&               // Loading into supposed next level
                currentGfLevelComplete != 02 &&                                 // Not loading into last level of Cambodia (undesired)
                currentGfLevelComplete != 08 && currentGfLevelComplete != 09 && // Not loading into backtracked levels of Karnak
                (currentGfLevelComplete < 15 || currentGfLevelComplete > 18) && // Not loading into backtracked levels of Alexandria or Lost Library
                (currentGfLevelComplete < 23 || currentGfLevelComplete > 26) && // Not loading into backtracked levels of Cairo
                (currentGfLevelComplete < 28 || currentGfLevelComplete > 32) && // Not loading into backtracked levels of Giza
                currentGfLevelComplete != 38                                    // Not loading into boss battle (undesired)
            )
            {
                TrackGeneralProgress();
                return true;
            }

            // Cambodia
            var currentCambodiaProgress = SectionProgressEntries[LevelSection.Cambodia];
            if (currentCambodiaProgress == 00 && currentLevel == 01 && currentGfLevelComplete == 02) // Angkor Wat to Race for the Iris
            {
                // Update CurrentProgressEntry.
                TrackGeneralAndSectionProgress(LevelSection.Cambodia, currentCambodiaProgress);

                // Update split logic helpers.
                SectionProgressEntries[LevelSection.Cambodia] = currentCambodiaProgress + 1;

                return false; // "Undesired"
            }

            // Karnak
            var currentKarnakProgress = SectionProgressEntries[LevelSection.Karnak];
            if (
                (currentKarnakProgress == 00 && currentLevel == 07 && currentGfLevelComplete == 08) || // Karnak to Hypostyle
                (currentKarnakProgress == 01 && currentLevel == 08 && currentGfLevelComplete == 09) || // Hypostyle to Sacred Lake
                (currentKarnakProgress == 02 && currentLevel == 09 && currentGfLevelComplete == 07) || // Sacred Lake to Karnak
                (currentKarnakProgress == 03 && currentLevel == 07 && currentGfLevelComplete == 08) || // Karnak to Hypostyle
                (currentKarnakProgress == 04 && currentLevel == 08 && currentGfLevelComplete == 09)    // Hypostyle to Sacred Lake
            )
            {
                // Update CurrentProgressEntry.
                TrackGeneralAndSectionProgress(LevelSection.Karnak, currentKarnakProgress);

                // Update split logic helpers.
                SectionProgressEntries[LevelSection.Karnak]++;
                
                return currentKarnakProgress != 01; // 1st Karnak to Hypostyle "undesired"
            }

            // Alexandria
            var currentAlexandriaProgress = SectionProgressEntries[LevelSection.Alexandria];
            if (
                (currentAlexandriaProgress == 00 && currentLevel == 14 && currentGfLevelComplete == 15) || // Alexandria to Coastal
                (currentAlexandriaProgress == 01 && currentLevel == 15 && currentGfLevelComplete == 18) || // Coastal to Catacombs
                (currentAlexandriaProgress == 02 && currentLevel == 18 && currentGfLevelComplete == 15) || // Catacombs to Coastal
                (currentAlexandriaProgress == 03 && currentLevel == 15 && currentGfLevelComplete == 18) || // Coastal to Catacombs
                (currentAlexandriaProgress == 04 && currentLevel == 18 && currentGfLevelComplete == 15) || // Catacombs to Coastal
                (currentAlexandriaProgress == 05 && currentLevel == 15 && currentGfLevelComplete == 18) || // Coastal to Catacombs
                (currentAlexandriaProgress == 06 && currentLevel == 18 && currentGfLevelComplete == 19) || // Catacombs to Poseidon
                (currentAlexandriaProgress == 07 && currentLevel == 19 && currentGfLevelComplete == 20) || // Poseidon to Lost Library
                (currentAlexandriaProgress == 08 && currentLevel == 16 && currentGfLevelComplete == 17) || // Isis to Cleopatra
                (currentAlexandriaProgress == 09 && currentLevel == 17 && currentGfLevelComplete == 16) || // Cleopatra to Isis
                (currentAlexandriaProgress == 10 && currentLevel == 16 && currentGfLevelComplete == 17)    // Isis to Cleopatra
            )
            {
                // Update CurrentProgressEntry.
                TrackGeneralAndSectionProgress(LevelSection.Alexandria, currentAlexandriaProgress);

                // Update split logic helpers.
                SectionProgressEntries[LevelSection.Alexandria]++;

                // To/from Coastal and Isis-Cleopatra backtracking "undesired"
                return currentAlexandriaProgress > 05 && currentAlexandriaProgress != 08 && currentAlexandriaProgress != 09;
            }

            // Cairo
            var currentCairoProgress = SectionProgressEntries[LevelSection.Cairo];
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
                TrackGeneralAndSectionProgress(LevelSection.Cairo, currentCairoProgress);
                
                // Update split logic helpers.
                SectionProgressEntries[LevelSection.Cairo]++;

                // Trenches "undesired" except first entering and last leaving Street Bazaar
                return currentCairoProgress == 0 || currentCairoProgress == 2 || currentCairoProgress == 5 || currentCairoProgress == 7;
            }   

            // Giza
            var currentGizaProgress = SectionProgressEntries[LevelSection.Giza];
            if (
                (currentGizaProgress == 0 && currentLevel == 27 && currentGfLevelComplete == 28) || // Citadel to Sphinx Complex
                (currentGizaProgress == 1 && currentLevel == 28 && currentGfLevelComplete == 30) || // Sphinx Complex to Underneath the Sphinx
                (currentGizaProgress == 2 && currentLevel == 30 && currentGfLevelComplete == 31) || // Underneath the Sphinx to Menkaure's Pyramid
                (currentGizaProgress == 3 && currentLevel == 31 && currentGfLevelComplete == 32) || // Menkaure's Pyramid to Inside Menkaure's Pyramid
                (currentGizaProgress == 4 && currentLevel == 32 && currentGfLevelComplete == 28)    // Inside Menkaure's Pyramid to Sphinx Complex
            )
            {
                // Update CurrentProgressEntry.
                TrackGeneralAndSectionProgress(LevelSection.Giza, currentGizaProgress);

                // Update split logic helpers.
                SectionProgressEntries[LevelSection.Giza]++;

                // Inside Menkaure's Pyramid to Sphinx Complex "undesired"
                return currentGizaProgress != 4;
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
            foreach (var key in SectionProgressEntries.Keys)
                SectionProgressEntries[key] = 0;
            SectionProgressEntries[LevelSection.All] = BaseGameData.Level.Current;
        }

        public override void OnUndoSplit()
        {
            if (ProgressTracker.Count <= 0)
                return;

            var undoneProgress = ProgressTracker.Pop(); // Glitched runs can disregard this.
            if (Settings.Glitchless)
                foreach (var item in undoneProgress)
                if (item.Section != (uint)LevelSection.All)
                        SectionProgressEntries[(LevelSection)item.Section] = item.Value;
        }
    }
}
