using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics.CodeAnalysis;
using TRUtil;

namespace TR3;

/// <summary>The supported game versions.</summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal enum GameVersion
{
    Int,                         // From Steam
    Int16x9AspectRatio,          // Int with bytes at address 0xA9410 changed to 39 8E E3 (float value 1.7777, 16/9)
    JpCracked,                   // No-CD cracked TR3 from JP Gold bundle release
    JpCracked16x9AspectRatio,    // JpCracked with bytes at address 0xA9410 changed to 39 8E E3 (float value 1.7777, 16/9)
    JpTlaCracked,                // No-CD cracked TLA from JP Gold bundle release
    JpTlaCracked16x9AspectRatio, // JpTlaCracked with bytes at address 0xA9410 changed to 39 8E E3 (float value 1.7777, 16/9)
}

/// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
internal sealed class GameData : ClassicGameData
{
    private const uint TR3FirstLevelTimeAddress = 0x6D2326;
    private const uint TlaFirstLevelTimeAddress = 0x6CAF46;

    /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
    internal GameData()
    {
        VersionHashes.Add("4044dc2c58f02bfea2572e80dd8f2abb", (uint)GameVersion.Int);
        VersionHashes.Add("46a780f8f5314d5284f1d1b3ab468ab2", (uint)GameVersion.Int16x9AspectRatio);
        VersionHashes.Add("66404f58bb5dbf30707abfd245692cd2", (uint)GameVersion.JpCracked);
        VersionHashes.Add("1c9bdf6b998b34752cb0c7d315129af6", (uint)GameVersion.JpCracked16x9AspectRatio);
        VersionHashes.Add("c3030264e597a496cc920d9c97324046", (uint)GameVersion.JpTlaCracked);
        VersionHashes.Add("64a166ef57aa4f786e6a7f41eae14806", (uint)GameVersion.JpTlaCracked16x9AspectRatio);

        ProcessSearchNames.Add("tomb3");
        ProcessSearchNames.Add("tr3gold");

        LevelSaveStructSize = 0x33; // All TR3 and TLA versions.
    }

    protected override void SetAddresses(uint version)
    {
        switch ((GameVersion)version)
        {
            case GameVersion.Int:
            case GameVersion.Int16x9AspectRatio:
                Watchers.Clear();
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x2A1C58)) { Name = "TitleScreen"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x233F54)) { Name = "LevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xC561C)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health"});
                FirstLevelTimeAddress = TR3FirstLevelTimeAddress;
                break;
            case GameVersion.JpCracked:
            case GameVersion.JpCracked16x9AspectRatio:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x2A1C60)) { Name = "TitleScreen"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x233F5C)) { Name = "LevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xC561C)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2D27CF)) { Name = "LevelTime"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x226458)) { Name = "PickedPassportFunction"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x22640C)) { Name = "Health"});
                FirstLevelTimeAddress = TR3FirstLevelTimeAddress;
                break;
            case GameVersion.JpTlaCracked:
            case GameVersion.JpTlaCracked16x9AspectRatio:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x29AA04)) { Name = "TitleScreen"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x22CE38)) { Name = "LevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x22CE34)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x2CB3EF)) { Name = "LevelTime"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x21F318)) { Name = "PickedPassportFunction"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x21F2DC)) { Name = "Health"});
                FirstLevelTimeAddress = TlaFirstLevelTimeAddress;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }
}