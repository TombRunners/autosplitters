using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using LiveSplit.ComponentUtil;

namespace TR123;

internal class GameMemory
{
    /// <summary>Base games included within the remastered EXE.</summary>
    private static readonly ImmutableList<Game> BaseGames = [Game.Tr1, Game.Tr2, Game.Tr3];

    /// <summary>Contains the names of the modules (DLLs) for each <see cref="Game" />.</summary>
    internal static readonly ImmutableDictionary<Game, string> GameModules = new Dictionary<Game, string>(3)
    {
        { Game.Tr1, "tomb1.dll" },
        { Game.Tr2, "tomb2.dll" },
        { Game.Tr3, "tomb3.dll" },
    }.ToImmutableDictionary();

    /// <summary>For each released remastered game version, contains each game's address offsets.</summary>
    internal static readonly ImmutableDictionary<GameVersion, Dictionary<Game, GameAddresses>> GameVersionAddresses =
        new Dictionary<GameVersion, Dictionary<Game, GameAddresses>>
        {
            {
                GameVersion.GogV10,
                new Dictionary<Game, GameAddresses>
                {
                    {
                        Game.Tr1,
                        new GameAddresses
                        {
                            BonusFlag = 0x36CBBEA, // LevelIgt + 0x1A
                            Cine = 0x35F86F0,
                            FirstLevelTime = 0x36CB610,
                            Health = 0xEAA30,
                            InventoryChosen = 0xD4F48,
                            InventoryMode = 0xD5C18, // InventoryChosen + 0xCD0
                            Level = 0x36CBBE8,
                            LevelComplete = 0xEA340,
                            LevelIgt = 0x36CBBD0,
                            LoadFade = 0x244BC4,
                            OverlayFlag = 0xCFA78,
                            TitleLoaded = 0xEA338,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x36FFE46, // LevelIgt + 0x1A
                            Cine = 0x3606928,
                            FirstLevelTime = 0x36FF870,
                            Health = 0x11C780,
                            InventoryChosen = 0x100024,
                            InventoryMode = 0x100084, // InventoryChosen + 0x60
                            Level = 0x11E7C8,
                            LevelComplete = 0x11ECE4,
                            LevelIgt = 0x36FFE2C,
                            LoadFade = 0x2796F4,
                            OverlayFlag = 0xFFEEC,
                            TitleLoaded = 0x11E884,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x3764994, // LevelIgt + 0x2C
                            Cine = 0x3666000,
                            FirstLevelTime = 0x3764184,
                            Health = 0x179C28,
                            InventoryChosen = 0x156224,
                            InventoryMode = 0x156254, // InventoryChosen + 0x30
                            Level = 0x17BECC,
                            LevelComplete = 0x17C3AC,
                            LevelIgt = 0x3764968,
                            LoadFade = 0x2D7650,
                            OverlayFlag = 0x155BDC,
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
                            BonusFlag = 0x371EBEA, // LevelIgt + 0x1A
                            Cine = 0x364B6F0,
                            FirstLevelTime = 0x371E610,
                            Health = 0xEFA18,
                            InventoryChosen = 0xD9F48,
                            InventoryMode = 0xDAC18, // InventoryChosen + 0xCD0
                            Level = 0x371EBE8,
                            LevelComplete = 0xEF340,
                            LevelIgt = 0x371EBD0,
                            LoadFade = 0x2619CE0,
                            OverlayFlag = 0xD4A78,
                            TitleLoaded = 0xEF338,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x3753E26, // LevelIgt + 0x1A
                            Cine = 0x365A908,
                            FirstLevelTime = 0x3753850,
                            Health = 0x122780,
                            InventoryChosen = 0x106024,
                            InventoryMode = 0x106084, // InventoryChosen + 0x60
                            Level = 0x1247C8,
                            LevelComplete = 0x124CE4,
                            LevelIgt = 0x3753E0C,
                            LoadFade = 0x2BF6D4,
                            OverlayFlag = 0x105EEC,
                            TitleLoaded = 0x124884,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x37B7974, // LevelIgt + 0x2C
                            Cine = 0x36B8FE0,
                            FirstLevelTime = 0x37B7164,
                            Health = 0x17EC28,
                            InventoryChosen = 0x15B224,
                            InventoryMode = 0x15B254, // InventoryChosen + 0x30
                            Level = 0x180ECC,
                            LevelComplete = 0x1813AC,
                            LevelIgt = 0x37B7948,
                            LoadFade = 0x31C630,
                            OverlayFlag = 0x15ABDC,
                            TitleLoaded = 0x180F98,
                        }
                    },
                }
            },
            {
                GameVersion.Patch1,
                new Dictionary<Game, GameAddresses>
                {
                    {
                        Game.Tr1,
                        new GameAddresses
                        {
                            BonusFlag = 0x372978A, // LevelIgt + 0x1A
                            Cine = 0x36561D8,
                            FirstLevelTime = 0x37291B0,
                            Health = 0xF3A88,
                            InventoryChosen = 0xDDF48,
                            InventoryMode = 0xDEC18, // InventoryChosen + 0xCD0
                            Level = 0x3729788,
                            LevelComplete = 0xF33A0,
                            LevelIgt = 0x3729770,
                            LoadFade = 0x292444,
                            OverlayFlag = 0xD8A78,
                            TitleLoaded = 0xF3398,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x375E9E6, // LevelIgt + 0x1A
                            Cine = 0x36633E8,
                            FirstLevelTime = 0x375E410,
                            Health = 0x1267B0,
                            InventoryChosen = 0x10A024,
                            InventoryMode = 0x10A084, // InventoryChosen + 0x60
                            Level = 0x128808,
                            LevelComplete = 0x128D24,
                            LevelIgt = 0x375E9CC,
                            LoadFade = 0x2C7F54,
                            OverlayFlag = 0x109EEC,
                            TitleLoaded = 0x1288C4,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x37C26F4, // LevelIgt + 0x2C
                            Cine = 0x36C1C80,
                            FirstLevelTime = 0x37C1EE4,
                            Health = 0x182C58,
                            InventoryChosen = 0x15F224,
                            InventoryMode = 0x15F254, // InventoryChosen + 0x30
                            Level = 0x184F2C,
                            LevelComplete = 0x18540C,
                            LevelIgt = 0x37C26C8,
                            LoadFade = 0x324ED0,
                            OverlayFlag = 0x15EBDC,
                            TitleLoaded = 0x184FF8,
                        }
                    },
                }
            },
            {
                GameVersion.Patch2,
                new Dictionary<Game, GameAddresses>
                {
                    {
                        Game.Tr1,
                        new GameAddresses
                        {
                            BonusFlag = 0x373BFEA, // LevelIgt + 0x1A
                            Cine = 0x3668A38,
                            FirstLevelTime = 0x373BA10,
                            Health = 0xF6B68,
                            InventoryChosen = 0xE0F48,
                            InventoryMode = 0xE1C18, // InventoryChosen + 0xCD0
                            Level = 0x373BFE8,
                            LevelComplete = 0xF6480,
                            LevelIgt = 0x373BFD0,
                            LoadFade = 0x295564,
                            OverlayFlag = 0xDBA78,
                            TitleLoaded = 0xF6478,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x3772246, // LevelIgt + 0x1A
                            Cine = 0x3674C28,
                            FirstLevelTime = 0x3771C70,
                            Health = 0x12A818,
                            InventoryChosen = 0x10E024,
                            InventoryMode = 0x10E084, // InventoryChosen + 0x60
                            Level = 0x12C908,
                            LevelComplete = 0x12CE24,
                            LevelIgt = 0x377222C,
                            LoadFade = 0x2CC094,
                            OverlayFlag = 0x10DEEC,
                            TitleLoaded = 0x12C9C4,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x37D8394, // LevelIgt + 0x2C
                            Cine = 0x36D4940,
                            FirstLevelTime = 0x37D7B84,
                            Health = 0x187CA8,
                            InventoryChosen = 0x164224,
                            InventoryMode = 0x164254, // InventoryChosen + 0x30
                            Level = 0x18A00C,
                            LevelComplete = 0x18A4EC,
                            LevelIgt = 0x37D8368,
                            LoadFade = 0x329FF0,
                            OverlayFlag = 0x163BDC,
                            TitleLoaded = 0x18A0D8,
                        }
                    },
                }
            },
            {
                GameVersion.Patch3,
                new Dictionary<Game, GameAddresses>
                {
                    {
                        Game.Tr1,
                        new GameAddresses
                        {
                            BonusFlag = 0x373E1EA, // LevelIgt + 0x1A
                            Cine = 0x366AC38,
                            FirstLevelTime = 0x373DC10,
                            Health = 0xF7C88,
                            InventoryChosen = 0xE1F88,
                            InventoryMode = 0xE2C58, // InventoryChosen + 0xCD0
                            Level = 0x373E1E8, // LevelIgt + 0x18
                            LevelComplete = 0xF7560, // TitleLoaded + 0x8
                            LevelIgt = 0x373E1D0,
                            LoadFade = 0x297764,
                            OverlayFlag = 0xDCA78,
                            TitleLoaded = 0xF7558,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x3775446, // LevelIgt + 0x1A
                            Cine = 0x3677E28,
                            FirstLevelTime = 0x3774E70,
                            Health = 0x12C8A8,
                            InventoryChosen = 0x110024,
                            InventoryMode = 0x110084, // InventoryChosen + 0x60
                            Level = 0x12E9E8,
                            LevelComplete = 0x12EF44,
                            LevelIgt = 0x377542C,
                            LoadFade = 0x2CF274,
                            OverlayFlag = 0x10FEEC,
                            TitleLoaded = 0x12EAA4,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x37D95B4, // LevelIgt + 0x2C
                            Cine = 0x36D5B60,
                            FirstLevelTime = 0x37D8DA4,
                            Health = 0x187D38,
                            InventoryChosen = 0x164224,
                            InventoryMode = 0x164254, // InventoryChosen + 0x30
                            Level = 0x18A0EC,
                            LevelComplete = 0x18A60C,
                            LevelIgt = 0x37D9588,
                            LoadFade = 0x32B1F0,
                            OverlayFlag = 0x163BDC,
                            TitleLoaded = 0x18A1B8,
                        }
                    },
                }
            },
            {
                GameVersion.Patch4,
                new Dictionary<Game, GameAddresses>
                {
                    {
                        Game.Tr1,
                        new GameAddresses
                        {
                            BonusFlag = 0x4C124A, // LevelIgt + 0x1A
                            Cine = 0x3EDC98,
                            FirstLevelTime = 0x4C0C70,
                            Health = 0xF9CC8,
                            InventoryChosen = 0xE3F88,
                            InventoryMode = 0xE4C58, // InventoryChosen + 0xCD0
                            Level = 0x4C1248, // LevelIgt + 0x18
                            LevelComplete = 0xF95A0, // TitleLoaded + 0x8
                            LevelIgt = 0x4C1230,
                            LoadFade = 0x2997E4,
                            OverlayFlag = 0xDEA78,
                            TitleLoaded = 0xF9598,
                        }
                    },
                    {
                        Game.Tr2,
                        new GameAddresses
                        {
                            BonusFlag = 0x4F7466, // LevelIgt + 0x1A
                            Cine = 0x3F9E48,
                            FirstLevelTime = 0x4F6E90,
                            Health = 0x12D878,
                            InventoryChosen = 0x110FD4,
                            InventoryMode = 0x111034, // InventoryChosen + 0x60
                            Level = 0x12F9E8,
                            LevelComplete = 0x12FF40,
                            LevelIgt = 0x4F744C,
                            LoadFade = 0x2D0298,
                            OverlayFlag = 0x110E9C,
                            TitleLoaded = 0x12FAA4,
                        }
                    },
                    {
                        Game.Tr3,
                        new GameAddresses
                        {
                            BonusFlag = 0x55A4D4, // LevelIgt + 0x2C
                            Cine = 0x456AA0,
                            FirstLevelTime = 0x559CC4,
                            Health = 0x187BE8,
                            InventoryChosen = 0x1640B4,
                            InventoryMode = 0x1640E4, // InventoryChosen + 0x30
                            Level = 0x189FCC,
                            LevelComplete = 0x18A4E8,
                            LevelIgt = 0x55A4A8,
                            LoadFade = 0x32B150,
                            OverlayFlag = 0x16404C,
                            TitleLoaded = 0x18A098,
                        }
                    },
                }
            },
            // TODO: Add Patch 4 Update 1 common addresses.
            // TODO: Add Patch 4 Update 2 common addresses.
        }.ToImmutableDictionary();

