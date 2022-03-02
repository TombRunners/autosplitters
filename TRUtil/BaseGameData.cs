using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiveSplit.ComponentUtil;
using ExtensionMethods;


namespace TRUtil
{
    public abstract class BaseGameData
    {
        /// <summary>Used to calculate <see cref="TimeSpan"/>s from IGT ticks.</summary>
        protected const int IGTTicksPerSecond = 30;

        /// <summary>Strings used when searching for a running game <see cref="Process"/>.</summary>
        protected static readonly List<string> ProcessSearchNames = new List<string>();

        /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
        /// <remarks>Ideally, this will be converted from an <see cref="Enum"/> for clarity.</remarks>
        protected static readonly Dictionary<string, uint> VersionHashes = new Dictionary<string, uint>();

        /// <summary>Contains memory addresses, accessible by named members, used in autosplitting logic.</summary>
        protected static readonly MemoryWatcherList Watchers = new MemoryWatcherList();

        /// <summary>Sometimes directly read, especially for reading level times.</summary>
        protected static Process Game;

        /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
        public static uint Version;

        /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
        /// <param name="version">The version found; ideally, this will be converted from an <see cref="Enum"/> for clarity</param>
        public delegate void GameFoundDelegate(uint version);
        
        /// <summary>Allows subscribers to know when and what game version was found.</summary>
        public GameFoundDelegate OnGameFound;

        /// <summary>A constructor existing primarily to clear static fields expected to be managed only in derived classes' constructors.</summary>
        protected BaseGameData()
        {
            VersionHashes.Clear();
            ProcessSearchNames.Clear();
        }
        
        #region MemoryWatcherList Items

        /// <summary>Gives the value of the active level; for TR1, also matches active cutscene, FMV, or demo.</summary>
        /// <remarks>Usually matches chronological number (TR3 is an exception due to area selection)</remarks>
        public static MemoryWatcher<uint> Level => (MemoryWatcher<uint>)Watchers?["Level"];

        /// <summary>Gives the IGT value for the current level.</summary>
        public static MemoryWatcher<uint> LevelTime => (MemoryWatcher<uint>)Watchers?["LevelTime"];

        /// <summary>Lara's current HP.</summary>
        /// <remarks>Max HP is 1000. When it hits 0, Lara dies.</remarks>
        public static MemoryWatcher<short> Health => (MemoryWatcher<short>)Watchers?["Health"];

        #endregion

        /// <summary>Sets addresses based on <paramref name="version"/>.</summary>
        /// <param name="version">Version to base addresses on; ideally, this will be converted from an <see cref="Enum"/> for clarity</param>
        protected abstract void SetAddresses(uint version);

        /// <summary>Updates <see cref="ClassicGameData"/> implementation and its addresses' values.</summary>
        /// <returns><see langword="true"/> if game data was able to be updated, <see langword="false"/> otherwise</returns>
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

        /// <summary>If applicable, finds a <see cref="Process"/> running an expected version of the game.</summary>
        /// <returns><see langword="true"/> if <see cref="Game"/> and <see cref="Version"/> were meaningfully set, <see langword="false"/> otherwise</returns>
        private bool SetGameProcessAndVersion()
        {
            // Find game Process, if any, and set Version member accordingly.
            Process gameProcess = ProcessSearchNames.SelectMany(Process.GetProcessesByName)
                                                    .First(p => VersionHashes.TryGetValue(p.GetMd5Hash(), out Version));
            if (gameProcess is null)
            {
                // Leave game unset and ensure Version is at its default value.
                Version = 0;
                return false;
            }
            
            // Set Game and do some event management.
            Game = gameProcess;
            Game.EnableRaisingEvents = true;
            Game.Exited += (s, e) => OnGameFound.Invoke(0);
            return true;
        }

        /// <summary>Converts level time ticks to a double representing time elapsed in decimal seconds.</summary>
        public static double LevelTimeAsDouble(uint ticks) => (double)ticks / IGTTicksPerSecond;

        /// <summary>Sums completed levels' times.</summary>
        /// <returns>The sum of completed levels' times</returns>
        public abstract double SumCompletedLevelTimes(IEnumerable<uint> completedLevels, uint currentLevel);
    }
}
