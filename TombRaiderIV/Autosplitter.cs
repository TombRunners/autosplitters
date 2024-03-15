using System;
using System.Collections.Generic;
using LiveSplit.Model;
using TRUtil;

namespace TR4;

/// <summary>Implementation of <see cref="LaterClassicAutosplitter"/>.</summary>
internal sealed class Autosplitter : LaterClassicAutosplitter
{
    private static readonly HashSet<Tr4Level> GlitchedNextSplitLevels =
    [
        Tr4Level.TheTombOfSeth,
        Tr4Level.ValleyOfTheKings,
        Tr4Level.TempleOfKarnak,
        Tr4Level.TombOfSemerkhet,
        Tr4Level.DesertRailroad,
        Tr4Level.Alexandria,
        Tr4Level.CityOfTheDead,
        Tr4Level.Citadel,
        Tr4Level.SphinxComplex,
        Tr4Level.TempleOfHorus,
    ];

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter(Version version) : base(version)
    {
        Settings = new ComponentSettings(version);

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
        {
            var currentLevel = (Tr4Level)BaseGameData.Level.Current;
            // Below bool is never true for The Times Exclusive; its level values never match these TR4 levels.
            bool specialExceptionForGlitchlessPostLoadSplits =
                Settings.FullGame && currentLevel is Tr4Level.Catacombs;
            return specialExceptionForGlitchlessPostLoadSplits && GlitchlessShouldSplit();
        }

        // In the case of The Times Exclusive, there is only one playable level with a value of 2;
        // the main menu is 0, and the opening cutscene has a level value of 1.
        bool playingTheTimesExclusive = BaseGameData.Version == (uint)GameVersion.TheTimesExclusive;
        if (playingTheTimesExclusive && BaseGameData.Level.Current != (uint)TteLevel.TheTimesExclusive)
            return false;

        // Handle all of TTE as well as TR4 ILs for both rulesets.
        if (!Settings.FullGame || playingTheTimesExclusive)
        {
            // This assumes all level transitions are desirable splits.
            bool loadingAnotherLevel = currentGfLevelComplete != 0;
            return loadingAnotherLevel;
        }

        // Handle FG for each ruleset (TR4 only).
        bool glitchless = Settings.Option;
        return glitchless ? GlitchlessShouldSplit() : GlitchedShouldSplit();
    }

    private static bool GlitchedShouldSplit()
    {
        uint oldGfLevelComplete = LaterClassicGameData.GfLevelComplete.Old;
        bool leavingUnusedLevelValue = oldGfLevelComplete is 10 or 29;
        if (leavingUnusedLevelValue)
            return false;

        uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;
        bool enteringUnusedLevelValue = currentGfLevelComplete is 10 or 29;
        bool enteringNextSplitLevel = GlitchedNextSplitLevels.Contains((Tr4Level) currentGfLevelComplete);
        bool finishedGame = currentGfLevelComplete == 39; // 39 is hardcoded to trigger credits for both TR4 and TTE.
        return enteringUnusedLevelValue || enteringNextSplitLevel || finishedGame;
    }

    private static bool GlitchlessShouldSplit()
        => BaseGameData.Level.Current switch
        {
            >= (uint) Tr4LevelSection.Giza             => GlitchlessShouldSplitGiza(),
            >= (uint) Tr4LevelSection.Cairo            => GlitchlessShouldSplitCairo(),
            >= (uint) Tr4LevelSection.Alexandria       => GlitchlessShouldSplitAlexandria(),
            >= (uint) Tr4LevelSection.EasternDesert    => GlitchlessShouldSplitEasternDesert(),
            >= (uint) Tr4LevelSection.Karnak           => GlitchlessShouldSplitKarnak(),
            >= (uint) Tr4LevelSection.ValleyOfTheKings => GlitchlessShouldSplitValleyOfTheKings(),
            _                                          => GlitchlessShouldSplitCambodia(),
        };

    private static bool GlitchlessShouldSplitCambodia()
    {
        /* Route
            Transition 00 | currentLevel == 00 && currentGfLevelComplete == 01 | Main Menu to Angkor Wat (covered by ShouldStart).
            Transition 01 | currentLevel == 01 && currentGfLevelComplete == 02 | Angkor Wat to Race for the Iris (x).
            Transition 02 | currentLevel == 02 && currentGfLevelComplete == 03 | Race for the Iris to The Tomb of Seth.
        */
        /* Default undesired splits
            Loading into Race for the Iris (2)
        */
        var currentLevel = (Tr4Level)BaseGameData.Level.Current;
        var currentGfLevelComplete = (Tr4Level)LaterClassicGameData.GfLevelComplete.Current;

        bool loadingIntoNextLevel = currentGfLevelComplete == currentLevel + 1;
        bool loadingRaceForTheIris = currentGfLevelComplete == Tr4Level.RaceForTheIris;
        return loadingIntoNextLevel && !loadingRaceForTheIris;
    }

