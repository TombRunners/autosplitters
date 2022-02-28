using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LiveSplit.ComponentUtil;
using ExtensionMethods;


namespace TRUtil
{
    public abstract class ClassicGameData
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

        /// <summary>Used to locate the first in-memory saved level time.</summary>
        protected uint FirstLevelTimeAddress;

        /// <summary>The memory struct size of save game info; used to find subsequent level time addresses.</summary>
        protected static uint LevelSaveStructSize;

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
        protected ClassicGameData()
        {
            VersionHashes.Clear();
            ProcessSearchNames.Clear();
        }
        
        #region MemoryWatcherList Items
        
        /// <summary>Indicates if the game is on the title screen (main menu).</summary>
        /// <remarks>Goes back to 0 during demos, if applicable to the game.</remarks>
        public static MemoryWatcher<bool> TitleScreen => (MemoryWatcher<bool>)Watchers?["TitleScreen"];

        /// <summary>Indicates if the current level is finished.</summary>
        /// <remarks>
        ///     1 while an end-level stats screen is active.
        ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
        ///         Applies to Opera House, Diving Area, The Deck, Ice Palace, and Dragon's Lair.
        ///     Before most end-level in-game cutscenes, the value changes from 0 to 1 then back to 0 immediately.
        ///         Applies to Great Wall, Opera House, Diving Area, Temple of Xian, but NOT Home Sweet Home.
        ///     Otherwise, the value is 0.
        /// </remarks>
        public static MemoryWatcher<bool> LevelComplete => (MemoryWatcher<bool>)Watchers?["LevelComplete"];

        /// <summary>Gives the value of the active level; for TR1, also matches active cutscene, FMV, or demo.</summary>
        /// <remarks>Usually matches chronological number (TR3 is an exception)</remarks>
        public static MemoryWatcher<uint> Level => (MemoryWatcher<uint>)Watchers?["Level"];

        /// <summary>Gives the IGT value for the current level.</summary>
        public static MemoryWatcher<uint> LevelTime => (MemoryWatcher<uint>)Watchers?["LevelTime"];

        /// <summary>Indicates the passport function chosen by the user.</summary>
        /// <remarks>
        ///     0 if <c>Load Game</c> was picked.
        ///     Changes to 1 when choosing <c>New Game</c> or <c>Save Game</c>.
        ///     The value stays 1 until the inventory is reopened.
        ///     The value stays 1 through opening FMVs if you pick <c>New Game</c> from Lara's Home.
        ///     The value is always 2 when using the <c>Exit To Title</c> or <c>Exit Game</c> pages.
        ///     Elsewhere, the value is 0.
        /// </remarks>
        public static MemoryWatcher<uint> PickedPassportFunction => (MemoryWatcher<uint>)Watchers?["PickedPassportFunction"];

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
        public virtual double SumCompletedLevelTimes(IEnumerable<uint> completedLevels, uint currentLevel)
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
