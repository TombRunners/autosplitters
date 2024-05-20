using System;
using TRUtil;

namespace TR3;

public sealed class ComponentSettings(Version version) : ClassicComponentSettings(version)
{
    public override void SetGameVersion(uint version, string hash)
    {
        const string intText = "International (INT) [TR3]";
        const string jpTr3Text = "Japanese (JP) [TR3]";
        const string jpTlaText = "Japanese (JP) [TLA]";

        string versionText;
        switch ((Tr3Version)version)
        {
            case Tr3Version.Int:
            case Tr3Version.Int16x9AspectRatio:
                versionText = intText;
                break;

            case Tr3Version.JpCracked:
            case Tr3Version.JpCracked16x9AspectRatio:
                versionText = jpTr3Text;
                break;

            case Tr3Version.JpTlaCracked:
            case Tr3Version.JpTlaCracked16x9AspectRatio:
                versionText = jpTlaText;
                break;

            case Tr3Version.None:
            default:
                base.SetGameVersion(version, hash);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}