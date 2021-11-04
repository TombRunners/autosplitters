<<<<<<< HEAD:TombRaider1996/GameMemory.cs
﻿using System;
using System.Diagnostics;
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
    ///     The memory sizes of supported game versions (in bytes).
    /// </summary>
    internal enum ExpectedSize
    {
        ATI = 3092480,
        DOSBox = 40321024
    }

    /// <summary>
    ///     The game's watched memory addresses.
    /// </summary>
    internal class GameData : MemoryWatcherList
    {
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
        public MemoryWatcher<bool> LevelComplete { get; }

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
        public MemoryWatcher<Level> Level { get; }        
        
        /// <summary>
        ///     Gives the IGT value for the current level.
        /// </summary>
        /// <remarks>
        ///     For TR1 specifically, this address does not track the cumulative level time; it only tracks the current level time.
        /// </remarks>
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
        ///     Timer determining whether to start Demo Mode or not.
        /// </summary>
        /// <remarks>
        ///     Value is initialized to zero, and it doesn't change outside the menu.
        ///     In the menu, value is set to zero if the user presses any key.
        ///     If no menu item is activated, and the value gets higher than 480, Demo Mode is started.
        ///     If any menu item is active, the value just increases and Demo Mode is not activated.
        /// </remarks>
        public MemoryWatcher<uint> DemoTimer { get; }

        /// <summary>
        ///     Lara's current HP.
        /// </summary>
        /// <remarks>
        ///     Max HP is 1000. When it hits 0, Lara dies.
        /// </remarks>
        public MemoryWatcher<short> Health { get; }

        /// <summary>
        ///     Tells if the game is on the title screen or not.
        /// </summary>    
        /// <remarks>
        ///     True if on the title screen, false otherwise.
        ///     This is a 4 byte integer under the hood (helps DOSBox search).
        /// </remarks>
        public MemoryWatcher<bool> TitleScreen { get; }

        /// <summary>
        ///     Initializes <see cref="GameData"/> based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version"></param>
        public GameData(GameVersion version)
        {
            if (version == GameVersion.ATI)
            {
                LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x5A014));
                Level = new MemoryWatcher<Level>(new DeepPointer(0x53C4C));
                LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x5BB0A));
                PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x5A080));
                DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x59F4C));
                Health = new MemoryWatcher<short>(new DeepPointer(0x5A02C));
                TitleScreen = new MemoryWatcher<bool>(new DeepPointer(0x5A324));
            }
            else
            {
                LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x243D3C));
                Level = new MemoryWatcher<Level>(new DeepPointer(0xA786B4, 0x243D38));
                LevelTime = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x2513AC));
                PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x245C04));
                DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243BD4));
                Health = new MemoryWatcher<short>(new DeepPointer(0xA786B4, 0x244448));
                TitleScreen = new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x247B34));
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
        private GameVersion _version;

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

                    Data = new GameData(_version);
                    OnGameFound.Invoke(_version);
                    /* crashes LiveSplit so temporarily disabled
                    Game.EnableRaisingEvents = true;
                    Game.Exited += (s, e) => OnGameFound.Invoke(null);
                    */
                    return true;
                }

                // Due to issues with UpdateAll and AutoSplitComponent, these are done individually.
                Data.LevelComplete.Update(Game);
                Data.Level.Update(Game);
                Data.LevelTime.Update(Game);
                Data.PickedPassportFunction.Update(Game);
                Data.DemoTimer.Update(Game);
                Data.Health.Update(Game);
                Data.TitleScreen.Update(Game);

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
            Process[] atiProcesses = Process.GetProcessesByName("tombati");
            Process[] dosProcesses = Process.GetProcessesByName("dosbox");

            // The Steam Workshop launcher uses the name "dosbox" and remains as a background process after launching the game.
            bool workshopLauncherAndATIGameAreBothRunning = atiProcesses.Length != 0 && dosProcesses.Length != 0;
            bool atiLooksLikeATI = atiProcesses.Length != 0 && atiProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ATI;
            // Some Workshop guides have the user rename the ATI EXE back to "dosbox" for Steam compatibility.
            bool dosLooksLikeATI = dosProcesses.Length != 0 && dosProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ATI;
            bool dosLooksLikeDOS = dosProcesses.Length != 0 && dosProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.DOSBox;

            if (workshopLauncherAndATIGameAreBothRunning || atiLooksLikeATI)
            {
                Game = atiProcesses[0];
                _version = GameVersion.ATI;
            }
            else if (dosLooksLikeATI)
            {
                Game = dosProcesses[0];
                _version = GameVersion.ATI;
            }
            else if (dosLooksLikeDOS)
            {
                Game = dosProcesses[0];
                _version = GameVersion.DOSBox;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
=======
﻿using System;
using System.Diagnostics;
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
    ///     The memory sizes of supported game versions (in bytes).
    /// </summary>
    internal enum ExpectedSize
    {
        ATI = 3092480,
        DOSBox = 40321024
    }

    /// <summary>
    ///     Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.
    /// </summary>
    internal class GameData
    {
        private static readonly MemoryWatcherList _addresses = new MemoryWatcherList();
        private GameVersion _version;
        public Process Game;

        public delegate void GameFoundDelegate(GameVersion? version);
        public GameFoundDelegate OnGameFound;

        /// <summary>
        ///     Tells if the game is on the title screen or not.
        /// </summary>    
        /// <remarks>
        ///     True if on the title screen, false otherwise.
        ///     This is a 4 byte integer under the hood (helps DOSBox search).
        /// </remarks>
        public static MemoryWatcher<bool> TitleScreen => (MemoryWatcher<bool>)_addresses["TitleScreen"];

        /// <summary>
        ///     Timer determining whether to start Demo Mode or not.
        /// </summary>
        /// <remarks>
        ///     Value is initialized to zero, and it doesn't change outside the menu.
        ///     In the menu, value is set to zero if the user presses any key.
        ///     If no menu item is activated, and the value gets higher than 480, Demo Mode is started.
        ///     If any menu item is active, the value just increases and Demo Mode is not activated.
        /// </remarks>
        public static MemoryWatcher<uint> DemoTimer => (MemoryWatcher<uint>)_addresses["DemoTimer"];

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
        public static MemoryWatcher<bool> LevelComplete => (MemoryWatcher<bool>)_addresses["LevelComplete"];

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
        public static MemoryWatcher<Level> Level => (MemoryWatcher<Level>)_addresses["Level"];

        /// <summary>
        ///     Gives the IGT value for the current level.
        /// </summary>
        /// <remarks>
        ///     For TR1 specifically, this address does not track the cumulative level time; it only tracks the current level time.
        /// </remarks>
        public static MemoryWatcher<uint> LevelTime => (MemoryWatcher<uint>)_addresses["LevelTime"];

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
        public static MemoryWatcher<uint> PickedPassportFunction => (MemoryWatcher<uint>)_addresses["PickedPassportFunction"];

        /// <summary>
        ///     Lara's current HP.
        /// </summary>
        /// <remarks>
        ///     Max HP is 1000. When it hits 0, Lara dies.
        /// </remarks>
        public static MemoryWatcher<short> Health => (MemoryWatcher<short>)_addresses["Health"];

        /// <summary>
        ///     Sets <see cref="GameData"/> addresses based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version"></param>
        public static void SetAddresses(GameVersion version)
        {
            if (version == GameVersion.ATI)
            {
                _addresses.Clear();
                _addresses.Add(new MemoryWatcher<bool>(new DeepPointer(0x5A324)) { Name = "TitleScreen" });
                _addresses.Add(new MemoryWatcher<uint>(new DeepPointer(0x59F4C)) { Name = "DemoTimer" });
                _addresses.Add(new MemoryWatcher<bool>(new DeepPointer(0x5A014)) { Name = "LevelComplete" });
                _addresses.Add(new MemoryWatcher<Level>(new DeepPointer(0x53C4C)) { Name = "Level" });
                _addresses.Add(new MemoryWatcher<uint>(new DeepPointer(0x5BB0A)) { Name = "LevelTime" });
                _addresses.Add(new MemoryWatcher<uint>(new DeepPointer(0x5A080)) { Name = "PickedPassportFunction" });
                _addresses.Add(new MemoryWatcher<short>(new DeepPointer(0x5A02C)) { Name = "Health" });
            }
            else
            {
                _addresses.Clear();
                _addresses.Add(new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x247B34)) { Name = "TitleScreen" });
                _addresses.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243BD4)) { Name = "DemoTimer" });
                _addresses.Add(new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x243D3C)) { Name = "LevelComplete" });
                _addresses.Add(new MemoryWatcher<Level>(new DeepPointer(0xA786B4, 0x243D38)) { Name = "Level" });
                _addresses.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x2513AC)) { Name = "LevelTime" });
                _addresses.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x245C04)) { Name = "PickedPassportFunction" });
                _addresses.Add(new MemoryWatcher<short>(new DeepPointer(0xA786B4, 0x244448)) { Name = "Health" });
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
                if (Game == null || Game.HasExited)
                {
                    if (!SetGameProcessAndVersion())
                        return false;

                    SetAddresses(_version);
                    OnGameFound.Invoke(_version);
                    Game.EnableRaisingEvents = true;
                    Game.Exited += (s, e) => OnGameFound.Invoke(null);
                    return true;
                }

                _addresses.UpdateAll(Game);

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
            Process[] atiProcesses = Process.GetProcessesByName("tombati");
            Process[] dosProcesses = Process.GetProcessesByName("dosbox");

            // The Steam Workshop launcher uses the name "dosbox" and remains as a background process after launching the game.
            bool workshopLauncherAndATIGameAreBothRunning = atiProcesses.Length != 0 && dosProcesses.Length != 0;
            bool atiLooksLikeATI = atiProcesses.Length != 0 && atiProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ATI;
            // Some Workshop guides have the user rename the ATI EXE back to "dosbox" for Steam compatibility.
            bool dosLooksLikeATI = dosProcesses.Length != 0 && dosProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ATI;
            bool dosLooksLikeDOS = dosProcesses.Length != 0 && dosProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.DOSBox;

            if (workshopLauncherAndATIGameAreBothRunning || atiLooksLikeATI)
            {
                Game = atiProcesses[0];
                _version = GameVersion.ATI;
            }
            else if (dosLooksLikeATI)
            {
                Game = dosProcesses[0];
                _version = GameVersion.ATI;
            }
            else if (dosLooksLikeDOS)
            {
                Game = dosProcesses[0];
                _version = GameVersion.DOSBox;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
>>>>>>> Made GameData classes static and encapsulate a MemoryWatcherList; renamed GameMemory to GameDataManager.:TombRaider1996/GameDataManager.cs
