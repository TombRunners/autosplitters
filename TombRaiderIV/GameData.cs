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
