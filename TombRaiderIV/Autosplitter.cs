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
    
    /// <summary>Implementation of <see cref="LaterClassicAutosplitter"/>.</summary>
    internal sealed class Autosplitter : LaterClassicAutosplitter
    {
        private static readonly HashSet<Level> NextSplitLevels = new HashSet<Level>()
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

        /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
        public Autosplitter()
        {
            Settings = new ComponentSettings();

            LevelCount = 38;
            CompletedLevels.Capacity = LevelCount;

            Data = new GameData();
            Data.OnGameFound += Settings.SetGameVersion;
        }

        public override bool ShouldSplit(LiveSplitState state)
        {
            // Deathrun
            if (Settings.Deathrun)
            {
                bool laraJustDied = BaseGameData.Health.Old > 0 && BaseGameData.Health.Current <= 0;
                return laraJustDied;
            }

            if (Settings.Glitchless)
                return GlitchlessShouldSplit();

            uint currentGfLevelComplete = LaterClassicGameData.GfLevelComplete.Current;
            bool enteringNextSplitLevel = NextSplitLevels.Contains((Level)currentGfLevelComplete);
            bool finishedGame = currentGfLevelComplete == 39; // 39 is hardcoded to trigger credits.

            return enteringNextSplitLevel || finishedGame;
        }

        private bool GlitchlessShouldSplit()
        {
            throw new NotImplementedException();
        }
    }
}
