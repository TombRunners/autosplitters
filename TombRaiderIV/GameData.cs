using System;
using System.Collections.Generic;
using LiveSplit.ComponentUtil;
using TRUtil;

namespace TR4
{       
    /// <summary>The supported game versions.</summary>
    internal enum GameVersion
    {
        SteamOrGog
    }

    /// <summary>Manages the game's watched memory values for <see cref="Autosplitter"/>'s use.</summary>
    internal sealed class GameData : LaterClassicGameData
    { 

        /// <summary>A constructor that primarily exists to set/modify static values/objects.</summary>
        internal GameData()
        {
            VersionHashes.Add("bff3fea78480671ee81831cc6c6e8805", (uint)GameVersion.SteamOrGog);

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
        ///     Only the first 3 bits of the <see cref="byte"/> are used. The game checks for the parts using bitwise &:
        ///         <see cref="MechanicalScarab"/> & 1 => Mechanical Scarab With Key (00000001)
        ///         <see cref="MechanicalScarab"/> & 2 => Winding Key (00000010)
        ///         <see cref="MechanicalScarab"/> & 4 => Mechanical Scarab (00000100)
        ///     When Lara has both Winding Key and Mechanical Scarab before combining them: (00000110)
        /// </remarks>
        public static MemoryWatcher<byte> MechanicalScarab => (MemoryWatcher<byte>)Watchers["MechanicalScarab"];
        
        /// <summary>
        ///     This is the 8th (index 7) of 12 items in the puzzle items char array.
        /// </summary>
        /// <remarks>The shaft keys are indices 5 - 8. Their value is 1 if they are in Lara's inventory.</remarks>
        public static MemoryWatcher<byte> EasternShaftKey => (MemoryWatcher<byte>)Watchers["EasternShaftKey"];

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
                    Watchers.Add(new MemoryWatcher<byte>(new DeepPointer(0x40E109)) { Name = "EasternShaftKey" });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(version), version, null);
            }
        }
        
        public override double SumCompletedLevelTimes(IEnumerable<uint> completedLevels, uint currentLevel)
        {
            throw new NotImplementedException();
        }
    }
}
