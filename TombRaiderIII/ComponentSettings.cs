using System;
using ClassicUtil;
using Util;

namespace TR3;

public sealed class ComponentSettings(Version version) : ClassicComponentSettings(version)
{
    public override void SetGameVersion(VersionDetectionResult result)
    {
        const string intText = "International (INT) [TR3]";
        const string jpTr3Text = "Japanese (JP) [TR3]";
        const string jpTlaText = "Japanese (JP) [TLA]";

        switch (result)
        {
            case VersionDetectionResult.None:
            case VersionDetectionResult.Unknown:
                base.SetGameVersion(result);
                return;

            case VersionDetectionResult.Found found:
                GameVersionLabel.Text =
                    "Game Version: " +
                    (Tr3Version)found.Version switch
                    {
                        Tr3Version.Int or Tr3Version.Int16x9                   => intText,
                        Tr3Version.JpCracked or Tr3Version.JpCracked16x9       => jpTr3Text,
                        Tr3Version.JpTlaCracked or Tr3Version.JpTlaCracked16x9 => jpTlaText,
                        _ => throw new ArgumentOutOfRangeException(nameof(found.Version)),
                    };
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }
}