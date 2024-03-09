using TRUtil;

namespace TR2;

public sealed class ComponentSettings : ClassicComponentSettings
{
    public override void SetGameVersion(uint version)
    {
        string versionText;
        switch ((GameVersion)version)
        {
            case GameVersion.MP:
                versionText = "Multipatch [TR2]";
                break;
            case GameVersion.EPC:
                versionText = "Eidos Premier Collection [TR2]";
                break;
            case GameVersion.P1:
                versionText = "CORE's Patch 1 [TR2]";
                break;
            case GameVersion.UKB:
                versionText = "Eidos UK Box [TR2]";
                break;
            case GameVersion.StellaGold:
                versionText = "Stella [TR2G]";
                break;
            case GameVersion.StellaGoldCracked:
                versionText = "Stella No-CD [TR2G]";
                break;
            case GameVersion.None:
            default:
                versionText = "Unknown/Undetected";
                break;
        }
        
        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}