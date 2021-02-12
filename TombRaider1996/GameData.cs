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
        ///     16: Qualopec cutscene until the next level starts.
        ///     17: Tihocan cutscene until the next level starts.
        ///     18: After Natla's Mines stats screen until the next level starts.
        ///     19: Atlantis cutscene after the FMV until the next level starts.
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
        ///     The value stays 1 until the inventory is reopened.
        ///     The value is also 1 during the first FMV if you pick <c>New Game</c> from Lara's Home.
        ///     The value is always 2 when using the <c>Exit To Title</c> or <c>Exit Game</c> pages.
        ///     Anywhere else the value is 0.
        ///     PC:
        ///     Changes to 1 when choosing <c>New Game</c> or <c>Save Game</c>.
        ///     PSX:
        ///     Changes to 1 when choosing <c>Start Game</c> or <c>Restart Level</c>.
        /// </remarks>
        public MemoryWatcher<uint> PickedPassportFunction { get; }

        /// <summary>
        ///     Timer determining whether to start Demo Mode or not.
        /// </summary>
        /// <remarks>
        ///     Value is initialized to zero, and it doesn't change outside the menu.
        ///     If no menu item is activated, and the value gets higher than 480, Demo Mode is started.
        ///     PC:
        ///     In the menu, value is set to zero if the user presses any mapped key.
        ///     If any menu item is active, the value just increases and Demo Mode is not activated.
        ///     PSX:
        ///     In the menu, pressing the left/right arrow stops the timer, pressing other keys sets it to zero.
        ///     If any menu item is active, the value gets reset to zero and it doesn't change.
        /// </remarks>
        public MemoryWatcher<uint> DemoTimer { get; }

        /// <summary>
        ///     Initializes <see cref="GameData"/> based on <paramref name="processVersion"/> and <paramref name="psxGameVersion"/>.
        /// </summary>
        /// <param name="processVersion">ATI, DOSBox, or one of the ePSXe versions allowed on the leaderboards.</param>
        /// <param name="psxGameVersion">One of the versions listed in <see cref="PSXGameVersion"/>.</param>
        public GameData(ProcessVersion processVersion, PSXGameVersion? psxGameVersion)
        {
            if (processVersion == ProcessVersion.ATI)
            {
                LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x5A014));
                Level = new MemoryWatcher<Level>(new DeepPointer(0x53C4C));
                LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x5BB0A));
                PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x5A080));
                DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x59F4C));
            }
            else if (processVersion == ProcessVersion.DOSBox)
            {
                LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x243D3C));
                Level = new MemoryWatcher<Level>(new DeepPointer(0xA786B4, 0x243D38));
                LevelTime = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x2513AC));
                PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x245C04));
                DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243BD4));
            }
            else if (processVersion == ProcessVersion.ePSXe180)
            {
                switch (psxGameVersion)
                {
                    case PSXGameVersion.USA_final:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DA510));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DA508));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6DC8DC));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6DB9DC));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DA488));
                        break;
                    case PSXGameVersion.USA_1_0:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DA428));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DA420));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6DC7DC));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6DB8F4));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DA3A0));
                        break;
                    case PSXGameVersion.GER:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DA668));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DA660));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6DCB28));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6DBC28));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DA5E0));
                        break;
                    case PSXGameVersion.EU:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DA5A0));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DA598));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6DC96C));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6DBA6C));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DA518));
                        break;
                    case PSXGameVersion.JP:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DA5B8));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DA5B0));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6E5888));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6DBACC));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DA530));
                        break;
                    case PSXGameVersion.FR:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DA628));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DA620));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6DCA88));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6DBB88));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DA5A0));
                        break;
                }
            }
            else if (processVersion == ProcessVersion.ePSXe190)
            {
                switch (psxGameVersion)
                {
                    case PSXGameVersion.USA_final:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DF010));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DF008));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6E13DC));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6E04DC));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DEF88));
                        break;
                    case PSXGameVersion.USA_1_0:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DEF28));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DEF20));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6E12DC));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6E03F4));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DEEA0));
                        break;
                    case PSXGameVersion.GER:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DF168));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DF160));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6EA4E4));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6E0728));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DF0E0));
                        break;
                    case PSXGameVersion.EU:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DF0A0));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DF098));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6EA328));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6E056C));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DF018));
                        break;
                    case PSXGameVersion.JP:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DF0B8));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DF0B0));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6E14CC));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6E05CC));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DF030));
                        break;
                    case PSXGameVersion.FR:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x6DF128));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x6DF120));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x6E1588));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x6E0688));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x6DF0A0));
                        break;
                }
            }
            else if (processVersion == ProcessVersion.ePSXe1925)
            {
                switch (psxGameVersion)
                {
                    case PSXGameVersion.USA_final:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x712D10));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x712D08));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x7150DC));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x7141DC));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x712C88));
                        break;
                    case PSXGameVersion.USA_1_0:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x712C28));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x712C20));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x714FDC));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x7140F4));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x712BA0));
                        break;
                    case PSXGameVersion.GER:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x712E68));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x712E60));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x71E1E4));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x714428));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x712DE0));
                        break;
                    case PSXGameVersion.EU:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x712DA0));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x712D98));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x71516C));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x71426C));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x712D18));
                        break;
                    case PSXGameVersion.JP:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x712DB8));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x712DB0));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x7151CC));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x7142CC));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x712D30));
                        break;
                    case PSXGameVersion.FR:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x712E28));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x712E20));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x715288));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x714388));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x712DA0));
                        break;
                }
            }
            else if (processVersion == ProcessVersion.ePSXe200)
            {
                switch (psxGameVersion)
                {
                    case PSXGameVersion.USA_final:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x8A1690));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x8A1688));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x8AC918));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x8A2B5C));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x8A1608));
                        break;
                    case PSXGameVersion.USA_1_0:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x8A15A8));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x8A15A0));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x8A395C));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x8A2A74));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x8A1520));
                        break;
                    case PSXGameVersion.GER:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x8A17E8));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x8A17E0));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x8A3CA8));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x8A2DA8));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x8A1760));
                        break;
                    case PSXGameVersion.EU:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x8A1720));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x8A1718));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x8AC9A8));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x8A2BEC));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x8A1698));
                        break;
                    case PSXGameVersion.JP:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x8A1738));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x8A1730));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x8A3B4C));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x8A2C4C));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x8A16B0));
                        break;
                    case PSXGameVersion.FR:
                        LevelComplete = new MemoryWatcher<bool>(new DeepPointer(0x8A17A8));
                        Level = new MemoryWatcher<Level>(new DeepPointer(0x8A17A0));
                        LevelTime = new MemoryWatcher<uint>(new DeepPointer(0x8ACAC4));
                        PickedPassportFunction = new MemoryWatcher<uint>(new DeepPointer(0x8A2D08));
                        DemoTimer = new MemoryWatcher<uint>(new DeepPointer(0x8A1720));
                        break;
                }
            }

        }
    }
}
