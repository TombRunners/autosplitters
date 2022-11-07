using LiveSplit.Model;
using TRUtil;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace TR5;

/// <summary>The game's level and demo values.</summary>
internal enum Level
{
    MainMenu                = 00,
    // Rome
    StreetsOfRome           = 01,
    TrajansMarkets          = 02,
    TheColosseum            = 03,
    // Russian Submarine
    TheBase                 = 04,
    TheSubmarine            = 05,
    DeepseaDive             = 06,
    TempleofKarnak          = 07,
    // Ireland
    GallowsTree             = 08,
    Labyrinth               = 09,
    OldMill                 = 10,
    // VC Headquarters
    ThirteenthFloor         = 11,
    EscapeWithTheIris       = 12,
    CutsceneSecurityBreach  = 13,
    RedAlert                = 14
}

/// <summary>The "areas" of the game.</summary>
internal enum LevelSection
{
    Rome                = Level.StreetsOfRome,
    RussianSubmarine    = Level.TheColosseum,
    Ireland             = Level.TempleofKarnak,
    VcHeadquarters      = Level.EscapeWithTheIris
}

/// <summary>Implementation of <see cref="LaterClassicAutosplitter"/>.</summary>
internal sealed class Autosplitter : LaterClassicAutosplitter
{
    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter()
    {
        Settings = new ComponentSettings();

        Data = new GameData();
        Data.OnGameFound += Settings.SetGameVersion;
    }

    public override bool ShouldReset(LiveSplitState state)
    {
        if (Settings.DisableAutoReset)
            return false;
        bool loadingIntoMainMenu = LaterClassicGameData.GfLevelComplete.Current == 0 && BaseGameData.Level.Current == 0 && LaterClassicGameData.Loading.Current;
        bool comingFromAnotherLevel = GameData.GfInitializeGame.Current && !LaterClassicGameData.Loading.Old && GameData.GfGameMode.Current == 0;
        return loadingIntoMainMenu && comingFromAnotherLevel;
    }

    public override bool ShouldSplit(LiveSplitState state)
    {
        // Handle deathruns for both rulesets.
        if (Settings.Deathrun)
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

        // Handle ILs and FG for both rulesets.
        bool loadingAnotherLevel = currentGfLevelComplete != 0;
        if (!Settings.Option)  // Property name should be SplitCutscene.
            loadingAnotherLevel = loadingAnotherLevel && currentGfLevelComplete != (uint)Level.CutsceneSecurityBreach;
        return loadingAnotherLevel;
    }
}