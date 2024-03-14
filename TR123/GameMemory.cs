using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using LiveSplit.ComponentUtil;

namespace TR123;

public static partial class GameData
{
    private static partial class GameMemory
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
                    GameVersion.PublicV10,
                    new Dictionary<Game, GameAddresses>
                    {
                        {
                            Game.Tr1,
                            new GameAddresses
                            {
                                BonusFlag = 0x36CBBEA, // LevelIgt + 0x1A
                                FirstLevelTime = 0x36CB610,
                                Health = 0xEAA30,
                                InventoryChosen = 0xD4F48,
                                InventoryMode = 0xD5C18, // InventoryChosen + 0xCD0
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
                                BonusFlag = 0x36FFE46, // LevelIgt + 0x1A
                                FirstLevelTime = 0x36FF870,
                                Health = 0x11C780,
                                InventoryChosen = 0x100024,
                                InventoryMode = 0x100084, // InventoryChosen + 0x60
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
                                BonusFlag = 0x3764994, // LevelIgt + 0x2C
                                FirstLevelTime = 0x3764184,
                                Health = 0x179C28,
                                InventoryChosen = 0x156224,
                                InventoryMode = 0x156254, // InventoryChosen + 0x30
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
                                BonusFlag = 0x371EBEA, // LevelIgt + 0x1A
                                FirstLevelTime = 0x371E610,
                                Health = 0xEFA18,
                                InventoryChosen = 0xD9F48,
                                InventoryMode = 0xDAC18, // InventoryChosen + 0xCD0
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
                                BonusFlag = 0x3753E26, // LevelIgt + 0x1A
                                FirstLevelTime = 0x3753850,
                                Health = 0x122780,
                                InventoryChosen = 0x106024,
                                InventoryMode = 0x106084, // InventoryChosen + 0x60
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
                                BonusFlag = 0x37B7974, // LevelIgt + 0x2C
                                FirstLevelTime = 0x37B7164,
                                Health = 0x17EC28,
                                InventoryChosen = 0x15B224,
                                InventoryMode = 0x15B254, // InventoryChosen + 0x30
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
                                BonusFlag = 0x372978A, // LevelIgt + 0x1A
                                FirstLevelTime = 0x37291B0,
                                Health = 0xF3A88,
                                InventoryChosen = 0xDDF48,
                                InventoryMode = 0xDEC18, // InventoryChosen + 0xCD0
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
                                BonusFlag = 0x375E9E6, // LevelIgt + 0x1A
                                FirstLevelTime = 0x375E410,
                                Health = 0x1267B0,
                                InventoryChosen = 0x10A024,
                                InventoryMode = 0x10A084, // InventoryChosen + 0x60
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
                                BonusFlag = 0x37C26F4, // LevelIgt + 0x2C
                                FirstLevelTime = 0x37C1EE4,
                                Health = 0x182C58,
                                InventoryChosen = 0x15F224,
                                InventoryMode = 0x15F254, // InventoryChosen + 0x30
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

        #region MemoryWatcher Definitions

        /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
        internal static readonly MemoryWatcherList Watchers = [];

        /// <summary>Gives the value of the active game, where TR1 is 0, TR2 is 1, TR3 is 2.</summary>
        /// <remarks>The uint should be converted to <see cref="GameVersion" />.</remarks>
        internal static MemoryWatcher<int> ActiveGame => (MemoryWatcher<int>)Watchers?["ActiveGame"];

        /// <summary>The game's bonus flag which marks NG(+).</summary>
        /// <remarks>0 is NG, 1 is NG+; this flag has no effects on expansions.</remarks>
        internal static ImmutableDictionary<Game, MemoryWatcher<bool>> BonusFlagWatchers =>
            new Dictionary<Game, MemoryWatcher<bool>>(3)
            {
                { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1BonusFlag"] },
                { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2BonusFlag"] },
                { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3BonusFlag"] },
            }.ToImmutableDictionary();

        /// <summary>Lara's current HP.</summary>
        /// <remarks>Max HP is 1000. When less than or equal to 0, Lara dies.</remarks>
        internal static ImmutableDictionary<Game, MemoryWatcher<short>> HealthWatchers =>
            new Dictionary<Game, MemoryWatcher<short>>(3)
            {
                { Game.Tr1, (MemoryWatcher<short>)Watchers?["Tr1Health"] },
                { Game.Tr2, (MemoryWatcher<short>)Watchers?["Tr2Health"] },
                { Game.Tr3, (MemoryWatcher<short>)Watchers?["Tr3Health"] },
            }.ToImmutableDictionary();

        /// <summary>Gives a value for a chosen inventory item, or -1 if not in the inventory / title.</summary>
        internal static ImmutableDictionary<Game, MemoryWatcher<short>> InventoryChosenWatchers =>
            new Dictionary<Game, MemoryWatcher<short>>(3)
            {
                { Game.Tr1, (MemoryWatcher<short>)Watchers?["Tr1InventoryChosen"] },
                { Game.Tr2, (MemoryWatcher<short>)Watchers?["Tr2InventoryChosen"] },
                { Game.Tr3, (MemoryWatcher<short>)Watchers?["Tr3InventoryChosen"] },
            }.ToImmutableDictionary();

        /// <summary>Value backed by <see cref="TR123.InventoryMode" />.</summary>
        internal static ImmutableDictionary<Game, MemoryWatcher<InventoryMode>> InventoryModeWatchers =>
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
        internal static ImmutableDictionary<Game, MemoryWatcher<byte>> LevelWatchers =>
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
        internal static ImmutableDictionary<Game, MemoryWatcher<bool>> LevelCompleteWatchers =>
            new Dictionary<Game, MemoryWatcher<bool>>(3)
            {
                { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1LevelComplete"] },
                { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2LevelComplete"] },
                { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3LevelComplete"] },
            }.ToImmutableDictionary();

        /// <summary>Gives the running IGT of the current level.</summary>
        internal static ImmutableDictionary<Game, MemoryWatcher<uint>> LevelIgtWatchers =>
            new Dictionary<Game, MemoryWatcher<uint>>(3)
            {
                { Game.Tr1, (MemoryWatcher<uint>)Watchers?["Tr1LevelIgt"] },
                { Game.Tr2, (MemoryWatcher<uint>)Watchers?["Tr2LevelIgt"] },
                { Game.Tr3, (MemoryWatcher<uint>)Watchers?["Tr3LevelIgt"] },
            }.ToImmutableDictionary();

        /// <summary>Gives the value of the loading screen fade-in and fade-out (like an alpha).</summary>
        internal static ImmutableDictionary<Game, MemoryWatcher<uint>> LoadFadeWatchers =>
            new Dictionary<Game, MemoryWatcher<uint>>(3)
            {
                { Game.Tr1, (MemoryWatcher<uint>)Watchers?["Tr1LoadFade"] },
                { Game.Tr2, (MemoryWatcher<uint>)Watchers?["Tr2LoadFade"] },
                { Game.Tr3, (MemoryWatcher<uint>)Watchers?["Tr3LoadFade"] },
            }.ToImmutableDictionary();

        /// <summary>Tells if the game is currently in the title screen.</summary>
        internal static ImmutableDictionary<Game, MemoryWatcher<bool>> TitleLoadedWatchers =>
            new Dictionary<Game, MemoryWatcher<bool>>(3)
            {
                { Game.Tr1, (MemoryWatcher<bool>)Watchers?["Tr1TitleLoaded"] },
                { Game.Tr2, (MemoryWatcher<bool>)Watchers?["Tr2TitleLoaded"] },
                { Game.Tr3, (MemoryWatcher<bool>)Watchers?["Tr3TitleLoaded"] },
            }.ToImmutableDictionary();

        internal static MemoryWatcher<uint> Tr1LevelCutscene => (MemoryWatcher<uint>)Watchers?["Tr1LevelCutscene"];

        #endregion

        /// <summary>Sets addresses based on <paramref name="version" />.</summary>
        /// <param name="version">Version to base addresses on; the uint will be converted to <see cref="GameVersion" />.</param>
        internal static void InitializeMemoryWatchers(GameVersion version)
        {
            Watchers.Clear();

            switch (version)
            {
                case GameVersion.PublicV10:
                    // Base game EXE (tomb123.exe)
                    Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0x1A5B78)) { Name = "ActiveGame" });
                    // One-offs from DLLs
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xCFA54)) { Name = "Tr1LevelCutscene" });
                    // Common items for all game's DLLs
                    AddCommonDllWatchers(GameVersion.PublicV10);
                    break;

                case GameVersion.PublicV101:
                    // Base game EXE (tomb123.exe)
                    Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0xD6B68)) { Name = "ActiveGame" });
                    // One-offs from DLLs
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xD4A54)) { Name = "Tr1LevelCutscene" });
                    // Common items for all game's DLLs
                    AddCommonDllWatchers(GameVersion.PublicV101);
                    break;

                case GameVersion.PublicV102:
                    // Base game EXE (tomb123.exe)
                    Watchers.Add(new MemoryWatcher<int>(new DeepPointer(0xDFB68)) { Name = "ActiveGame" });
                    // One-offs from DLLs
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(GameModules[Game.Tr1], 0xD8A54)) { Name = "Tr1LevelCutscene" });
                    // Common items for all game's DLLs
                    AddCommonDllWatchers(GameVersion.PublicV102);
                    break;

                case GameVersion.None:
                case GameVersion.Unknown:
                case GameVersion.EgsDebug:
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }

            PreLoadWatchers();
        }

        /// <summary>Adds MemoryWatchers which are common across all games.</summary>
        /// <param name="version">Game version</param>
        private static void AddCommonDllWatchers(GameVersion version)
        {
            foreach (var game in BaseGames)
            {
                string moduleName = GameModules[game];
                var addresses = GameVersionAddresses[version][game];

                int bonusFlagOffset = addresses.BonusFlag;
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, bonusFlagOffset)) { Name = $"{game}BonusFlag" });

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

                int titleLoadedOffset = addresses.TitleLoaded;
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(moduleName, titleLoadedOffset)) { Name = $"{game}TitleLoaded" });
            }
        }

        /// <summary>
        ///     This method should be called when initializing MemoryWatchers to ensure that they do not have
        ///     default / zeroed values on initialization, which complicates or ruins autosplitter logic.
        /// </summary>
        private static void PreLoadWatchers()
        {
            Watchers.UpdateAll(_gameProcess); // Loads Current values.
            Watchers.UpdateAll(_gameProcess); // Moves Current to Old and loads new Current values.
        }
    }
}