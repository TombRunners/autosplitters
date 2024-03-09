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
        const string unknownText = "Unknown/Undetected";
        
        string versionText = (GameVersion) version switch
        {
            GameVersion.SteamOrGog => digitalText,
            GameVersion.TheTimesExclusive => tteText,
            _ => unknownText,
        };

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}