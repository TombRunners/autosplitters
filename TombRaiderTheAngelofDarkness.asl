state("TRAOD", "TRAOD.exe, v39")
{
	byte gameAction: 0x439AB8;
	string30 mapName: 0x4887EA;
}

state("TRAOD_P3", "TRAOD_P3.exe, v39")
{
	byte gameAction: 0x372A64;
	string30 mapName: 0x41CDAA;
}

state("TRAOD_P4", "TRAOD_P4.exe, v39")
{
	byte gameAction: 0x3833E4;
	string30 mapName: 0x42D72A;
}

state("TRAOD", "TRAOD.exe, v42")
{
	byte gameAction: 0x4506C8;
	string30 mapName: 0x49F3EA;
}

state("TRAOD_P3", "TRAOD_P3.exe, v42")
{
	byte gameAction: 0x37CDE4;
	string30 mapName: 0x42712A;
}

state("TRAOD_P4", "TRAOD_P4.exe, v42")
{
	byte gameAction: 0x38D724;
	string30 mapName: 0x437A6A;
}

state("TRAOD", "TRAOD.exe, v49")
{
	byte gameAction: 0x46D348;
	string30 mapName: 0x4BC06A;
}

state("TRAOD_P3", "TRAOD_P3.exe, v49")
{
	byte gameAction: 0x3A5104;
	string30 mapName: 0x44F44A;
}

state("TRAOD_P4", "TRAOD_P4.exe, v49")
{
	byte gameAction: 0x3B5A44;
	string30 mapName: 0x45FD8A;
}

state("TRAOD", "TRAOD.exe, v52")
{
	byte gameAction: 0x46E208;
	string30 mapName: 0x4BCF2A;
}

state("TRAOD_P3", "TRAOD_P3.exe, v52")
{
	byte gameAction: 0x3A5144;
	string30 mapName: 0x44F48A;
}

state("TRAOD_P4", "TRAOD_P4.exe, v52")
{
	byte gameAction: 0x3B6A84;
	string30 mapName: 0x460DCA;
}

// The exe files on the Russian Noviy Disk (ND) version have the "dat" extension.
// The addresses (the ones that we use in this script) in the ND version are identical to the ones in the international v52 edition.
state("TRAOD.dat", "TRAOD.exe, v52")
{
	byte gameAction: 0x46E208;
	string30 mapName: 0x4BCF2A;
}

state("TRAOD_P3.dat", "TRAOD_P3.exe, v52")
{
	byte gameAction: 0x3A5144;
	string30 mapName: 0x44F48A;
}

state("TRAOD_P4.dat", "TRAOD_P4.exe, v52")
{
	byte gameAction: 0x3B6A84;
	string30 mapName: 0x460DCA;
}

state("TRAOD", "TRAOD.exe, v52J")
{
	byte gameAction: 0x4A25FC;
	string30 mapName: 0x4F138A;
}

state("TRAOD_P3", "TRAOD_P3.exe, v52J")
{
	byte gameAction: 0x3E0B04;
	string30 mapName: 0x48AE0A;
}

state("TRAOD_P4", "TRAOD_P4.exe, v52J")
{
	byte gameAction: 0x3F1344;
	string30 mapName: 0x49B64A;
}

