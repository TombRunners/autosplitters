using System;
using System.Linq;
using LiveSplit.Model;
using TRUtil;

namespace TR4;

/// <summary>Implementation of <see cref="LaterClassicAutosplitter{TData,TSettings}"/>.</summary>
internal sealed class Autosplitter : LaterClassicAutosplitter<GameData, ComponentSettings>
{
    private const uint HardcodedCreditsTrigger = 39;

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter(Version version) : base(new GameData(), new ComponentSettings(version))
        => Data.OnGameVersionChanged += Settings.SetGameVersion;

    public override bool ShouldSplit(LiveSplitState state)
    {
        if (Settings.Deathrun)
            return DeathrunShouldSplit();

        if (Settings.SplitSecrets && SecretShouldSplit())
            return true;

        uint currentGfLevelComplete = Data.GfLevelComplete.Current;
        uint oldGfLevelComplete = Data.GfLevelComplete.Old;

        // Prevent double-splits.
        bool sameLoadState = currentGfLevelComplete == oldGfLevelComplete;
        if (sameLoadState)
        {
            if (!Settings.LegacyGlitchless || !Settings.FullGame)
                return false;

            // Below bool is never true for The Times Exclusive; its level values never match these TR4 levels.
            var currentLevel = (Tr4Level)Data.Level.Current;
            bool possibleGlitchlessPostLoadSplitLevel = currentLevel is Tr4Level.Catacombs or Tr4Level.Trenches;
            return possibleGlitchlessPostLoadSplitLevel && GlitchlessShouldSplit();
        }

        // Handle IL / Section runs, which assumes all level transitions are desirable splits.
        if (!Settings.FullGame)
        {
            bool loadingAnotherLevel = currentGfLevelComplete != 0;
            return loadingAnotherLevel;
        }

        // Handle when credits are triggered.
        if (currentGfLevelComplete == HardcodedCreditsTrigger)
            return true;

        // Handle Legacy Glitchless runs.
        if (Settings.LegacyGlitchless)
            return GlitchlessShouldSplit();

        // Handle Full Game, non-Glitchless runs.
        bool playingTheTimesExclusive = (Tr4Version)Data.GameVersion == Tr4Version.TheTimesExclusive;
        return playingTheTimesExclusive ? TteShouldSplit() : Tr4ShouldSplit();
    }

    /// <remarks>This assumes double-splits have already been prevented.</remarks>
    private bool Tr4ShouldSplit()
    {
        uint nextLevel = Data.GfLevelComplete.Current;
        uint currentLevel = Data.Level.Current;
        if (nextLevel == currentLevel || nextLevel == 0)
            return false;

        Tr4Level lower;
        Tr4Level lowerLevel;
        Tr4Level higherLevel;
        TransitionDirection direction;
        bool currentRoomIsInLowerLevel;
        if (nextLevel >= currentLevel)
        {
            direction = TransitionDirection.OneWayFromLower;
            lowerLevel = (Tr4Level)currentLevel;
            higherLevel = (Tr4Level)nextLevel;
            currentRoomIsInLowerLevel = true;
        }
        else
        {
            direction = TransitionDirection.OneWayFromHigher;
            lowerLevel = (Tr4Level)nextLevel;
            higherLevel = (Tr4Level)currentLevel;
            currentRoomIsInLowerLevel = false;
        }

        short room = Data.Room.Current;
        var activeMatches = Settings
            .Tr4LevelTransitions
            .Where(t =>
                t.Active &&
                t.LowerLevel == lowerLevel &&
                t.HigherLevel == higherLevel &&
                (t.Directionality == TransitionDirection.TwoWay || t.Directionality == direction) &&
                t.RoomMatchedOrNotRequired(room, currentRoomIsInLowerLevel)
            )
            .ToList();

        if (!activeMatches.Any())
        {
#if DEBUG
            // Warn is the minimum threshold when using LiveSplit's Event Viewer logging.
            LiveSplit.Options.Log.Warning($"No active transition match found!\nTransition: {lowerLevel} | {direction} | {higherLevel} | room: {room}\n");
#endif
            return false;
        }

        if (activeMatches.Count > 1) // Should be impossible if hardcoded default transitions are set up correctly.
            LiveSplit.Options.Log.Error($"Level Transition Settings improperly coded, found multiple matches!\n"
                                        + $"Transition: {lowerLevel} | {direction} | {higherLevel} | room: {room}\n"
                                        + $"Matches: {string.Join(", ", activeMatches.Select(static s => s.DisplayName()))}"
            );

        return true;
    }

