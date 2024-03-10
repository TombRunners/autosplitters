using TRUtil;

namespace TR4;

public sealed class ComponentSettings : LaterClassicComponentSettings
{
    public ComponentSettings()
    {
        SuspendLayout();
        OptionCheckbox.Name = "GlitchlessCheckbox";
        OptionCheckbox.Text = "Glitchless";
        ResumeLayout(false);
        PerformLayout();
    }

    public override void SetGameVersion(uint version)
    {
        const string digitalText = "Steam/GOG [TR4]";
        const string tteText = "The Times Exclusive [TTE]";

        string versionText;
        switch ((GameVersion)version)
        {
            case GameVersion.SteamOrGog:
                versionText = digitalText;
                break;

            case GameVersion.TheTimesExclusive:
                versionText = tteText;
                break;

            case GameVersion.None:
            default:
                base.SetGameVersion(version);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}