using TRUtil;

namespace TR3;

public sealed class ComponentSettings : ClassicComponentSettings
{
    public override void SetGameVersion(uint version)
    {
        const string intText = "International (INT) [TR3]";
        const string jpTr3Text = "Japanese (JP) [TR3]";
        const string jpTlaText = "Japanese (JP) [TLA]";
        const string unknownText = "Unknown/Undetected";

        string versionText = (GameVersion) version switch
        {
            GameVersion.Int => intText,
            GameVersion.Int16x9AspectRatio => intText,
            GameVersion.JpCracked => jpTr3Text,
            GameVersion.JpCracked16x9AspectRatio => jpTr3Text,
            GameVersion.JpTlaCracked => jpTlaText,
            GameVersion.JpTlaCracked16x9AspectRatio => jpTlaText,
            _ => unknownText
        };

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}