    private static bool GlitchlessShouldSplitValleyOfTheKings()
    {
        /* Route
            Transition 00 | currentLevel == 02 && currentGfLevelComplete == 03 | Race for the Iris to Tomb of Seth (covered elsewhere).
            Transition 01 | currentLevel == 03 && currentGfLevelComplete == 04 | Tomb of Seth to Burial Chambers.
            Transition 02 | currentLevel == 04 && currentGfLevelComplete == 05 | Burial Chambers to Valley of the Kings.
            Transition 03 | currentLevel == 05 && currentGfLevelComplete == 06 | Valley of the Kings to KV5.
            Transition 04 | currentLevel == 06 && currentGfLevelComplete == 07 | KV5 to Karnak.
        */
        var currentLevel = (Tr4Level)BaseGameData.Level.Current;
        var currentGfLevelComplete = (Tr4Level)LaterClassicGameData.GfLevelComplete.Current;

        return currentGfLevelComplete == currentLevel + 1;
    }

    private static bool GlitchlessShouldSplitKarnak()
    {
        /* Likely route
            Transition 00 | currentLevel == 06 && currentGfLevelComplete == 07 | KV5 to Karnak (covered elsewhere).
            Transition 01 | currentLevel == 07 && currentGfLevelComplete == 08 | Karnak to Hypostyle (x).
            Transition 02 | currentLevel == 08 && currentGfLevelComplete == 09 | Hypostyle to Sacred Lake.
            Transition 03 | currentLevel == 09 && currentGfLevelComplete == 07 | Sacred Lake to Karnak.
            Transition 04 | currentLevel == 07 && currentGfLevelComplete == 08 | Karnak to Hypostyle.
            Transition 05 | currentLevel == 08 && currentGfLevelComplete == 09 | Hypostyle to Sacred Lake.
            Transition 05 | currentLevel == 09 && currentGfLevelComplete == 11 | Sacred Lake to Tomb of Semerkhet.
            Transition 06 | currentLevel == 11 && currentGfLevelComplete == 12 | Tomb of Semerkhet to Guardian of Semerkhet.
            Transition 07 | currentLevel == 12 && currentGfLevelComplete == 13 | Guardian of Semerkhet to Desert Railroad.
        */
        /* Default undesired splits
            The Great Hypostyle Hall (08):
                Only split when Lara enters with required progression item/parts.
        */
        var currentLevel = (Tr4Level)BaseGameData.Level.Current;
        var currentGfLevelComplete = (Tr4Level)LaterClassicGameData.GfLevelComplete.Current;

        bool loadingFromKarnakToHypostyle = currentLevel == Tr4Level.TempleOfKarnak && currentGfLevelComplete == Tr4Level.GreatHypostyleHall;
        if (loadingFromKarnakToHypostyle)
        {
            bool laraHasHypostyleKey = (GameData.KeyItems.Current & 0b0000_0010) == 0b0000_0010;
            return laraHasHypostyleKey;
        }

        bool loadingNextLevel = currentGfLevelComplete > currentLevel;
        bool circlingBackToTheBeginning = currentLevel == Tr4Level.SacredLake && currentGfLevelComplete == Tr4Level.TempleOfKarnak;
        return loadingNextLevel || circlingBackToTheBeginning;
    }

    private static bool GlitchlessShouldSplitEasternDesert()
        /* Route
        Transition 00 | currentLevel == 12 && currentGfLevelComplete == 13 | Guardian of Semerkhet to Desert Railroad (covered elsewhere).
        Transition 01 | currentLevel == 13 && currentGfLevelComplete == 14 | Desert Railroad to Alexandria.
        */
        => LaterClassicGameData.GfLevelComplete.Current == (uint) Tr4Level.Alexandria;