    #region MemoryWatcher Definitions

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
    internal readonly MemoryWatcherList Watchers = [];

    /// <summary>Gives the value of the active game, where TR1 is 0, TR2 is 1, TR3 is 2.</summary>
    /// <remarks>The value should be converted to <see cref="GameVersion" />.</remarks>
    internal MemoryWatcher<int> ActiveGame => (MemoryWatcher<int>)Watchers?["ActiveGame"];

    /// <summary>
    ///     From when a load occurs (level, FMV, in-game cutscene, title screen),
    ///     resets to 0 and then increments at the rate of IGT ticks (30 per second).
    ///     During actual loading time (asset loading, etc.), freezes.
    /// </summary>
    internal MemoryWatcher<int> GFrameIndex => (MemoryWatcher<int>)Watchers?["GlobalFrameIndex"];

    /// <summary>The game's bonus flag which marks NG(+).</summary>
    /// <remarks>0 is NG, 1 is NG+; this flag has no effects on expansions.</remarks>
    internal ImmutableDictionary<Game, MemoryWatcher<bool>> BonusFlagWatchers =>
        new Dictionary<Game, MemoryWatcher<bool>>(3)
        {
            { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1BonusFlag"] },
            { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2BonusFlag"] },
            { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3BonusFlag"] },
        }.ToImmutableDictionary();

    /// <summary>This value is set immediately before a file read of an upcoming cutscene.</summary>
    internal ImmutableDictionary<Game, MemoryWatcher<short>> CineWatchers =>
        new Dictionary<Game, MemoryWatcher<short>>(3)
        {
            { Game.Tr1,( MemoryWatcher<short>)Watchers?["Tr1Cine"] },
            { Game.Tr2,( MemoryWatcher<short>)Watchers?["Tr2Cine"] },
            { Game.Tr3,( MemoryWatcher<short>)Watchers?["Tr3Cine"] },
        }.ToImmutableDictionary();

    /// <summary>Lara's current HP.</summary>
    /// <remarks>Max HP is 1000. When less than or equal to 0, Lara dies.</remarks>
    internal ImmutableDictionary<Game, MemoryWatcher<short>> HealthWatchers =>
        new Dictionary<Game, MemoryWatcher<short>>(3)
        {
            { Game.Tr1, (MemoryWatcher<short>)Watchers?["Tr1Health"] },
            { Game.Tr2, (MemoryWatcher<short>)Watchers?["Tr2Health"] },
            { Game.Tr3, (MemoryWatcher<short>)Watchers?["Tr3Health"] },
        }.ToImmutableDictionary();

    /// <summary>Gives a value for a chosen inventory item, or -1 if not in the inventory / title.</summary>
    internal ImmutableDictionary<Game, MemoryWatcher<short>> InventoryChosenWatchers =>
        new Dictionary<Game, MemoryWatcher<short>>(3)
        {
            { Game.Tr1, (MemoryWatcher<short>)Watchers?["Tr1InventoryChosen"] },
            { Game.Tr2, (MemoryWatcher<short>)Watchers?["Tr2InventoryChosen"] },
            { Game.Tr3, (MemoryWatcher<short>)Watchers?["Tr3InventoryChosen"] },
        }.ToImmutableDictionary();

    /// <summary>Value backed by <see cref="TR123.InventoryMode" />.</summary>
    internal ImmutableDictionary<Game, MemoryWatcher<InventoryMode>> InventoryModeWatchers =>
        new Dictionary<Game, MemoryWatcher<InventoryMode>>(3)
        {
            { Game.Tr1, (MemoryWatcher<InventoryMode>)Watchers?["Tr1InventoryMode"] },
            { Game.Tr2, (MemoryWatcher<InventoryMode>)Watchers?["Tr2InventoryMode"] },
            { Game.Tr3, (MemoryWatcher<InventoryMode>)Watchers?["Tr3InventoryMode"] },
        }.ToImmutableDictionary();

    /// <summary>Gives the value of the active level.</summary>
    /// <remarks>
    ///     Usually matches chronological number. Some exceptions are TR3 due to level order choice and TR1's Unfinished
    ///     Business.
    /// </remarks>
    internal ImmutableDictionary<Game, MemoryWatcher<byte>> LevelWatchers =>
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
    internal ImmutableDictionary<Game, MemoryWatcher<bool>> LevelCompleteWatchers =>
        new Dictionary<Game, MemoryWatcher<bool>>(3)
        {
            { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1LevelComplete"] },
            { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2LevelComplete"] },
            { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3LevelComplete"] },
        }.ToImmutableDictionary();

    /// <summary>Gives the running IGT of the current level.</summary>
    internal ImmutableDictionary<Game, MemoryWatcher<uint>> LevelIgtWatchers =>
        new Dictionary<Game, MemoryWatcher<uint>>(3)
        {
            { Game.Tr1, (MemoryWatcher<uint>)Watchers?["Tr1LevelIgt"] },
            { Game.Tr2, (MemoryWatcher<uint>)Watchers?["Tr2LevelIgt"] },
            { Game.Tr3, (MemoryWatcher<uint>)Watchers?["Tr3LevelIgt"] },
        }.ToImmutableDictionary();

    /// <summary>Gives the value of the loading screen fade-in and fade-out (like an alpha).</summary>
    internal ImmutableDictionary<Game, MemoryWatcher<uint>> LoadFadeWatchers =>
        new Dictionary<Game, MemoryWatcher<uint>>(3)
        {
            { Game.Tr1, (MemoryWatcher<uint>)Watchers?["Tr1LoadFade"] },
            { Game.Tr2, (MemoryWatcher<uint>)Watchers?["Tr2LoadFade"] },
            { Game.Tr3, (MemoryWatcher<uint>)Watchers?["Tr3LoadFade"] },
        }.ToImmutableDictionary();

    /// <summary>Value backed by <see cref="TR123.OverlayFlag" />.</summary>
    /// <remarks>
    ///    In the Save Game shortcut menu (F5), it's -2.
    ///    In the Load Game shortcut menu (F9), it's -1. (If F9 is used when no saves are present, the game defaults to Save Game, but this value is still -1.)
    ///    In the inventory, it's 0.
    ///    In any gameplay, FMVs, cutscenes, stats screens, loading screens, a crystal save menu, a death menu, credits, & title screen, it's 1.
    /// </remarks>
    internal ImmutableDictionary<Game, MemoryWatcher<OverlayFlag>> OverlayFlagWatchers =>
        new Dictionary<Game, MemoryWatcher<OverlayFlag>>(3)
        {
            { Game.Tr1, (MemoryWatcher<OverlayFlag>)Watchers?["Tr1OverlayFlag"] },
            { Game.Tr2, (MemoryWatcher<OverlayFlag>)Watchers?["Tr2OverlayFlag"] },
            { Game.Tr3, (MemoryWatcher<OverlayFlag>)Watchers?["Tr3OverlayFlag"] },
        }.ToImmutableDictionary();

    /// <summary>Tells if the game is currently in the title screen.</summary>
    internal ImmutableDictionary<Game, MemoryWatcher<bool>> TitleLoadedWatchers =>
        new Dictionary<Game, MemoryWatcher<bool>>(3)
        {
            { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1TitleLoaded"] },
            { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2TitleLoaded"] },
            { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3TitleLoaded"] },
        }.ToImmutableDictionary();

    internal MemoryWatcher<uint> Tr1LevelCutscene => (MemoryWatcher<uint>)Watchers?["Tr1LevelCutscene"];

    #endregion

    /// <summary>Sets addresses based on <paramref name="version" />.</summary>
    /// <param name="version">Game version to base addresses on</param>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    internal void InitializeMemoryWatchers(uint version, Process gameProcess)
    {
        Watchers.Clear();

        switch ((GameVersion)version)
        {
            case GameVersion.GogV10:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0x1A5B78)) { Name = "ActiveGame" });
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0x3B9C44)) { Name = "GlobalFrameIndex" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xCFA54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddCommonDllWatchers(GameVersion.GogV10);
                break;

            case GameVersion.PublicV101:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0xD6B68)) { Name = "ActiveGame" });
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0x2EAC08)) { Name = "GlobalFrameIndex" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xD4A54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddCommonDllWatchers(GameVersion.PublicV101);
                break;

            case GameVersion.Patch1:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0xDFB68)) { Name = "ActiveGame" });
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0x2F3D0C)) { Name = "GlobalFrameIndex" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xD8A54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddCommonDllWatchers(GameVersion.Patch1);
                break;

            case GameVersion.Patch2:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0xDFB68)) { Name = "ActiveGame" });
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0x2F3D34)) { Name = "GlobalFrameIndex" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xDBA54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddCommonDllWatchers(GameVersion.Patch2);
                break;

            case GameVersion.Patch3:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0xDFB68)) { Name = "ActiveGame" });
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0x2F3DB4)) { Name = "GlobalFrameIndex" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xDCA54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddCommonDllWatchers(GameVersion.Patch3);
                break;

            case GameVersion.Patch4:
                // Base game EXE (tomb123.exe)
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0xE0B68)) { Name = "ActiveGame" });
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0x2EF37C)) { Name = "GlobalFrameIndex" });
                // One-offs from DLLs
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xDEA54)) { Name = "Tr1LevelCutscene" });
                // Common items for all game's DLLs
                AddCommonDllWatchers(GameVersion.Patch4);
                break;

            case GameVersion.Patch4Update1:
                // TODO: Add Patch 4 Update 1 non-shared addresses.
                AddCommonDllWatchers(GameVersion.Patch4Update1);
                break;

            case GameVersion.Patch4Update2:
                // TODO: Add Patch 4 Update 2 non-shared addresses.
                AddCommonDllWatchers(GameVersion.Patch4Update2);
                break;

            case GameVersion.EgsDebug:
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }

        PreLoadWatchers(gameProcess);
    }

    /// <summary>Adds MemoryWatchers which are common across all games.</summary>
    /// <param name="version">Game version</param>
    private void AddCommonDllWatchers(GameVersion version)
    {
        foreach (Game game in BaseGames)
        {
            string moduleName = GameModules[game];
            GameAddresses addresses = GameVersionAddresses[version][game];

            int bonusFlagOffset = addresses.BonusFlag;
            Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, bonusFlagOffset)) { Name = $"{game}BonusFlag" });

            int cineOffset = addresses.Cine;
            Watchers.Add(new MemoryWatcher<short>(new DeepPointer(moduleName, cineOffset)) { Name = $"{game}Cine" });

            int healthOffset = addresses.Health;
            Watchers.Add(new MemoryWatcher<short>(new DeepPointer(moduleName, healthOffset)) { Name = $"{game}Health" });

            int inventoryChosenOffset = addresses.InventoryChosen;
            Watchers.Add(new MemoryWatcher<short>(new DeepPointer(moduleName, inventoryChosenOffset)) { Name = $"{game}InventoryChosen" });

            int inventoryModeOffset = addresses.InventoryMode;
            Watchers.Add(new MemoryWatcher<InventoryMode>(new DeepPointer(moduleName, inventoryModeOffset)) { Name = $"{game}InventoryMode" });

            int levelOffset = addresses.Level;
            Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(moduleName, levelOffset)) { Name = $"{game}Level" });

            int levelCompleteOffset = addresses.LevelComplete;
            Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, levelCompleteOffset)) { Name = $"{game}LevelComplete" });

            int levelIgtOffset = addresses.LevelIgt;
            Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(moduleName, levelIgtOffset)) { Name = $"{game}LevelIgt" });

            int loadFadeOffset = addresses.LoadFade;
            Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(moduleName, loadFadeOffset)) { Name = $"{game}LoadFade" });

            int overlayFlagOffset = addresses.OverlayFlag;
            Watchers.Add(new MemoryWatcher<OverlayFlag>(new DeepPointer(moduleName, overlayFlagOffset)) { Name = $"{game}OverlayFlag" });

            int titleLoadedOffset = addresses.TitleLoaded;
            Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, titleLoadedOffset)) { Name = $"{game}TitleLoaded" });
        }
    }

    /// <summary>
    ///     This method should be called when initializing MemoryWatchers to ensure that they do not have
    ///     default / zeroed values on initialization, which complicates or ruins autosplitter logic.
    /// </summary>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    private void PreLoadWatchers(Process gameProcess)
    {
        Watchers.UpdateAll(gameProcess); // Loads Current values.
        Watchers.UpdateAll(gameProcess); // Moves Current to Old and loads new Current values.
    }
}
