using System;
using ClassicUtil;
using LiveSplit.Model;

namespace TR2;

/// <summary>Implementation of <see cref="ClassicAutosplitter{TData}"/>.</summary>
internal sealed class Autosplitter : ClassicAutosplitter<GameData>
{
    private bool _newGamePageSelected;

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter(Version version) : base(version, new GameData())
    {
        Settings = new ComponentSettings(version);

        LevelCount = 18; // This is the highest between TR2 at 18 and TR2G at 5.
        CompletedLevels.Capacity = LevelCount;

        Data.OnGameVersionChanged += Settings.SetGameVersion;
    }

    public override bool ShouldStart(LiveSplitState state)
    {
        uint currentLevel = Data.Level.Current;
        uint oldLevel = Data.Level.Old;

        // Ignore demos.
        if (currentLevel >= (uint) Tr2Level.DemoVenice) // Never true for TR2G; level values are never so high.
            return false;

        // Check to see if the player has navigated to the New Game page of the passport.
        // This prevents some misfires from LiveSplit hooking late.
        // If LiveSplit hooks after the player has already navigated to the New Game page, this fails.
        uint oldPassportPage = Data.PickedPassportFunction.Old;
        uint currentPassportPage = Data.PickedPassportFunction.Current;
        if (oldPassportPage == 0 && currentPassportPage == 1)
            _newGamePageSelected = true;

        // Determine if a new game was started; this applies to all runs but for FG runs, this is the only start condition.
        if (_newGamePageSelected)
        {
            bool cameFromTitleScreen = Data.TitleScreen.Old && !Data.TitleScreen.Current;
            bool cameFromLarasHome = oldLevel         == (uint) Tr2Level.LarasHome; // Never true for TR2G.
            bool justStartedFirstLevel = currentLevel == 1;                         // This value is good for GreatWall and TheColdWar.
            bool newGameStarted = (cameFromTitleScreen || cameFromLarasHome) && justStartedFirstLevel;
            if (newGameStarted)
                return true;
        }
        else if (Settings.FullGame)
        {
            return false;
        }

        // The remaining logic only applies to non-FG runs starting on a level besides the first.
        uint oldTime = Data.LevelTime.Old;
        uint currentTime = Data.LevelTime.Current;
        bool wentToNextLevel = oldLevel == currentLevel - 1;
        return wentToNextLevel && oldTime > currentTime;
    }

    public override void OnStart()
    {
        _newGamePageSelected = false;
        base.OnStart();
    }
}