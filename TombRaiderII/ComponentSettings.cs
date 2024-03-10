using TRUtil;

namespace TR2;

public sealed class ComponentSettings : ClassicComponentSettings
{
    public override void SetGameVersion(uint version)
    {
        const string multipatch = "Multipatch [TR2]";
        const string eidosPremierCollection = "Eidos Premier Collection [TR2]";
        const string corePatch1 = "CORE's Patch 1 [TR2]";
        const string eidosUkBox = "Eidos UK Box [TR2]";
        const string stella = "Stella [TR2G]";
        const string stellaNoCd = "Stella No-CD [TR2G]";

        string versionText;
        switch ((GameVersion)version)
        {
            case GameVersion.MP:
                versionText = multipatch;
                break;

            case GameVersion.EPC:
                versionText = eidosPremierCollection;
                break;

            case GameVersion.P1:
                versionText = corePatch1;
                break;

            case GameVersion.UKB:
                versionText = eidosUkBox;
                break;

            case GameVersion.StellaGold:
                versionText = stella;
                break;

            case GameVersion.StellaGoldCracked:
                versionText = stellaNoCd;
                break;

            case GameVersion.None:
            default:
                base.SetGameVersion(version);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}