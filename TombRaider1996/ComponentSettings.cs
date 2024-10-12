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
        switch ((Tr1Version)version)
        {
            case Tr1Version.DOSBox:
                versionText = dosboxText;
                break;

            case Tr1Version.Ati:
                versionText = atiTr1Text;
                break;

            case Tr1Version.AtiUnfinishedBusiness:
                versionText = atiTrUbText;
                break;

            case Tr1Version.None:
            default:
                base.SetGameVersion(version, hash);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}