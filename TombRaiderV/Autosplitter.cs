using System;
using LiveSplit.Model;
using TRUtil;

namespace TR5;

/// <summary>Implementation of <see cref="LaterClassicAutosplitter{TData}"/>.</summary>
internal sealed class Autosplitter : LaterClassicAutosplitter<GameData>
{
    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter(Version version) : base(version, new GameData())
    {
        Settings = new ComponentSettings(version);

        Data.OnGameVersionChanged += Settings.SetGameVersion;
    }

    public override bool ShouldReset(LiveSplitState state)
    {
        if (!Settings.EnableAutoReset)
            return false;

        bool loadingIntoMainMenu = Data.GfLevelComplete.Current == 0 && Data.Level.Current == 0 && Data.Loading.Current;
        bool comingFromAnotherLevel = Data.GfInitializeGame.Current && !Data.Loading.Old && Data.GfGameMode.Current == 0;
        return loadingIntoMainMenu && comingFromAnotherLevel;
    }

    public override bool ShouldSplit(LiveSplitState state)
    {
        // Handle deathruns for both rulesets.
        if (Settings.Deathrun)
        {
            bool laraJustDied = Data.Health.Old > 0 && Data.Health.Current <= 0;
            return laraJustDied;
        }

        uint currentGfLevelComplete = Data.GfLevelComplete.Current;
        uint oldGfLevelComplete = Data.GfLevelComplete.Old;

        // Prevent double-splits; applies to ILs and FG for both glitched and glitchless.
        bool ignoringSubsequentFramesOfThisLoadState = currentGfLevelComplete == oldGfLevelComplete;
        if (ignoringSubsequentFramesOfThisLoadState)
            return false;

        // Handle ILs and FG for both rulesets.
        bool loadingAnotherLevel = currentGfLevelComplete != 0;
        if (!Settings.Option)  // Property name should be SplitCutscene.
            loadingAnotherLevel = loadingAnotherLevel && currentGfLevelComplete != (uint)Tr5Level.CutsceneSecurityBreach;
        return loadingAnotherLevel;
    }
}