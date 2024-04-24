namespace TR123;

public enum GameVersion
{
    None             = 0,
    Unknown          = 1,
    EgsDebug         = 2, // Unsupported Epic Games Store release with unfinished features and PDB files, later reverted to match GOG v1.01.
    PublicV10        = 3, // GOG v1.0
    PublicV101       = 4, // GOG v1.01, Steam 13430979
    PublicV101Patch1 = 5, // GOG v1.01 Patch 1, Steam 13617493
    PublicV101Patch2 = 6, // GOG v1.02 Patch 1, Steam 13946608
}