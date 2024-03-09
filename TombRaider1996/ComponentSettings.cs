using TRUtil;

namespace TR1;

public sealed class ComponentSettings : ClassicComponentSettings
{
    public override void SetGameVersion(uint version)
    {
        const string dosboxText = "DOSBox [TR 1996]";
        const string atiTr1Text = "TombATI [TR 1996]";
        const string atiTrUbText = "TombATI [TR:UB]";
        const string unknownText = "Unknown/Undetected";

        string versionText = (GameVersion) version switch
        {
            GameVersion.DOSBox                => dosboxText,
            GameVersion.Ati                   => atiTr1Text,
            GameVersion.AtiUnfinishedBusiness => atiTrUbText,
            _                                 => unknownText,
        };

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}