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

    public override void SetGameVersion(uint version, string hash)
    {
        const string digitalText = "Steam/GOG [TR5]";
        const string jpNoCdText = "Japanese No-CD [TR5]";

        string versionText;
        switch ((Tr5Version)version)
        {
            case Tr5Version.SteamOrGog:
                versionText = digitalText;
                break;

            case Tr5Version.JapaneseNoCd:
                versionText = jpNoCdText;
                break;

            case Tr5Version.None:
            default:
                base.SetGameVersion(version, hash);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}