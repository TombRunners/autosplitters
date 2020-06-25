using System;
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

    internal class GameData : MemoryWatcherList
    {
        /// <summary>
        ///     Sometimes indicates if the stats screen is active.
        /// </summary>
        /// <remarks>
        ///     1 while an end-level stats screen is active.
        ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
        ///     It also remains 1 during the FMV at the end of Natla's Mines and Atlantis.
        ///     At the end of Tomb of Qualopec and Tihocan, just before the in-game cutscene, the value changes from 0 to 1 then back to 0 immediately.
        ///     Otherwise the value is zero.
        /// </remarks>
        public MemoryWatcher<bool> StatsScreenIsActive { get; }

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
        public MemoryWatcher<uint> Level { get; }        
        
        /// <summary>
        ///     Gives the IGT value for the current level.
        /// </summary>
        /// <remarks>
        ///     For TR1 specifically, this address does not track the cumulative level time; it only tracks the current level time.
        /// </remarks>
        public MemoryWatcher<uint> LevelTime { get; }

        /// <summary>
        ///     Shows the index value of the chosen passport page.
        /// </summary>
        /// <remarks>
        ///     0 if the first page was picked.
        ///     Changes to 1 when choosing the second page (<c>New Game</c> OR <c>Save Game</c>).
        ///     If the game is saved, the value is 1 until the passport is reopened.
        ///     The value is also 1 during the first FMV if you pick New Game from Lara's Home.
        ///     Changes to 2 when using the third page (<c>Exit To Title</c> or <c>Exit Game</c>).
        ///     Anywhere else the value is 0.
        /// </remarks>
        public MemoryWatcher<uint> PickedPassportPage { get; }

        public GameData(GameVersion version)
        {
            if (version == GameVersion.ATI)
            {
                StatsScreenIsActive = new MemoryWatcher<bool>(new DeepPointer(0x5A014));
                Level = new MemoryWatcher<uint>(new DeepPointer(0x53C4C));
                LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x5BB0A));
                PickedPassportPage = new MemoryWatcher<uint>(new DeepPointer(0x5A080));
            }
            else
            {
                StatsScreenIsActive = new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x243D3C));
                Level = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243D38));
                LevelTime = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x2513AC));
                PickedPassportPage = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x245C04));
            }
        }
    }

    internal class GameMemory
    {
        public Process Game;
        public GameData Data;
        private GameVersion _version;

        public bool Update()
        {
            if (Game == null || Game.HasExited)
            {
                if (!SetGameProcessAndVersion())
                    return false;

                if (Data == null)
                    Data = new GameData(_version);
            }

            // Due to issues with UpdateAll and AutoSplitComponent, these are done individually.
            Data.StatsScreenIsActive.Update(Game);
            Data.CutsceneFlag.Update(Game);
            Data.Level.Update(Game);
            Data.LevelTime.Update(Game);
            Data.PickedPassportPage.Update(Game);

            return true;
        }

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
            // Note: This doesn't deal with DLLs, so using MainModule is perfectly fine and cleans up Event Viewer a bit.

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
                return false;

            return true;
        }
    }
}
