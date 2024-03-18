using System;
using LiveSplit.Model;
using TRUtil;

namespace TR5;

/// <summary>Implementation of <see cref="LaterClassicAutosplitter"/>.</summary>
internal sealed class Autosplitter : LaterClassicAutosplitter
{
    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter(Version version) : base(version)
    {
        Settings = new ComponentSettings(version);

        GameData.InitializeGameData();
        BaseGameData.OnGameVersionChanged += Settings.SetGameVersion;
    }

    public override bool ShouldReset(LiveSplitState state)
    {
        if (!Settings.EnableAutoReset)
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