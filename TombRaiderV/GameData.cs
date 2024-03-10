using System;
using LiveSplit.ComponentUtil;
using TRUtil;

namespace TR5;

/// <summary>The supported game versions.</summary>
internal enum GameVersion
{
    SteamOrGog,
    JapaneseNoCd,
}

/// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
internal sealed class GameData : LaterClassicGameData
{
    /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
    internal GameData()
    {
        VersionHashes.Add("179164156e3ca6641708d0419d6a91e9", (uint)GameVersion.SteamOrGog);
        VersionHashes.Add("e7cb29194a4ab2eb8bf759ffc3fe7e3d", (uint)GameVersion.JapaneseNoCd);

        ProcessSearchNames.Add("PCTomb5");
        // ReSharper disable once StringLiteralTypo
        ProcessSearchNames.Add("PCTomb5-nocd");
    }

    #region MemoryWatcherList Items

    /// <summary>Used to determine when to reset many of the game's global variables.</summary>
    /// <remarks>
    ///     1 during the main menu and when loading/starting game from the main menu, and when loading/watching credits; otherwise, 0.
    /// </remarks>
    public static MemoryWatcher<bool> GfInitializeGame => (MemoryWatcher<bool>)Watchers?["GfInitializeGame"];

    /// <summary>Used as a sort of enumeration to allow game flow and other global variables to act appropriately.</summary>
    /// <remarks>
    ///     0 during any gameplay, 1 while in the main menu, 4 while loading a save.
    /// </remarks>
    public static MemoryWatcher<byte> GfGameMode => (MemoryWatcher<byte>)Watchers?["GfGameMode"];

    #endregion

    protected override void SetAddresses(uint version)
    {
        Watchers.Clear();
        switch ((GameVersion)version)
        {
            case GameVersion.SteamOrGog:
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C2F0)) { Name = "GfLevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C2D0)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C27C)) { Name = "GameTimer"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xA5BF60)) { Name = "Loading"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xA5BF08, 0x22)) { Name = "Health"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x1082C1)) { Name = "GfInitializeGame" });
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x1082C0)) { Name = "GfGameMode" });
                break;
            case GameVersion.JapaneseNoCd:
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C3F0)) { Name = "GfLevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C3D0)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C37C)) { Name = "GameTimer"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xA5C060)) { Name = "Loading"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xA5C008, 0x22)) { Name = "Health"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x1082D1)) { Name = "GfInitializeGame" });
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x1082D0)) { Name = "GfGameMode" });
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }
}