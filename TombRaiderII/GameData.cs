using System;
using LiveSplit.ComponentUtil;
using TRUtil;

namespace TR2
{
    /// <summary>
    ///     The supported game versions.
    /// </summary>
    internal enum GameVersion
    {
        None = 0,
        MP   = 1, // Multipatch (Steam/GoG default)
        EPC  = 2, // Eidos Premier Collection
        P1   = 3, // CORE's Patch 1
        UKB  = 4  // Eidos UK Box
    }

    /// <summary>
    ///     Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.
    /// </summary>
    internal sealed class GameData : ClassicGameData
    {
        /// <summary>
        ///     A constructor that primarily exists to set/modify static values/objects.
        /// </summary>
        internal GameData()
        {
            VersionHashes.Add("964f0c4e08ff44a905e8fc9a78f605dc", (uint)GameVersion.MP);
            VersionHashes.Add("793c67c79a50984d9bd17ad391f03c57", (uint)GameVersion.EPC);
            VersionHashes.Add("39cab6b4ae3c761b67ae308a0ab22e44", (uint)GameVersion.P1);
            VersionHashes.Add("12d56521ce038b55efba97463357a3d7", (uint)GameVersion.UKB);
            
            ProcessSearchNames.Add("tomb2");
            ProcessSearchNames.Add("tr2");

            // Valid for all supported game versions.
            FirstLevelTimeAddress = 0x51EA24;
            LevelSaveStructSize = 0x2C;
        }

        /// <summary>
        ///     Sets <see cref="GameData"/> addresses based on <paramref name="version"/>.
        /// </summary>
        /// <param name="version"><see cref="GameVersion"/> for which addresses should be assigned</param>
        protected override void SetAddresses(uint version)
        {
            switch ((GameVersion)version)
            {
                case GameVersion.UKB:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11BDA0)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xD9EC4)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD9EC0)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD7980)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xD7928)) { Name = "Health" });
                    break;
                case GameVersion.EPC:
                case GameVersion.MP:
                case GameVersion.P1:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11BD90)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xD9EB4)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD9EB0)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD7970)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xD7918)) { Name = "Health" });
                    break;
                case GameVersion.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }
    }
}
