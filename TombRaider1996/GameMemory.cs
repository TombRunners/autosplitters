using System.Diagnostics;

namespace TR1
{
    /// <summary>
    ///     The game version's original target platform.
    /// </summary>
    internal enum Platform
    {
        PC,
        PSX
    }

    /// <summary>
    ///     The supported process versions.
    /// </summary>
    internal enum ProcessVersion
    {
        ATI,
        DOSBox,     // Steam's DOSBox
        ePSXe180,
        ePSXe190,
        ePSXe1925,
        ePSXe200
    }

    /// <summary>
    ///     The memory sizes of supported process versions (in bytes).
    /// </summary>
    internal enum ExpectedSize
    {
        ATI = 3092480,
        DOSBox = 40321024,
        /* TODO
        ePSXe180 = ,
        ePSXe190 = ,
        ePSXe1925 = ,
        ePSXe200 = */
    }

    /// <summary>
    ///     The supported PSX versions of the game.
    /// </summary>
    internal enum PSXGameVersion
    {
        USA_1_0,
        USA_final,
        EU,
        FR,
        GER,
        JP
    }

    /// <summary>
    ///     Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.
    /// </summary>
    internal class GameMemory
    {
        private Process _game;
        public GameData Data;
        private EmulatorData emuData;
        private ProcessVersion _version;
        private Platform _platform;
        private bool PSXGameInitialized = false; 

        /// <summary>
        ///     Updates <see cref="GameData"/> and its addresses' values.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if game data was able to be updated, <see langword="false"/> otherwise
        /// </returns>
        public bool Update()
        {
            if (_game == null || _game.HasExited)
            {
                if (!SetGameProcessAndVersion()) 
                    return false;

                if (_platform == Platform.PC)
                    Data = new GameData(_version);
                else
                {
                    emuData = new EmulatorData(_version);
                    PSXGameInitialized = false;
                }
                return true;
            }

            if (_platform == Platform.PSX)
            {
                // update the emulator's watchers (name of the running executable and perhaps more)   
            }

            if (_platform == Platform.PSX && !PSXGameInitialized)
            {
                PSXGameInitialized = SetPSXGameVersion();
                return PSXGameInitialized;
            }

            // Due to issues with UpdateAll and AutoSplitComponent, these are done individually.
            Data.LevelComplete.Update(_game);
            Data.Level.Update(_game);
            Data.LevelTime.Update(_game);
            Data.PickedPassportFunction.Update(_game);
            Data.DemoTimer.Update(_game);

            return true;
        }

        /// <summary>
        ///     If applicable, finds a <see cref="Process"/> running an expected <see cref="ProcessVersion"/>.
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
                _game = atiProcesses[0];
                _version = ProcessVersion.ATI;
                _platform = Platform.PC;
            }
            else if (dosLooksLikeATI)
            {
                _game = dosProcesses[0];
                _version = ProcessVersion.ATI;
                _platform = Platform.PC;
            }
            else if (dosLooksLikeDOS)
            {
                _game = dosProcesses[0];
                _version = ProcessVersion.DOSBox;
                _platform = Platform.PC;
            }
            // TODO ePSXe versions
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Determines what game version has been loaded into the PSX emulator.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the game version has been successfully set, <see langword="false"/> otherwise.
        /// </returns>
        private bool SetPSXGameVersion()
        {
            // TODO
            return false;
        }
    }
}
