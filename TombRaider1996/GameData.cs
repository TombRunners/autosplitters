using LiveSplit.ComponentUtil;

namespace TR1
{
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
            }
            else
            {
                LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x243D3C));
                Level = new MemoryWatcher<Level>(new DeepPointer(0xA786B4, 0x243D38));
                LevelTime = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x2513AC));
                PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x245C04));
                DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243BD4));
            }
        }
    }
}
