using System;
using ClassicUtil;

namespace TR2;

public sealed class ComponentSettings(Version version) : ClassicComponentSettings(version)
{
    public override void SetGameVersion(uint version, string hash)
    {
        const string multipatch = "Multipatch [TR2]";
        const string eidosPremierCollection = "Eidos Premier Collection [TR2]";
        const string corePatch1 = "CORE's Patch 1 [TR2]";
        const string eidosUkBox = "Eidos UK Box [TR2]";
        const string stella = "Stella [TR2G]";
        const string stellaNoCd = "Stella No-CD [TR2G]";

        string versionText;
        switch ((Tr2Version)version)
        {
            case Tr2Version.MP:
                versionText = multipatch;
                break;

            case Tr2Version.EPC:
                versionText = eidosPremierCollection;
                break;

            case Tr2Version.P1:
                versionText = corePatch1;
                break;

            case Tr2Version.UKB:
                versionText = eidosUkBox;
                break;

            case Tr2Version.StellaGold:
                versionText = stella;
                break;

            case Tr2Version.StellaGoldCracked:
                versionText = stellaNoCd;
                break;

            case Tr2Version.None:
            default:
                base.SetGameVersion(version, hash);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}