    /// <remarks>This assumes double-splits have already been prevented.</remarks>
    private bool TteShouldSplit()
    {
        // There are only 2 non-menu levels; 1 is the cutscene and 2 is the playable level.
        // The playable level is hardcoded to trigger credits, and this transition is always enabled.
        // So, if the player also wants to split the cutscene (level 1), this is effectively the same as IL Mode.
        uint currentGfLevelComplete = Data.GfLevelComplete.Current;

        var levelsToSplit = Settings.TteLevelTransitions.Where(static t => t.Active).ToHashSet();
        return levelsToSplit.Count == 2
            ? currentGfLevelComplete != 0
            : currentGfLevelComplete == HardcodedCreditsTrigger;
    }

    #region Legacy Glitchless Logic

    private bool GlitchlessShouldSplit()
        => Data.Level.Current switch
        {
            >= (uint) Tr4LevelSection.Giza             => GlitchlessShouldSplitGiza(),
            >= (uint) Tr4LevelSection.Cairo            => GlitchlessShouldSplitCairo(),
            >= (uint) Tr4LevelSection.Alexandria       => GlitchlessShouldSplitAlexandria(),
            >= (uint) Tr4LevelSection.EasternDesert    => GlitchlessShouldSplitEasternDesert(),
            >= (uint) Tr4LevelSection.Karnak           => GlitchlessShouldSplitKarnak(),
            >= (uint) Tr4LevelSection.ValleyOfTheKings => GlitchlessShouldSplitValleyOfTheKings(),
            _                                          => GlitchlessShouldSplitCambodia(),
        };

    private bool GlitchlessShouldSplitCambodia()
    {
        /* Route
            Transition 00 | currentLevel == 00 && currentGfLevelComplete == 01 | Main Menu to Angkor Wat (covered by ShouldStart).
            Transition 01 | currentLevel == 01 && currentGfLevelComplete == 02 | Angkor Wat to Race for the Iris (x).
            Transition 02 | currentLevel == 02 && currentGfLevelComplete == 03 | Race for the Iris to The Tomb of Seth.
        */
        /* Default undesired splits
            Loading into Race for the Iris (2)
        */
        var currentLevel = (Tr4Level)Data.Level.Current;
        var currentGfLevelComplete = (Tr4Level)Data.GfLevelComplete.Current;

        bool loadingIntoNextLevel = currentGfLevelComplete == currentLevel + 1;
        bool loadingRaceForTheIris = currentGfLevelComplete == Tr4Level.RaceForTheIris;
        return loadingIntoNextLevel && !loadingRaceForTheIris;
    }

    private bool GlitchlessShouldSplitValleyOfTheKings()
    {
        /* Route
            Transition 00 | currentLevel == 02 && currentGfLevelComplete == 03 | Race for the Iris to Tomb of Seth (covered elsewhere).
            Transition 01 | currentLevel == 03 && currentGfLevelComplete == 04 | Tomb of Seth to Burial Chambers.
            Transition 02 | currentLevel == 04 && currentGfLevelComplete == 05 | Burial Chambers to Valley of the Kings.
            Transition 03 | currentLevel == 05 && currentGfLevelComplete == 06 | Valley of the Kings to KV5.
            Transition 04 | currentLevel == 06 && currentGfLevelComplete == 07 | KV5 to Karnak.
        */
        var currentLevel = (Tr4Level)Data.Level.Current;
        var currentGfLevelComplete = (Tr4Level)Data.GfLevelComplete.Current;

        return currentGfLevelComplete == currentLevel + 1;
    }

