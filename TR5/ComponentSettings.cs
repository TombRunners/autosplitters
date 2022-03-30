using TRUtil;

namespace TR5
{
    public sealed class ComponentSettings : LaterClassicComponentSettings
    {
        public ComponentSettings() : base()
        {
            SuspendLayout();
            GlitchlessCheckbox.Checked = true;
            GlitchlessCheckbox.Name = "SplitCutsceneCheckbox";
            GlitchlessCheckbox.Text = "Split Security Breach Cutscene";
            ResumeLayout(false);
            PerformLayout();
        }

        public override void SetGameVersion(uint version)
        {
            string versionText;
            switch ((GameVersion)version)
            {
                case GameVersion.SteamOrGog:
                    versionText = "Steam/GOG [TR5]";
                    break;
                default:
                    versionText = "Unknown/Undetected";
                    break;
            }
            GameVersionLabel.Text = "Game Version: " + versionText;
        }
    }
}
