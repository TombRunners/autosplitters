using System;
using LiveSplit.Model;
using TRUtil;

namespace TR1;

/// <summary>Implementation of <see cref="ClassicAutosplitter{TData}"/>.</summary>
internal sealed class Autosplitter : ClassicAutosplitter<GameData>
{
    private bool _newGamePageSelected;

    private bool IsUnfinishedBusiness => Data.GameVersion == (uint)Tr1Version.AtiUnfinishedBusiness;
    private uint? LastRealLevel => IsUnfinishedBusiness ? Data.Level.Current : (uint?)GetLastRealLevel(Data.Level.Current);

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter(Version version) : base(version, new GameData())
    {
        Settings = new ComponentSettings(version);

        LevelCount = 15; // This is the highest between TR1 at 15 and TR:UB at 4.
        CompletedLevels.Capacity = LevelCount;

        GameData.CompletedLevelTicks.Capacity = LevelCount;
        Data.OnGameVersionChanged += Settings.SetGameVersion;
    }

    public override TimeSpan? GetGameTime(LiveSplitState state)
    {
        // Check that IGT is ticking and not reset.
        uint currentLevelTicks = Data.LevelTime.Current;
        uint oldLevelTicks = Data.LevelTime.Old;
        bool igtNotTicking = currentLevelTicks - oldLevelTicks == 0;
        // IGT reset: the moment when the game is restarted after a quit-out,
        // or when you transition from a level to the next,
        // or when you start a new game.
        bool igtGotReset = oldLevelTicks != 0 && currentLevelTicks == 0;
        if (igtNotTicking || igtGotReset)
            return null;

        if (!IsUnfinishedBusiness)
        {
            // Check the demo isn't running.
            uint currentDemoTimer = Data.DemoTimer.Current;
            const uint demoStartThreshold = 480;
            if (currentDemoTimer > demoStartThreshold)
                return null;

            // Check that the player is in a real level.
            uint currentLevel = Data.Level.Current;
            var lastRealLevel = GetLastRealLevel(currentLevel);
            if (lastRealLevel is null)
                return null;
        }

        // Sum the current and completed levels' IGT.
        ulong ticks = currentLevelTicks + Data.SumLevelTimes(CompletedLevels, LastRealLevel);
        return TimeSpan.FromSeconds(BaseGameData.LevelTimeAsDouble(ticks));
    }

    /// <summary>
    ///     Gets the last non-cutscene <see cref="Tr1Level"/> the runner was on.
    /// </summary>
    /// <param name="level"><see cref="Tr1Level"/></param>
    /// <returns>The last non-cutscene <see cref="Tr1Level"/></returns>
    private static Tr1Level? GetLastRealLevel(uint level)
    {
        var lastLevel = (Tr1Level)level;
        if (lastLevel <= Tr1Level.TheGreatPyramid)
            return lastLevel;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return lastLevel switch
        {
            Tr1Level.QualopecCutscene => Tr1Level.Qualopec,
            Tr1Level.TihocanCutscene => Tr1Level.Tihocan,
            Tr1Level.MinesToAtlantis => Tr1Level.NatlasMines,
            Tr1Level.AfterAtlantisFMV => Tr1Level.Atlantis,
            _ => null,
        };
    }

    public override bool ShouldSplit(LiveSplitState state) => LastRealLevel is not null && !CompletedLevels.Contains(LastRealLevel.Value) && base.ShouldSplit(state);

    public override bool ShouldStart(LiveSplitState state)
    {
        // These initialize to 0, and the game's timer will not tick until the title screen is reached.
        bool gameJustLaunched = Data.LevelTime.Current == 0 && Data.Level.Old == 0;
        if (gameJustLaunched)
            return false;

        // Check to see if the player has navigated to the New Game page of the passport.
        // This prevents some misfires from LiveSplit hooking late.
        // If LiveSplit hooks after the player has already navigated to the New Game page, this fails.
        uint oldPassportPage = Data.PickedPassportFunction.Old;
        uint currentPassportPage = Data.PickedPassportFunction.Current;
        if (oldPassportPage == 0 && currentPassportPage == 1)
            _newGamePageSelected = true;

        // Determine if a new game was started; this applies to all runs but for FG runs, this is the only start condition.
        uint currentLevel = Data.Level.Current;
        uint oldLevel = Data.Level.Old;
        if (_newGamePageSelected)
        {
            if (IsUnfinishedBusiness)
            {
                bool cameFromTitleScreen = oldLevel == (uint)TrUbLevel.Title;
                bool justStartedFirstLevel = currentLevel == (uint)TrUbLevel.ReturnToEgypt;
                bool newGameStarted = cameFromTitleScreen && justStartedFirstLevel;
                if (newGameStarted)
                    return true;
            }
            else
            {
                bool cameFromTitleScreen = Data.TitleScreen.Old && !Data.TitleScreen.Current;
                bool cameFromLarasHome = oldLevel == (uint)Tr1Level.LarasHome;
                bool justStartedFirstLevel = currentLevel == (uint)Tr1Level.Caves;
                bool newGameStarted = (cameFromTitleScreen || cameFromLarasHome) && justStartedFirstLevel;
                if (newGameStarted)
                    return true;
            }
        }
        else if (Settings.FullGame)
        {
            return false;
        }

        // The remaining logic only applies to non-FG runs starting on a level besides the first.
        bool wentToNextLevel;
        if (IsUnfinishedBusiness)
        {
            wentToNextLevel = currentLevel == oldLevel + 1;
        }
        else
        {
            // Don't start while on a non-level.
            var oldRealLevel = GetLastRealLevel(oldLevel);
            var currentRealLevel = GetLastRealLevel(currentLevel);
            if (oldRealLevel is null || currentRealLevel is null)
                return false;

            wentToNextLevel = currentRealLevel == oldRealLevel + 1;
        }

        uint oldTime = Data.LevelTime.Old;
        uint currentTime = Data.LevelTime.Current;
        // The level values switch before the timer resets to 0. If LiveSplit polls then,
        // a check for currentTime <= MAGIC_LOW_NUMBER would fail. This more lenient solution
        // could result in a false start if the player would load into the next level
        // with a save which has a higher level IGT than their current position.
        bool timerCouldBeResetting = oldTime >= currentTime;
        return wentToNextLevel && timerCouldBeResetting;
    }

    public override void OnStart()
    {
        GameData.CompletedLevelTicks.Clear();
        _newGamePageSelected = false;
        base.OnStart();
    }

    public override void OnSplit(uint completedLevel)
    {
        GameData.CompletedLevelTicks.Add(Data.LevelTime.Current);
        base.OnSplit(LastRealLevel!.Value);
    }

    public override void OnUndoSplit()
    {
        GameData.CompletedLevelTicks.RemoveAt(GameData.CompletedLevelTicks.Count - 1);
        base.OnUndoSplit();
    }
}