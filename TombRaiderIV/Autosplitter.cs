using LiveSplit.Model;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TRUtil;

namespace TR4
{
    /// <summary>The game's level and demo values.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum Tr4Level
    {
        MainMenu                = 0,
        // Cambodia
        AngkorWat               = 1,
        RaceForTheIris          = 2,
        // Valley of the Kings
        TheTombOfSeth           = 3,
        BurialChambers          = 4,
        ValleyOfTheKings        = 5,
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
        PharosTempleOfIsis      = 16,
        CleopatrasPalaces       = 17,
        Catacombs               = 18,
        TempleOfPoseidon        = 19,
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

    /// <summary>The game's level and demo values.</summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal enum TteLevel
    {
        MainMenu          = 0,
        Office            = 1, // Cutscene
        // Playable Level
        TheTimesExclusive = 2  // At the end of the level, gfLevelComplete is set to 39 to trigger credits.
    }

    /// <summary>The "areas" of the game.</summary>
    internal enum Tr4LevelSection
    {
        Cambodia            = Tr4Level.AngkorWat,
        ValleyOfTheKings    = Tr4Level.TheTombOfSeth,
        Karnak              = Tr4Level.TempleofKarnak,
        EasternDesert       = Tr4Level.DesertRailroad,
        Alexandria          = Tr4Level.Alexandria,
        Cairo               = Tr4Level.CityOfTheDead,
        Giza                = Tr4Level.SphinxComplex
    }

    /// <summary>Implementation of <see cref="LaterClassicAutosplitter"/>.</summary>
    internal sealed class Autosplitter : LaterClassicAutosplitter
    {
        private static readonly HashSet<Tr4Level> GlitchedNextSplitLevels = new HashSet<Tr4Level>()
        {
            Tr4Level.TheTombOfSeth,
            Tr4Level.ValleyOfTheKings,
            Tr4Level.TempleofKarnak,
            Tr4Level.TombofSemerkhet,
            Tr4Level.DesertRailroad,
            Tr4Level.Alexandria,
            Tr4Level.CityOfTheDead,
            Tr4Level.Citadel,
            Tr4Level.SphinxComplex,
            Tr4Level.TempleOfHorus
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
            // Handle deathruns for both rulesets.
            if (Settings.Deathrun && !LaterClassicGameData.Loading.Current)
            {
                bool laraJustDied = BaseGameData.Health.Old > 0 && BaseGameData.Health.Current <= 0;
                return laraJustDied;
            }

            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;
            uint oldGfLevelComplete = LaterClassicGameData.GfLevelComplete.Old;

            // Prevent double-splits; applies to ILs and FG for both glitched and glitchless.
            bool ignoringSubsequentFramesOfThisLoadState = currentGfLevelComplete == oldGfLevelComplete;
            if (ignoringSubsequentFramesOfThisLoadState)
                return false;

            // In the case of The Times Exclusive, there is only one playable level with a value of 2;
            // the main menu is 0, and the opening cutscene has a level value of 1.
            bool playingTheTimesExclusive = BaseGameData.Version == (uint)GameVersion.TheTimesExclusive;
            if (playingTheTimesExclusive && BaseGameData.Level.Current != (uint)TteLevel.TheTimesExclusive)
                return false;

            // Handle ILs for both rulesets.            
            if (!Settings.FullGame || playingTheTimesExclusive)
            {
                // This assumes all level transitions are desirable splits.
                bool loadingAnotherLevel = currentGfLevelComplete != 0;
                return loadingAnotherLevel;
            }

            // Handle FG for glitchless (TR4 only).
            if (Settings.Option && !playingTheTimesExclusive)
                return GlitchlessShouldSplit();

            // Handle FG for glitched.
            bool enteringNextSplitLevel = GlitchedNextSplitLevels.Contains((Tr4Level)currentGfLevelComplete);
            bool finishedGame = currentGfLevelComplete == 39; // 39 is hardcoded to trigger credits for both TR4 and TTE.
            return enteringNextSplitLevel || finishedGame;
        }

        private bool GlitchlessShouldSplit()
        {
            uint currentLevel = BaseGameData.Level.Current;
            if (currentLevel >= (uint)Tr4LevelSection.Giza)
                return GlitchlessShouldSplitGiza();
            else if (currentLevel >= (uint)Tr4LevelSection.Cairo)
                return GlitchlessShouldSplitCairo();
            else if (currentLevel >= (uint)Tr4LevelSection.Alexandria)
                return GlitchlessShouldSplitAlexandria();
            else if (currentLevel >= (uint)Tr4LevelSection.EasternDesert)
                return LaterClassicGameData.GfLevelComplete.Current == (uint)Tr4Level.Alexandria;
            else if (currentLevel >= (uint)Tr4LevelSection.Karnak)
                return GlitchlessShouldSplitKarnak();
            else if (currentLevel >= (uint)Tr4LevelSection.ValleyOfTheKings)
                return GlitchlessShouldSplitValleyOfTheKings();
            else
                return GlitchlessShouldSplitCambodia();
        }

        private bool GlitchlessShouldSplitCambodia()
        {
            /* Route
                Transition 00 | currentLevel == 00 && currentGfLevelComplete == 01 | Main Menu to Angkor Wat (covered by ShouldStart)
                Transition 01 | currentLevel == 01 && currentGfLevelComplete == 02 | Angkor Wat to Race for the Iris (x)
                Transition 02 | currentLevel == 02 && currentGfLevelComplete == 03 | Race for the Iris to The Tomb of Seth
            */
            /* Default undesired splits
                Loading into Race for the Iris (2)
            */
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;

            bool loadingIntoNextLevel = currentGfLevelComplete == currentLevel + 1;
            bool loadingRaceForTheIris = currentGfLevelComplete == (uint)Tr4Level.RaceForTheIris;
            return loadingIntoNextLevel && !loadingRaceForTheIris;
        }

        private bool GlitchlessShouldSplitValleyOfTheKings()
        {
            /* Route
                Transition 00 | currentLevel == 02 && currentGfLevelComplete == 03 | Race for the Iris to Tomb of Seth (covered elsewhere)
                Transition 01 | currentLevel == 03 && currentGfLevelComplete == 04 | Tomb of Seth to Burial Chambers
                Transition 02 | currentLevel == 04 && currentGfLevelComplete == 05 | Burial Chambers to Valley of the Kings
                Transition 03 | currentLevel == 05 && currentGfLevelComplete == 06 | Valley of the Kings to KV5
                Transition 04 | currentLevel == 06 && currentGfLevelComplete == 07 | KV5 to Karnak
            */
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;

            return currentGfLevelComplete == currentLevel + 1;
        }

        private bool GlitchlessShouldSplitKarnak()
        {
            /* Likely route
                Transition 00 | currentLevel == 06 && currentGfLevelComplete == 07 | KV5 to Karnak (covered elsewhere)
                Transition 01 | currentLevel == 07 && currentGfLevelComplete == 08 | Karnak to Hypostyle (x)
                Transition 02 | currentLevel == 08 && currentGfLevelComplete == 09 | Hypostyle to Sacred Lake
                Transition 03 | currentLevel == 09 && currentGfLevelComplete == 07 | Sacred Lake to Karnak
                Transition 04 | currentLevel == 07 && currentGfLevelComplete == 08 | Karnak to Hypostyle
                Transition 05 | currentLevel == 08 && currentGfLevelComplete == 09 | Hypostyle to Sacred Lake
                Transition 05 | currentLevel == 09 && currentGfLevelComplete == 11 | Sacred Lake to Tomb of Semerkhet
                Transition 06 | currentLevel == 11 && currentGfLevelComplete == 12 | Tomb of Semerkhet to Guardian of Semerkhet
                Transition 07 | currentLevel == 12 && currentGfLevelComplete == 13 | Guardian of Semerkhet to Desert Railroad
            */
            /* Default undesired splits
                The Great Hypostyle Hall (08):
                    Only split when Lara enters with required progression item/parts.
            */
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;

            bool loadingFromKarnakToHypostyle = currentLevel == (uint)Tr4Level.TempleofKarnak && currentGfLevelComplete == (uint)Tr4Level.GreatHypostyleHall;
            if (loadingFromKarnakToHypostyle)
            {
                bool laraHasHypostyleKey = (GameData.KeyItems.Current & 0b0000_0010) == 0b0000_0010;
                return laraHasHypostyleKey;
            }

            bool loadingNextLevel = currentGfLevelComplete > currentLevel;
            bool circlingBackToTheBeginning = currentLevel == (uint)Tr4Level.SacredLake && currentGfLevelComplete == (uint)Tr4Level.TempleofKarnak;
            return loadingNextLevel || circlingBackToTheBeginning;
        }

        private bool GlitchlessShouldSplitAlexandria()
        {
            /* Possible route
                Transition 00 | currentLevel == 13 && currentGfLevelComplete == 14 | Desert Railroad to Alexandria (covered elsewhere)
                Transition 01 | currentLevel == 14 && currentGfLevelComplete == 15 | Alexandria to Coastal (x)
                Transition 02 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs (x)
                Transition 03 | currentLevel == 18 && currentGfLevelComplete == 15 | Catacombs to Coastal (x)
                Transition 04 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs (x)
                Transition 05 | currentLevel == 18 && currentGfLevelComplete == 15 | Catacombs to Coastal (x)
                Transition 06 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs
                Transition 07 | currentLevel == 18 && currentGfLevelComplete == 19 | Catacombs to Poseidon
                Transition 08 | currentLevel == 19 && currentGfLevelComplete == 20 | Poseidon to Lost Library
                Transition 09 | currentLevel == 20 && currentGfLevelComplete == 21 | Lost Library to Hall of Demetrius (x)
                Transition 10 | currentLevel == 21 && currentGfLevelComplete == 15 | Hall of Demetrius to Coastal (x)
                Transition 11 | currentLevel == 15 && currentGfLevelComplete == 16 | Coastal to Pharos
                Transition 12 | currentLevel == 16 && currentGfLevelComplete == 17 | Pharos to Cleopatra (x)
                Transition 13 | currentLevel == 17 && currentGfLevelComplete == 16 | Cleopatra to Pharos (x)
                Transition 14 | currentLevel == 16 && currentGfLevelComplete == 17 | Pharos to Cleopatra
                Transition 15 | currentLevel == 17 && currentGfLevelComplete == 22 | Cleopatra to City of the Dead
            */
            /* Default undesired splits
                Coastal Ruins (15):
                    Never split when entering; only split when leaving to load Catacombs for the last time or Pharos, Temple of Isis
                Hall of Demetrius (21)
                Cleopatra's Palaces (17):
                    Only split when Lara enters with required progression item/parts.
            */
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;
            
            bool finishedLoadingCatacombs = currentLevel == (uint)Tr4Level.Catacombs && LaterClassicGameData.Loading.Old && !LaterClassicGameData.Loading.Current;
            if (finishedLoadingCatacombs)
            {
                // The level must finish loading before the ITEM_INFO array can be checked.
                ItemInfo platform = GameData.GetItemInfoAtIndex(79);
                return platform.flags == 0x20;
            }

            bool loadingFromCoastalToPharos = currentLevel == (uint)Tr4Level.CoastalRuins && currentGfLevelComplete == (uint)Tr4Level.PharosTempleOfIsis;
            if (loadingFromCoastalToPharos)
                return true;

            bool loadingFromPharosToCleopatra = currentLevel == (uint)Tr4Level.PharosTempleOfIsis && currentGfLevelComplete == (uint)Tr4Level.CleopatrasPalaces;
            if (loadingFromPharosToCleopatra)
            {
                bool laraHasCombinedClockworkBeetle = (GameData.MechanicalScarab.Current & 0b0000_0001) == 0b0000_0001;
                bool laraHasBothClockworkBeetleParts = (GameData.MechanicalScarab.Current & 0b0000_0110) == 0b0000_0110;
                return laraHasCombinedClockworkBeetle || laraHasBothClockworkBeetleParts;
            }

            bool loadingFromCatacombsToPoseidon = currentLevel == (uint)Tr4Level.Catacombs && currentGfLevelComplete == (uint)Tr4Level.TempleOfPoseidon;
            bool loadingFromPoseidonToLostLibrary = currentLevel == (uint)Tr4Level.TempleOfPoseidon && currentGfLevelComplete == (uint)Tr4Level.TheLostLibrary;
            bool loadingCityOfTheDead = currentGfLevelComplete == (uint)Tr4Level.CityOfTheDead;
            return loadingFromCatacombsToPoseidon || loadingFromPoseidonToLostLibrary || loadingCityOfTheDead;
        }

        private bool GlitchlessShouldSplitCairo()
        {
            /* Likely route
                Transition 00 | currentLevel == 17 && currentGfLevelComplete == 22 | Cleopatra's Palaces to City (covered elsewhere)
                Transition 01 | currentLevel == 22 && currentGfLevelComplete == 24 | City to Tulun
                Transition 02 | currentLevel == 24 && currentGfLevelComplete == 23 | Tulun to Trenches (x)
                Transition 03 | currentLevel == 23 && currentGfLevelComplete == 25 | Trenches to Street Bazaar
                Transition 04 | currentLevel == 25 && currentGfLevelComplete == 23 | Street Bazaar to Trenches (x)
                Transition 05 | currentLevel == 23 && currentGfLevelComplete == 25 | Trenches to Street Bazaar (x)
                Transition 06 | currentLevel == 25 && currentGfLevelComplete == 23 | Street Bazaar to Trenches
                Transition 07 | currentLevel == 23 && currentGfLevelComplete == 26 | Trenches to Citadel Gate (x)
                Transition 08 | currentLevel == 26 && currentGfLevelComplete == 27 | Citadel Gate to Citadel
                Transition 09 | currentLevel == 27 && currentGfLevelComplete == 28 | Citadel to Sphinx Complex
            */
            /* Default undesired splits
                Trenches (23):
                    Only split when first entering Street Bazaar (25), and when last leaving Street Bazaar (25) with required progression items
            */
            uint currentLevel = BaseGameData.Level.Current;
            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;

            bool loadingFromTrenchesToStreetBazaar = currentLevel == (uint)Tr4Level.Trenches && currentGfLevelComplete == (uint)Tr4Level.StreetBazaar;
            if (loadingFromTrenchesToStreetBazaar)
            {
                bool laraHasDetonatorBody = (GameData.PuzzleItemsCombo.Current & 0b0100_0000_0000_0000) == 0b0100_0000_0000_0000;
                return !laraHasDetonatorBody;
            }
            
            bool loadingFromStreetBazaarToTrenches = currentLevel == (uint)Tr4Level.StreetBazaar && currentGfLevelComplete == (uint)Tr4Level.Trenches;
            if (loadingFromStreetBazaarToTrenches)
            {
                bool laraHasCombinedDetonator = GameData.PuzzleItems.Current.MineDetonator == 1;
                bool laraHasDetonatorParts = (GameData.PuzzleItemsCombo.Current & 0b1100_0000_0000_0000) == 0b1100_0000_0000_0000;
                return laraHasCombinedDetonator || laraHasDetonatorParts;
            }

            bool loadingFromCityToTulun = currentLevel == (uint)Tr4Level.CityOfTheDead && currentGfLevelComplete == (uint)Tr4Level.ChambersOfTulun;
            bool loadingFromCitadelGateToCitadel = currentLevel == (uint)Tr4Level.CitadelGate && currentGfLevelComplete == (uint)Tr4Level.Citadel;
            bool loadingFromCitadelToSphinxComplex = currentLevel == (uint)Tr4Level.SphinxComplex && currentGfLevelComplete == (uint)Tr4Level.Citadel;

            return loadingFromCityToTulun || loadingFromCitadelGateToCitadel || loadingFromCitadelToSphinxComplex;
        }

        private bool GlitchlessShouldSplitGiza()
        {
            /* Likely route
                Transition 00 | currentLevel == 27 && currentGfLevelComplete == 28 | Citadel to Sphinx Complex (covered elsewhere)
                Transition 01 | currentLevel == 28 && currentGfLevelComplete == 30 | Sphinx Complex to Underneath the Sphinx
                Transition 02 | currentLevel == 30 && currentGfLevelComplete == 31 | Underneath the Sphinx to Menkaure's Pyramid
                Transition 03 | currentLevel == 31 && currentGfLevelComplete == 32 | Menkaure's Pyramid to Inside Menkaure's Pyramid
                Transition 04 | currentLevel == 32 && currentGfLevelComplete == 28 | Inside Menkaure's Pyramid to Sphinx Complex (x)
                Transition 05 | currentLevel == 28 && currentGfLevelComplete == 33 | Sphinx Complex to Mastabas
                Transition 06 | currentLevel == 33 && currentGfLevelComplete == 34 | Mastabas to The Great Pyramid
                Transition 07 | currentLevel == 34 && currentGfLevelComplete == 35 | The Great Pyramid to Khufu's Queen's Pyramids
                Transition 08 | currentLevel == 35 && currentGfLevelComplete == 36 | Khufu's Queen's Pyramids to Inside the Great Pyramid (*)
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

            bool justFinishedLoadingALevel = currentGfLevelComplete == 0;
            if (justFinishedLoadingALevel)
                return false;

            bool currentLevelIsBeforeMastabas = currentLevel < (uint)Tr4Level.TheMastabas;
            if (currentLevelIsBeforeMastabas)
            {
                bool loadingFromInsideMenkauresToSphinx = currentLevel == (uint)Tr4Level.InsideMenkauresPyramid && currentGfLevelComplete == (uint)Tr4Level.SphinxComplex;
                return !loadingFromInsideMenkauresToSphinx;
            }
            
            bool loadingInsideTheGreatPyramid = currentGfLevelComplete == (uint)Tr4Level.InsideTheGreatPyramid;
            if (loadingInsideTheGreatPyramid)
            {
                bool laraHasEasternShaftKey = GameData.PuzzleItems.Current.EasternShaftKey == 1;
                return laraHasEasternShaftKey;
            }

            bool loadingNextLevel = currentGfLevelComplete > currentLevel;
            bool loadingBoss = currentGfLevelComplete == (uint)Tr4Level.HorusBoss;
            return loadingNextLevel && !loadingBoss;
        }
    }
}
