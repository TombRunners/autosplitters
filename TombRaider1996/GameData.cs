using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using LiveSplit.ComponentUtil;

namespace TR1
{
    /// <summary>
    ///     The supported game versions.
    /// </summary>
    internal enum GameVersion
    {
        ATI,
        DOSBox
    }

    /// <summary>
    ///     Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.
    /// </summary>
    internal class GameData
    {
        private const int IGTTicksPerSecond = 30;

        private static readonly MemoryWatcherList Watchers = new MemoryWatcherList
        {
            new MemoryWatcher<bool>(new DeepPointer(0x5A324)) { Name = "TitleScreen" },
            new MemoryWatcher<uint>(new DeepPointer(0x59F4C)) { Name = "DemoTimer" },
            new MemoryWatcher<bool>(new DeepPointer(0x5A014)) { Name = "LevelComplete" },
            new MemoryWatcher<Level>(new DeepPointer(0x53C4C)) { Name = "Level" },
            new MemoryWatcher<uint>(new DeepPointer(0x5BB0A)) { Name = "LevelTime" },
            new MemoryWatcher<uint>(new DeepPointer(0x5A080)) { Name = "PickedPassportFunction" },
            new MemoryWatcher<short>(new DeepPointer(0x5A02C)) { Name = "Health" }
        };
        private GameVersion _version;
        private Process _game;

        public delegate void GameFoundDelegate(GameVersion? version);
        public GameFoundDelegate OnGameFound;

        /// <summary>
        ///     Tells if the game is on the title screen or not.
        /// </summary>    
        /// <remarks>
        ///     True if on the title screen, false otherwise.
        ///     This is a 4 byte integer under the hood (helps DOSBox search).
        /// </remarks>
        public static MemoryWatcher<bool> TitleScreen => (MemoryWatcher<bool>)Watchers["TitleScreen"];

        /// <summary>
        ///     Timer determining whether to start Demo Mode or not.
        /// </summary>
        /// <remarks>
        ///     Value is initialized to zero, and it doesn't change outside the menu.
        ///     In the menu, value is set to zero if the user presses any key.
        ///     If no menu item is activated, and the value gets higher than 480, Demo Mode is started.
        ///     If any menu item is active, the value just increases and Demo Mode is not activated.
        /// </remarks>
        public static MemoryWatcher<uint> DemoTimer => (MemoryWatcher<uint>)Watchers["DemoTimer"];

        /// <summary>
        ///     Indicates if the current <see cref="Level"/> is finished.
        /// </summary>
        /// <remarks>
        ///     1 while an end-level stats screen is active.
        ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
        ///     It also remains 1 during the FMV at the end of Natla's Mines and Atlantis.
        ///     At the end of Tomb of Qualopec and Tihocan, just before the in-game cutscene, the value changes from 0 to 1 then back to 0 immediately.
        ///     Otherwise the value is zero.
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
        /// <remarks>
        ///     For TR1 specifically, this address does not track the cumulative level time; it only tracks the current level time.
        /// </remarks>
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
        ///     Sets <see cref="GameData"/> addresses based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version"><see cref="GameVersion"/> for which addresses should be assigned</param>
        private static void SetAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.ATI:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x5A324)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x59F4C)) { Name = "DemoTimer" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x5A014)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<Level>(new DeepPointer(0x53C4C)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x5BB0A)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x5A080)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x5A02C)) { Name = "Health" });
                    break;
                case GameVersion.DOSBox:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x247B34)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243BD4)) { Name = "DemoTimer" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x243D3C)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<Level>(new DeepPointer(0xA786B4, 0x243D38)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x2513AC)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x245C04)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xA786B4, 0x244448)) { Name = "Health" });
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
                {"e4b95c0479d7256af56b8a9897ed4b13", GameVersion.ATI },
                {"de6b2bf4c04a93f0833b9717386e4a3b", GameVersion.DOSBox }
            };

            Process[] atiProcesses = Process.GetProcessesByName("tombati");
            Process[] dosProcesses = Process.GetProcessesByName("dosbox");
            foreach (Process process in atiProcesses.Concat(dosProcesses))
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

        /// <summary>
        ///     Computes the MD5 hash and formats as a simple, lowercased string.
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
    }
}