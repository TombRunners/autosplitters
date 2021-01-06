using System.Diagnostics;

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
    internal class GameMemory
    {
        private Process _game;
        public GameData Data;
        private GameVersion _version;

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
                
                Data = new GameData(_version);
                return true;
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
                _game = atiProcesses[0];
                _version = GameVersion.ATI;
            }
            else if (dosLooksLikeATI)
            {
                _game = dosProcesses[0];
                _version = GameVersion.ATI;
            }
            else if (dosLooksLikeDOS)
            {
                _game = dosProcesses[0];
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