    private static bool GlitchlessShouldSplitAlexandria()
    {
        /* Likely route
            Transition 00 | currentLevel == 13 && currentGfLevelComplete == 14 | Desert Railroad to Alexandria (covered elsewhere).
            Transition 01 | currentLevel == 14 && currentGfLevelComplete == 15 | Alexandria to Coastal (x).
            Transition 02 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs (x).
            Transition 03 | currentLevel == 18 && currentGfLevelComplete == 15 | Catacombs to Coastal (x).
            Transition 04 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs (x).
            Transition 05 | currentLevel == 18 && currentGfLevelComplete == 15 | Catacombs to Coastal (x).
            Transition 06 | currentLevel == 15 && currentGfLevelComplete == 18 | Coastal to Catacombs.
            Transition 07 | currentLevel == 18 && currentGfLevelComplete == 19 | Catacombs to Poseidon.
            Transition 08 | currentLevel == 19 && currentGfLevelComplete == 20 | Poseidon to Lost Library.
            Transition 09 | currentLevel == 20 && currentGfLevelComplete == 21 | Lost Library to Hall of Demetrius (x).
            Transition 10 | currentLevel == 21 && currentGfLevelComplete == 20 | Hall of Demetrius to Lost Library (x).
            Transition 11 | currentLevel == 20 && currentGfLevelComplete == 19 | Lost Library to Poseidon.
            Transition 12 | currentLevel == 19 && currentGfLevelComplete == 15 | Poseidon to Coastal (x).
            Transition 13 | currentLevel == 15 && currentGfLevelComplete == 16 | Coastal to Pharos (x).
            Transition 14 | currentLevel == 16 && currentGfLevelComplete == 17 | Pharos to Cleopatra (x).
            Transition 15 | currentLevel == 17 && currentGfLevelComplete == 16 | Cleopatra to Pharos (x).
            Transition 16 | currentLevel == 16 && currentGfLevelComplete == 17 | Pharos to Cleopatra.
            Transition 17 | currentLevel == 17 && currentGfLevelComplete == 22 | Cleopatra to City of the Dead.
        */
        /* Default undesired splits
            Coastal Ruins (15):
                When entering: only split from Poseidon (20) with Pharos Knot and Pharos Pillar.
                When leaving: only split into Catacombs (18) if the platform has been stabilized by the pillar below.
            Hall of Demetrius (21)
            Cleopatra's Palaces (17):
                Only split when Lara enters with required progression item/parts.
        */
        var currentLevel = (Tr4Level)BaseGameData.Level.Current;
        var currentGfLevelComplete = (Tr4Level)LaterClassicGameData.GfLevelComplete.Current;
        var oldGfLevelComplete = (Tr4Level)LaterClassicGameData.GfLevelComplete.Old;

        // Handle special exception case(s) that ignore that the game is in the same load state.
        bool sameLoadState = currentGfLevelComplete == oldGfLevelComplete;
        if (sameLoadState)
        {
            bool finishedLoadingCatacombs = currentLevel == Tr4Level.Catacombs && LaterClassicGameData.Loading.Old && !LaterClassicGameData.Loading.Current;
            if (!finishedLoadingCatacombs)
                return false;

            // The level must finish loading before its ITEM_INFO array can be checked.
            var platform = GameData.GetItemInfoAtIndex(79);
            return platform.flags == 0x20;
        }

        bool loadingFromDemetriusToCoastal = currentLevel == Tr4Level.HallOfDemetrius && currentGfLevelComplete == Tr4Level.CoastalRuins;
        bool loadingFromLostLibraryToPoseidon = currentLevel == Tr4Level.TheLostLibrary && currentGfLevelComplete == Tr4Level.TempleOfPoseidon;
        if (loadingFromDemetriusToCoastal || loadingFromLostLibraryToPoseidon)
        {
            bool laraHasPharosPillar = GameData.PuzzleItems.Current.PharosPillar == 1;
            bool laraHasPharosKnot = GameData.PuzzleItems.Current.PharosKnot == 1;
            return laraHasPharosPillar && laraHasPharosKnot;
        }

        bool loadingFromPharosToCleopatra = currentLevel == Tr4Level.PharosTempleOfIsis && currentGfLevelComplete == Tr4Level.CleopatrasPalaces;
        if (loadingFromPharosToCleopatra)
        {
            bool laraHasCombinedClockworkBeetle = (GameData.MechanicalScarab.Current & 0b0000_0001) == 0b0000_0001;
            bool laraHasBothClockworkBeetleParts = (GameData.MechanicalScarab.Current & 0b0000_0110) == 0b0000_0110;
            return laraHasCombinedClockworkBeetle || laraHasBothClockworkBeetleParts;
        }

        bool loadingFromCatacombsToPoseidon = currentLevel == Tr4Level.Catacombs && currentGfLevelComplete == Tr4Level.TempleOfPoseidon;
        bool loadingFromPoseidonToLostLibrary = currentLevel == Tr4Level.TempleOfPoseidon && currentGfLevelComplete == Tr4Level.TheLostLibrary;
        bool loadingCityOfTheDead = currentGfLevelComplete == Tr4Level.CityOfTheDead;
        return loadingFromCatacombsToPoseidon || loadingFromPoseidonToLostLibrary || loadingCityOfTheDead;
    }

