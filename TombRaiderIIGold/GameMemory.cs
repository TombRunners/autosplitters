using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using LiveSplit.ComponentUtil;

namespace TR2Gold
{       
    /// <summary>
    ///     The supported game versions.
    /// </summary>
    internal enum GameVersion
    {
        StellaCracked
    }

    /// <summary>
    ///     The memory sizes of supported game versions (in bytes).
    /// </summary>
    internal enum ExpectedSize
    {
        StellaCracked = 1695744
    }

    /// <summary>
    ///     The game's watched memory addresses.
    /// </summary>
    internal class GameData : MemoryWatcherList
    {
        public const int FirstLevelTimeAddress = 0x521A84;  // Valid for all supported game versions.

        /// <summary>
        ///     Indicates if the game is on the title screen (main menu).
        /// </summary>
        /// <remarks>
        ///     Goes back to 0 during demos.
        /// </remarks>
        public MemoryWatcher<bool> TitleScreen { get; }

        /// <summary>
        ///     Indicates if the current <see cref="Level"/> is finished.
        /// </summary>
        /// <remarks>
        ///     1 while an end-level stats screen is active.
        ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
        ///     Before most end-level in-game cutscenes, the value changes from 0 to 1 then back to 0 immediately.
        ///     Otherwise, the value is 0.
        /// </remarks>
        public MemoryWatcher<bool> LevelComplete { get; }

        /// <summary>
        ///     Gives the value of the active level, cutscene, or FMV.
        /// </summary>
        /// <remarks>
        ///     Matches the chronological number of the current level.
        /// </remarks>
        public MemoryWatcher<Level> Level { get; }

        /// <summary>
        ///     Gives the IGT value for the current level.
        /// </summary>
        public MemoryWatcher<uint> LevelTime { get; }

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
        public MemoryWatcher<uint> PickedPassportFunction { get; }

        /// <summary>
        ///     Lara's current HP.
        /// </summary>
        /// <remarks>
        ///     Max HP is 1000. When it hits 0, Lara dies.
        /// </remarks>
        public MemoryWatcher<short> Health { get; }

        /// <summary>
        ///     Initializes <see cref="GameData"/> based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version"></param>
        public GameData(GameVersion version)
        {
            if (version == GameVersion.StellaCracked)
            {
                TitleScreen = new MemoryWatcher<bool>(new DeepPointer(0x11EE00));
                LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0xDCF28));
                Level = new MemoryWatcher<Level>(new DeepPointer(0xDCF24));
                LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x121E60));
                PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0xDA9A0));
                Health = new MemoryWatcher<short>(new DeepPointer(0xDA948));
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    ///     Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.
    /// </summary>
    internal class GameMemory
    {
        public Process Game;
        public GameData Data;
        private GameVersion Version;

        public delegate void GameFoundDelegate(GameVersion? version);
        public GameFoundDelegate OnGameFound;

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
                if (Game == null || Game.HasExited)
                {
                    if (!SetGameProcessAndVersion())
                        return false;

                    Data = new GameData(Version);
                    OnGameFound.Invoke(Version);
                    /* Crashes LiveSplit so temporarily disabled
                    Game.EnableRaisingEvents = true;
                    Game.Exited += (s, e) => OnGameFound.Invoke(null);
                    */
                    return true;
                }

                // Due to issues with UpdateAll and AutoSplitComponent, these are done individually.
                Data.TitleScreen.Update(Game);
                Data.LevelComplete.Update(Game);
                Data.Level.Update(Game);
                Data.LevelTime.Update(Game);
                Data.PickedPassportFunction.Update(Game);
                Data.Health.Update(Game);

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
            Process[] t2GoldProcesses = Process.GetProcessesByName("t2gold");

            // Get a process's filename, if found.
            Process process = null;
            if (t2GoldProcesses.Length != 0)
                process = t2GoldProcesses[0];
            string exePath = process?.MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
                return false;

            // Compare the running EXE's hash to known values.
            var versionHashes = new Dictionary<string, GameVersion>
            {
                {"3f262621d07a3c6c6fdd6f654814f988", GameVersion.StellaCracked}
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
                    Game = process;
                    Version = kvp.Value;
                    return true;
                }
            }
            return false;
        }
    }
}
