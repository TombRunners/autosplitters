using TRUtil;

namespace TR3
{
    public sealed class ComponentSettings : ClassicComponentSettings
    {
        public override void SetGameVersion(uint version)
        {
            string versionText;
            switch ((GameVersion)version)
            {
                case GameVersion.Int:
                case GameVersion.Int16x9AspectRatio:
                    versionText = "International (INT) [TR3]";
                    break;
                case GameVersion.JpCracked:
                case GameVersion.JpCracked16x9AspectRatio:
                    versionText = "Japanese (JP) [TR3]";
                    break;
                case GameVersion.JpTlaCracked:
                case GameVersion.JpTlaCracked16x9AspectRatio:
                    versionText = "Japanese (JP) [TLA]";
                    break;
                default:
                    versionText = "Unknown/Undetected";
                    break;
            }

            GameVersionLabel.Text = "Game Version: " + versionText;
        }
    }
}
