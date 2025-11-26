using System;
using LaterClassicUtil;
using LiveSplit.ComponentUtil;

// ReSharper disable ClassNeverInstantiated.Global

namespace TR4;

/// <summary>Manages the game's watched memory values for <see cref="Autosplitter" />'s use.</summary>
internal class GameData : LaterClassicGameData
{
    private const uint SizeOfItemInfo = 0x15F6;
    private static readonly IntPtr FirstItemInfoPointer = (IntPtr) 0x7FE28C;

    /// <summary>A constructor that primarily exists to set/modify values/objects.</summary>
    public GameData()
    {
        VersionHashes.Add("bff3fea78480671ee81831cc6c6e8805", (uint) Tr4Version.SteamOrGog);
        VersionHashes.Add("8911cb2762051da52092bd4a752c2aaa", (uint) Tr4Version.SteamOrGog16x9);
        VersionHashes.Add("106f76bf6867b294035074ee005ab91a", (uint) Tr4Version.TheTimesExclusive);
        VersionHashes.Add("bc0c6169022f59176867daa884425547", (uint) Tr4Version.TheTimesExclusive16x9);

        ProcessSearchNames.Add("tomb4");
    }

    /// <summary>Used to disambiguate Lara's start position once loaded into a new level.</summary>
    /// <remarks>0 for levels without multiple Lara start positions; non-zero when going to the non-default start position.</remarks>
    public MemoryWatcher<byte> GfRequiredStartPosition => (MemoryWatcher<byte>) Watchers["GfRequiredStartPosition"];

    /// <summary>
    ///     A bitfield indicating the "progress" at which Lara has collected the parts for or has completed
    ///     the creation of the "Mechanical Scarab With Key", needed to progress through Cleopatra's Palaces.
    ///     Since both the Winding Key and Mechanical Scarab parts are located in Temple of Isis, this can be
    ///     used to decide if the autosplitter should split when Lara enters the Cleopatra's Palaces level;
    ///     if Lara has both parts or the combined object, there is no need to revisit Temple of Isis.
    /// </summary>
    /// <remarks>
    ///     Only the first 3 bits of the <see cref="byte" /> are used. The game checks for the parts using bitwise '&':
    ///         <see cref="MechanicalScarab" /> & 1 => Mechanical Scarab With Key (0000 0001)
    ///         <see cref="MechanicalScarab" /> & 2 => Winding Key (0000 0010)
    ///         <see cref="MechanicalScarab" /> & 4 => Mechanical Scarab (0000 0100)
    ///     When Lara has both Winding Key and Mechanical Scarab before combining them: (0000 0110)
    /// </remarks>
    public MemoryWatcher<byte> MechanicalScarab => (MemoryWatcher<byte>) Watchers["MechanicalScarab"];

    /// <inheritdoc cref="TR4.PuzzleItems" />
    /// <remarks>
    ///     The corresponding items/indices are relevant for the autosplitter's logic:
    ///         Index 7, Item 8 || Cairo || Mine Detonator (the combined item)
    ///         Index 8, Item 9 || Giza  || Eastern Shaft Key
    ///     When unique items are in Lara's inventory, the address's value is 1.
    ///     Non-unique puzzle items, such as the Golden Skull secrets in Cambodia, continually increment their assigned index.
    /// </remarks>
    public MemoryWatcher<PuzzleItems> PuzzleItems => (MemoryWatcher<PuzzleItems>) Watchers["PuzzleItemsArray"];

    /// <summary>
    ///     An unsigned short used as a bitfield to track which combinable puzzle items Lara has in her inventory.
    /// </summary>
    /// <remarks>
    ///     The corresponding bits are relevant for the autosplitter's logic:
    ///         <see cref="PuzzleItemsCombo" /> & 0x40 => Mine Detonator Body (0100 0000 0000 0000)
    ///         <see cref="PuzzleItemsCombo" /> & 0x80 => Mine Position Data  (1000 0000 0000 0000)
    ///     When Lara has both Mine Detonator Body and Mine Position Data before combining them: (1100 0000 0000 0000)
    /// </remarks>
    public MemoryWatcher<ushort> PuzzleItemsCombo => (MemoryWatcher<ushort>) Watchers["PuzzleItemsCombo"];

    /// <summary>
    ///     An unsigned short used as a bitfield to track which keys Lara has in her inventory.
    /// </summary>
    /// <remarks>
    ///     The corresponding bits are relevant for the autosplitter's logic:
    ///         <see cref="KeyItems" /> & 2 => Hypostyle Key (0000 0000 0000 0010)
    /// </remarks>
    public MemoryWatcher<ushort> KeyItems => (MemoryWatcher<ushort>) Watchers["KeyItems"];

    /// <summary>Lara's current room.</summary>
    public MemoryWatcher<short> Room => (MemoryWatcher<short>) Watchers?["Room"];

    /// <inheritdoc />
    protected override void SetMemoryAddresses(uint version)
    {
        Watchers.Clear();
        switch ((Tr4Version) version)
        {
            case Tr4Version.SteamOrGog:
            case Tr4Version.SteamOrGog16x9:
                // Base
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD290)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x40E13C, 0x22)) { Name = "Health" });
                // Later Classic
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xAF452)) { Name = "GfInitializeGame" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD2B0)) { Name = "GfLevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD258)) { Name = "GameTimer" });
                Watchers.Add(new MemoryWatcher<long>(new DeepPointer(0xBF3C8)) { Name = "InventoryActive" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x1333A8)) { Name = "Loading" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x40E13C, 0x18)) { Name = "Room" });
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x3F7864)) { Name = "Secrets" });
                // Game
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x3FD270)) { Name = "GfRequiredStartPosition" });
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x40E0FB)) { Name = "MechanicalScarab" });
                Watchers.Add(new MemoryWatcher<PuzzleItems>(new DeepPointer(0x040E101)) { Name = "PuzzleItemsArray" });
                Watchers.Add(new MemoryWatcher<ushort>(new DeepPointer(0x040E10D)) { Name = "PuzzleItemsCombo" });
                Watchers.Add(new MemoryWatcher<ushort>(new DeepPointer(0x040E10F)) { Name = "KeyItems" });
                break;

            case Tr4Version.TheTimesExclusive:
            case Tr4Version.TheTimesExclusive16x9:
                // Base
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD2D0)) { Name = "Level" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x40E17C, 0x22)) { Name = "Health" });
                // Later Classic
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0xAF452)) { Name = "GfInitializeGame" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD2F0)) { Name = "GfLevelComplete" });
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD298)) { Name = "GameTimer" });
                Watchers.Add(new MemoryWatcher<long>(new DeepPointer(0xBF408)) { Name = "InventoryActive" });
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x1333E8)) { Name = "Loading" });
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x40E17C, 0x18)) { Name = "Room" });
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x3F78A4)) { Name = "Secrets" });
                // Game
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x3FD2B0)) { Name = "GfRequiredStartPosition" });
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }

    /// <inheritdoc />
    protected override bool IsGameInitialized() => true;

    internal ItemInfo GetItemInfoAtIndex(uint itemNumber)
    {
        uint offset = SizeOfItemInfo * itemNumber;
        IntPtr firstItemInfoAddress = GameProcess.ReadPointer(FirstItemInfoPointer);
        var finalAddress = new IntPtr(firstItemInfoAddress.ToInt64() + offset);
        return GameProcess.ReadValue<ItemInfo>(finalAddress);
    }
}