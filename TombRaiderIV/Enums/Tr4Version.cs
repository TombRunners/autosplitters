// ReSharper disable InconsistentNaming

namespace TR4;

/// <summary>The supported game versions.</summary>
public enum Tr4Version
{
    SteamOrGog            = 1,
    SteamOrGog16x9        = 2, // Bytes at address 0xA9410 changed to 39 8E E3 (float value 1.7777, 16/9)
    TheTimesExclusive     = 3,
    TheTimesExclusive16x9 = 4, // Bytes at address 0xA93E0 changed to 39 8E E3 (float value 1.7777, 16/9)
}