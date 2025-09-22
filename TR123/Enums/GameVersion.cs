namespace TR123;

public enum GameVersion: uint
{
    EgsDebug         = 1, // Unsupported Epic Games Store release with unfinished features and PDB files, later reverted to match GOG v1.01.
    GogV10           = 2, // GOG v1.0
    PublicV101       = 3, // GOG v1.01, Steam 13430979
    Patch1           = 4, // GOG v1.01 Patch 1, Steam 13617493
    Patch2           = 5, // GOG v1.01 Patch 2, Steam 13946608
    Patch3           = 6, // GOG v1.01 Patch 3, Steam 14397396
    Patch4           = 7, // GOG v1.01 Patch 4, Steam 15795727
    Patch4Update1    = 8, // Steam 19001004
    Patch4Update2    = 9, // Steam 19617537
}