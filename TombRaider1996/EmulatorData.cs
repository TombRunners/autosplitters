using LiveSplit.ComponentUtil;
using System;

namespace TR1
{
    // TODO clean up serial if it's not needed
    internal class EmulatorData
    {
        /// <summary>
        /// Counter initialized to zero at ePSXe's launch.
        /// Starts counting up when the game is loaded into the emulator and the game is running.
        /// The counter stops when you press Esc to pause the game.
        /// Gets set back to zero when ePSXe is reset.
        /// </summary>
        public MemoryWatcher<uint> counter { get; }

        /// <summary>
        /// Serial of the running game.
        /// For all TR1 version serials see: http://redump.org/discs/system/psx/letter/t/
        /// </summary>
        // public StringWatcher serial { get; }

        /// <summary>
        /// Monitors a region of the memory which contains the names of the files contained in the root directory of the currently running game.
        /// Used to tell apart game versions.
        /// </summary>
        public StringWatcher rootDirectoryContents { get; }

        public EmulatorData(ProcessVersion version)
        {
            switch (version)
            {
                case ProcessVersion.ePSXe180:
                    counter = new MemoryWatcher<uint>(new DeepPointer(0xE5960));
                    // serial = new StringWatcher(new DeepPointer(0x65E757‬), 11);
                    rootDirectoryContents = new StringWatcher(new DeepPointer(0xE5C41), 760);
                    break;
                case ProcessVersion.ePSXe190:
                    counter = new MemoryWatcher<uint>(new DeepPointer(0xEA0E4));
                    rootDirectoryContents = new StringWatcher(new DeepPointer(0xEA409), 760);
                    break;
                case ProcessVersion.ePSXe1925:
                    counter = new MemoryWatcher<uint>(new DeepPointer(0x11D184));
                    rootDirectoryContents = new StringWatcher(new DeepPointer(0x1206C1), 760);
                    break;
                case ProcessVersion.ePSXe200:
                    counter = new MemoryWatcher<uint>(new DeepPointer(0x3173A0));
                    rootDirectoryContents = new StringWatcher(new DeepPointer(0x30E661), 760);
                    break;
            }
        }
    }
}
