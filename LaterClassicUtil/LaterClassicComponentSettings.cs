using System;
using System.Windows.Forms;
using Util;

namespace LaterClassicUtil;

public class LaterClassicComponentSettings : UserControl
{
    public bool FullGame = true;
    public bool Deathrun;
    public bool SplitSecrets;
    public bool EnableAutoReset;

    public Label GameVersionLabel;
    public Label AutosplitterVersionLabel;
    protected Label AslWarningLabel;

    public void SetAslWarningLabelVisibility(bool aslComponentIsPresent) => AslWarningLabel.Visible = aslComponentIsPresent;

    public virtual void SetGameVersion(VersionDetectionResult result)
    {
        const string noneUndetected = "Game Version: None / Undetected";

        GameVersionLabel.Text = result switch
        {
            VersionDetectionResult.None => noneUndetected,
            VersionDetectionResult.Unknown unknown => $"Found unknown version, MD5 hash: {unknown.Hash}",
            VersionDetectionResult.Found => GameVersionLabel.Text,
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };
    }

    protected virtual void FullGameModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = true;
        Deathrun = false;
    }

    protected virtual void ILModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = false;
        Deathrun = false;
    }

    protected virtual void DeathrunModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = false;
        Deathrun = true;
    }

    protected void EnableAutoResetCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox)sender;
        EnableAutoReset = checkbox.Checked;
    }

    protected void SplitSecretsCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox)sender;
        SplitSecrets = checkbox.Checked;
    }
}