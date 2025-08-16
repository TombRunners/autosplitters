using System;
using ClassicUtil;
using Util;

namespace TR1;

public sealed class ComponentSettings(Version version) : ClassicComponentSettings(version)
{
    public override void SetGameVersion(VersionDetectionResult result)
    {
        const string dosboxText = "DOSBox [TR 1996]";
        const string atiTr1Text = "TombATI [TR 1996]";
        const string atiTrUbText = "TombATI [TR:UB]";

        switch (result)
        {
            case VersionDetectionResult.None:
            case VersionDetectionResult.Unknown:
                base.SetGameVersion(result);
                return;

            case VersionDetectionResult.Found found:
                GameVersionLabel.Text =
                    "Game Version: " +
                    (Tr1Version)found.Version switch
                    {
                        Tr1Version.Ati => atiTr1Text,
                        Tr1Version.AtiUnfinishedBusiness => atiTrUbText,
                        Tr1Version.DOSBox => dosboxText,
                        _ => throw new ArgumentOutOfRangeException(nameof(found.Version)),
                    };
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }
}