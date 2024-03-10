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
        const string unknownText = "Unknown/Undetected";

        string versionText = (GameVersion)version switch
        {
            GameVersion.MP                => multipatch,
            GameVersion.EPC               => eidosPremierCollection,
            GameVersion.P1                => corePatch1,
            GameVersion.UKB               => eidosUkBox,
            GameVersion.StellaGold        => stella,
            GameVersion.StellaGoldCracked => stellaNoCd,
            _                             => unknownText,
        };
        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}