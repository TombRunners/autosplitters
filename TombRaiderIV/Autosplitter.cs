using LiveSplit.Model;
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
                return laraJustDied;
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

            return enteringNextSplitLevel || finishedGame;
        }

        private bool OldGlitchlessShouldSplit()
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
                return true;
            }

            // Cambodia
            var currentCambodiaProgress = SectionProgressEntries[LevelSection.Cambodia];
            if (currentCambodiaProgress == 00 && currentLevel == 01 && currentGfLevelComplete == 02) // Angkor Wat to Race for the Iris
            {
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
                return currentKarnakProgress != 00; // 1st Karnak to Hypostyle "undesired"
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
                TrackGeneralAndSectionProgress(currentCairoProgress);
                
                // Update split logic helpers.
                SectionProgressEntries[LevelSection.Cairo]++;

                // Trenches "undesired" except first entering and last leaving Street Bazaar
                return currentCairoProgress == 0 || currentCairoProgress == 2 || currentCairoProgress == 5 || currentCairoProgress == 7;
            }   

            return false;
        }

        private bool GlitchlessShouldSplit()
        {
            uint currentLevel = BaseGameData.Level.Current;
            if (currentLevel >= (uint)LevelSection.Giza)
                return GlitchlessShouldSplitGiza();
            else if (currentLevel >= (uint)LevelSection.Cairo)
                return GlitchlessShouldSplitCairo();
            else if (currentLevel >= (uint)LevelSection.Alexandria)
                return GlitchlessShouldSplitAlexandria();
            else if (currentLevel >= (uint)LevelSection.EasternDesert)
                return LaterClassicGameData.GfLevelComplete.Current == (uint)Level.Alexandria;
            else if (currentLevel >= (uint)LevelSection.Karnak)
                return GlitchlessShouldSplitKarnak();
            else if (currentLevel >= (uint)LevelSection.ValleyOfTheKings)
                return GlitchlessShouldSplitValleyOfTheKings();
            else
                return GlitchlessShouldSplitCambodia();
        }

        private bool GlitchlessShouldSplitCambodia()
        {
            throw new System.NotImplementedException();
        }

        private bool GlitchlessShouldSplitValleyOfTheKings()
        {
            throw new System.NotImplementedException();
        }

        private bool GlitchlessShouldSplitKarnak()
        {
            throw new System.NotImplementedException();
        }

        private bool GlitchlessShouldSplitAlexandria()
        {
            /* Likely route
                Transition 00 | currentLevel == 14 && currentGfLevelComplete == 15 | Alexandria to Coastal
                Transition 01 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs
                Transition 02 | currentLevel == 18 && currentGfLevelComplete == 15 | Catacombs to Coastal
                Transition 03 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs
                Transition 04 | currentLevel == 18 && currentGfLevelComplete == 15 | Catacombs to Coastal
                Transition 05 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs
                Transition 06 | currentLevel == 18 && currentGfLevelComplete == 19 | Catacombs to Poseidon
                Transition 07 | currentLevel == 19 && currentGfLevelComplete == 20 | Poseidon to Lost Library
                Transition 08 | currentLevel == 16 && currentGfLevelComplete == 17 | Isis to Cleopatra
                Transition 09 | currentLevel == 17 && currentGfLevelComplete == 16 | Cleopatra to Isis
                Transition 10 | currentLevel == 16 && currentGfLevelComplete == 17 | Isis to Cleopatra
            */
            /* Undesired splits
                To/from Coastal Ruins (15)
                Cleopatra's Palaces (17):
                    Only split when Lara enters Cleopatra's Palaces with required progression item/parts.
            */
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;

            bool loadingCleopatraFromPharos = currentLevel == (uint)Level.PharosTempleOfIsis && currentGfLevelComplete == (uint)Level.CleopatrasPalaces;
            if (loadingCleopatraFromPharos)
            {
                bool laraHasCombinedClockworkBeetle = (GameData.MechanicalScarab.Current & 0b0000_0001) == 0b0000_0001;
                bool laraHasBothClockworkBeetleParts = (GameData.MechanicalScarab.Current & 0b0000_0110) == 0b0000_0110;
                return laraHasCombinedClockworkBeetle || laraHasBothClockworkBeetleParts;
            }

            bool loadingPoseidonFromCatacombs = currentLevel == (uint)Level.Catacombs && currentGfLevelComplete == (uint)Level.TempleOfPoseidon;
            bool loadingLostLibraryFromPoseidon = currentLevel == (uint)Level.TempleOfPoseidon && currentGfLevelComplete == (uint)Level.TheLostLibrary;
            return loadingPoseidonFromCatacombs || loadingLostLibraryFromPoseidon;
        }

        private bool GlitchlessShouldSplitCairo()
        {
            throw new System.NotImplementedException();
        }

        private bool GlitchlessShouldSplitGiza()
        {
            /* Likely route
                Transition 00 | currentLevel == 27 && currentGfLevelComplete == 28 | Citadel to Sphinx Complex
                Transition 01 | currentLevel == 28 && currentGfLevelComplete == 30 | Sphinx Complex to Underneath the Sphinx
                Transition 02 | currentLevel == 30 && currentGfLevelComplete == 31 | Underneath the Sphinx to Menkaure's Pyramid
                Transition 03 | currentLevel == 31 && currentGfLevelComplete == 32 | Menkaure's Pyramid to Inside Menkaure's Pyramid
                Transition 04 | currentLevel == 32 && currentGfLevelComplete == 28 | Inside Menkaure's Pyramid to Sphinx Complex
                Transition 05 | currentLevel == 28 && currentGfLevelComplete == 33 | Sphinx Complex to Mastabas
                Transition 06 | currentLevel == 33 && currentGfLevelComplete == 34 | Mastabas to The Great Pyramid
                Transition 07 | currentLevel == 34 && currentGfLevelComplete == 35 | The Great Pyramid to Khufu's Queen's Pyramids
                Transition 08 | currentLevel == 35 && currentGfLevelComplete == 36 | Khufu's Queen's Pyramids to Inside the Great Pyramid
                Transition 09 | currentLevel == 36 && currentGfLevelComplete == 37 | Inside the Great Pyramid to Temple of Horus
                Transition 10 | currentLevel == 37 && currentGfLevelComplete == 38 | Temple of Horus to Boss
                Transition 11 | currentLevel == 38 && currentGfLevelComplete == 39 | Boss to Credits (value of 39 is hardcoded to trigger credits)
            */
            /* Undesired splits
                Small backtrack through Sphinx Complex (28) to get to The Mastabas (33).
                Loading into Inside the Great Pyramid (36):
                    Only split when Lara enters with required progression item/parts.
                Loading into boss (38)
            */
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;

            if (currentLevel < (uint)Level.TheMastabas)
            {
                bool loadingSphinxFromMenkaures = currentLevel == (uint)Level.InsideMenkauresPyramid && currentGfLevelComplete == (uint)Level.SphinxComplex;
                return !loadingSphinxFromMenkaures;
            }
            else if (currentGfLevelComplete == (uint)Level.InsideTheGreatPyramid)
            {
                bool laraHasEasternShaftKey = GameData.EasternShaftKey.Current == 1;
                return laraHasEasternShaftKey;
            }

            bool loadingIntoBoss = currentGfLevelComplete == 38;
            return !loadingIntoBoss;
        }
    }
}
