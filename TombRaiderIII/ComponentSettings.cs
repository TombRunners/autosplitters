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
        switch ((GameVersion)version)
        {
            case GameVersion.Int:
            case GameVersion.Int16x9AspectRatio:
                versionText = intText;
                break;

            case GameVersion.JpCracked:
            case GameVersion.JpCracked16x9AspectRatio:
                versionText = jpTr3Text;
                break;

            case GameVersion.JpTlaCracked:
            case GameVersion.JpTlaCracked16x9AspectRatio:
                versionText = jpTlaText;
                break;

            case GameVersion.None:
            default:
                base.SetGameVersion(version, hash);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}