startup
{
	// Data for autosplitting (except for the final split point). If you want a new split point, just add a new tuple to this array.
	// The items in one tuple:
	// - Item1: name of the level to which the tuple belongs to. This is the string displayed in LiveSplit's settings.
	// - Item2: name of the setting, used only in this script to access the state of the setting (checked = true or unchecked = false).
	// - Item3: name of the map file which you leave when the split occurs.
	// - Item4: name of the map file you enter when the split happens.
	vars.levelInfo = new Tuple<string, string, string, string>[]
	{
		Tuple.Create("Parisian Back Streets", "backstreets", "PARIS1.GMX", "PARIS1A.GMX"),
		Tuple.Create("Derelict Apartment Block", "derelict", "PARIS1A.GMX", "PARIS1C.GMX"),
		Tuple.Create("Industrial Roof Tops", "industrial", "PARIS1C.GMX", "PARIS1B.GMX"),
		Tuple.Create("Margot Carvier's Apartment", "carvier", "PARIS1B.GMX", "PARIS2_1.GMX"),
		Tuple.Create("The Serpent Rouge", "serpent", "PARIS2B.GMX", "PARIS2_1.GMX"),
		Tuple.Create("St. Aicard's Graveyard", "aicard", "PARIS2G.GMX", "PARIS2H.GMX"),
		Tuple.Create("Bouchard's Hideout", "bouchard", "PARIS2H.GMX", "PARIS2E.GMX"),
		Tuple.Create("Rennes' Pawnshop", "rennes", "CUTSCENE\\CS_2_51A.GMX", "PARIS3.GMX"),
		Tuple.Create("Louvre Storm Drains", "stormdrains", "PARIS3.GMX", "PARIS4.GMX"),
		Tuple.Create("Louvre Galleries", "galleries", "PARIS4.GMX", "PARIS5A.GMX"),
		Tuple.Create("The Archaeological Dig", "dig", "PARIS5A.GMX", "PARIS5.GMX"),
		Tuple.Create("Tomb of Ancients", "ancients", "PARIS5.GMX", "PARIS5_1.GMX"),
		Tuple.Create("Neptune's Hall", "neptune", "PARIS5_2.GMX", "PARIS5_1.GMX"),
		Tuple.Create("Wrath of the Beast", "beast", "PARIS5_3.GMX", "PARIS5_1.GMX"),
		Tuple.Create("The Sanctuary of Flame", "flame", "PARIS5_4.GMX", "PARIS5_1.GMX"),
		Tuple.Create("The Breath of Hades", "hades", "PARIS5_5.GMX", "PARIS5_1.GMX"),
		Tuple.Create("The Hall of Seasons", "seasons", "PARIS5_1.GMX", "PARIS5.GMX"),
		Tuple.Create("Tomb of Ancients (flooded)", "ancients2", "PARIS5.GMX", "PARIS5A.GMX"),
		Tuple.Create("The Archaeological Dig", "dig2", "PARIS5A.GMX", "PARIS4A.GMX"),
		Tuple.Create("Galleries Under Siege", "siege", "CUTSCENE\\CS_6_21B.GMX", "PARIS6.GMX"),
		Tuple.Create("Von Croy's Apartment", "voncroy", "CUTSCENE\\CS_7_19.GMX", "PRAGUE1.GMX"),
		Tuple.Create("The Monstrum Crimescene", "crimescene", "CUTSCENE\\CS_9_1.GMX", "PRAGUE2.GMX"),
		Tuple.Create("The Strahov Fortress", "strahov", "PRAGUE2.GMX", "PRAGUE3.GMX"),
		Tuple.Create("The Bio-Research Facility", "biores", "CUTSCENE\\CS_10_14.GMX", "PRAGUE4.GMX"), 
		Tuple.Create("The Sanitarium", "sanitarium", "PRAGUE4.GMX", "PRAGUE4A.GMX"),
		Tuple.Create("Maximum Containment Area", "containment", "CUTSCENE\\CS_12_1.GMX", "PRAGUE3A.GMX"),
		Tuple.Create("Aquatic Research Area", "aqua", "PRAGUE3A.GMX", "PRAGUE5.GMX"),
		Tuple.Create("Vault of Trophies", "vault", "CUTSCENE\\CS_13_9.GMX", "PRAGUE5A.GMX"),
		Tuple.Create("Boaz Returns", "boaz", "CUTSCENE\\CS_14_6.GMX", "PRAGUE6A.GMX"),
		Tuple.Create("The Lost Domain", "domain", "PRAGUE6A.GMX", "PRAGUE6.GMX")
	};

	settings.Add("autosplit", true, "Split automatically at the end of:");
	foreach(var levelTuple in vars.levelInfo)
	{
		settings.Add(levelTuple.Item2, false, levelTuple.Item1, "autosplit");
	}
	settings.Add("eckhardt", true, "Eckhardt's Lab", "autosplit");
	settings.SetToolTip("boaz", "This is Vault of Trophies' end in the any% route.");
	settings.SetToolTip("dig", "Tick this if you're heading to Tomb of Ancients and you want a split point as The Archaeological Dig level ends.");
	settings.SetToolTip("dig2", "Tick this if you're heading to Galleries Under Siege and you want a split point as The Archaeological Dig level ends.");
	settings.SetToolTip("bouchard", "Tick this if you want a split point as you leave to St. Aicard's Church from Bouchard's Hideout.");

	// This array is for preventing splitting multiple times on the same split point.
	vars.hasSplit = new bool[vars.levelInfo.Length];

	// This variable is used to prevent splits during loads started from loading a save file.
	vars.newLevelLoading = new bool();
	
	// All the addresses required for the load removal code injection. The items in one tuple:
	// - Item1: the version to which the addresses belong in the tuple.
	// - Item2: the starting address of the function sysBeginLoadingScreen.
	// - Item3, 4: addresses containing the calls for sysBeginLoadingScreen which we'll replace with jumps to our code caves.
	// - Item5: the starting address of the function sysEndLoadingScreen.
	// - Item6, 7: addresses containing the calls for sysEndLoadingScreen which we want to replace.
	var injectionAddresses = new Tuple<string, int, int, int, int, int, int>[]
	{
		Tuple.Create("TRAOD.exe, v39", 0x4244C0, 0x500278, 0x5027D6, 0x424890, 0x5027F2, 0x5002AA),
		Tuple.Create("TRAOD_P3.exe, v39", 0x42B4EC, 0x529D34, 0x52C01E, 0x42B598, 0x529D69, 0x52C3E2),
		Tuple.Create("TRAOD_P4.exe, v39", 0x42C9B0, 0x52F1F7, 0x531522, 0x42CA5C, 0x52F22C, 0x531902),
		Tuple.Create("TRAOD.exe, v42", 0x4254C0, 0x501468, 0x5039C6, 0x425890, 0x50149A, 0x5039E2),
		Tuple.Create("TRAOD_P3.exe, v42", 0x42C84C, 0x52B100, 0x52D3EA, 0x42C8F8, 0x52B135, 0x52D7AE),
		Tuple.Create("TRAOD_P4.exe, v42", 0x42DB20, 0x5306FB, 0x532A2A, 0x42DBCC, 0x530730, 0x532E0A),
		Tuple.Create("TRAOD.exe, v49", 0x425630, 0x5029E8, 0x504F36, 0x425A00, 0x502A1A, 0x504F52),
		Tuple.Create("TRAOD_P3.exe, v49", 0x42CAC8, 0x52CB90, 0x52EE7A, 0x42CB74, 0x52CBC5, 0x52F20E),
		Tuple.Create("TRAOD_P4.exe, v49", 0x42E368, 0x5322B3, 0x5345E2, 0x42E414, 0x5322E8, 0x534992),
		Tuple.Create("TRAOD.exe, v52", 0x425510, 0x5026F8, 0x504C36, 0x4258E0, 0x50272A, 0x504C52),
		Tuple.Create("TRAOD_P3.exe, v52", 0x42C9F4, 0x52C98C, 0x52EC72, 0x42CAA0, 0x52C9C1, 0x52F002),
		Tuple.Create("TRAOD_P4.exe, v52", 0x42E1F8, 0x5320AF, 0x5343DA, 0x42E2A4, 0x5320E4, 0x534786),
		Tuple.Create("TRAOD.exe, v52J", 0x424DB0, 0x5009C7, 0x502CA6, 0x425180, 0x5009F9, 0x502CC2),
		Tuple.Create("TRAOD_P3.exe, v52J", 0x42BFA8, 0x52A03D, 0x52C046, 0x42C054, 0x52A072, 0x52C39E),
		Tuple.Create("TRAOD_P4.exe, v52J", 0x42D630, 0x52F8F8, 0x53191A, 0x42D6DC, 0x52F92D, 0x531C86)
	};
	
	// This dictionary contains the MD5 hashes for all AoD executables handled by this script file.
	var exeVersions = new Dictionary<string, string>
	{
		{"784419878ABE05BA02F43BB69DD9547C", "TRAOD.exe, v39"},
		{"912CC51033F2CD850EA22BDCA1177E1A", "TRAOD_P3.exe, v39"},
		{"ED2FFB1FE4A1E5623FE640A8BEBF3971", "TRAOD_P4.exe, v39"},
		{"1713F4F416897CC44D74A7EE15E27E45", "TRAOD.exe, v42"},
		{"23577E33938443DD4495F87285FBEA2B", "TRAOD_P3.exe, v42"},
		{"D010CC2342C98FE232604C5394A35F5E", "TRAOD_P4.exe, v42"},
		{"06E495E9912227B5B271AAC6E7BBA17C", "TRAOD.exe, v49"},
		{"37074F2BED441D6F51159BC5DDFC9439", "TRAOD_P3.exe, v49"},
		{"4DB4E66B38FCC674B7D92E99FAE71F00", "TRAOD_P3.exe, v49"},  // AMD fix.
		{"CB89EAA650A1464DFF4C160240ACF02D", "TRAOD_P4.exe, v49"},
		{"F8609E81CD0BF26BA170CE038E281195", "TRAOD.exe, v52"},
		{"4C784198862A840424E494861575CF70", "TRAOD_P3.exe, v52"},
		{"29B8ABE83BA984754EF0F0ED31F57DD4", "TRAOD_P4.exe, v52"},
		{"1DEE3F701AB8A3F1DF265F0A59D68A41", "TRAOD.exe, v52"},	    //
		{"965768EA109C414704A4E2ABA5A7E246", "TRAOD_P3.exe, v52"},  // Russian executables.
		{"07B47CCDC6AB45951D15AAD76987A498", "TRAOD_P4.exe, v52"},  // 
		{"A05001AC983D096AC439BCA3F9BAB362", "TRAOD.exe, v52J"},
		{"867E1741166604D696AFD7F5905F7E11", "TRAOD_P3.exe, v52J"},
		{"3FB88FBC678BD8EFEB2A6AD6F3AA2CEA", "TRAOD_P4.exe, v52J"}
	};

	// Rest of the startup block contains various function declarations.

	vars.DetermineVersion = (Func<Process, string>)(proc =>
	{
		string exePath = proc.MainModule.FileName; // Why not using MainModuleWow64Safe()? Explained at the bottom of this file.
		string hashInHex = "0";
		using (var md5 = System.Security.Cryptography.MD5.Create())
    		{
        		using (var stream = File.Open(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        		{
            			var hash = md5.ComputeHash(stream);
				hashInHex = BitConverter.ToString(hash).Replace("-", "");
        		}
    		}
		
		foreach(KeyValuePair<string, string> kvp in exeVersions)
		{
			if(kvp.Key == hashInHex)
			{
				return kvp.Value;
			}
		}
		
		return "Unrecognized";
	});

	vars.SetPointers = (Action<string>)(gameVer =>
	{
		foreach(var addressTuple in injectionAddresses)
		{
			if(gameVer == addressTuple.Item1)
			{
				vars.sysBeginLoadingScreen = new IntPtr(addressTuple.Item2);
				vars.sBLSCalls = new IntPtr[]{new IntPtr(addressTuple.Item3), new IntPtr(addressTuple.Item4)};
				vars.sysEndLoadingScreen = new IntPtr(addressTuple.Item5);
				vars.sELSCalls = new IntPtr[]{new IntPtr(addressTuple.Item6), new IntPtr(addressTuple.Item7)};
			}
		}
	});

	vars.CreateBLSDetourBytes = (Func<byte[], byte[]>)(loading =>
	{
		var sBLSCallDetour = new List<byte>(){0xC7, 0x05}; // MOV opcode.
		sBLSCallDetour.AddRange(loading);
		sBLSCallDetour.AddRange(new byte[]{1, 0, 0, 0});
		sBLSCallDetour.AddRange(new byte[]{0xFF, 0xFF, 0xFF, 0xFF, 0xFF}); // CALL placeholder.
		sBLSCallDetour.AddRange(new byte[]{0xFF, 0xFF, 0xFF, 0xFF, 0xFF}); // JMP placeholder.
		return sBLSCallDetour.ToArray();
	});

	vars.CreateELSDetourBytes = (Func<byte[], byte[]>)(loading =>
	{
		var sELSCallDetour = new List<byte>(){0xFF, 0xFF, 0xFF, 0xFF, 0xFF}; // CALL placeholder.
		sELSCallDetour.AddRange(new byte[]{0xC7, 0x05});
		sELSCallDetour.AddRange(loading);
		sELSCallDetour.AddRange(new byte[]{0, 0, 0, 0});
		sELSCallDetour.AddRange(new byte[]{0xFF, 0xFF, 0xFF, 0xFF, 0xFF}); // JMP placeholder.
		return sELSCallDetour.ToArray();
	});

	vars.InstallLoadRemovalHooks = (Action<Process>)(proc =>
	{
		proc.Suspend();

		vars.sBLSDetFuncPtrs = new IntPtr[2];
		vars.sELSDetFuncPtrs = new IntPtr[2];
		vars.loadingPtr = proc.AllocateMemory(sizeof(int));
		vars.loadingPtrBytes = BitConverter.GetBytes((uint)vars.loadingPtr);
		byte[] sBLSDetBy = vars.CreateBLSDetourBytes(vars.loadingPtrBytes);
		byte[] sELSDetBy = vars.CreateELSDetourBytes(vars.loadingPtrBytes);

		// There is one sysBeginLoadingScreen-sysEndLoadingScreen call pair for cutscene level loads and one for normal level loads.
		for(int i = 0; i < 2; i++)
		{
			vars.sBLSDetFuncPtrs[i] = proc.AllocateMemory(sBLSDetBy.Length); // Deallocated in shutdown.
			proc.WriteBytes((IntPtr)vars.sBLSDetFuncPtrs[i], sBLSDetBy);
			proc.WriteCallInstruction(IntPtr.Add((IntPtr)vars.sBLSDetFuncPtrs[i], 10), (IntPtr)vars.sysBeginLoadingScreen); // Writing the CALL to the CALL placeholder.
			proc.WriteJumpInstruction(IntPtr.Add((IntPtr)vars.sBLSDetFuncPtrs[i], 15), IntPtr.Add((IntPtr)vars.sBLSCalls[i], 5)); // Replacing the JMP placeholder.
			proc.WriteJumpInstruction((IntPtr)vars.sBLSCalls[i], (IntPtr)vars.sBLSDetFuncPtrs[i]);
			
			vars.sELSDetFuncPtrs[i] = proc.AllocateMemory(sELSDetBy.Length);
			proc.WriteBytes((IntPtr)vars.sELSDetFuncPtrs[i], sELSDetBy);
			proc.WriteCallInstruction((IntPtr)vars.sELSDetFuncPtrs[i], (IntPtr)vars.sysEndLoadingScreen);
			proc.WriteJumpInstruction(IntPtr.Add((IntPtr)vars.sELSDetFuncPtrs[i], 15), IntPtr.Add((IntPtr)vars.sELSCalls[i], 5));
			proc.WriteJumpInstruction((IntPtr)vars.sELSCalls[i], (IntPtr)vars.sELSDetFuncPtrs[i]);
		}

		proc.Resume();
	});
}

init
{
	version = vars.DetermineVersion(game);
		
	// Version is unrecognized = we don't know where the functions are. So we do nothing in that case.
	if(version != "Unrecognized")
	{
		vars.SetPointers(version);
		vars.InstallLoadRemovalHooks(game);
	}
}

update
{
	if(version == "Unrecognized")
	{
		return false;
	}

	vars.isLoading = game.ReadValue<bool>((IntPtr)vars.loadingPtr);

	if(old.gameAction != 3 && current.gameAction == 3)
	{
		vars.newLevelLoading = true;
	}
	
	if(old.gameAction == 3 && current.gameAction != 3)
	{
		vars.newLevelLoading = false;
	}
}

isLoading
{
	return vars.isLoading;
}

start
{
	for(int i = 0; i < vars.hasSplit.Length; i++)
	{
		vars.hasSplit[i] = false;
	}
	
	// Documentation of the magic constants can be found at the bottom of this script file.
	return (old.gameAction == 1 && current.gameAction == 0);
}

reset
{
	return (old.gameAction == 0 && current.gameAction == 1);
}

split
{
	if(vars.newLevelLoading)
	{
		for(int i = 0; i < vars.levelInfo.Length; i++)
		{
			if(settings[vars.levelInfo[i].Item2] == true && old.mapName == vars.levelInfo[i].Item3 && current.mapName == vars.levelInfo[i].Item4 && vars.hasSplit[i] == false)
			{
				vars.hasSplit[i] = true;
				return true;
			}
		}
	}
	
	// Final split (doesn't occur when vars.newLevelLoading == 1 so has to be handled separately).
	return (settings["eckhardt"] == true && old.mapName == "PRAGUE6.GMX" && current.mapName == "FRONTEND.GMX"); 
}

shutdown
{
	if(version != "Unrecognized" && game != null)
	{
		game.Suspend();

		foreach(IntPtr calls in vars.sBLSCalls)
		{
			game.WriteCallInstruction(calls, (IntPtr)vars.sysBeginLoadingScreen);
		}

		foreach(IntPtr calls in vars.sELSCalls)
		{
			game.WriteCallInstruction(calls, (IntPtr)vars.sysEndLoadingScreen);
		}

		foreach(var allocPtr in vars.sBLSDetFuncPtrs)
		{
			game.FreeMemory((IntPtr)allocPtr);
		}

		foreach(var allocPtr in vars.sELSDetFuncPtrs)
		{
			game.FreeMemory((IntPtr)allocPtr);
		}

		game.FreeMemory((IntPtr)vars.loadingPtr);

		game.Resume();
	}
}

// gameAction (the name comes from the symbol files) values:
	// 0: In every menu (main, paused, inventory, exit game etc.), in-game, during conversations, during cutscenes, during the intro FMVs before the menu, and during the Karel reveal FMV.
	// 1: After pressing New Game, during the FMV and during the loading screen following the FMV.
	// 2: During savegame loads.
	// 3: During the FMV at the end of Siege, during loads which are after a level's EoL trigger, and during the fadeouts after activating an EoL trigger with Lara or Kurtis.
	// 4: During exiting to the main menu.
	// 5: During loads after cutscene levels.
	// 6: Cutscene level loads and during the fadeouts before cutscenes.
	// 7: During the fadeout leading up to conversations.
	// 8: Just before the inventory shows up.
	// 9: Unknown (searching for MOV [gameAction], 9's bytecode in CE shows no results).
	// 10: Just before the Paused menu shows up.
	// 11: Just before the Game Over menu shows up.
	// 12: Just before the inventory screen shows up when you sell items to Rennes.
	
// MainModuleWow64Safe() - https://github.com/LiveSplit/LiveSplit/blob/master/LiveSplit/LiveSplit.Core/ComponentUtil/ProcessExtensions.cs#L46
// - It occasionally gets ntdll as the main module.
// - ModulesWow64Safe() sometimes throw exceptions (invalid handle, Read/WriteProcessMemory fail) when it tries to do things with the 32-bit dll modules.
// - The exceptions only get thrown if you're on a 64-bit Windows.
// - ModulesWow64Safe() is called by every ASL script so that's why you sometimes see LiveSplit errors in the Event Viewer about inv. handle or RPM/WPM fail, even if the script contains nothing.
// - Process.Modules only enumerate the executable module and the 64-bit dlls while ModulesWow64Safe() enumerate both 64 and 32-bit modules and the exe. So the former is enough for me, and it doesn't spam the Event Viewer with error messages. 
