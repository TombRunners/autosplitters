using System;
using TRUtil;

namespace TR4;

public sealed class ComponentSettings : LaterClassicComponentSettings
{
    public ComponentSettings(Version version) : base(version)
    {
        SuspendLayout();
        OptionCheckbox.Name = "GlitchlessCheckbox";
        OptionCheckbox.Text = "Glitchless";
        ResumeLayout(false);
        PerformLayout();
    }

    public override void SetGameVersion(uint version, string hash)
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
                base.SetGameVersion(version, hash);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}