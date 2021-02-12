using LiveSplit.ComponentUtil;

namespace TR1
{
    /// <summary>
    /// The emulator's watched memory addresses.
    /// </summary>
    internal class EmulatorData
    {
        /// <summary>
        /// Serial of the running game.
        /// For all TR1 version serials see: http://redump.org/discs/system/psx/letter/t/
        /// Used to differentiate game versions.
        /// </summary>
        public StringWatcher Serial { get; }

        /// <summary>
        /// Monitors a region of the memory which contains the names of the files contained in the root directory of the currently running game.
        /// Used to tell apart versions US_1_0 and US_final as they have the same serial.
        /// </summary>
        public StringWatcher RootDirectoryContents { get; }

        /// <summary>
        /// Initializes <see cref="EmulatorData"/> based on <paramref name="processVersion"/>.
        /// </summary>
        /// <param name="processVersion">One of the - currently only - ePSXe versions.</param>
        public EmulatorData(ProcessVersion processVersion)
        {
            switch (processVersion)
            {
                case ProcessVersion.ePSXe180:
                    Serial = new StringWatcher(new DeepPointer(0x927420), 11);
                    RootDirectoryContents = new StringWatcher(new DeepPointer(0xE5C41), 12);
                    break;
                case ProcessVersion.ePSXe190:
                    Serial = new StringWatcher(new DeepPointer(0x92C000), 11);
                    RootDirectoryContents = new StringWatcher(new DeepPointer(0xEA409), 12);
                    break;
                case ProcessVersion.ePSXe1925:
                    Serial = new StringWatcher(new DeepPointer(0x960480), 11);
                    RootDirectoryContents = new StringWatcher(new DeepPointer(0x1206C1), 12);
                    break;
                case ProcessVersion.ePSXe200:
                    Serial = new StringWatcher(new DeepPointer(0x122DBC0), 11);
                    RootDirectoryContents = new StringWatcher(new DeepPointer(0x30E661), 12);
                    break;
            }
        }
    }
}
