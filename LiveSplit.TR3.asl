state("tomb3", "INT")
{
    // true during cutscene, false otherwise
    bool isCutscene: 0x233F54;

    // true while stats screen is showing, false otherwise
    bool isStatsScreen: 0x2266AC;

    // true on the title screen, false otherwise
    bool isTitle: 0x2A1C58;

    // 00: Manor
    // India
    // 01: Jungle
    // 02: Temple Ruins
    // 03: The River Ganges
    // 04: Caves of Kaliya
    // South Pacific
    // 05: Coastal Village
    // 06: Crash Site
    // 07: Madubu Gorge
    // 08: Temple of Puna
    // London
    // 09: Thames Wharf
    // 10: Aldwych
    // 11: Lud's Gate
    // 12: City
    // Nevada
    // 13: Nevada Desert
    // 14: High Security Compound
    // 15: Area 51
    // Antarctica
    // 16: Antarctica
    // 17: RX-Tech Mines
    // 18: Lost City of Tinnos
    // 19: Meteorite Cavern
    // Bonus (All Secrets)
    // 20: All Hallows
    uint level: 0xC561C;

    // 0 During play.
    // Flicks to 1 and back at the start of in-game cutscenes.
    // Goes to 1 if an FMV plays or an end-level stats screen opens,
    // going back to 0 at the next level's start.
    bool levelComplete: 0x233F54;

    // Current level time as measured by in-game "ticks" (30 / second)
    uint currentLevelTime: 0x2D27CF;

    // 0 if Load Game was picked.
    // Changes to 1 when choosing New Game or Save Game.
    // The value stays 1 until the inventory is reopened.
    // The value is always 2 when using the Exit To Title or Exit Game pages.
    // Anywhere else the value is 0.
    uint pickedPassportFunction: 0x226458;
}

state("tomb3", "JP")
{
    int isCutscene:              0x233F44;
    bool isStatsScreen:          0x2266AC;
    bool isTitle:                0x2A1C60;
    uint level:                  0xC561C;
    bool levelComplete:          0x233f5C;
    uint currentLevelTime:       0x2D27CF;
    uint pickedPassportFunction: 0x226458;
}

startup
{
    // No need for specific FG rulesets/categories because this
    // splitter (if coded properly) will work with any category
    // including Any%, Secrets, and 100/Max% (with known speedrunning tech).
    settings.Add("FG", true, "Full Game");
    settings.SetToolTip("FG", "A full game run, as opposed to an IL (Individual Level) run.");
    settings.Add("IL", false, "Individual Level - RTA");
    settings.SetToolTip("IL", "RTA (Real-Time Attack) ILs; do not use this for IGT (In-Game Time) ILs nor full-game runs.");

    // TR3 has addresses that store completed levels' times; however, these do not get
    // zeroed out if an NG+ game is started. So in order to make the gameTime block
    // compatible with NG+, the times must be tracked manually.
    vars.levelCount = 19;  // This counts the first level as 0 and includes the bonus level.
    vars.completedLevels = new List<uint>(vars.levelCount + 1);
    vars.completedLevelTimes = new List<uint>(vars.levelCount + 1);
}

init
{
    // See: https://github.com/rtrger/Components/blob/master/TombRaiderTheAngelofDarkness.asl#L223
    // for hash code inspiration.
    var versionHashes = new Dictionary<string, string>
    {
        {"4044dc2c58f02bfea2572e80dd8f2abb", "INT"},
        {"66404f58bb5dbf30707abfd245692cd2", "JP"}
    };

    string exePath = game.MainModule.FileName;
    string hashInHex = "0";
    using (var md5 = System.Security.Cryptography.MD5.Create())
    {
        using (var stream = File.Open(exePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            var hash = md5.ComputeHash(stream);
            hashInHex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

    foreach (KeyValuePair<string, string> kvp in versionHashes)
    {
        if (kvp.Key == hashInHex)
        {
            version = kvp.Value;
            return;
        }
    }
    version = "Unrecognized";
}

start
{
    bool newGameStarted = current.level <= 20 && current.currentLevelTime == 0 && old.isTitle;
    if (newGameStarted)
    {
        vars.completedLevels.Clear();
        vars.completedLevelTimes.Clear();
        return true;
    }

    if (settings["FG"])
    {
        return false;
    }
    else  // IL-specific logic
    {
        bool goingToAnotherLevel = old.isStatsScreen && !current.isStatsScreen && current.currentLevelTime == 0;
        return goingToAnotherLevel;
    }
}

reset
{
    return current.pickedPassportFunction == 2;
}

split
{
    // Determine if a split should occur.
    bool shouldSplit;
    if (current.level == 4 || current.level == 19)
        shouldSplit = current.levelComplete && !old.levelComplete;
    else
        shouldSplit = current.levelComplete && current.isStatsScreen && !old.isStatsScreen;

    // Handle completed level and time arrays for full-game runs.
    if (shouldSplit && settings["FG"])
    {
        var index = vars.completedLevels.IndexOf(current.level);
        if (index != -1)
        {
            // If the level was previously completed, overwrite the existing time.
            vars.completedLevelTimes[index] = current.currentLevelTime;
        }
        else
        {
            // Otherwise, append the level and time to their respective arrays.
            vars.completedLevels.Add(current.level);
            vars.completedLevelTimes.Add(current.currentLevelTime);
        }
    }

    return shouldSplit;
}

// Inspired by rtrger's TR3G ASL's gameTime block.
gameTime
{
    const uint ticksPerSecond = 30;
    if (settings["IL"])
    {
        return TimeSpan.FromSeconds((double)current.currentLevelTime / ticksPerSecond);
    }
    else
    {
        // The game stores statistics (including time) for each completed level on separate addresses;
        // these addresses do not store anything for the level which is currently being played.
        // Level order in TR3 is variable, so we can't nicely loop up to the current.level value.
        // The simplest brute force solution is to loop through all completed level time addresses to sum them,
        // then add the current level's time. For NG+ full-game runs, this will result in an erroneous number
        // since the completed level information is not zeroed out before NG+ begins. The final result might be
        // correct, but only if the runner completes All Hallows in the NG+ run.
        // To better support NG+, we don't use these addresses.
        /* IntPtr firstLevelTimeAddress = (IntPtr)0x6D2326;  // This is the same on all supported versions. */
        uint finishedLevelsTime = 0;
        for (int i = 0; i < vars.completedLevels.Count; ++i)
        {
            if (current.level == vars.completedLevels[i])
                break;
            else
                finishedLevelsTime += vars.completedLevelTimes[i];
        }

        return TimeSpan.FromSeconds((double)(current.currentLevelTime + finishedLevelsTime) / ticksPerSecond);
    }
}