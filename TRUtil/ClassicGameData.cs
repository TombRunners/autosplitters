using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using LiveSplit.ComponentUtil;

namespace TRUtil
{
    public abstract class ClassicGameData
    {
        protected const int IGTTicksPerSecond = 30;

        protected static readonly Dictionary<string, uint> VersionHashes = new Dictionary<string, uint>();
        protected static readonly MemoryWatcherList Watchers = new MemoryWatcherList();
        protected static readonly List<string> ProcessSearchNames = new List<string>();

        protected static int FirstLevelTimeAddress;
        protected static int LevelSaveStructSize;
        protected static uint Version;
        protected static Process Game;

        public delegate void GameFoundDelegate(uint version);
        public GameFoundDelegate OnGameFound;

        protected ClassicGameData()
        {
            // Clear static fields we expect to be managed only in derived classes' constructors.
            VersionHashes.Clear();
            ProcessSearchNames.Clear();
        }
        
        #region MemoryWatcherList Items
        
        /// <summary>
        ///     Indicates if the game is on the title screen (main menu).
        /// </summary>
        /// <remarks>
        ///     Goes back to 0 during demos.
        /// </remarks>
        public static MemoryWatcher<bool> TitleScreen => (MemoryWatcher<bool>)Watchers?["TitleScreen"];

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
        public static MemoryWatcher<bool> LevelComplete => (MemoryWatcher<bool>)Watchers?["LevelComplete"];

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
        public static MemoryWatcher<uint> Level => (MemoryWatcher<uint>)Watchers?["Level"];

        /// <summary>
        ///     Gives the IGT value for the current level.
        /// </summary>
        public static MemoryWatcher<uint> LevelTime => (MemoryWatcher<uint>)Watchers?["LevelTime"];

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
        public static MemoryWatcher<uint> PickedPassportFunction => (MemoryWatcher<uint>)Watchers?["PickedPassportFunction"];

        /// <summary>
        ///     Lara's current HP.
        /// </summary>
        /// <remarks>
        ///     Max HP is 1000. When it hits 0, Lara dies.
        /// </remarks>
        public static MemoryWatcher<short> Health => (MemoryWatcher<short>)Watchers?["Health"];

        #endregion

        /// <summary>
        ///     Sets addresses based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version">version for which addresses should be assigned</param>
        protected abstract void SetAddresses(uint version);

        /// <summary>
        ///     Updates <see cref="ClassicGameData"/> implementation and its addresses' values.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if game data was able to be updated, <see langword="false"/> otherwise
        /// </returns>
        public bool Update()
        {
            try
            {
                if (Game is null || Game.HasExited)
                {
                    if (!SetGameProcessAndVersion())
                        return false;
                    
                    SetAddresses(Version);
                    OnGameFound.Invoke(Version);
                    return true;
                }

                Watchers.UpdateAll(Game);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     If applicable, finds a <see cref="Process"/> running an expected version of the game.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if <see cref="Game"/> and <see cref="Version"/> were set to non-default values, <see langword="false"/> otherwise
        /// </returns>
        private bool SetGameProcessAndVersion()
        {
            var processes = new List<Process>();
            foreach (Process[] namedProcesses in ProcessSearchNames.Select(Process.GetProcessesByName))
            {
                processes.AddRange(namedProcesses);
            }

            foreach (Process process in processes)
            {
                string exePath = process?.MainModule?.FileName;
                if (string.IsNullOrEmpty(exePath))
                    break;

                string md5Hash = GetMd5Hash(exePath);
                if (!VersionHashes.TryGetValue(md5Hash, out Version)) // Sets Version
                    break;
                
                // Set Game and do some event management.
                Game = process;
                Game.EnableRaisingEvents = true;
                Game.Exited += (s, e) => OnGameFound.Invoke(0);
                return true;
            }
            
            Version = 0;
            return false;
        }

        /// <summary>
        ///     Computes the MD5 hash formatted as a simple, lowercased string.
        /// </summary>
        /// <param name="exePath">The file to hash</param>
        /// <returns>Lowercased, invariant string representing the MD5 <paramref name="exePath"/></returns>
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

        /// <summary>
        ///     Converts level time ticks to a double representing time elapsed.
        /// </summary>
        public static double LevelTimeAsDouble(uint ticks) => (double)ticks / IGTTicksPerSecond;

        /// <summary>
        ///     Sums completed levels' times.
        /// </summary>
        /// <returns>The sum of completed levels' times</returns>
        public static double SumCompletedLevelTimes(IEnumerable<uint> completedLevels, uint currentLevel)
        {
            uint finishedLevelsTicks = completedLevels
                .TakeWhile(completedLevel => completedLevel != currentLevel)
                .Select(completedLevel => (completedLevel - 1) * LevelSaveStructSize)
                .Select(levelOffset => (IntPtr)(FirstLevelTimeAddress + levelOffset))
                .Aggregate<IntPtr, uint>(0, (ticks, levelAddress) => ticks + Game.ReadValue<uint>(levelAddress));

            return LevelTimeAsDouble(finishedLevelsTicks);
        }
    }
}
