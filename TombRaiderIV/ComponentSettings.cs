using TRUtil;

namespace TR4
{
    public sealed class ComponentSettings : LaterClassicComponentSettings
    {
        public override void SetGameVersion(uint version)
        {
            string versionText;
            switch ((GameVersion)version)
            {
                case GameVersion.SteamOrGog:
                    versionText = "Steam/GOG [TR4]";
                    break;
                default:
                    versionText = "Unknown/Undetected";
                    break;
            }
            GameVersionLabel.Text = "Game Version: " + versionText;
        }
    }
}
