// Tomb Raider I auto splitter for LiveSplit created by lowosos
// with contributions from RiiFT, Midge, and rtrger.

state("tombati", "ATI")
{
    // 1 while an end-level stats screen is active. Remains 1
    // through FMVs that immediately follow a stats screen;
    // in other words, until the next level starts.
    // In some cases, fluctuates between 1 and 0 during FMVs and
    // cutscenes. These cases are documented in the Split block.
    bool stats:    0x5A014;

    // Acts as a boolean when in the immediate time of an
    // in-game cutscene, but cannot be fully interpreted as a
    // boolean due to some cases of "random" values at other points
    // which would be interpreted as "true" despite no cutscene running.
    // Note for tombati, it apparently behaves the opposite of the
    // DOSBOX `cutscene` variable.
    uint cutscene: 0x56688;

    // Matches the chronological number of the current level,
    // but also matches the file number for in-game cutscenes and FMVs.
    // 0 through 15: matches the active level
    // 16: From start of Qualopec cutscene until next level start.
    // 17: From start of Tihocan cutscene until next level start.
    // 18: After Natla's Mines stats screen until next level start.
    // 19: Atlantis cutscene after the FMV until next level start.
    // 20: While on title screen and through opening FMV.
    uint level:    0x53C4C;

    // Normally at 0.
    // Changes to 1 when using the `New Game` page OR saving;
    // remains 1 in such cases until the inventory is opened.
    // Changes to 2 when using `Exit to Title` OR `Exit Game`.
    // returns to 0 upon reaching the title screen or the game closes.
    // The name comes from its variable name in known game source code;
    // pointed out by rtrger.
    uint GF_StartGame: 0x5A080;
}

state("dosbox", "DOSBox")
{
    bool stats:           0xA786B4, 0x243D3C;
    uint cutscene:        0xA786B4, 0x1623A4;
    uint level:           0xA786B4, 0x243D38;
    uint GF_StartGame:    0xA786B4, 0x245C04;
}

// Account for cases where people have followed ATI patch guides which
// renamed the patched EXE to `dosbox.exe` to keep Steam compatibility.
state("dosbox", "ATI")
{
    bool stats:        0x5A014;
    uint cutscene:     0x56688;
    uint level:        0x53C4C;
    uint GF_StartGame: 0x5A080;
}

startup
{
    // No need for specific FG rulesets/categories because this
    // splitter (if coded properly) will work with any category
    // including Any%, Secrets, and 100% (with known speedrunning tech.
    settings.Add("FG", true, "Full Game");
    settings.SetToolTip("FG", "A full game run, as opposed to an IL (Individual Level) run.");
    settings.Add("IL", false, "Individual Level - RTA");
    settings.SetToolTip("IL", "RTA (Real-Time Attack) ILs; do not use this for IGT (In-Game Time) ILs nor full-game runs.");

    // Track the furthest level split on.
    vars.FG_farthest_level = 1;
}

init
{
    vars.timer = new Stopwatch();
    if (modules.First().ModuleMemorySize == 3092480)
        version = "ATI";
    else if (modules.First().ModuleMemorySize == 40321024)
        version = "DOSBox";
}

update
{
    // Check if both the Steam workshop launcher (uses the name Dosbox)
    // and the ATI game it opens are both open. ASL scripts hook onto
    // the first of any valid process name they find, so the launcher
    // is problematic; although it minimizes upon launching
    // the game, its process remains in the background.
    if (Process.GetProcessesByName("dosbox").Length != 0 &&
        Process.GetProcessesByName("tombati").Length != 0)
    {
        // Take action only if the "dosbox" process is the launcher.
        if (version != "DOSBox" && version != "ATI")
            vars.timer.Start();
    }	
    
    if (vars.timer.ElapsedMilliseconds > 2500) {		
        game.Kill();  // TODO: See if we can reassign `game` instead.
        vars.timer.Reset();
    }
    
    if (settings["FG"] && settings["IL"])
        return false;
}

start
{
    vars.FG_farthest_level = 1;

    // We check for the old level because it is feasible that you save
    // while in Caves, triggering a false positive due to how the
    // GF_StartGame variable works (documented above).
    // The current level check is needed so that the script doesn't
    // prematurely start the timer; i.e., only starts once in Caves.
    bool can_start_new_game = old.level == 0 || old.level == 20;
    bool new_game_was_started = can_start_new_game && current.level == 1 && current.GF_StartGame == 1;
    if (new_game_was_started)
        return true;

    // This logic is similar to the code block in the `Split` action.
    // If there is a way to describe a function with access to all the
    // variables found in state, it could be used to remove many duplicate.
    // operations.
    if (settings["IL"])
    {
        // The last level is 15.
        if (current.level >= 16)
            return false;
            
        // Check if the level loaded follows a cutscene.
        if (old.level >= 16)
        {
            return current.level == 5 || current.level == 10 ||
                   current.level == 14 || current.level == 15;
        }
        // If not on the Qualopec or Tihocan levels, inspect the normal case.
        else if (current.level != 4 && current.level != 9)
        {
            return !current.stats && old.stats;
        }
    }
}

reset
{
    // It is possible to use FG_farthest_level to reset if loading
    // to a level beyond their current level, but this isn't the best
    // to do because maybe a runner dies and accidentally loads a
    // later level from a different save file from what they meant
    // to load.
    if (current.GF_StartGame == 2)
    {
        vars.FG_farthest_level = 1;
        return true;
    }
}

split
{
    if (settings["FG"])
    {
        // Say a player completes a level, but then quickly fails and reloads
        // into the previous level. This prevents a resplit at an already-complete level.
        if (current.level < vars.FG_farthest_level)
        {
            return false;
        }
        // Following checks are needed because the functionality of not
        // resplitting on a level is complicated by the fact that
        // the level values switch during cutscenes and FMVs.
        else if (vars.FG_farthest_level == 15)
        {
            if (current.level >= 16)
                return false;
        }
        else if (vars.FG_farthest_level > 9)
        {
            if (current.level >= 16 && current.level < 19)
                return false;
        }
        else if (vars.FG_farthest_level > 4)
        {
            if (current.level == 16)
                return false;
        }
    }

    // This is similar to the code block for IL in the `Start` action.
    bool should_split = false;
    // 16 is the cutscene number occurring at the end of level 4 (Qualopec).
    // 17 is the cutscene number occurring at the end of level 9 (Tihocan).
    if ((current.level == 16 && old.level == 4) ||
        (current.level == 17 && old.level == 9))
    {
        if (version == "ATI")
            should_split = current.cutscene == 0 && current.stats;
        // DOSBOX
        should_split = current.cutscene == 1 && current.stats;
    }

    // 18 is the cutscene after the stats screen in Natla's Mines;
    // we always return false, because at some points during said scene,
    // the stats value fluctuates and would return false positives.
    // Level 14 is Atlantis, and the stats screen does not occur until the
    // end of 19, the cutscene after the FMV at the end of Atlantis.
    // So we ignore the fluctuating stats value while level 14 is active.
    else if (current.level == 18 || current.level == 14)
    {
        should_split = false;
    }

    // If not on the Qualopec or Tihocan levels, inspect the normal case.
    else if (current.level != 4 && current.level != 9)
    {
        should_split = current.stats && !old.stats;
    }


    if (should_split && settings["FG"])
    {
        vars.FG_farthest_level++;
        return true;
    }

    // For ILs and returning false for FG.
    return should_split;
}
