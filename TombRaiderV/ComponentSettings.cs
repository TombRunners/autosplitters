using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using LaterClassicUtil;
using Util;

namespace TR5;

public sealed class ComponentSettings : LaterClassicComponentSettings
{
    private GroupBox _modeSelect;
    public RadioButton ILModeButton;
    public RadioButton FullGameModeButton;
    public RadioButton DeathrunModeButton;
    public CheckBox SplitSecurityBreachCheckbox;
    public CheckBox EnableAutoResetCheckbox;
    public CheckBox SplitSecretsCheckbox;
    public bool SplitSecurityBreach;

    public ComponentSettings(Version version)
    {
        InitializeComponent();
        AutosplitterVersionLabel.Text = $"Autosplitter Version: {version}";
    }

    [SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
    private void InitializeComponent()
    {
        _modeSelect = new GroupBox();
        FullGameModeButton = new RadioButton();
        ILModeButton = new RadioButton();
        DeathrunModeButton = new RadioButton();
        SplitSecurityBreachCheckbox = new CheckBox();
        EnableAutoResetCheckbox = new CheckBox();
        SplitSecretsCheckbox = new CheckBox();
        GameVersionLabel = new Label();
        AutosplitterVersionLabel = new Label();
        AslWarningLabel = new Label();
        _modeSelect.SuspendLayout();
        SuspendLayout();

        // _modeSelect
        _modeSelect.Controls.Add(FullGameModeButton);
        _modeSelect.Controls.Add(ILModeButton);
        _modeSelect.Controls.Add(DeathrunModeButton);
        _modeSelect.Location = new Point(10, 10);
        _modeSelect.Name = "_modeSelect";
        _modeSelect.Size = new Size(300, 55);
        _modeSelect.TabIndex = 0;
        _modeSelect.TabStop = false;
        _modeSelect.Text = "Mode Selection";

        // FullGameModeButton
        FullGameModeButton.AutoSize = true;
        FullGameModeButton.Checked = true;
        FullGameModeButton.Location = new Point(6, 20);
        FullGameModeButton.Name = "FullGameModeButton";
        FullGameModeButton.Size = new Size(72, 17);
        FullGameModeButton.TabIndex = 0;
        FullGameModeButton.TabStop = true;
        FullGameModeButton.Text = "Full Game";
        FullGameModeButton.UseVisualStyleBackColor = true;
        FullGameModeButton.CheckedChanged += FullGameModeButtonCheckedChanged;

        // ILModeButton
        ILModeButton.AutoSize = true;
        ILModeButton.Location = new Point(84, 20);
        ILModeButton.Name = "ILModeButton";
        ILModeButton.Size = new Size(139, 17);
        ILModeButton.TabIndex = 1;
        ILModeButton.Text = "IL or Section Run (RTA)";
        ILModeButton.UseVisualStyleBackColor = true;
        ILModeButton.CheckedChanged += ILModeButtonCheckedChanged;

        // DeathrunModeButton
        DeathrunModeButton.AutoSize = true;
        DeathrunModeButton.Location = new Point(225, 20);
        DeathrunModeButton.Name = "DeathrunModeButton";
        DeathrunModeButton.Size = new Size(69, 17);
        DeathrunModeButton.TabIndex = 2;
        DeathrunModeButton.Text = "Deathrun";
        DeathrunModeButton.UseVisualStyleBackColor = true;
        DeathrunModeButton.CheckedChanged += DeathrunModeButtonCheckedChanged;

        // EnableAutoResetCheckbox
        EnableAutoResetCheckbox.AutoSize = true;
        EnableAutoResetCheckbox.Checked = false;
        EnableAutoResetCheckbox.Location = new Point(12, 80);
        EnableAutoResetCheckbox.Size = new Size(72, 17);
        EnableAutoResetCheckbox.Name = "EnableAutoResetCheckbox";
        EnableAutoResetCheckbox.Text = "Enable Auto-Reset";
        EnableAutoResetCheckbox.TabIndex = 0;
        EnableAutoResetCheckbox.UseVisualStyleBackColor = true;
        EnableAutoResetCheckbox.CheckedChanged += EnableAutoResetCheckboxCheckedChanged;

        // SplitSecretsCheckbox
        SplitSecretsCheckbox.AutoSize = true;
        SplitSecretsCheckbox.Checked = false;
        SplitSecretsCheckbox.Location = new Point(12, 105);
        SplitSecretsCheckbox.Size = new Size(72, 17);
        SplitSecretsCheckbox.Name = "SplitSecretsCheckbox";
        SplitSecretsCheckbox.Text = "Split When Secret is Triggered";
        SplitSecretsCheckbox.TabIndex = 0;
        SplitSecretsCheckbox.UseVisualStyleBackColor = true;
        SplitSecretsCheckbox.CheckedChanged += SplitSecretsCheckboxCheckedChanged;

        // SplitSecurityBreachCheckbox
        SplitSecurityBreachCheckbox.AutoSize = true;
        SplitSecurityBreachCheckbox.Checked = false;
        SplitSecurityBreachCheckbox.Location = new Point(12, 130);
        SplitSecurityBreachCheckbox.Size = new Size(72, 17);
        SplitSecurityBreachCheckbox.Name = "SplitSecurityBreachCheckbox";
        SplitSecurityBreachCheckbox.Text = "Split Security Breach Cutscene";
        SplitSecurityBreachCheckbox.TabIndex = 0;
        SplitSecurityBreachCheckbox.UseVisualStyleBackColor = true;
        SplitSecurityBreachCheckbox.CheckedChanged += SplitSecurityBreachCheckboxCheckedChanged;

        // GameVersionLabel
        GameVersionLabel.AutoSize = true;
        GameVersionLabel.Location = new Point(10, 170);
        GameVersionLabel.Name = "GameVersionLabel";
        GameVersionLabel.Size = new Size(200, 15);
        GameVersionLabel.TabIndex = 1;
        GameVersionLabel.Text = "Game Version: Unknown/Undetected";

        // AutosplitterVersionLabel
        AutosplitterVersionLabel.AutoSize = true;
        AutosplitterVersionLabel.Location = new Point(10, 190);
        AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
        AutosplitterVersionLabel.Size = new Size(200, 15);
        AutosplitterVersionLabel.TabIndex = 2;
        AutosplitterVersionLabel.Text = "Autosplitter Version: Uninitialized";

        // _aslWarningLabel
        AslWarningLabel.AutoSize = true;
        AslWarningLabel.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        AslWarningLabel.ForeColor = Color.Crimson;
        AslWarningLabel.Location = new Point(24, 230);
        AslWarningLabel.Name = "AslWarningLabel";
        AslWarningLabel.Size = new Size(476, 20);
        AslWarningLabel.TabStop = false;
        AslWarningLabel.Text = "Scriptable Auto Splitter in Layout � Please Remove!";
        AslWarningLabel.Visible = false;

        // ComponentSettings
        Controls.Add(AslWarningLabel);
        Controls.Add(AutosplitterVersionLabel);
        Controls.Add(GameVersionLabel);
        Controls.Add(SplitSecurityBreachCheckbox);
        Controls.Add(EnableAutoResetCheckbox);
        Controls.Add(SplitSecretsCheckbox);
        Controls.Add(_modeSelect);
        Name = "ComponentSettings";
        Size = new Size(476, 250);
        _modeSelect.ResumeLayout(false);
        _modeSelect.PerformLayout();

        ResumeLayout(false);
        PerformLayout();
    }

    private void SplitSecurityBreachCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox) sender;
        SplitSecurityBreach = checkbox.Checked;
    }

    public override void SetGameVersion(VersionDetectionResult result)
    {
        const string digitalText = "Steam/GOG [TR5]";
        const string digitalNoCutsceneText = "Steam/GOG [TR5] with Cutscene Skipper Mod";
        const string jpNoCdText = "Japanese No-CD [TR5]";

        switch (result)
        {
            case VersionDetectionResult.None:
            case VersionDetectionResult.Unknown:
                base.SetGameVersion(result);
                return;

            case VersionDetectionResult.Found found:
                GameVersionLabel.Text =
                    "Game Version: " +
                    (Tr5Version)found.Version switch
                    {
                        Tr5Version.SteamOrGog             => digitalText,
                        Tr5Version.SteamOrGogCutsceneless => digitalNoCutsceneText,
                        Tr5Version.JapaneseNoCd           => jpNoCdText,
                        _ => throw new ArgumentOutOfRangeException(nameof(found.Version)),
                    };
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }
}