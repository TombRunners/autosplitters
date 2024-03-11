using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using LiveSplit.ComponentUtil;

namespace TR123;

public class GameData
{
    /// <summary>Used to calculate <see cref="TimeSpan" />s from IGT ticks.</summary>
    private const int IgtTicksPerSecond = 30;

    /// <summary>Reads the current active game or expansion, accounting for NG+ variations for base games.</summary>
    public static Game CurrentActiveGame
    {
        get
        {
            uint currentLevel = CurrentLevel();
            var baseGame = CurrentActiveBaseGame;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (baseGame)
            {
                case Game.Tr1:
                    if ((Tr1Level)currentLevel >= Tr1Level.ReturnToEgypt)
                        return Game.Tr1UnfinishedBusiness;
                    break;

                case Game.Tr2:
                    if ((Tr2Level)currentLevel >= Tr2Level.TheColdWar)
                        return Game.Tr2GoldenMask;
                    break;

                case Game.Tr3:
                    if ((Tr3Level)currentLevel >= Tr3Level.HighlandFling)
                        return Game.Tr3TheLostArtifact;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(ActiveGame.Current), "Unknown Game value read from CurrentActiveBaseGame.");
            }

            bool bonusFlag = BonusFlag.Current;
            if (!bonusFlag)
                return baseGame;

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return baseGame switch
            {
                Game.Tr1 => (Tr1Level)currentLevel < Tr1Level.ReturnToEgypt ? Game.Tr1NgPlus : Game.Tr1,
                Game.Tr2 => (Tr2Level)currentLevel < Tr2Level.TheColdWar ? Game.Tr2NgPlus : Game.Tr2,
                Game.Tr3 => (Tr3Level)currentLevel < Tr3Level.HighlandFling ? Game.Tr3NgPlus : Game.Tr3,
                _ => throw new ArgumentOutOfRangeException(nameof(ActiveGame.Current), "Unknown Game value read from CurrentActiveBaseGame."),
            };
        }
    }

    /// <summary>Identifies the game without NG+ identification.</summary>
    private static Game CurrentActiveBaseGame => (Game)(ActiveGame.Current * 3);

    public static MemoryWatcher<short> Health => HealthWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<short> InventoryChosen => InventoryChosenWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<bool> LevelComplete => LevelCompleteWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<uint> LevelIgt => LevelIgtWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<uint> LoadFade => LoadFadeWatchers[CurrentActiveBaseGame];
    public static MemoryWatcher<bool> TitleLoaded => TitleLoadedWatchers[CurrentActiveBaseGame];

    private static MemoryWatcher<bool> BonusFlag => BonusFlagWatchers[CurrentActiveBaseGame];
    private static MemoryWatcher<byte> Level => LevelWatchers[CurrentActiveBaseGame];

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the current level.</summary>
    /// <returns>Correct current level of the game</returns>
    public static uint CurrentLevel()
    {
        if (CurrentActiveBaseGame is not Game.Tr1)
            return Level.Current;

        uint levelCutsceneValue = Tr1LevelCutscene.Current;
        bool levelCutsceneIsAfterStatsScreen = (Tr1Level)levelCutsceneValue is Tr1Level.AfterNatlasMines;
        if (levelCutsceneIsAfterStatsScreen) // A cutscene after a stats screen increments the value to the level which hasn't started yet.
            return Level.Current - 1U;

        // First level check is necessary because Level value is based on save-game info, so it is not updated until the first level ends.
        // Otherwise, we use the Level value because we don't want to use cutscene values.
        bool levelCutsceneIsFirstLevel = (Tr1Level)levelCutsceneValue is Tr1Level.Caves or Tr1Level.AtlanteanStronghold;
        return levelCutsceneIsFirstLevel ? levelCutsceneValue : Level.Current;
    }

    /// <summary>Base games included within the remastered EXE.</summary>
    private static readonly ImmutableList<Game> BaseGames = [Game.Tr1, Game.Tr2, Game.Tr3];

    /// <summary>Contains the sizes of the save game structs for each base <see cref="Game" />.</summary>
    private static readonly ImmutableDictionary<Game, uint> GameSaveStructSizes = new Dictionary<Game, uint>(3)
    {
        { Game.Tr1, 0x30 },
        { Game.Tr2, 0x30 },
        { Game.Tr3, 0x40 },
    }.ToImmutableDictionary();

    /// <summary>Strings used when searching for a running game <see cref="Process" />.</summary>
    private static readonly ImmutableList<string> ProcessSearchNames = ["tomb123"];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>The uint will be converted from <see cref="GameVersion" />.</remarks>
    private static readonly ImmutableDictionary<string, GameVersion> VersionHashes = new Dictionary<string, GameVersion>
    {
        { "0C0C1C466DAE013ABBB976F11B52C726".ToLowerInvariant(), (uint)GameVersion.EgsDebug },
        { "0A937857C0AF755AEEAA98F4520CA0D2".ToLowerInvariant(), (uint)GameVersion.PublicV10 },
        { "769B1016F945167C48C6837505E37748".ToLowerInvariant(), (uint)GameVersion.PublicV101 },
        { "5B1644AFFD7BAD65B2AC5D76F15139C6".ToLowerInvariant(), (uint)GameVersion.PublicV102 },
    }.ToImmutableDictionary();

    /// <summary>Contains the names of the modules (DLLs) for each <see cref="Game" />.</summary>
    private static readonly ImmutableDictionary<Game, string> GameModules = new Dictionary<Game, string>(3)
    {
        { Game.Tr1, "tomb1.dll" },
        { Game.Tr2, "tomb2.dll" },
        { Game.Tr3, "tomb3.dll" },
    }.ToImmutableDictionary();

    /// <summary>For each released remastered game version, contains each game's address offsets.</summary>
    private static readonly ImmutableDictionary<GameVersion, Dictionary<Game, GameAddresses>> GameVersionAddresses =
        new Dictionary<GameVersion, Dictionary<Game, GameAddresses>>
        {
            {
                GameVersion.PublicV10,
                new Dictionary<Game, GameAddresses>
                {
                    {
                        Game.Tr1,
                        new GameAddresses
                        {
                            BonusFlag = 0x36CBBEA,
                            FirstLevelTime = 0x36CB610,
                            Health = 0xEAA30,
                            InventoryChosen = 0xD4F48,
                            Level = 0x36CBBE8,
                            LevelComplete = 0xEA340,
                            LevelIgt = 0x36CBBD0,
                            LoadFade = 0x2D7650,
                            TitleLoaded = 0xEA338,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x36FFE46,
                            FirstLevelTime = 0x36FF870,
                            Health = 0x11C780,
                            InventoryChosen = 0x100024,
                            Level = 0x11E7C8,
                            LevelComplete = 0x11ECE4,
                            LevelIgt = 0x36FFE2C,
                            LoadFade = 0x2796F4,
                            TitleLoaded = 0x11E884,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x3764994,
                            FirstLevelTime = 0x3764184,
                            Health = 0x179C28,
                            InventoryChosen = 0x156224,
                            Level = 0x17BECC,
                            LevelComplete = 0x17C3AC,
                            LevelIgt = 0x3764968,
                            LoadFade = 0x244BC4,
                            TitleLoaded = 0x17BF98,
                        }
                    },
                }
            },
            {
                GameVersion.PublicV101,
                new Dictionary<Game, GameAddresses>
                {
                    {
                        Game.Tr1,
                        new GameAddresses
                        {
                            BonusFlag = 0x371EBEA,
                            FirstLevelTime = 0x371E610,
                            Health = 0xEFA18,
                            InventoryChosen = 0xD9F48,
                            Level = 0x371EBE8,
                            LevelComplete = 0xEF340,
                            LevelIgt = 0x371EBD0,
                            LoadFade = 0x2619CE0,
                            TitleLoaded = 0xEF338,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x3753E26,
                            FirstLevelTime = 0x3753850,
                            Health = 0x122780,
                            InventoryChosen = 0x106024,
                            Level = 0x1247C8,
                            LevelComplete = 0x124CE4,
                            LevelIgt = 0x3753E0C,
                            LoadFade = 0x2BF6D4,
                            TitleLoaded = 0x124884,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x37B7974,
                            FirstLevelTime = 0x37B7164,
                            Health = 0x17EC28,
                            InventoryChosen = 0x15B224,
                            Level = 0x180ECC,
                            LevelComplete = 0x1813AC,
                            LevelIgt = 0x37B7948,
                            LoadFade = 0x31C630,
                            TitleLoaded = 0x180F98,
                        }
                    },
                }
            },
            {
                GameVersion.PublicV102,
                new Dictionary<Game, GameAddresses>
                {
                    {
                        Game.Tr1,
                        new GameAddresses
                        {
                            BonusFlag = 0x372978A,
                            FirstLevelTime = 0x37291B0,
                            Health = 0xF3A88,
                            InventoryChosen = 0xDDF48,
                            Level = 0x3729788,
                            LevelComplete = 0xF33A0,
                            LevelIgt = 0x3729770,
                            LoadFade = 0x292444,
                            TitleLoaded = 0xF3398,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x375E9E6,
                            FirstLevelTime = 0x375E410,
                            Health = 0x1267B0,
                            InventoryChosen = 0x10A024,
                            Level = 0x128808,
                            LevelComplete = 0x128D24,
                            LevelIgt = 0x375E9CC,
                            LoadFade = 0x2C7F54,
                            TitleLoaded = 0x1288C4,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x37C26F4,
                            FirstLevelTime = 0x37C1EE4,
                            Health = 0x182C58,
                            InventoryChosen = 0x15F224,
                            Level = 0x184F2C,
                            LevelComplete = 0x18540C,
                            LevelIgt = 0x37C26C8,
                            LoadFade = 0x324ED0,
                            TitleLoaded = 0x184FF8,
                        }
                    },
                }
            },
        }.ToImmutableDictionary();

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
    private static readonly MemoryWatcherList Watchers = [];

    #region MemoryWatcherList Items

    /// <summary>Gives the value of the active game, where TR1 is 0, TR2 is 1, TR3 is 2.</summary>
    /// <remarks>The uint should be converted to <see cref="GameVersion" />.</remarks>
    private static MemoryWatcher<uint> ActiveGame => (MemoryWatcher<uint>)Watchers?["ActiveGame"];

    /// <summary>The game's bonus flag which marks NG(+).</summary>
    /// <remarks>0 is NG, 1 is NG+; this flag has no effects on expansions.</remarks>
    private static ImmutableDictionary<Game, MemoryWatcher<bool>> BonusFlagWatchers =>
        new Dictionary<Game, MemoryWatcher<bool>>(3)
        {
            { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1BonusFlag"] },
            { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2BonusFlag"] },
            { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3BonusFlag"] },
        }.ToImmutableDictionary();

    /// <summary>Lara's current HP.</summary>
    /// <remarks>Max HP is 1000. When less than or equal to 0, Lara dies.</remarks>
    private static ImmutableDictionary<Game, MemoryWatcher<short>> HealthWatchers =>
        new Dictionary<Game, MemoryWatcher<short>>(3)
        {
            { Game.Tr1, (MemoryWatcher<short>)Watchers?["Tr1Health"] },
            { Game.Tr2, (MemoryWatcher<short>)Watchers?["Tr2Health"] },
            { Game.Tr3, (MemoryWatcher<short>)Watchers?["Tr3Health"] },
        }.ToImmutableDictionary();

    /// <summary>Gives a value for a chosen inventory item, or -1 if not in the inventory / title.</summary>
    private static ImmutableDictionary<Game, MemoryWatcher<short>> InventoryChosenWatchers =>
        new Dictionary<Game, MemoryWatcher<short>>(3)
        {
            { Game.Tr1, (MemoryWatcher<short>)Watchers?["Tr1InventoryChosen"] },
            { Game.Tr2, (MemoryWatcher<short>)Watchers?["Tr2InventoryChosen"] },
            { Game.Tr3, (MemoryWatcher<short>)Watchers?["Tr3InventoryChosen"] },
        }.ToImmutableDictionary();

    /// <summary>Gives the value of the active level.</summary>
    /// <remarks>
    ///     Usually matches chronological number. Some exceptions are TR3 due to level order choice and TR1's Unfinished
    ///     Business.
    /// </remarks>
    private static ImmutableDictionary<Game, MemoryWatcher<byte>> LevelWatchers =>
        new Dictionary<Game, MemoryWatcher<byte>>(3)
        {
            { Game.Tr1, (MemoryWatcher<byte>)Watchers?["Tr1Level"] },
            { Game.Tr2, (MemoryWatcher<byte>)Watchers?["Tr2Level"] },
            { Game.Tr3, (MemoryWatcher<byte>)Watchers?["Tr3Level"] },
        }.ToImmutableDictionary();

    /// <summary>Indicates if the current level is finished.</summary>
    /// <remarks>
    ///     1 while an end-level stats screen is active.
    ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
    ///     Before most end-level in-game cutscenes, the value changes from 0 to 1 then back to 0 immediately.
    ///     Otherwise, the value is 0.
    /// </remarks>
    private static ImmutableDictionary<Game, MemoryWatcher<bool>> LevelCompleteWatchers =>
        new Dictionary<Game, MemoryWatcher<bool>>(3)
        {
            { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1LevelComplete"] },
            { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2LevelComplete"] },
            { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3LevelComplete"] },
        }.ToImmutableDictionary();

    /// <summary>Gives the running IGT of the current level.</summary>
    private static ImmutableDictionary<Game, MemoryWatcher<uint>> LevelIgtWatchers =>
        new Dictionary<Game, MemoryWatcher<uint>>(3)
        {
            { Game.Tr1, (MemoryWatcher<uint>)Watchers?["Tr1LevelIgt"] },
            { Game.Tr2, (MemoryWatcher<uint>)Watchers?["Tr2LevelIgt"] },
            { Game.Tr3, (MemoryWatcher<uint>)Watchers?["Tr3LevelIgt"] },
        }.ToImmutableDictionary();

    /// <summary>Gives the value of the loading screen fade-in and fade-out (like an alpha).</summary>
    private static ImmutableDictionary<Game, MemoryWatcher<uint>> LoadFadeWatchers =>
        new Dictionary<Game, MemoryWatcher<uint>>(3)
        {
            { Game.Tr1, (MemoryWatcher<uint>)Watchers?["Tr1LoadFade"] },
            { Game.Tr2, (MemoryWatcher<uint>)Watchers?["Tr2LoadFade"] },
            { Game.Tr3, (MemoryWatcher<uint>)Watchers?["Tr3LoadFade"] },
        }.ToImmutableDictionary();

    /// <summary>Tells if the game is currently in the title screen.</summary>
    private static ImmutableDictionary<Game, MemoryWatcher<bool>> TitleLoadedWatchers =>
        new Dictionary<Game, MemoryWatcher<bool>>(3)
        {
            { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1TitleLoaded"] },
            { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2TitleLoaded"] },
            { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3TitleLoaded"] },
        }.ToImmutableDictionary();

    private static MemoryWatcher<uint> Tr1LevelCutscene => (MemoryWatcher<uint>)Watchers?["Tr1LevelCutscene"];

    #endregion

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    private static Process _gameProcess;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    private static GameVersion _version;

    /// <summary>Allows creation of an event regarding when an ASL Component was found in the LiveSplit layout.</summary>
    public delegate void AslComponentChangedDelegate(bool aslComponentIsPresent);

    /// <summary>Allows subscribers to know when an ASL Component was found in the LiveSplit layout.</summary>
    public AslComponentChangedDelegate OnAslComponentChanged;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The version found; the uint will be converted to <see cref="GameVersion" />.</param>
    public delegate void GameFoundDelegate(GameVersion version);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public GameFoundDelegate OnGameFound;

    /// <summary>Sets addresses based on <paramref name="version" />.</summary>
    /// <param name="version">Version to base addresses on; the uint will be converted to <see cref="GameVersion" />.</param>
    private static void SetAddresses(GameVersion version)
    {
        Watchers.Clear();

        switch (version)
        {
            case GameVersion.PublicV10:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x1A5B78)) { Name = "ActiveGame" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xCFA54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddWatchersForAllGames(GameVersion.PublicV10);
                break;

            case GameVersion.PublicV101:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD6B68)) { Name = "ActiveGame" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xD4A54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddWatchersForAllGames(GameVersion.PublicV101);
                break;

            case GameVersion.PublicV102:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xDFB68)) { Name = "ActiveGame" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xD8A54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddWatchersForAllGames(GameVersion.PublicV102);
                break;

            case GameVersion.None:
            case GameVersion.Unknown:
            case GameVersion.EgsDebug:
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }

    /// <summary>Adds MemoryWatchers which are common across all games.</summary>
    /// <param name="version">Game version</param>
    private static void AddWatchersForAllGames(GameVersion version)
    {
        foreach (var game in BaseGames)
        {
            string moduleName = GameModules[game];
            var addresses = GameVersionAddresses[version][game];

            int bonusFlagOffset = addresses.BonusFlag;
            Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, bonusFlagOffset)) { Name = $"{game}BonusFlag"});

            int healthOffset = addresses.Health;
            Watchers.Add(new MemoryWatcher<short>(new DeepPointer(moduleName, healthOffset)) { Name = $"{game}Health" });

            int inventoryChosenOffset = addresses.InventoryChosen;
            Watchers.Add(new MemoryWatcher<short>(new DeepPointer(moduleName, inventoryChosenOffset)) { Name = $"{game}InventoryChosen" });

            int levelOffset = addresses.Level;
            Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(moduleName, levelOffset)) { Name = $"{game}Level" });

            int levelCompleteOffset = addresses.LevelComplete;
            Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, levelCompleteOffset)) { Name = $"{game}LevelComplete" });

            int levelIgtOffset = addresses.LevelIgt;
            Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(moduleName, levelIgtOffset)) { Name = $"{game}LevelIgt" });

            int loadFadeOffset = addresses.LoadFade;
            Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(moduleName, loadFadeOffset)) { Name = $"{game}LoadFade" });

            int titleLoadedOffset = addresses.TitleLoaded;
            Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, titleLoadedOffset)) { Name = $"{game}TitleLoaded" });
        }
    }

    /// <summary>Updates <see cref="GameData" /> implementation and its addresses' values.</summary>
    /// <returns><see langword="true" /> if game data was updated, <see langword="false" /> otherwise</returns>
    public bool Update()
    {
        try
        {
            if (_gameProcess is null || _gameProcess.HasExited)
            {
                if (!SetGameProcessAndVersion())
                    return false;

                SetAddresses(_version);
                OnGameFound.Invoke(_version);
                return true;
            }

            Watchers.UpdateAll(_gameProcess);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>If applicable, finds a <see cref="Process" /> running an expected version of the game.</summary>
    /// <returns>
    ///     <see langword="true" /> if <see cref="_gameProcess" /> and <see cref="_version" /> were meaningfully set,
    ///     <see langword="false" /> otherwise
    /// </returns>
    private bool SetGameProcessAndVersion()
    {
        // Find game Processes.
        var processes = ProcessSearchNames.SelectMany(Process.GetProcessesByName).ToList();
        if (processes.Count == 0)
        {
            if (_version != GameVersion.None)
                OnGameFound.Invoke(GameVersion.None);

            _version = GameVersion.None;
            return false;
        }

        // Try finding a match from known version hashes.
        var gameProcess = processes.FirstOrDefault(static p => VersionHashes.TryGetValue(p.GetMd5Hash(), out _version));
        if (gameProcess is null)
        {
            if (_version != GameVersion.Unknown)
                OnGameFound.Invoke(GameVersion.Unknown);

            _version = GameVersion.Unknown;
            return false;
        }

        if (_version is GameVersion.EgsDebug)
        {
            OnGameFound.Invoke(GameVersion.EgsDebug);
            return false;
        }

        // Set GameProcess and do some event management.
        _gameProcess = gameProcess;
        _gameProcess.EnableRaisingEvents = true;
        _gameProcess.Exited += (_, _) => OnGameFound.Invoke(GameVersion.None);
        return true;
    }

    /// <summary>Converts IGT ticks to a double representing time elapsed in decimal seconds.</summary>
    public static double LevelTimeAsDouble(ulong ticks) => (double)ticks / IgtTicksPerSecond;

    /// <summary>Sums completed levels' times.</summary>
    /// <returns>The sum of completed levels' times</returns>
    public static ulong SumCompletedLevelTimesInMemory(IEnumerable<uint> completedLevels, uint? currentLevel)
    {
        var activeBaseGame = CurrentActiveBaseGame;
        string activeModuleName = GameModules[activeBaseGame];
        var activeModule = _gameProcess.ModulesWow64Safe().FirstOrDefault(module => module.ModuleName == activeModuleName);
        if (activeModule is null)
            return 0;

        uint levelSaveStructSize = GameSaveStructSizes[activeBaseGame];

        int firstLevelTimeAddress = GameVersionAddresses[_version][activeBaseGame].FirstLevelTime;
        var moduleBaseAddress = activeModule.BaseAddress;
        uint finishedLevelsTicks = completedLevels
            .TakeWhile(completedLevel => completedLevel != currentLevel)
            .Select(completedLevel => (completedLevel - 1) * levelSaveStructSize)
            .Select(levelOffset => (IntPtr)((long)moduleBaseAddress + firstLevelTimeAddress + levelOffset))
            .Aggregate<IntPtr, uint>(0, static (ticks, levelAddress) => ticks + _gameProcess.ReadValue<uint>(levelAddress));

        return finishedLevelsTicks;
    }
}