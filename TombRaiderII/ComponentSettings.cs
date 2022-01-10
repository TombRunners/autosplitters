using TRUtil;

namespace TR2
{
    public sealed class ComponentSettings : ClassicComponentSettings
    {
        public override void SetGameVersion(uint version)
        {
            string versionText;
            switch ((GameVersion)version)
            {
                case GameVersion.MP:
                    versionText = "Multipatch";
                    break;
                case GameVersion.EPC:
                    versionText = "Eidos Premier Collection";
                    break;
                case GameVersion.P1:
                    versionText = "CORE's Patch 1";
                    break;
                case GameVersion.UKB:
                    versionText = "Eidos UK Box";
                    break;
                case GameVersion.None:
                default:
                    versionText = "Unknown/Undetected";
                    break;
            }
            GameVersionLabel.Text = "Game Version: " + versionText;
        }
    }
}
