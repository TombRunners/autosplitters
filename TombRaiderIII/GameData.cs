using System;
using LiveSplit.ComponentUtil;
using TRUtil;

namespace TR3
{       
    /// <summary>The supported game versions.</summary>
    internal enum GameVersion
    {
        Int,       // From Steam
        JpCracked  // No-CD cracked TR3 from JP Gold bundle release
    }

    /// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
    internal sealed class GameData : ClassicGameData
    {
        /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
        internal GameData()
        {
            VersionHashes.Add("4044dc2c58f02bfea2572e80dd8f2abb", (uint)GameVersion.Int);
            VersionHashes.Add("66404f58bb5dbf30707abfd245692cd2", (uint)GameVersion.JpCracked);

            ProcessSearchNames.Add("tomb3");
            
            // Valid for all supported game versions.
            FirstLevelTimeAddress = 0x6D2326;
            LevelSaveStructSize = 0x33;
        }
        
        protected override void SetAddresses(uint version)
        {
            switch ((GameVersion)version)
            {
                case GameVersion.Int:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x2A1C58)) { Name = "TitleScreen"});
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x233F54)) { Name = "LevelComplete"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xC561C)) { Name = "Level"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction"});
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health"});
                    break;
                case GameVersion.JpCracked:
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x2A1C60)) { Name = "TitleScreen"});
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x233F5C)) { Name = "LevelComplete"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xC561C)) { Name = "Level"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime"});
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction"});
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health"});
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }
    }
}
