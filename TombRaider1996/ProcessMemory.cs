using System.Diagnostics;
using System;

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
        public GameData GameData;
        private EmulatorData _emulatorData;
        private ProcessVersion _processVersion;
        private Platform _platform;
        private PSXGameVersion? _psxGameVersion = null;
        private bool _psxGameInitialized = false; 

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
                if (_process == null || _process.HasExited)
                {
                    if (!SetProcessAndPlatformAndVersion())
                        return false;

                    if (_platform == Platform.PC)
                        GameData = new GameData(_processVersion, null);
                    else
                    {
                        _emulatorData = new EmulatorData(_processVersion);
                        _psxGameInitialized = false;
                    }
                }

                if (_platform == Platform.PSX)
                {
                    _emulatorData.RootDirectoryContents.Update(_process);
                    _emulatorData.Serial.Update(_process);

                    if (_emulatorData.Serial.Changed)
                        _psxGameInitialized = false;

                    if (!_psxGameInitialized)
                    {
                        _psxGameInitialized = SetPSXGameVersion();
                        if (_psxGameInitialized)
                            GameData = new GameData(_processVersion, _psxGameVersion);
                        else
                            return false;
                    }
                }

                // Due to issues with UpdateAll and AutoSplitComponent, these are done individually.
                GameData.LevelComplete.Update(_process);
                GameData.Level.Update(_process);
                GameData.LevelTime.Update(_process);
                GameData.PickedPassportFunction.Update(_process);
                GameData.DemoTimer.Update(_process);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
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

            bool ePSXe180Running = ePSXeProcesses.Length != 0 && ePSXeProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ePSXe180;
            bool ePSXe190Running = ePSXeProcesses.Length != 0 && ePSXeProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ePSXe190;
            bool ePSXe1925Running = ePSXeProcesses.Length != 0 && ePSXeProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ePSXe1925;
            bool ePSXe200Running = ePSXeProcesses.Length != 0 && ePSXeProcesses[0]?.MainModule?.ModuleMemorySize == (int) ExpectedSize.ePSXe200;

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
            if (_emulatorData.Serial.Current.Contains("SLUS_001.52"))
            {
                if (_emulatorData.RootDirectoryContents.Current.Contains("CACKLOGO.RAW"))
                    _psxGameVersion = PSXGameVersion.USA_final;
                else
                    _psxGameVersion = PSXGameVersion.USA_1_0;

                return true;
            }

            if (_emulatorData.Serial.Current.Contains("SLPS_006.17"))
            {
                _psxGameVersion = PSXGameVersion.JP;
                return true;
            }

            if (_emulatorData.Serial.Current.Contains("SLES_000.24"))
            {
                _psxGameVersion = PSXGameVersion.EU;
                return true;
            }

            if (_emulatorData.Serial.Current.Contains("SLES_004.85"))
            {
                _psxGameVersion = PSXGameVersion.FR;
                return true;
            }

            if (_emulatorData.Serial.Current.Contains("SLES_004.86"))
            {
                _psxGameVersion = PSXGameVersion.GER;
                return true;
            }

            return false;
        }
    }
}
