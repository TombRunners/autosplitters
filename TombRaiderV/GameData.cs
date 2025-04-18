﻿using System;
using LiveSplit.ComponentUtil;
using TRUtil;

// ReSharper disable ClassNeverInstantiated.Global

namespace TR5;

/// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
internal sealed class GameData : LaterClassicGameData
{
    /// <summary>A constructor that primarily exists to set/modify values/objects.</summary>
    public GameData()
    {
        VersionHashes.Add("179164156e3ca6641708d0419d6a91e9", (uint)Tr5Version.SteamOrGog);
        VersionHashes.Add("e7cb29194a4ab2eb8bf759ffc3fe7e3d", (uint)Tr5Version.JapaneseNoCd);
        VersionHashes.Add("502fd7d1461e934471cfe37c27f246fe", (uint)Tr5Version.SteamOrGogCutsceneless);

        // ReSharper disable StringLiteralTypo
        ProcessSearchNames.Add("PCTomb5");
        ProcessSearchNames.Add("PCTomb5-nocd");
        ProcessSearchNames.Add("cutscene_skipper");
        ProcessSearchNames.Add("cutscene_skipper_502fd7d1");
        // ReSharper restore StringLiteralTypo
    }

    #region MemoryWatcherList Items

    /// <summary>Used as a sort of enumeration to allow game flow and other global variables to act appropriately.</summary>
    /// <remarks>
    ///     0 during any gameplay, 1 while in the main menu, 4 while loading a save.
    /// </remarks>
    public MemoryWatcher<byte> GfGameMode => (MemoryWatcher<byte>)Watchers?["GfGameMode"];

    #endregion

    /// <inheritdoc />
    protected override void SetMemoryAddresses(uint version)
    {
        Watchers.Clear();
        switch ((Tr5Version)version)
        {
            case Tr5Version.SteamOrGog:
            case Tr5Version.SteamOrGogCutsceneless:
                // Base
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C2D0)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xA5BF08, 0x22)) { Name = "Health" });
                // Later Classic
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x1082C1)) { Name = "GfInitializeGame" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C2F0)) { Name = "GfLevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C27C)) { Name = "GameTimer" });
                Watchers.Add(new MemoryWatcher<long>(new DeepPointer(0x11CF20)) { Name = "InventoryActive" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xA5BF60)) { Name = "Loading" });
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0xA53040)) { Name = "Secrets" });
                // Game
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x1082C0)) { Name = "GfGameMode" });
                break;

            case Tr5Version.JapaneseNoCd:
                // Base
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C3D0)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0xA5C008, 0x22)) { Name = "Health" });
                // Later Classic
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x1082D1)) { Name = "GfInitializeGame" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C3F0)) { Name = "GfLevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0xA5C37C)) { Name = "GameTimer" });
                Watchers.Add(new MemoryWatcher<long>(new DeepPointer(0x11D030)) { Name = "InventoryActive" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xA5C060)) { Name = "Loading" });
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0xA53140)) { Name = "Secrets" });
                // Game
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x1082D0)) { Name = "GfGameMode" });
                break;

            case Tr5Version.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }

    /// <inheritdoc />
    protected override bool IsGameInitialized() => true;
}