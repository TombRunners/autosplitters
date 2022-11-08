using LiveSplit.Model;
using System;
using TRUtil;

// ReSharper disable UnusedMember.Global

namespace TR1;

/// <summary>The game's level, FMV, and cutscene values.</summary>
public enum Tr1Level
{
    // Levels
    LarasHome        = 00,
    Caves            = 01,
    Vilcabamba       = 02,
    LostValley       = 03,
    Qualopec         = 04,
    StFrancisFolly   = 05,
    Colosseum        = 06,
    PalaceMidas      = 07,
    Cistern          = 08,
    Tihocan          = 09,
    CityOfKhamoon    = 10,
    ObeliskOfKhamoon = 11,
    SanctuaryScion   = 12,
    NatlasMines      = 13,
    Atlantis         = 14,
    TheGreatPyramid  = 15,
    // Cutscenes and title screen
    QualopecCutscene = 16,
    TihocanCutscene  = 17,
    MinesToAtlantis  = 18,
    AfterAtlantisFMV = 19,
    TitleAndFirstFMV = 20
}
    
/// <summary>The game's level values.</summary>
public enum TrUbLevel
{
    ReturnToEgypt       = 00,
    TempleOfTheCat      = 01,
    AtlanteanStronghold = 02,
    TheHive             = 03,
    Title               = 04
}

/// <summary>Implementation of <see cref="ClassicAutosplitter"/>.</summary>
internal sealed class Autosplitter : ClassicAutosplitter
{
    private bool _newGamePageSelected;

    private static bool IsUnfinishedBusiness => BaseGameData.Version == (uint)GameVersion.AtiUnfinishedBusiness;
    private static uint? LastRealLevel => IsUnfinishedBusiness ? BaseGameData.Level.Current : (uint?)GetLastRealLevel(BaseGameData.Level.Current);

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter()
    {
        Settings = new ComponentSettings();

        LevelCount = 15; // This is the highest between TR1 at 15 and TR:UB at 4.
        CompletedLevels.Capacity = LevelCount;
        GameData.CompletedLevelTicks.Capacity = LevelCount;

        Data = new GameData();
        Data.OnGameFound += Settings.SetGameVersion;
    }

    public override TimeSpan? GetGameTime(LiveSplitState state)
    {
        // Check that IGT is ticking and not reset.
        uint currentLevelTicks = ClassicGameData.LevelTime.Current;
        uint oldLevelTicks = ClassicGameData.LevelTime.Old;
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
            uint currentDemoTimer = GameData.DemoTimer.Current;
            const uint demoStartThreshold = 480;
            if (currentDemoTimer > demoStartThreshold)
                return null;

            // Check that the player is in a real level.
            uint currentLevel = BaseGameData.Level.Current;
            var lastRealLevel = GetLastRealLevel(currentLevel);
            if (lastRealLevel is null)
                return null;
        }

        // Sum the current and completed levels' IGT.
        double currentLevelTime = BaseGameData.LevelTimeAsDouble(currentLevelTicks);
        double finishedLevelsTime = Data.SumCompletedLevelTimes(CompletedLevels, LastRealLevel);
        return TimeSpan.FromSeconds(currentLevelTime + finishedLevelsTime);
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
            _ => null
        };
    }

    public override bool ShouldSplit(LiveSplitState state) => LastRealLevel is not null && !CompletedLevels.Contains(LastRealLevel.Value) && base.ShouldSplit(state);

    public override bool ShouldStart(LiveSplitState state)
    {
        // These initialize to 0 and the game's timer will not tick until the title screen is reached.
        bool gameJustLaunched = ClassicGameData.LevelTime.Current == 0 && BaseGameData.Level.Old == 0;
        if (gameJustLaunched)
            return false;
            
        // Check to see if the player has navigated to the New Game page of the passport.
        // This prevent some misfires from LiveSplit hooking late.
        // If LiveSplit hooks after the player has already navigated to the New Game page, this fails.
        uint oldPassportPage = ClassicGameData.PickedPassportFunction.Old;
        uint currentPassportPage = ClassicGameData.PickedPassportFunction.Current;
        if (oldPassportPage == 0 && currentPassportPage == 1)
            _newGamePageSelected = true;

        // Determine if a new game was started; this applies to all runs but for FG, this is the only start condition.
        uint currentLevel = BaseGameData.Level.Current;
        uint oldLevel = BaseGameData.Level.Old;
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
                bool cameFromTitleScreen = ClassicGameData.TitleScreen.Old && !ClassicGameData.TitleScreen.Current;
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

        uint oldTime = ClassicGameData.LevelTime.Old;
        uint currentTime = ClassicGameData.LevelTime.Current;
        // The level values switch before the timer resets to 0. If LiveSplit polls at this time,
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
        GameData.CompletedLevelTicks.Add(ClassicGameData.LevelTime.Current);
        base.OnSplit(LastRealLevel!.Value);
    }

    public override void OnUndoSplit()
    {
        GameData.CompletedLevelTicks.RemoveAt(GameData.CompletedLevelTicks.Count - 1);
        base.OnUndoSplit();
    }
}