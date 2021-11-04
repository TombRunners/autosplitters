using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    ///     The memory sizes of supported game versions (in bytes).
    /// </summary>
    internal enum ExpectedSize
    {
        Others = 1691648,
        UKB = 1724416
    }

    /// <summary>
    ///     Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.
    /// </summary>
    internal class GameData
    {
        public const int FirstLevelTimeAddress = 0x51EA24;  // Valid for all supported game versions.
        private static readonly MemoryWatcherList _watchers = new MemoryWatcherList()
        {
            new MemoryWatcher<bool>(new DeepPointer(0x11BDA0)) { Name = "TitleScreen" },
            new MemoryWatcher<bool>(new DeepPointer(0xD9EC4)) { Name = "LevelComplete" },
            new MemoryWatcher<Level>(new DeepPointer(0xD9EC0)) { Name = "Level" },
            new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" },
            new MemoryWatcher<uint>(new DeepPointer(0xD7980)) { Name = "PickedPassportFunction" },
            new MemoryWatcher<short>(new DeepPointer(0xD7928)) { Name = "Health" }
        };
        private GameVersion _version;
        public Process Game;        

        public delegate void GameFoundDelegate(GameVersion? version);
        public GameFoundDelegate OnGameFound;

        /// <summary>
        ///     Indicates if the game is on the title screen (main menu).
        /// </summary>
        /// <remarks>
        ///     Goes back to 0 during demos.
        /// </remarks>
        public static MemoryWatcher<bool> TitleScreen => (MemoryWatcher<bool>)_watchers["TitleScreen"];

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
        public static MemoryWatcher<bool> LevelComplete => (MemoryWatcher<bool>)_watchers["LevelComplete"];

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
        public static MemoryWatcher<Level> Level => (MemoryWatcher<Level>)_watchers["Level"];

        /// <summary>
        ///     Gives the IGT value for the current level.
        /// </summary>
        public static MemoryWatcher<uint> LevelTime => (MemoryWatcher<uint>)_watchers["LevelTime"];

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
        public static MemoryWatcher<uint> PickedPassportFunction => (MemoryWatcher<uint>)_watchers["PickedPassportFunction"];

        /// <summary>
        ///     Lara's current HP.
        /// </summary>
        /// <remarks>
        ///     Max HP is 1000. When it hits 0, Lara dies.
        /// </remarks>
        public static MemoryWatcher<short> Health => (MemoryWatcher<short>)_watchers["Health"];

        /// <summary>
        ///     Sets <see cref="Addresses"/> addresses based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version"></param>
        public static void SetAddresses(GameVersion version)
        {
            if (version == GameVersion.UKB)
            {
                _watchers.Clear();
                _watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11BDA0)) { Name = "TitleScreen" });
                _watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xD9EC4)) { Name = "LevelComplete" });
                _watchers.Add(new MemoryWatcher<Level>(new DeepPointer(0xD9EC0)) { Name = "Level" });
                _watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" });
                _watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD7980)) { Name = "PickedPassportFunction" });
                _watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xD7928)) { Name = "Health" });
            }
            else
            {
                _watchers.Clear();
                _watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11BD90)) { Name = "TitleScreen" });
                _watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xD9EB4)) { Name = "LevelComplete" });
                _watchers.Add(new MemoryWatcher<Level>(new DeepPointer(0xD9EB0)) { Name = "Level" });
                _watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" });
                _watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD7970)) { Name = "PickedPassportFunction" });
                _watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xD7918)) { Name = "Health" });
            }
        }

        /// <summary>
        ///     Updates <see cref="Addresses"/> and its addresses' values.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if game data was able to be updated, <see langword="false"/> otherwise
        /// </returns>
        public bool Update()
        {
            try
            {
                if (Game == null || Game.HasExited)
                {
                    if (!SetGameProcessAndVersion())
                        return false;

                    SetAddresses(_version);
                    OnGameFound.Invoke(_version);
>>>>>>> Made GameData classes static and encapsulate a MemoryWatcherList; renamed GameMemory to GameDataManager.:TombRaiderII/GameDataManager.cs
                    Game.EnableRaisingEvents = true;
                    Game.Exited += (s, e) => OnGameFound.Invoke(null);
                    */
                    return true;
                }

                _watchers.UpdateAll(Game);

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
            Process[] tomb2Processes = Process.GetProcessesByName("tomb2");  // Standard name
            Process[] tr2Processes = Process.GetProcessesByName("tr2");      // Some users rename the EXE to fix installation issues

            // Get a process's filename, if found.
            Process process = null;
            if (tomb2Processes?.Length != 0)
                process = tomb2Processes[0];
            else if (tr2Processes?.Length != 0)
                process = tr2Processes[0];
            string exePath = process?.MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
                return false;

            // Compare the running EXE's hash to known values.
            var versionHashes = new Dictionary<string, GameVersion>
            {
                {"964f0c4e08ff44a905e8fc9a78f605dc", GameVersion.MP},
                {"793c67c79a50984d9bd17ad391f03c57", GameVersion.EPC},
                {"39cab6b4ae3c761b67ae308a0ab22e44", GameVersion.P1},
                {"12d56521ce038b55efba97463357a3d7", GameVersion.UKB}
            };
            string md5Hash = "";
            using (var md5 = MD5.Create())
            {
                using (var stream = File.Open(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var hash = md5.ComputeHash(stream);
                    md5Hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            foreach (KeyValuePair<string, GameVersion> kvp in versionHashes)
            {
                if (kvp.Key == md5Hash)
                {
                    Game = process;
                    _version = kvp.Value;
                    return true;
                }
            }
            return false;
        }
    }
}
