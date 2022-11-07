using System;
using LiveSplit.ComponentUtil;
using TRUtil;

namespace TR4;

/// <summary>The supported game versions.</summary>
internal enum GameVersion
{
    SteamOrGog,
    TheTimesExclusive
}

/// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
internal sealed class GameData : LaterClassicGameData
{ 
    private const uint SizeOfItemInfo = 0x15F6;
    private static readonly IntPtr FirstItemInfoPointer = (IntPtr)0x7FE28C;

    /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
    internal GameData()
    {
        VersionHashes.Add("bff3fea78480671ee81831cc6c6e8805", (uint)GameVersion.SteamOrGog);
        VersionHashes.Add("106f76bf6867b294035074ee005ab91a", (uint)GameVersion.TheTimesExclusive);

        ProcessSearchNames.Add("tomb4");
    }

    /// <summary>
    ///     A bitfield indicating the "progress" at which Lara has collected the parts for or has completed
    ///     the creation of the "Mechanical Scarab With Key", needed to progress through Cleopatra's Palaces.
    ///     Since both the Winding Key and Mechanical Scarab parts are located in Temple of Isis, this can be
    ///     used to decide if the autosplitter should split when Lara enters the Cleopatra's Palaces level;
    ///     if Lara has both parts or the combined object, there is no need to revisit Temple of Isis.
    /// </summary>
    /// <remarks>
    ///     Only the first 3 bits of the <see cref="byte"/> are used. The game checks for the parts using bitwise '&':
    ///         <see cref="MechanicalScarab"/> & 1 => Mechanical Scarab With Key (0000 0001)
    ///         <see cref="MechanicalScarab"/> & 2 => Winding Key (0000 0010)
    ///         <see cref="MechanicalScarab"/> & 4 => Mechanical Scarab (0000 0100)
    ///     When Lara has both Winding Key and Mechanical Scarab before combining them: (0000 0110)
    /// </remarks>
    public static MemoryWatcher<byte> MechanicalScarab => (MemoryWatcher<byte>)Watchers["MechanicalScarab"];

    /// <inheritdoc cref="TR4.PuzzleItems"/>
    /// <remarks>
    ///     The corresponding items/indices are relevant for the autosplitter's logic:
    ///         Index 7, Item 8 || Cairo || Mine Detonator (the combined item)
    ///         Index 8, Item 9 || Giza  || Eastern Shaft Key
    ///     When unique items are in Lara's inventory, the address's value is 1. 
    ///     Non-unique puzzle items, such as the Golden Skull secrets in Cambodia, continually increment their assigned index.
    /// </remarks>
    public static MemoryWatcher<PuzzleItems> PuzzleItems => (MemoryWatcher<PuzzleItems>)Watchers["PuzzleItemsArray"];

    /// <summary>
    ///     An unsigned short used as a bitfield to track which combinable puzzle items Lara has in her inventory.
    /// </summary>
    /// <remarks>
    ///     The corresponding bits are relevant for the autosplitter's logic:
    ///         <see cref="PuzzleItemsCombo"/> & 0x40 => Mine Detonator Body (0100 0000 0000 0000)
    ///         <see cref="PuzzleItemsCombo"/> & 0x80 => Mine Position Data  (1000 0000 0000 0000)
    ///     When Lara has both Mine Detonator Body and Mine Position Data before combining them: (1100 0000 0000 0000)
    /// </remarks>
    public static MemoryWatcher<ushort> PuzzleItemsCombo => (MemoryWatcher<ushort>)Watchers["PuzzleItemsCombo"];
        
    /// <summary>
    ///     An unsigned short used as a bitfield to track which keys Lara has in her inventory.
    /// </summary>
    /// <remarks>
    ///     The corresponding bits are relevant for the autosplitter's logic:
    ///         <see cref="KeyItems"/> & 2 => Hypostyle Key (0000 0000 0000 0010)
    /// </remarks>
    public static MemoryWatcher<ushort> KeyItems => (MemoryWatcher<ushort>)Watchers["KeyItems"];

    protected override void SetAddresses(uint version)
    {
        Watchers.Clear();
        switch ((GameVersion)version)
        {
            case GameVersion.SteamOrGog:
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD2B0)) { Name = "GfLevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD290)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<ulong>(new DeepPointer(0x3FD258)) { Name = "GameTimer"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x1333A8)) { Name = "Loading"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x40E13C, 0x22)) { Name = "Health"});
                Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x40E0FB)) { Name = "MechanicalScarab" });
                Watchers.Add(new MemoryWatcher<PuzzleItems>(new DeepPointer(0x040E101)) { Name = "PuzzleItemsArray" });
                Watchers.Add(new MemoryWatcher<ushort>(new DeepPointer(0x040E10D)) { Name = "PuzzleItemsCombo"});
                Watchers.Add(new MemoryWatcher<ushort>(new DeepPointer(0x040E10F)) { Name = "KeyItems"});
                break;
            case GameVersion.TheTimesExclusive:
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD2F0)) { Name = "GfLevelComplete"});
                Watchers.Add(new MemoryWatcher<uint>(new DeepPointer(0x3FD2D0)) { Name = "Level"});
                Watchers.Add(new MemoryWatcher<ulong>(new DeepPointer(0x3FD298)) { Name = "GameTimer"});
                Watchers.Add(new MemoryWatcher<bool>(new DeepPointer(0x1333E8)) { Name = "Loading"});
                Watchers.Add(new MemoryWatcher<short>(new DeepPointer(0x40E17C, 0x22)) { Name = "Health"});
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }
    }

    internal static ItemInfo GetItemInfoAtIndex(uint itemNumber)
    {
        uint offset = SizeOfItemInfo * itemNumber;
        var firstItemInfoAddress = Game.ReadPointer(FirstItemInfoPointer);
        var finalAddress = new IntPtr(firstItemInfoAddress.ToInt64() + offset);
        return Game.ReadValue<ItemInfo>(finalAddress);
    }
}