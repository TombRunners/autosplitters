using System;
using LiveSplit.ComponentUtil;
using TRUtil;

namespace TR1
{
    /// <summary>The supported game versions.</summary>
    internal enum GameVersion
    {
        ATI,
        DOSBox
    }

    /// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
    internal sealed class GameData : ClassicGameData
    {
        /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
        internal GameData()
        {
            VersionHashes.Add("e4b95c0479d7256af56b8a9897ed4b13", (uint)GameVersion.ATI);
            VersionHashes.Add("de6b2bf4c04a93f0833b9717386e4a3b", (uint)GameVersion.DOSBox);

            ProcessSearchNames.Add("tombati");
            ProcessSearchNames.Add("dosbox");
        }

        /// <summary>
        ///     Timer determining whether to start Demo Mode or not.
        /// </summary>
        /// <remarks>
        ///     Value is initialized to zero, and it doesn't change outside the menu.
        ///     In the menu, value is set to zero if the user presses any key.
        ///     If no menu item is activated, and the value gets higher than 480, Demo Mode is started.
        ///     If any menu item is active, the value just increases and Demo Mode is not activated.
        /// </remarks>
        public static MemoryWatcher<uint> DemoTimer => (MemoryWatcher<uint>)Watchers["DemoTimer"];
        
        protected override void SetAddresses(uint version)
        {
            switch ((GameVersion)version)
            {
                case GameVersion.ATI:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x5A324)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x59F4C)) { Name = "DemoTimer" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x5A014)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x53C4C)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x5BB0A)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x5A080)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x5A02C)) { Name = "Health" });
                    break;
                case GameVersion.DOSBox:
                    Watchers.Clear();
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x247B34)) { Name = "TitleScreen" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243BD4)) { Name = "DemoTimer" });
                    Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xA786B4, 0x243D3C)) { Name = "LevelComplete" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x243D38)) { Name = "Level" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x2513AC)) { Name = "LevelTime" });
                    Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA786B4, 0x245C04)) { Name = "PickedPassportFunction" });
                    Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xA786B4, 0x244448)) { Name = "Health" });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }
    }
}