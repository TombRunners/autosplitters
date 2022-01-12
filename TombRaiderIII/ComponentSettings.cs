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
                    versionText = "International [INT]";
                    break;
                case GameVersion.JpCracked:
                    versionText = "Japanese [JP]";
                    break;
                default:
                    versionText = "Unknown/Undetected";
                    break;
            }
            GameVersionLabel.Text = "Game Version: " + versionText;
        }
    }
}