    private bool GlitchlessShouldSplitKarnak()
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
        var currentLevel = (Tr4Level)Data.Level.Current;
        var currentGfLevelComplete = (Tr4Level)Data.GfLevelComplete.Current;

        bool loadingFromKarnakToHypostyle = currentLevel == Tr4Level.TempleOfKarnak && currentGfLevelComplete == Tr4Level.GreatHypostyleHall;
        if (loadingFromKarnakToHypostyle)
        {
            bool laraHasHypostyleKey = (Data.KeyItems.Current & 0b0000_0010) == 0b0000_0010;
            return laraHasHypostyleKey;
        }

        bool loadingNextLevel = currentGfLevelComplete > currentLevel;
        bool circlingBackToTheBeginning = currentLevel == Tr4Level.SacredLake && currentGfLevelComplete == Tr4Level.TempleOfKarnak;
        return loadingNextLevel || circlingBackToTheBeginning;
    }

    private bool GlitchlessShouldSplitEasternDesert()
        /* Route
        Transition 00 | currentLevel == 12 && currentGfLevelComplete == 13 | Guardian of Semerkhet to Desert Railroad (covered elsewhere).
        Transition 01 | currentLevel == 13 && currentGfLevelComplete == 14 | Desert Railroad to Alexandria.
        */
        => Data.GfLevelComplete.Current == (uint) Tr4Level.Alexandria;

    private bool GlitchlessShouldSplitAlexandria()
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
        var currentLevel = (Tr4Level)Data.Level.Current;
        var currentGfLevelComplete = (Tr4Level)Data.GfLevelComplete.Current;
        var oldGfLevelComplete = (Tr4Level)Data.GfLevelComplete.Old;

        /* Handle special case(s) with a post-load split.
        / Needed first because there is a timing issue / inconsistency on the frame where the Loading value flips back to 0.
        / GfLevelComplete may also switch to 0 (if applicable), sometimes it happens on the frame after.
        / Since we cannot rely on either GfLevelComplete being the same as last frame or switching, assume neither.
        */
        bool finishedLoadingCatacombs = Data.Loading.Old && !Data.Loading.Current && currentLevel == Tr4Level.Catacombs;
        if (finishedLoadingCatacombs)
        {
            // When loading a save from the same level, GfLevelComplete remains 0 for the whole load.
            bool fromAnotherLevel = oldGfLevelComplete == Tr4Level.Catacombs || currentGfLevelComplete == Tr4Level.Catacombs;
            if (!fromAnotherLevel)
                return false;

            // The level must finish loading before its ITEM_INFO array can be checked reliably.
            var platform = Data.GetItemInfoAtIndex(79);
            bool platformTriggerUndone = (platform.flags & 0x3E00) == 0;
            return platformTriggerUndone;
        }

        // End post-load splits.
        if (currentGfLevelComplete == oldGfLevelComplete)
            return false;

        bool loadingFromDemetriusToCoastal = currentLevel == Tr4Level.HallOfDemetrius && currentGfLevelComplete == Tr4Level.CoastalRuins;
        bool loadingFromLostLibraryToPoseidon = currentLevel == Tr4Level.TheLostLibrary && currentGfLevelComplete == Tr4Level.TempleOfPoseidon;
        if (loadingFromDemetriusToCoastal || loadingFromLostLibraryToPoseidon)
        {
            bool laraHasPharosPillar = Data.PuzzleItems.Current.PharosPillar == 1;
            bool laraHasPharosKnot = Data.PuzzleItems.Current.PharosKnot == 1;
            return laraHasPharosPillar && laraHasPharosKnot;
        }

        bool loadingFromPharosToCleopatra = currentLevel == Tr4Level.PharosTempleOfIsis && currentGfLevelComplete == Tr4Level.CleopatrasPalaces;
        if (loadingFromPharosToCleopatra)
        {
            bool laraHasCombinedClockworkBeetle = (Data.MechanicalScarab.Current & 0b0000_0001) == 0b0000_0001;
            bool laraHasBothClockworkBeetleParts = (Data.MechanicalScarab.Current & 0b0000_0110) == 0b0000_0110;
            return laraHasCombinedClockworkBeetle || laraHasBothClockworkBeetleParts;
        }

        bool loadingFromCatacombsToPoseidon = currentLevel == Tr4Level.Catacombs && currentGfLevelComplete == Tr4Level.TempleOfPoseidon;
        bool loadingFromPoseidonToLostLibrary = currentLevel == Tr4Level.TempleOfPoseidon && currentGfLevelComplete == Tr4Level.TheLostLibrary;
        bool loadingCityOfTheDead = currentGfLevelComplete == Tr4Level.CityOfTheDead;
        return loadingFromCatacombsToPoseidon || loadingFromPoseidonToLostLibrary || loadingCityOfTheDead;
    }

    private bool GlitchlessShouldSplitCairo()
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
        var currentLevel = (Tr4Level)Data.Level.Current;
        var currentGfLevelComplete = (Tr4Level)Data.GfLevelComplete.Current;
        var oldGfLevelComplete = (Tr4Level)Data.GfLevelComplete.Old;

        /* Handle special case(s) with a post-load split.
        / Needed first because there is a timing issue / inconsistency on the frame where the Loading value flips back to 0.
        / GfLevelComplete may also switch to 0 (if applicable), sometimes it happens on the frame after.
        / Since we cannot rely on either GfLevelComplete being the same as last frame or switching, assume neither.
        */
        bool finishedLoadingTrenches = Data.Loading.Old && !Data.Loading.Current && currentLevel == Tr4Level.Trenches;
        if (finishedLoadingTrenches)
        {
            // When loading a save from the same level, GfLevelComplete remains 0 for the whole load.
            bool fromAnotherLevel = oldGfLevelComplete == Tr4Level.Trenches || currentGfLevelComplete == Tr4Level.Trenches;
            if (!fromAnotherLevel)
                return false;

            // The level must finish loading before its ITEM_INFO array can be checked reliably.
            bool laraHasCombinedDetonator = Data.PuzzleItems.Current.MineDetonator == 1;
            bool laraHasDetonatorParts = (Data.PuzzleItemsCombo.Current & 0b1100_0000_0000_0000) == 0b1100_0000_0000_0000;
            bool laraStartedNextToMinefield = Data.GetItemInfoAtIndex(51).room_number == 26;
            return (laraHasCombinedDetonator || laraHasDetonatorParts) && laraStartedNextToMinefield;
        }

        // End post-load splits.
        if (currentGfLevelComplete == oldGfLevelComplete)
            return false;

        bool loadingFromTrenchesToStreetBazaar = currentLevel == Tr4Level.Trenches && currentGfLevelComplete == Tr4Level.StreetBazaar;
        if (loadingFromTrenchesToStreetBazaar)
        {
            bool laraHasCombinedDetonator = Data.PuzzleItems.Current.MineDetonator == 1;
            bool laraHasNeitherDetonatorPart = (Data.PuzzleItemsCombo.Current & 0b1100_0000_0000_0000) == 0b0000_0000_0000_0000;
            return !laraHasCombinedDetonator && laraHasNeitherDetonatorPart;
        }

        bool loadingFromCityToTulun = currentLevel == Tr4Level.CityOfTheDead && currentGfLevelComplete == Tr4Level.ChambersOfTulun;
        bool loadingFromCitadelGateToCitadel = currentLevel == Tr4Level.CitadelGate && currentGfLevelComplete == Tr4Level.Citadel;
        bool loadingFromCitadelToSphinxComplex = currentLevel == Tr4Level.Citadel && currentGfLevelComplete == Tr4Level.SphinxComplex;

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
        var currentLevel = (Tr4Level)Data.Level.Current;
        var currentGfLevelComplete = (Tr4Level)Data.GfLevelComplete.Current;

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
            bool laraHasEasternShaftKey = Data.PuzzleItems.Current.EasternShaftKey == 1;
            return laraHasEasternShaftKey;
        }

        bool loadingNextLevel = currentGfLevelComplete > currentLevel;
        bool loadingBoss = currentGfLevelComplete == Tr4Level.HorusBoss;
        return loadingNextLevel && !loadingBoss;
    }

    #endregion
}