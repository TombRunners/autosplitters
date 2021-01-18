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
        ePSXe180 = 10231808,
        ePSXe190 = 10301440,
        ePSXe1925 = 10518528,
        ePSXe200 = 20287488
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
    ///     Manages the process's watched memory values for <see cref="Autosplitter"/>'s use.
    /// </summary>
    internal class ProcessMemory
    {
        private Process _process;
        public GameData gameData;
        private EmulatorData emuData;
        private ProcessVersion _processVersion;
        private Platform _platform;
        private PSXGameVersion? _PSXGameVersion = null;
        private bool PSXGameInitialized = false; 

        /// <summary>
        ///     Updates <see cref="GameData"/> and its addresses' values.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if game data was able to be updated, <see langword="false"/> otherwise
        /// </returns>
        public bool Update()
        {
            if (_process == null || _process.HasExited)
            {
                if (!SetProcessAndPlatformAndVersion()) 
                    return false;

                if (_platform == Platform.PC)
                    gameData = new GameData(_processVersion, null);
                else
                {
                    emuData = new EmulatorData(_processVersion);
                    PSXGameInitialized = false;
                }
                return true;
            }

            if (_platform == Platform.PSX)
            {
                emuData.counter.Update(_process);

                if (emuData.counter.Old == 0 && emuData.counter.Current != 0)
                {
                    emuData.rootDirectoryContents.Update(_process);
                    emuData.serial.Update(_process);
                    PSXGameInitialized = false;
                }
            }

            if (_platform == Platform.PSX && !PSXGameInitialized)
            {
                PSXGameInitialized = SetPSXGameVersion();
                if (PSXGameInitialized)
                    gameData = new GameData(_processVersion, _PSXGameVersion);
                return PSXGameInitialized;
            }

            // Due to issues with UpdateAll and AutoSplitComponent, these are done individually.
            gameData.LevelComplete.Update(_process);
            gameData.Level.Update(_process);
            gameData.LevelTime.Update(_process);
            gameData.PickedPassportFunction.Update(_process);
            gameData.DemoTimer.Update(_process);

            return true;
        }

        /// <summary>
        ///     If applicable, finds a <see cref="Process"/> running an expected <see cref="ProcessVersion"/>.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if values were set, <see langword="false"/> otherwise
        /// </returns>
        private bool SetProcessAndPlatformAndVersion()
        {
            Process[] atiProcesses = Process.GetProcessesByName("tombati");
            Process[] dosProcesses = Process.GetProcessesByName("dosbox");
            Process[] ePSXeProcesses = Process.GetProcessesByName("ePSXe");

            // The Steam Workshop launcher uses the name "dosbox" and remains as a background process after launching the game.
            bool workshopLauncherAndATIGameAreBothRunning = atiProcesses.Length != 0 && dosProcesses.Length != 0;
            bool atiLooksLikeATI = atiProcesses.Length != 0 && atiProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ATI;
            // Some Workshop guides have the user rename the ATI EXE back to "dosbox" for Steam compatibility.
            bool dosLooksLikeATI = dosProcesses.Length != 0 && dosProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ATI;
            bool dosLooksLikeDOS = dosProcesses.Length != 0 && dosProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.DOSBox;

            bool ePSXe180Running = ePSXeProcesses.Length != 0 && ePSXeProcesses[0].MainModule?.ModuleMemorySize == (int) ExpectedSize.ePSXe180;
            bool ePSXe190Running = ePSXeProcesses.Length != 0 && ePSXeProcesses[0].MainModule?.ModuleMemorySize == (int) ExpectedSize.ePSXe190;
            bool ePSXe1925Running = ePSXeProcesses.Length != 0 && ePSXeProcesses[0].MainModule?.ModuleMemorySize == (int) ExpectedSize.ePSXe1925;
            bool ePSXe200Running = ePSXeProcesses.Length != 0 && ePSXeProcesses[0].MainModule?.ModuleMemorySize == (int) ExpectedSize.ePSXe200;

            if (workshopLauncherAndATIGameAreBothRunning || atiLooksLikeATI)
            {
                _process = atiProcesses[0];
                _processVersion = ProcessVersion.ATI;
                _platform = Platform.PC;
            }
            else if (dosLooksLikeATI)
            {
                _process = dosProcesses[0];
                _processVersion = ProcessVersion.ATI;
                _platform = Platform.PC;
            }
            else if (dosLooksLikeDOS)
            {
                _process = dosProcesses[0];
                _processVersion = ProcessVersion.DOSBox;
                _platform = Platform.PC;
            }
            else if (ePSXe180Running)
            {
                _process = ePSXeProcesses[0];
                _processVersion = ProcessVersion.ePSXe180;
                _platform = Platform.PSX;
            }
            else if (ePSXe190Running)
            {
                _process = ePSXeProcesses[0];
                _processVersion = ProcessVersion.ePSXe190;
                _platform = Platform.PSX;
            }
            else if (ePSXe1925Running)
            {
                _process = ePSXeProcesses[0];
                _processVersion = ProcessVersion.ePSXe1925;
                _platform = Platform.PSX;
            }
            else if (ePSXe200Running)
            {
                _process = ePSXeProcesses[0];
                _processVersion = ProcessVersion.ePSXe200;
                _platform = Platform.PSX;
            }
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
            if (emuData.serial.Current.Contains("SLUS_001.52"))
            {
                if (emuData.rootDirectoryContents.Current.Contains("CACKLOGO.RAW"))
                    _PSXGameVersion = PSXGameVersion.USA_final;
                else
                    _PSXGameVersion = PSXGameVersion.USA_1_0;

                return true;
            }

            if (emuData.serial.Current.Contains("SLPS_006.17"))
            {
                _PSXGameVersion = PSXGameVersion.JP;
                return true;
            }

            if (emuData.serial.Current.Contains("SLES_000.24"))
            {
                _PSXGameVersion = PSXGameVersion.EU;
                return true;
            }

            if (emuData.serial.Current.Contains("SLES_004.85"))
            {
                _PSXGameVersion = PSXGameVersion.FR;
                return true;
            }

            if (emuData.serial.Current.Contains("SLES_004.86"))
            {
                _PSXGameVersion = PSXGameVersion.GER;
                return true;
            }

            return false;
        }
    }
}
