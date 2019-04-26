state("TRAOD_P3", "P3.exe, v49")
{
	byte gameAction: 0x3A5104;
	string30 mapName: 0x44F44A;
}

state("TRAOD_P3", "P3.exe, v52J")
{
	byte gameAction: 0x3E0B04;
	string30 mapName: 0x48AE0A;
}

state("TRAOD_P4", "P4.exe, v52J")
{
	byte gameAction: 0x3F1344;
	string30 mapName: 0x49B64A;
}

state("TRAOD", "Unsupported")
{
	// This is only here so LiveSplit can show a message to those who launched the game with an unsupported exe.
}

start
{
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
		vars.doSplit = false;
		
		for(int i = 0; i < vars.levelInfo.Length; i++)
		{
			if(settings[vars.levelInfo[i].Item2] == true && old.mapName == vars.levelInfo[i].Item3 && current.mapName == vars.levelInfo[i].Item4 && vars.hasSplit[i] == false)
			{
				vars.doSplit = true;
				vars.hasSplit[i] = true;
			}
		}

		return vars.doSplit;
	}
}

startup
{
	// This is for writing TC instead of Tuple.Create.
	Func<string, string, string, string, Tuple<string, string, string, string>> TC = Tuple.Create;

	// Data for autosplitting. If you want a new split point, just add a new tuple to this array.
	// Each tuple is built the following way: TC(<levelname shown in LS' settings>, <settings name>, <file name of the map you leave>, <file name of the map you enter>).
	// The array is in vars so it can be reached outside startup's scope.
	vars.levelInfo = new Tuple<string, string, string, string>[]
	{
		TC("Parisian Back Streets", "backstreets", "PARIS1.GMX", "PARIS1A.GMX"),
		TC("Derelict Apartment Block", "derelict", "PARIS1A.GMX", "PARIS1C.GMX"),
		TC("Industrial Roof Tops", "industrial", "PARIS1C.GMX", "PARIS1B.GMX"),
		TC("Margot Carvier's Apartment", "carvier", "PARIS1B.GMX", "PARIS2_1.GMX"),
		TC("The Serpent Rouge", "serpent", "PARIS2B.GMX", "PARIS2_1.GMX"),
		TC("St. Aicard's Graveyard", "aicard", "PARIS2G.GMX", "PARIS2H.GMX"),
		TC("Bouchard's Hideout", "bouchard", "PARIS2H.GMX", "PARIS2E.GMX"),
		TC("Rennes' Pawnshop", "rennes", "CUTSCENE\\CS_2_51A.GMX", "PARIS3.GMX"),
		TC("Louvre Storm Drains", "stormdrains", "PARIS3.GMX", "PARIS4.GMX"),
		TC("Louvre Galleries", "galleries", "PARIS4.GMX", "PARIS5A.GMX"),
		TC("The Archaeological Dig", "dig", "PARIS5A.GMX", "PARIS5.GMX"),
		TC("Tomb of Ancients", "ancients", "PARIS5.GMX", "PARIS5_1.GMX"),
		TC("Neptune's Hall", "neptune", "PARIS5_2.GMX", "PARIS5_1.GMX"),
		TC("Wrath of the Beast", "beast", "PARIS5_3.GMX", "PARIS5_1.GMX"),
		TC("The Sanctuary of Flame", "flame", "PARIS5_4.GMX", "PARIS5_1.GMX"),
		TC("The Breath of Hades", "hades", "PARIS5_5.GMX", "PARIS5_1.GMX"),
		TC("The Hall of Seasons", "seasons", "PARIS5_1.GMX", "PARIS5.GMX"),
		TC("Tomb of Ancients (flooded)", "ancients2", "PARIS5.GMX", "PARIS5A.GMX"),
		TC("The Archaeological Dig", "dig2", "PARIS5A.GMX", "PARIS4A.GMX"),
		TC("Galleries Under Siege", "siege", "CUTSCENE\\CS_6_21B.GMX", "PARIS6.GMX"),
		TC("Von Croy's Apartment", "voncroy", "CUTSCENE\\CS_7_19.GMX", "PRAGUE1.GMX"),
		TC("The Monstrum Crimescene", "crimescene", "CUTSCENE\\CS_9_1.GMX", "PRAGUE2.GMX"),
		TC("The Strahov Fortress", "strahov", "PRAGUE2.GMX", "PRAGUE3.GMX"),
		TC("The Bio-Research Facility", "biores", "CUTSCENE\\CS_10_14.GMX", "PRAGUE4.GMX"), 
		TC("The Sanitarium", "sanitarium", "PRAGUE4.GMX", "PRAGUE4A.GMX"),
		TC("Maximum Containment Area", "containment", "CUTSCENE\\CS_12_1.GMX", "PRAGUE3A.GMX"),
		TC("Aquatic Research Area", "aqua", "PRAGUE3A.GMX", "PRAGUE5.GMX"),
		TC("Vault of Trophies", "vault", "CUTSCENE\\CS_13_9.GMX", "PRAGUE5A.GMX"),
		TC("Boaz Returns", "boaz", "CUTSCENE\\CS_14_6.GMX", "PRAGUE6A.GMX"),
		TC("The Lost Domain", "domain", "PRAGUE6A.GMX", "PRAGUE6.GMX"),
		TC("Eckhardt's Lab", "eckhardt", "PRAGUE6.GMX", "FRONTEND.GMX")
	};

	settings.Add("autosplit", true, "Split automatically at the end of:");
	foreach(var levelTuple in vars.levelInfo)
	{
		settings.Add(levelTuple.Item2, false, levelTuple.Item1, "autosplit");
	}
	settings.SetToolTip("boaz", "This is Vault of Trophies' end in the any% route.");
	settings.SetToolTip("dig", "Tick this if you're heading to Tomb of Ancients and you want a split point as The Archaeological Dig level ends.");
	settings.SetToolTip("dig2", "Tick this if you're heading to Galleries Under Siege and you want a split point as The Archaeological Dig level ends.");
	settings.SetToolTip("bouchard", "Tick this if you want a split point as you leave to St. Aicard's Church from Bouchard's Hideout.");
	settings.Add("info", true, "Show error message if launching an unsupported executable.");
	settings.SetToolTip("info", "Uncheck this if you don't want the message box with the \"Unsupported executable!...\" text to show up again.");

	// This array is for preventing splitting multiple times on the same split point.
	vars.hasSplit = new bool[vars.levelInfo.Length];

	// This variable is used to prevent splits outside non-savegame loading screens.
	vars.newLevelLoading = new bool();

	vars.DetermineVersion = (Func<Process, string>)(proc =>
	{
		string exePath = proc.MainModuleWow64Safe().FileName;
		string hashInHex;
		using (var sha256 = System.Security.Cryptography.SHA256.Create())
    		{
        		using (var stream = File.Open(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        		{
            			var hash = sha256.ComputeHash(stream);
				hashInHex = BitConverter.ToString(hash).Replace("-", "");
        		}
    		}
		switch(hashInHex)
		{
			case "E8F9A7FE42058DE8D4F10672EBA5DBFA3C34EDB2D1F1BA12ADB93321C8F2A7E0":
				return "P3.exe, v49";
			case "24E557D61536C486208C072B45DE09E8463F93AFBA6B74BF3EA0DE9A3C4FE68C":
				return "P3.exe, v49"; // AMD fix
			case "AD691745992E4A646AF9C58495828466D7BCC42D087B6A89109EB8B0E09BAD1F":
				return "P3.exe, v52J";
			case "6CAD85F5C287762CCC5F355AE86BD644AD5423B30CEC262538CF69FDFFE499A9":
				return "P4.exe, v52J";
			default:	
				return "Unsupported";
		}
	});
	
	vars.ShowInfoMessage = (Action)(() =>
	{
		var infMsg = new Form
		{
			Text = "LiveSplit script for Angel of Darkness",
			FormBorderStyle = FormBorderStyle.FixedDialog,
			Size = new System.Drawing.Size(435, 164),
			MaximizeBox = false,
			MinimizeBox = false,
			StartPosition = FormStartPosition.CenterScreen,
			TopMost = true,
			BackColor = System.Drawing.Color.FromName("White")
		};
		var ok = new Button
		{
			Text = "OK",
			DialogResult = DialogResult.OK,
			BackColor = System.Drawing.Color.FromArgb(0xE1E1E1)
		};
		ok.Location = new System.Drawing.Point((infMsg.ClientSize.Width-ok.Width)/2, 3*infMsg.ClientSize.Height/4);
		infMsg.AcceptButton = ok;
		// Splitting up the two sentences due to aesthetic purposes.
		var snt1P1 = new Label
		{
			Text = "Unsupported executable! Please use TRAOD_P3.exe (v49, or the Japanese version), ",
			AutoSize = false,
			Bounds = new System.Drawing.Rectangle(5, 6, 420, 13)
		};
		var snt1P2 = new Label
		{
			Text = "or use TRAOD_P4.exe from the Japanese version.",
			AutoSize = false,
			Bounds = new System.Drawing.Rectangle(6, 24, 420, 13)
		};
		var snt2 = new Label
		{
			Text = "For more information, visit ",
			Bounds = new System.Drawing.Rectangle(6, 60, 126, 12)
		};
		var snt2Link = new LinkLabel
		{
			Text = " speedrun.com: AoD Autosplitter Setup Guide.",
			LinkArea = new LinkArea(1, 43),
			Bounds = new System.Drawing.Rectangle(126, 60, 300, 12)
		};
		snt2Link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler((sender, e) =>
		{
			System.Diagnostics.Process.Start("https://www.speedrun.com/traod/guide/vvcm2");
		});
		infMsg.Controls.AddRange(new Control[]{ok, snt1P1, snt1P2, snt2, snt2Link});
		infMsg.ShowDialog();
		if(ok.DialogResult == DialogResult.OK)
		{
			infMsg.Close();
		}
	});

	vars.SetPointers = (Action<string>)(gameVer =>
	{
		if(gameVer == "P3.exe, v49")
		{
			vars.sBLSCalls = new IntPtr[]{new IntPtr(0x52EE7A), new IntPtr(0x52CB90)};
			vars.sysBeginLoadingScreen = new IntPtr(0x42CAC8);
			vars.sELSCalls = new IntPtr[]{new IntPtr(0x52F20E), new IntPtr(0x52CBC5)};
			vars.sysEndLoadingScreen = new IntPtr(0x42CB74);
		}
		else if(gameVer == "P3.exe, v52J")
		{
			vars.sBLSCalls = new IntPtr[]{new IntPtr(0x52A030), new IntPtr(0x52C046)};
			vars.sysBeginLoadingScreen = new IntPtr(0x42BFA8);
			vars.sELSCalls = new IntPtr[]{new IntPtr(0x52A072), new IntPtr(0x52C39E)};
			vars.sysEndLoadingScreen = new IntPtr(0x42C054);
		}
		else if(gameVer == "P4.exe, v52J")
		{
			vars.sBLSCalls = new IntPtr[]{new IntPtr(0x53191A), new IntPtr(0x52F8F8)};
			vars.sysBeginLoadingScreen = new IntPtr(0x42D630);
			vars.sELSCalls = new IntPtr[]{new IntPtr(0x52F92D), new IntPtr(0x531C86)};
			vars.sysEndLoadingScreen = new IntPtr(0x42D6DC);
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

	vars.InstallLoadRemovalHooks = (Action<Process, IntPtr[], IntPtr[], IntPtr, IntPtr, byte[]>)((proc, beginLoadCalls, endLoadCalls, sBLS, sELS, loading) =>
	{
		proc.Suspend();

		vars.sBLSDetFuncPtrs = new IntPtr[2];
		vars.sELSDetFuncPtrs = new IntPtr[2];
		byte[] sBLSDetBy = vars.CreateBLSDetourBytes(loading);
		byte[] sELSDetBy = vars.CreateELSDetourBytes(loading);

		// There is one sysBeginLoadingScreen-sysEndLoadingScreen call pair for cutscene level loads and one for normal level loads.
		for(int i = 0; i < 2; i++)
		{
			vars.sBLSDetFuncPtrs[i] = proc.AllocateMemory(sBLSDetBy.Length);
			proc.WriteBytes((IntPtr)vars.sBLSDetFuncPtrs[i], sBLSDetBy);
			proc.WriteCallInstruction(IntPtr.Add((IntPtr)vars.sBLSDetFuncPtrs[i], 10), sBLS); // Writing the CALL to the CALL placeholder.
			proc.WriteJumpInstruction(IntPtr.Add((IntPtr)vars.sBLSDetFuncPtrs[i], 15), IntPtr.Add(beginLoadCalls[i], 5)); // Replacing the JMP placeholder.
			proc.WriteJumpInstruction(beginLoadCalls[i], (IntPtr)vars.sBLSDetFuncPtrs[i]);
			
			vars.sELSDetFuncPtrs[i] = proc.AllocateMemory(sELSDetBy.Length);
			proc.WriteBytes((IntPtr)vars.sELSDetFuncPtrs[i], sELSDetBy);
			proc.WriteCallInstruction((IntPtr)vars.sELSDetFuncPtrs[i], sELS);
			proc.WriteJumpInstruction(IntPtr.Add((IntPtr)vars.sELSDetFuncPtrs[i], 15), IntPtr.Add(endLoadCalls[i], 5));
			proc.WriteJumpInstruction(endLoadCalls[i], (IntPtr)vars.sELSDetFuncPtrs[i]);
		}

		proc.Resume();
	});
}

init
{
	version = vars.DetermineVersion(game);

	// Version is unsupported = we don't know where the functions are. So we leave the exe alone in that case.
	if(version == "Unsupported")
	{
		if(settings["info"])
		{
			vars.ShowInfoMessage();
		}
	}
	else
	{
		Thread.Sleep(1000); // Waiting a bit so the game can boot.
		vars.SetPointers(version);		
		vars.loadingPtr = game.AllocateMemory(sizeof(int));
		vars.loadingPtrBytes = BitConverter.GetBytes((uint)vars.loadingPtr);
		vars.InstallLoadRemovalHooks(game, vars.sBLSCalls, vars.sELSCalls, vars.sysBeginLoadingScreen, vars.sysEndLoadingScreen, vars.loadingPtrBytes);
	}
}

update
{
	if(version == "Unsupported")
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
	
	// We want to initialize values whenever the timer stopped, both at auto and non-auto resets.
	current.timerPhase = timer.CurrentPhase;
	
	if(old.timerPhase == TimerPhase.Running && current.timerPhase == TimerPhase.NotRunning)
	{
		for(int i = 0; i < vars.hasSplit.Length; i++)
		{
			vars.hasSplit[i] = false;
		}
	}
}

isLoading
{
	return vars.isLoading;
}

shutdown
{
	// Restoring game code and freeing all allocated memory.
	if(version != "Unsupported" && game != null)
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

// gameAction values:
	// 0: Main menu, in-game (while you can move around), during conversations, during cutscenes, during the intro FMVs before the menu, and during the Karel reveal FMV.
	// 1: After pressing New Game, during the FMV and during the loading screen following the FMV.
	// 2: During savegame loads.
	// 3: During the FMV at the end of Siege and during loads which are after a level's EoL trigger.
	// 4: During exiting to the main menu.
	// 5: During loads after cutscene levels.
	// 6: Cutscene level loads and during the fadeouts before cutscenes.
	// 7: During the fadeout leading up to conversations.
	// 8: Just before the inventory shows up.
	// 9: Unknown (searching for MOV [gameAction], 9's bytecode in CE shows no results).
	// 10: Just before the Paused menu shows up.
	// 11: Just before the Game Over menu shows up.
	// 12: Just before the inventory screen shows up when you sell items to Rennes.