    private static bool GlitchlessShouldSplitCairo()
    {
        /* Likely route
            Transition 00 | currentLevel == 17 && currentGfLevelComplete == 22 | Cleopatra's Palaces to City (covered elsewhere).
            Transition 01 | currentLevel == 22 && currentGfLevelComplete == 24 | City to Tulun.
            Transition 02 | currentLevel == 24 && currentGfLevelComplete == 23 | Tulun to Trenches (x).
            Transition 03 | currentLevel == 23 && currentGfLevelComplete == 25 | Trenches to Street Bazaar.
            Transition 04 | currentLevel == 25 && currentGfLevelComplete == 23 | Street Bazaar to Trenches (x).
            Transition 05 | currentLevel == 23 && currentGfLevelComplete == 25 | Trenches to Street Bazaar (x).
            Transition 06 | currentLevel == 25 && currentGfLevelComplete == 23 | Street Bazaar to Trenches.
            Transition 07 | currentLevel == 23 && currentGfLevelComplete == 26 | Trenches to Citadel Gate (x).
            Transition 08 | currentLevel == 26 && currentGfLevelComplete == 27 | Citadel Gate to Citadel.
            Transition 09 | currentLevel == 27 && currentGfLevelComplete == 28 | Citadel to Sphinx Complex.
        */
        /* Default undesired splits
            Trenches (23):
                When leaving: only split when entering Street Bazaar (25) without any Mine Detonator combo/parts.
                When entering: only split when last leaving Street Bazaar (25) with Mine Detonator combo/parts.
        */
        var currentLevel = (Tr4Level)BaseGameData.Level.Current;
        var currentGfLevelComplete = (Tr4Level)LaterClassicGameData.GfLevelComplete.Current;

        bool inTrenches = currentLevel == Tr4Level.Trenches;
        if (inTrenches)
        {
            bool laraHasCombinedDetonator = GameData.PuzzleItems.Current.MineDetonator == 1;

            bool loadingToStreetBazaar = currentGfLevelComplete == Tr4Level.StreetBazaar;
            if (loadingToStreetBazaar)
            {
                bool laraHasNeitherDetonatorPart = (GameData.PuzzleItemsCombo.Current & 0b1100_0000_0000_0000) == 0b0000_0000_0000_0000;
                return !laraHasCombinedDetonator && laraHasNeitherDetonatorPart;
            }

            bool justFinishedLoadingIntoLevel = LaterClassicGameData.Loading.Old && !LaterClassicGameData.Loading.Current;
            if (!justFinishedLoadingIntoLevel)
                return false;

            bool laraHasDetonatorParts = (GameData.PuzzleItemsCombo.Current & 0b1100_0000_0000_0000) == 0b1100_0000_0000_0000;
            bool laraStartedNextToMinefield = GameData.GetItemInfoAtIndex(51).room_number == 26;
            return (laraHasCombinedDetonator || laraHasDetonatorParts) && laraStartedNextToMinefield;
        }

        bool loadingFromCityToTulun = currentLevel == Tr4Level.CityOfTheDead && currentGfLevelComplete == Tr4Level.ChambersOfTulun;
        bool loadingFromCitadelGateToCitadel = currentLevel == Tr4Level.CitadelGate && currentGfLevelComplete == Tr4Level.Citadel;
        bool loadingFromCitadelToSphinxComplex = currentLevel == Tr4Level.Citadel && currentGfLevelComplete == Tr4Level.SphinxComplex;

        return loadingFromCityToTulun || loadingFromCitadelGateToCitadel || loadingFromCitadelToSphinxComplex;
    }

    private static bool GlitchlessShouldSplitGiza()
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
        var currentLevel = (Tr4Level)BaseGameData.Level.Current;
        var currentGfLevelComplete = (Tr4Level)LaterClassicGameData.GfLevelComplete.Current;

        bool justFinishedLoadingALevel = currentGfLevelComplete == 0;
        if (justFinishedLoadingALevel)
            return false;

        bool currentLevelIsBeforeMastabas = currentLevel < Tr4Level.TheMastabas;
        if (currentLevelIsBeforeMastabas)
        {
            bool loadingFromInsideMenkauresToSphinx = currentLevel == Tr4Level.InsideMenkauresPyramid && currentGfLevelComplete == Tr4Level.SphinxComplex;
            return !loadingFromInsideMenkauresToSphinx;
        }

        bool loadingInsideTheGreatPyramid = currentGfLevelComplete == Tr4Level.InsideTheGreatPyramid;
        if (loadingInsideTheGreatPyramid)
        {
            bool laraHasEasternShaftKey = GameData.PuzzleItems.Current.EasternShaftKey == 1;
            return laraHasEasternShaftKey;
        }

        bool loadingNextLevel = currentGfLevelComplete > currentLevel;
        bool loadingBoss = currentGfLevelComplete == Tr4Level.HorusBoss;
        return loadingNextLevel && !loadingBoss;
    }
}