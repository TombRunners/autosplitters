using System;
using ClassicUtil;
using Util;

namespace TR2;

public sealed class ComponentSettings(Version version) : ClassicComponentSettings(version)
{
    public override void SetGameVersion(VersionDetectionResult result)
    {
        const string mpText = "Multipatch [TR2]";
        const string epcText = "Eidos Premier Collection [TR2]";
        const string p1Text = "CORE's Patch 1 [TR2]";
        const string ukbText = "Eidos UK Box [TR2]";
        const string stellaText = "Stella [TR2G]";
        const string stellaCrackedText = "Stella No-CD [TR2G]";

        switch (result)
        {
            case VersionDetectionResult.None:
            case VersionDetectionResult.Unknown:
                base.SetGameVersion(result);
                return;

            case VersionDetectionResult.Found found:
                GameVersionLabel.Text =
                    "Game Version: " +
                    (Tr2Version)found.Version switch
                    {
                        Tr2Version.MP                => mpText,
                        Tr2Version.EPC               => epcText,
                        Tr2Version.P1                => p1Text,
                        Tr2Version.UKB               => ukbText,
                        Tr2Version.StellaGold        => stellaText,
                        Tr2Version.StellaGoldCracked => stellaCrackedText,
                        _ => throw new ArgumentOutOfRangeException(nameof(found.Version)),
                    };
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }
}