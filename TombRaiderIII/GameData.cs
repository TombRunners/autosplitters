using System;
using System.Collections.Generic;
using System.Linq;
using LiveSplit.ComponentUtil;
using TRUtil;

namespace TR3;

/// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
internal sealed class GameData : ClassicGameData
{
    private const uint TR3FirstLevelTimeAddress = 0x6D2326;
    private const uint TlaFirstLevelTimeAddress = 0x6CAF46;

    /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
    internal GameData()
    {
        VersionHashes.Add("4044dc2c58f02bfea2572e80dd8f2abb", (uint)Tr3Version.Int);
        VersionHashes.Add("46a780f8f5314d5284f1d1b3ab468ab2", (uint)Tr3Version.Int16x9AspectRatio);
        VersionHashes.Add("66404f58bb5dbf30707abfd245692cd2", (uint)Tr3Version.JpCracked);
        VersionHashes.Add("1c9bdf6b998b34752cb0c7d315129af6", (uint)Tr3Version.JpCracked16x9AspectRatio);
        VersionHashes.Add("c3030264e597a496cc920d9c97324046", (uint)Tr3Version.JpTlaCracked);
        VersionHashes.Add("64a166ef57aa4f786e6a7f41eae14806", (uint)Tr3Version.JpTlaCracked16x9AspectRatio);

        ProcessSearchNames.Add("tomb3");
        ProcessSearchNames.Add("tr3gold");

        LevelSaveStructSize = 0x33; // All TR3 and TLA versions.

        SetAddresses += SetMemoryAddresses;
        SumLevelTimes += SumCompletedLevelTimes;
    }

    private void SetMemoryAddresses(uint version)
    {
        switch ((Tr3Version)version)
        {
            case Tr3Version.Int:
            case Tr3Version.Int16x9AspectRatio:
                Watchers.Clear();
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x2A1C58)) { Name = "TitleScreen"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x233F54)) { Name = "LevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xC561C)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health"});
                FirstLevelTimeAddress = TR3FirstLevelTimeAddress;
                break;

            case Tr3Version.JpCracked:
            case Tr3Version.JpCracked16x9AspectRatio:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x2A1C60)) { Name = "TitleScreen"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x233F5C)) { Name = "LevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xC561C)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health"});
                FirstLevelTimeAddress = TR3FirstLevelTimeAddress;
                break;

            case Tr3Version.JpTlaCracked:
            case Tr3Version.JpTlaCracked16x9AspectRatio:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x29AA04)) { Name = "TitleScreen"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x22CE38)) { Name = "LevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x22CE34)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2CB3EF)) { Name = "LevelTime"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x21F318)) { Name = "PickedPassportFunction"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x21F2DC)) { Name = "Health"});
                FirstLevelTimeAddress = TlaFirstLevelTimeAddress;
                break;

            case Tr3Version.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }

    /// <summary>Sums completed levels' times.</summary>
    /// <returns>The sum of completed levels' times</returns>
    private ulong SumCompletedLevelTimes(IEnumerable<uint> completedLevels, uint? currentLevel)
    {
        uint finishedLevelsTicks = completedLevels
            .TakeWhile(completedLevel => completedLevel != currentLevel)
            .Select(completedLevel => (completedLevel - 1) * LevelSaveStructSize)
            .Select(levelOffset => (IntPtr)(FirstLevelTimeAddress + levelOffset))
            .Aggregate<IntPtr, uint>(0, (ticks, levelAddress) => ticks + GameProcess.ReadValue<uint>(levelAddress));

        return finishedLevelsTicks;
    }
}