using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using LiveSplit.ComponentUtil;

namespace TR3
{       
    /// <summary>
    ///     The supported game versions.
    /// </summary>
    internal enum GameVersion
    {
        Int,       // From Steam
        JpCracked  // No-CD cracked TR3 from JP Gold bundle release
    }

    /// <summary>
    ///     The game's watched memory addresses.
    /// </summary>
    internal class GameData
    {
        private const int IGTTicksPerSecond = 30;
        private const int FirstLevelTimeAddress = 0x6D2326;  // Valid for all supported game versions.
        private const int LevelSaveStructSize = 0x33;

        private static readonly MemoryWatcherList Watchers = new MemoryWatcherList
        {
            new MemoryWatcher<bool>(new DeepPointer(0x2A1C58)) { Name = "TitleScreen" },
            new MemoryWatcher<bool>(new DeepPointer(0x233F54)) { Name = "LevelComplete" },
            new MemoryWatcher<Level>(new DeepPointer(0xC561C)) { Name = "Level" },
            new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime" },
            new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction" },
            new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health" }
        };

        private Process _game;
        private GameVersion _version;

        public delegate void GameFoundDelegate(GameVersion? version);
        public GameFoundDelegate OnGameFound;

        /// <summary>
        ///     Indicates if the game is on the title screen (main menu).
        /// </summary>
        /// <remarks>
        ///     Goes back to 0 during demos.
        /// </remarks>
        public MemoryWatcher<bool> TitleScreen => (MemoryWatcher<bool>)Watchers["TitleScreen"];

        /// <summary>
        ///     Indicates if the current <see cref="Level"/> is finished.
        /// </summary>
        /// <remarks>
        ///     1 while an end-level stats screen is active.
        ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
        ///     Before most end-level in-game cutscenes, the value changes from 0 to 1 then back to 0 immediately.
        ///     Otherwise, the value is 0.
        /// </remarks>
        public MemoryWatcher<bool> LevelComplete => (MemoryWatcher<bool>)Watchers["LevelComplete"];

        /// <summary>
        ///     Gives the value of the last active level.
        /// </summary>
        /// <remarks>
        ///     For TR3, level order is player-chosen, thus the value may not match the order of play.
        /// </remarks>
        public MemoryWatcher<Level> Level => (MemoryWatcher<Level>)Watchers["Level"];

        /// <summary>
        ///     Gives the IGT value for the current level.
        /// </summary>
        public MemoryWatcher<uint> LevelTime => (MemoryWatcher<uint>)Watchers["LevelTime"];

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
        public MemoryWatcher<uint> PickedPassportFunction => (MemoryWatcher<uint>)Watchers["PickedPassportFunction"];

        /// <summary>
        ///     Lara's current HP.
        /// </summary>
        /// <remarks>
        ///     Max HP is 1000. When it hits 0, Lara dies.
        /// </remarks>
        public MemoryWatcher<short> Health => (MemoryWatcher<short>)Watchers["Health"];

        /// <summary>
        ///     Sets <see cref="GameData"/> addresses based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version"><see cref="GameVersion"/> for which addresses should be assigned</param>
        private static void SetAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.Int:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x2A1C58)) { Name = "TitleScreen"});
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x233F54)) { Name = "LevelComplete"});
                    Watchers.Add(new MemoryWatcher<Level>(new DeepPointer(0xC561C)) { Name = "Level"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction"});
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health"});
                    break;
                case GameVersion.JpCracked:
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x2A1C60)) { Name = "TitleScreen"});
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x233F5C)) { Name = "LevelComplete"});
                    Watchers.Add(new MemoryWatcher<Level>(new DeepPointer(0xC561C)) { Name = "Level"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction"});
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health"});
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }

        /// <summary>
        ///     Converts level time ticks to a double representing time elapsed.
        /// </summary>
        public static double LevelTimeAsDouble(uint ticks) => (double)ticks / IGTTicksPerSecond;

        /// <summary>
        ///     Sums completed levels' times.
        /// </summary>
        /// <returns>The sum of completed levels' times as a double.</returns>
        /// <remarks>
        ///     Because TR3's level order is variable and we must maintain compatibility with Section and NG+ runs,
        ///     we read addresses based on <c>_completedLevels</c>.
        /// </remarks>
        public double SumCompletedLevelTimes(IEnumerable<Level> completedLevels)
        {
            uint finishedLevelsTicks = completedLevels
                .TakeWhile(completedLevel => completedLevel != Level.Current)
                .Select(completedLevel => ((int)completedLevel - 1) * LevelSaveStructSize)
                .Select(levelOffset => (IntPtr)(FirstLevelTimeAddress + levelOffset))
                .Aggregate<IntPtr, uint>(0, (ticks, levelAddress) => ticks + _game.ReadValue<uint>(levelAddress));

            return LevelTimeAsDouble(finishedLevelsTicks);
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
            Process[] gameProcesses = Process.GetProcessesByName("tomb3");

            // Get a process's filename, if found.
            Process process = null;
            if (gameProcesses.Length != 0)
                process = gameProcesses[0];
            string exePath = process?.MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
                return false;

            // Compare the running EXE's hash to known values.
            var versionHashes = new Dictionary<string, GameVersion>
            {
                {"4044dc2c58f02bfea2572e80dd8f2abb", GameVersion.Int},
                {"66404f58bb5dbf30707abfd245692cd2", GameVersion.JpCracked}
            };
            string md5Hash;
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
                    _game = process;
                    _version = kvp.Value;
                    return true;
                }
            }
            return false;
        }
    }
}
