using System;
using TRUtil;

namespace TR1;

public sealed class ComponentSettings(Version version) : ClassicComponentSettings(version)
{
    public override void SetGameVersion(uint version, string hash)
    {
        const string dosboxText = "DOSBox [TR 1996]";
        const string atiTr1Text = "TombATI [TR 1996]";
        const string atiTrUbText = "TombATI [TR:UB]";

        string versionText;
        switch ((GameVersion)version)
        {
            case GameVersion.DOSBox:
                versionText = dosboxText;
                break;

            case GameVersion.Ati:
                versionText = atiTr1Text;
                break;

            case GameVersion.AtiUnfinishedBusiness:
                versionText = atiTrUbText;
                break;

            case GameVersion.None:
            default:
                base.SetGameVersion(version, hash);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}