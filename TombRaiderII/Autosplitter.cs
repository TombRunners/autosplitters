using LiveSplit.Model;
using System.Diagnostics.CodeAnalysis;
using TRUtil;

// ReSharper disable UnusedMember.Global

namespace TR2;

/// <summary>The game's level and demo values.</summary>
[SuppressMessage("ReSharper", "IdentifierTypo")]
public enum Tr2Level
{
    // Levels
    LarasHome = 00,
    GreatWall = 01,
    Venice = 02,
    BartolisHideout = 03,
    OperaHouse = 04,
    OffshoreRig = 05,
    DivingArea = 06,
    FortyFathoms = 07,
    WreckOfTheMariaDoria = 08,
    LivingQuarters = 09,
    TheDeck = 10,
    TibetanFoothills = 11,
    BarkhangMonastery = 12,
    CatacombsOfTheTalion = 13,
    IcePalace = 14,
    TempleOfXian = 15,
    FloatingIslands = 16,
    DragonsLair = 17,
    HomeSweetHome = 18,

    // Demos
    DemoVenice = 19,
    DemoWreckOfTheMariaDoria = 20,
    DemoTibetanFoothills = 21,
}

/// <summary>The game's level and demo values.</summary>
public enum Tr2GoldLevel
{
    // Levels
    TheColdWar = 01,
    FoolsGold = 02,
    FurnaceOfTheGods = 03,
    Kingdom = 04,

    // Bonus
    NightmareInVegas = 05,
}

/// <summary>Implementation of <see cref="ClassicAutosplitter"/>.</summary>
internal sealed class Autosplitter : ClassicAutosplitter
{
    private bool _newGamePageSelected;

    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter()
    {
        Settings = new ComponentSettings();

        LevelCount = 18; // This is the highest between TR2 at 18 and TR2G at 5.
        CompletedLevels.Capacity = LevelCount;

        Data = new GameData();
        Data.OnGameFound += Settings.SetGameVersion;
    }

    public override bool ShouldStart(LiveSplitState state)
    {
        uint currentLevel = BaseGameData.Level.Current;
        uint oldLevel = BaseGameData.Level.Old;

        // Ignore demos.
        if (currentLevel >= (uint)Tr2Level.DemoVenice) // Never true for TR2G; level values are never so high.
            return false;

        // Check to see if the player has navigated to the New Game page of the passport.
        // This prevents some misfires from LiveSplit hooking late.
        // If LiveSplit hooks after the player has already navigated to the New Game page, this fails.
        uint oldPassportPage = ClassicGameData.PickedPassportFunction.Old;
        uint currentPassportPage = ClassicGameData.PickedPassportFunction.Current;
        if (oldPassportPage == 0 && currentPassportPage == 1)
            _newGamePageSelected = true;

        // Determine if a new game was started; this applies to all runs but for FG runs, this is the only start condition.
        if (_newGamePageSelected)
        {
            bool cameFromTitleScreen = ClassicGameData.TitleScreen.Old && !ClassicGameData.TitleScreen.Current;
            bool cameFromLarasHome = oldLevel == (uint)Tr2Level.LarasHome; // Never true for TR2G.
            bool justStartedFirstLevel = currentLevel == 1; // This value is good for GreatWall and TheColdWar.
            bool newGameStarted = (cameFromTitleScreen || cameFromLarasHome) && justStartedFirstLevel;
            if (newGameStarted)
                return true;
        }
        else if (Settings.FullGame)
        {
            return false;
        }

        // The remaining logic only applies to non-FG runs starting on a level besides the first.
        uint oldTime = ClassicGameData.LevelTime.Old;
        uint currentTime = ClassicGameData.LevelTime.Current;
        bool wentToNextLevel = oldLevel == currentLevel - 1;
        return wentToNextLevel && oldTime > currentTime;
    }

    public override void OnStart()
    {
        _newGamePageSelected = false;
        base.OnStart();
    }
}