using TRUtil;

namespace TR2Gold
{
    public sealed class ComponentSettings : ClassicComponentSettings
    {
        public override void SetGameVersion(uint version)
        {
            string versionText;
            switch ((GameVersion)version)
            {
                case GameVersion.Stella:
                    versionText = "Stella";
                    break;
                case GameVersion.StellaCracked:
                    versionText = "Stella No-CD";
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
