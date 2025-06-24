namespace TR123;

public enum GameVersion: uint
{
    EgsDebug         = 1, // Unsupported Epic Games Store release with unfinished features and PDB files, later reverted to match GOG v1.01.
    PublicV10        = 2, // GOG v1.0
    PublicV101       = 3, // GOG v1.01, Steam 13430979
    PublicV101Patch1 = 4, // GOG v1.01 Patch 1, Steam 13617493
    PublicV101Patch2 = 5, // GOG v1.01 Patch 2, Steam 13946608
    PublicV101Patch3 = 6, // GOG v1.01 Patch 3, Steam 14397396
    PublicV101Patch4 = 7, // GOG v1.01 Patch 4, Steam 15795727
}