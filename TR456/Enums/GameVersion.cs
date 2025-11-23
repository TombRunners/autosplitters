namespace TR456;

public enum GameVersion : uint
{
    GogV10          = 1, // GOG v1.0
    PublicV10       = 2, // GOG v1.0.0, Steam 17156603, EGS TRX2_250128_19221_WIN
    Patch1          = 3, // GOG v1.0.0_Patch_1, Steam 17983102, EGS TRX2_250404_20819_WIN
    Patch2          = 4, // Steam 19062321, EGS TRX2_250701_21868_WIN.7z -- Yes, they misnamed the EGS version and didn't publish on GOG.
    Patch2Hotfix1   = 5, // Steam 19923088, EGS TRX2_250910_21914_WIN.7z -- Yes, they misnamed the EGS version again and didn't publish on GOG again.
}