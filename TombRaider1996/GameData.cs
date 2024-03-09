using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using TRUtil;

// ReSharper disable StringLiteralTypo

namespace TR1;

/// <summary>The supported game versions.</summary>
internal enum GameVersion
{
    Ati,
    AtiUnfinishedBusiness,
    DOSBox,
}

/// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
internal sealed class GameData : ClassicGameData
{
    internal static readonly List<uint> CompletedLevelTicks = [];

    /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
    internal GameData()
    {
        VersionHashes.Add("e4b95c0479d7256af56b8a9897ed4b13", (uint)GameVersion.Ati);
        VersionHashes.Add("de6b2bf4c04a93f0833b9717386e4a3b", (uint)GameVersion.DOSBox);
        VersionHashes.Add("1e086eaa88568b23d322283d9cb664d6", (uint)GameVersion.AtiUnfinishedBusiness);

        ProcessSearchNames.Add("dosbox");
        ProcessSearchNames.Add("tombati");
        ProcessSearchNames.Add("tombub");
    }

    /// <summary>
    ///     Timer determining whether to start Demo Mode or not.
    /// </summary>
    /// <remarks>
    ///     Value is initialized to zero, and it doesn't change outside the menu.
    ///     In the menu, value is set to zero if the user presses any key.
    ///     If no menu item is activated, and the value gets higher than 480, Demo Mode starts.
    ///     If any menu item is active, the value increases and Demo Mode does not activate.
    /// </remarks>
    public static MemoryWatcher<uint> DemoTimer => (MemoryWatcher<uint>)Watchers["DemoTimer"];

    protected override void SetAddresses(uint version)
    {
        Watchers.Clear();
        switch ((GameVersion)version)
        {
            case GameVersion.Ati:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x5A324)) { Name = "TitleScreen" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x59F4C)) { Name = "DemoTimer" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x5A014)) { Name = "LevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x53C4C)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x5BB0A)) { Name = "LevelTime" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x5A080)) { Name = "PickedPassportFunction" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x5A02C)) { Name = "Health" });
                break;
            case GameVersion.AtiUnfinishedBusiness:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x59CFC)) { Name = "TitleScreen" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x599EC)) { Name = "LevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x599E8)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x59A00)) { Name = "LevelTime" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x59A58)) { Name = "PickedPassportFunction" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x59A04)) { Name = "Health" });
                break;
            case GameVersion.DOSBox:
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

    public override double SumCompletedLevelTimes(IEnumerable<uint> completedLevels, uint? currentLevel)
    {
        int validLevelCount = completedLevels.TakeWhile(completedLevel => completedLevel != currentLevel).Count();
        var finishedLevelsTicks = (uint)CompletedLevelTicks
            .Take(validLevelCount)
            .Sum(static x => x);

        return LevelTimeAsDouble(finishedLevelsTicks);
    }
}