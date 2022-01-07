using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using LiveSplit.ComponentUtil;

namespace TR2
{
    /// <summary>
    ///     The supported game versions.
    /// </summary>
    internal enum GameVersion
    {
        MP,   // Multipatch (Steam/GoG default)
        EPC,  // Eidos Premier Collection
        P1,   // CORE's Patch 1
        UKB   // Eidos UK Box
    }

    /// <summary>
    ///     Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.
    /// </summary>
    internal class GameData
    {
        private const int IGTTicksPerSecond = 30;
        private const int FirstLevelTimeAddress = 0x51EA24;  // Valid for all supported game versions.

        private static readonly MemoryWatcherList Watchers = new MemoryWatcherList
        {
            new MemoryWatcher<bool>(new DeepPointer(0x11BDA0)) { Name = "TitleScreen" },
            new MemoryWatcher<bool>(new DeepPointer(0xD9EC4)) { Name = "LevelComplete" },
            new MemoryWatcher<Level>(new DeepPointer(0xD9EC0)) { Name = "Level" },
            new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" },
            new MemoryWatcher<uint>(new DeepPointer(0xD7980)) { Name = "PickedPassportFunction" },
            new MemoryWatcher<short>(new DeepPointer(0xD7928)) { Name = "Health" }
        };
        private GameVersion _version;
        private Process _game;

        public delegate void GameFoundDelegate(GameVersion? version);
        public GameFoundDelegate OnGameFound;

        /// <summary>
        ///     Indicates if the game is on the title screen (main menu).
        /// </summary>
        /// <remarks>
        ///     Goes back to 0 during demos.
        /// </remarks>
        public static MemoryWatcher<bool> TitleScreen => (MemoryWatcher<bool>)Watchers["TitleScreen"];

        /// <summary>
        ///     Indicates if the current <see cref="Level"/> is finished.
        /// </summary>
        /// <remarks>
        ///     1 while an end-level stats screen is active.
        ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
        ///         Applies to Opera House, Diving Area, The Deck, Ice Palace, and Dragon's Lair.
        ///     Before most end-level in-game cutscenes, the value changes from 0 to 1 then back to 0 immediately.
        ///         Applies to Great Wall, Opera House, Diving Area, Temple of Xian, but NOT Home Sweet Home.
        ///     Otherwise, the value is 0.
        /// </remarks>
        public static MemoryWatcher<bool> LevelComplete => (MemoryWatcher<bool>)Watchers["LevelComplete"];

        /// <summary>
        ///     Gives the value of the active level, cutscene, or FMV.
        /// </summary>
        /// <remarks>
        ///     Matches the chronological number of the current level, but also matches the file number for in-game cutscenes and FMVs.
        ///     0 through 15: Active level
        ///     16: Qualopec cutscene until next level start.
        ///     17: Tihocan cutscene until next level start.
        ///     18: After Natla's Mines stats screen until next level start.
        ///     19: Atlantis cutscene after the FMV until next level start.
        ///     20: Title screen and opening FMV.
        /// </remarks>
        public static MemoryWatcher<Level> Level => (MemoryWatcher<Level>)Watchers["Level"];

        /// <summary>
        ///     Gives the IGT value for the current level.
        /// </summary>
        public static MemoryWatcher<uint> LevelTime => (MemoryWatcher<uint>)Watchers["LevelTime"];

        /// <summary>
        ///     Indicates the passport function chosen by the user.
        /// </summary>
        /// <remarks>
        ///     0 if <c>Load Game</c> was picked.
        ///     Changes to 1 when choosing <c>New Game</c> or <c>Save Game</c>.
        ///     The value stays 1 until the inventory is reopened.
        ///     The value is also 1 during the first FMV if you pick <c>New Game</c> from Lara's Home.
        ///     The value is always 2 when using the <c>Exit To Title</c> or <c>Exit Game</c> pages.
        ///     Anywhere else the value is 0.
        /// </remarks>
        public static MemoryWatcher<uint> PickedPassportFunction => (MemoryWatcher<uint>)Watchers["PickedPassportFunction"];

