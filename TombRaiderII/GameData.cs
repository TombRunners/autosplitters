using LiveSplit.ComponentUtil;
using System;
using TRUtil;

namespace TR2;

/// <summary>The supported game versions.</summary>
internal enum GameVersion
{
    None              = 0,
    MP                = 1, // Multipatch (Steam/GoG default)
    EPC               = 2, // Eidos Premier Collection
    P1                = 3, // CORE's Patch 1
    UKB               = 4, // Eidos UK Box
    StellaGold        = 5, // TR2G available from Stella's website
    StellaGoldCracked = 6, // No-CD cracked version of StellaGold
}

/// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
internal sealed class GameData : ClassicGameData
{
    private const uint TR2FirstLevelTimeAddress = 0x51EA24;
    private const uint TR2GFirstLevelTimeAddress = 0x521A84;

    /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
    internal GameData()
    {
        VersionHashes.Add("964f0c4e08ff44a905e8fc9a78f605dc", (uint)GameVersion.MP);
        VersionHashes.Add("793c67c79a50984d9bd17ad391f03c57", (uint)GameVersion.EPC);
        VersionHashes.Add("39cab6b4ae3c761b67ae308a0ab22e44", (uint)GameVersion.P1);
        VersionHashes.Add("12d56521ce038b55efba97463357a3d7", (uint)GameVersion.UKB);
        VersionHashes.Add("13fa4e8585d1a1d52d342a513f65f19f", (uint)GameVersion.StellaGold);
        VersionHashes.Add("3f262621d07a3c6c6fdd6f654814f988", (uint)GameVersion.StellaGoldCracked);

        ProcessSearchNames.Add("tomb2");
        ProcessSearchNames.Add("tr2");
        ProcessSearchNames.Add("t2gold");

        LevelSaveStructSize = 0x2C; // All TR2 and TR2G versions
    }

    protected override void SetAddresses(uint version)
    {
        Watchers.Clear();
        switch ((GameVersion)version)
        {
            case GameVersion.UKB:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11BDA0)) { Name = "TitleScreen" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xD9EC4)) { Name = "LevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD9EC0)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD7980)) { Name = "PickedPassportFunction" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xD7928)) { Name = "Health" });
                FirstLevelTimeAddress = TR2FirstLevelTimeAddress;
                break;
            case GameVersion.EPC:
            case GameVersion.MP:
            case GameVersion.P1:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11BD90)) { Name = "TitleScreen" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xD9EB4)) { Name = "LevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD9EB0)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x11EE00)) { Name = "LevelTime" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xD7970)) { Name = "PickedPassportFunction" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xD7918)) { Name = "Health" });
                FirstLevelTimeAddress = TR2FirstLevelTimeAddress;
                break;
            case GameVersion.StellaGold:
            case GameVersion.StellaGoldCracked:
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x11EE00)) { Name = "TitleScreen" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xDCF28)) { Name = "LevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xDCF24)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x121E60)) { Name = "LevelTime" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xDA9A0)) { Name = "PickedPassportFunction" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xDA948)) { Name = "Health" });
                FirstLevelTimeAddress = TR2GFirstLevelTimeAddress;
                break;
            case GameVersion.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }
}