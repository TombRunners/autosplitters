using System;
using LiveSplit.ComponentUtil;
using TRUtil;

namespace TR2Gold
{       
    /// <summary>The supported game versions.</summary>
    internal enum GameVersion
    {
        None          = 0,   
        Stella        = 1,
        StellaCracked = 2  // No-CD cracked
    }

    /// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
    internal sealed class GameData : ClassicGameData
    {
        /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
        internal GameData()
        {
            VersionHashes.Add("13fa4e8585d1a1d52d342a513f65f19f", (uint)GameVersion.Stella);
            VersionHashes.Add("3f262621d07a3c6c6fdd6f654814f988", (uint)GameVersion.StellaCracked);

            ProcessSearchNames.Add("t2gold");
            
            // Valid for all supported game versions.
            FirstLevelTimeAddress = 0x521A84;  
            LevelSaveStructSize = 0x2C;
        }

        protected override void SetAddresses(uint version)
        {
            switch ((GameVersion)version)
            {
                case GameVersion.Stella:
                case GameVersion.StellaCracked:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11EE00)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xDCF28)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xDCF24)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x121E60)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xDA9A0)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xDA948)) { Name = "Health" });
                    break;
                case GameVersion.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }
    }
}
