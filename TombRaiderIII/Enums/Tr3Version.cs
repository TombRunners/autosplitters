namespace TR3;

/// <summary>The supported game versions.</summary>
internal enum Tr3Version
{
    Int              = 1, // From Steam
    Int16x9          = 2, // Int with bytes at address 0xA9410 changed to 39 8E E3 (float value 1.7777, 16/9)
    JpCracked        = 3, // No-CD cracked TR3 from JP Gold bundle release
    JpCracked16x9    = 4, // JpCracked with bytes at address 0xA9410 changed to 39 8E E3 (float value 1.7777, 16/9)
    JpTlaCracked     = 5, // No-CD cracked TLA from JP Gold bundle release
    JpTlaCracked16x9 = 6, // JpTlaCracked with bytes at address 0xA9410 changed to 39 8E E3 (float value 1.7777, 16/9)
}