        /// <summary>
        ///     Lara's current HP.
        /// </summary>
        /// <remarks>
        ///     Max HP is 1000. When it hits 0, Lara dies.
        /// </remarks>
        public static MemoryWatcher<short> Health => (MemoryWatcher<short>)Watchers["Health"];

        /// <summary>
        ///     Converts level time ticks to a double representing time elapsed.
        /// </summary>
        public static double LevelTimeAsDouble(uint ticks) => (double)ticks / IGTTicksPerSecond;
        
        /// <summary>
        ///     Sums completed levels' times.
        /// </summary>
        /// <returns>The sum of completed levels' times</returns>
        public double SumCompletedLevelTimes()
        {
            // Add up the level times stored in the game's memory.
            uint finishedLevelsTicks = 0;
            for (int i = 0; i < (int)Level.Current - 1; i++)
            {
                var levelAddress = (IntPtr)(FirstLevelTimeAddress + i * 0x2c);
                finishedLevelsTicks += _game.ReadValue<uint>(levelAddress);
            }
            return LevelTimeAsDouble(finishedLevelsTicks);
        }

        /// <summary>
        ///     Sets <see cref="GameData"/> addresses based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version"><see cref="GameVersion"/> for which addresses should be assigned</param>
        private static void SetAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.UKB:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11BDA0)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xD9EC4)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<Level>(new DeepPointer(0xD9EC0)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD7980)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xD7928)) { Name = "Health" });
                    break;
                case GameVersion.EPC:
                case GameVersion.MP:
                case GameVersion.P1:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11BD90)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xD9EB4)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<Level>(new DeepPointer(0xD9EB0)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD7970)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xD7918)) { Name = "Health" });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }

        /// <summary>
        ///     Updates <see cref="GameData"/> and its addresses' values.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if game data was able to be updated, <see langword="false"/> otherwise
        /// </returns>
        public bool Update()
        {
            try
            {
                if (_game == null || _game.HasExited)
                {
                    if (!SetGameProcessAndVersion())
                        return false;

                    SetAddresses(_version);
                    OnGameFound.Invoke(_version);
                    _game.EnableRaisingEvents = true;
                    _game.Exited += (s, e) => OnGameFound.Invoke(null);
                    return true;
                }

                Watchers.UpdateAll(_game);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     If applicable, finds a <see cref="Process"/> running an expected <see cref="GameVersion"/>.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if values were set, <see langword="false"/> otherwise
        /// </returns>
        private bool SetGameProcessAndVersion()
        {
            var versionHashes = new Dictionary<string, GameVersion>
            {
                {"964f0c4e08ff44a905e8fc9a78f605dc", GameVersion.MP},
                {"793c67c79a50984d9bd17ad391f03c57", GameVersion.EPC},
                {"39cab6b4ae3c761b67ae308a0ab22e44", GameVersion.P1},
                {"12d56521ce038b55efba97463357a3d7", GameVersion.UKB}
            };

            Process[] tomb2Processes = Process.GetProcessesByName("tomb2");  // Standard name
            Process[] tr2Processes = Process.GetProcessesByName("tr2");      // Some users rename the EXE to fix installation issues
            foreach (Process process in tomb2Processes.Concat(tr2Processes))
            {
                string exePath = process?.MainModule?.FileName;
                if (string.IsNullOrEmpty(exePath))
                    break;

                string md5Hash = GetMd5Hash(exePath);
                if (!versionHashes.TryGetValue(md5Hash, out GameVersion version)) 
                    break;

                _version = version;
                _game = process;
                return true;
            }

            return false;
        }

        private static string GetMd5Hash(string exePath)
        {
            string md5Hash;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.Open(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var hash = md5.ComputeHash(stream);
                    md5Hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

            return md5Hash;
        }
    }
}
