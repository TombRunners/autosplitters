using System;
using TRUtil;

namespace TR5;

public sealed class ComponentSettings : LaterClassicComponentSettings
{
    public ComponentSettings(Version version) : base(version)
    {
        SuspendLayout();
        OptionCheckbox.Checked = Option = true;
        OptionCheckbox.Name = "SplitCutsceneCheckbox";
        OptionCheckbox.Text = "Split Security Breach Cutscene";
        ResumeLayout(false);
        PerformLayout();
    }

    public override void SetGameVersion(uint version)
    {
        const string digitalText = "Steam/GOG [TR5]";
        const string jpNoCdText = "Japanese No-CD [TR5]";

        string versionText;
        switch ((GameVersion)version)
        {
            case GameVersion.SteamOrGog:
                versionText = digitalText;
                break;

            case GameVersion.JapaneseNoCd:
                versionText = jpNoCdText;
                break;

            case GameVersion.None:
            default:
                base.SetGameVersion(version);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}