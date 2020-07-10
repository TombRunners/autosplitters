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
    bool newGameStarted = current.level == 1 && current.currentLevelTime == 0 && old.isTitle;
    if (newGameStarted)
        return true;

    if (settings["FG"])
    {
        return false;
    }
    else  // IL-specific logic
    {
        bool goingToNextRealLevel = current.level == old.level + 1 && current.currentLevelTime == 0;
        return goingToNextRealLevel;
    }
}

reset
{
    return current.pickedPassportFunction == 2;
}

// TODO: Create IL split logic.
split
{
    if (current.level == 19)
        return current.isCutscene && !old.isCutscene;
    return current.levelComplete && !old.levelComplete;
}

// Copied from rtrger's TR3G ASL with IntPtr value and some var names changed.
// TODO: Adjust the loop and/or other logic to account for unenforced level order.
gameTime
{
    const int ticksPerSecond = 30;
    const uint levelCount = 19; // This does not include the bonus level

    if (settings["IL"])
    {
        return TimeSpan.FromSeconds((double) current.currentLevelTime / ticksPerSecond);
    }
    else
    {
        var firstLevelTime = (IntPtr)0x6D2326;
        int finishedLevelsTime = 0;
        // The game stores statistics (including time) for each completed level on separate addresses.
        for (int i = 1; i <= levelCount; i++)
            finishedLevelsTime += memory.ReadValue<int>((IntPtr)(firstLevelTime + (i * 0x33)));

        return TimeSpan.FromSeconds((double) (current.currentLevelTime + finishedLevelsTime) / ticksPerSecond);
    }
}