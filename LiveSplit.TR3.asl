state("tomb3", "US")
{
    int isCutscene:    0x233F54;
    int isStatsScreen: 0x2266AC;
    int isTitle:       0x2A1B78;
    int level:         0xC561C;
}

state("tomb3", "JP")
{
    int isCutscene:    0x233F44;
    int isStatsScreen: 0x2266AC;
    int isTitle:       0x2A1B80;
    int level:         0xC561C;
}

init
{
    // TODO: Differentiate and set the version according to which one is running.
    // Both version's ModuleMemorySize == 3141632, so that method won't work.
    Console.Writeline("Process found!");
    const uint refreshRate = 30;
}

start
{
    return current.level == 1 && current.isTitle == 0 && old.isTitle == 1;
}

reset
{
    return current.isTitle == 1 && old.isTitle == 0;
}

split
{
    if (current.level == 19)
        return current.isCutscene == 1 && old.isCutscene == 0;
    return current.isStatsScreen == 1 && old.isStatsScreen == 0;
}