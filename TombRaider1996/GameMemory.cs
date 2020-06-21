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
    ///     The memory sizes of supported game versions.
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
        ///     In some cases, fluctuates between 1 and 0 during FMVs and cutscenes; such cases are documented in the Split logic.
        /// </remarks>
        public MemoryWatcher<bool> StatsScreenIsActive { get; }

        /// <summary>
        ///     Indicates when an in-game cutscene is active.
        /// </summary>
        /// <remarks>
        ///     This can't be watched as a bool to do cases of "random" values that would be falsely read as true.
        ///     For <see cref="GameVersion.DOSBox"/>, the value is <c>1</c> during cutscenes.
        ///     For <see cref="GameVersion.ATI"/>, the value is <c>0</c> during cutscenes.
        /// </remarks>
        public MemoryWatcher<uint> CutsceneFlag { get; }

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
        ///     Tells if a new game is loading or if game is exiting to main menu.
        /// </summary>
        /// <remarks>
        ///     Normally at 0.
        ///     Changes to 1 when using the <c>New Game</c> page OR saving. Remains at 1 until the inventory is opened.
        ///     Changes to 2 when using <c>Exit To Title</c> or <c>Exit Game</c>. Returns to 0 at the title screen or when the game closes.
        /// </remarks>
        public MemoryWatcher<uint> StartGameFlag { get; }

        public GameData(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.ATI:
                    StatsScreenIsActive = new MemoryWatcher<bool>(new DeepPointer(0x5A014));
                    CutsceneFlag = new MemoryWatcher<uint>(new DeepPointer(0x56688));
                    Level = new MemoryWatcher<uint>(new DeepPointer(0x53C4C));
                    LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x45BB0A));
                    StartGameFlag = new MemoryWatcher<uint>(new DeepPointer(0x5A080));
                    break;
                case GameVersion.DOSBox:
                    StatsScreenIsActive = new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x243D3C));
                    CutsceneFlag = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x1623A4));
                    Level = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243D38));
                    LevelTime = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x2513AC));
                    StartGameFlag = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x245C04));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, "Bad GameVersion");
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
            Data.StartGameFlag.Update(Game);

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
            {
                return false;
            }

            return true;
        }
    }
}
