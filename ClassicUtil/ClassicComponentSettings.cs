using System;
using System.Drawing;
using System.Windows.Forms;
using Util;

namespace ClassicUtil;

public class ClassicComponentSettings : UserControl
{
    protected GroupBox ModeSelect;
    public RadioButton ILModeButton;
    public RadioButton FullGameModeButton;
    public RadioButton DeathrunModeButton;
    public CheckBox EnableAutoResetCheckbox;
    public Label GameVersionLabel;
    public Label AutosplitterVersionLabel;
    private Label _aslWarningLabel;
    public bool FullGame = true;
    public bool Deathrun;
    public bool EnableAutoReset;

    public ClassicComponentSettings(Version version)
    {
        InitializeComponent();
        AutosplitterVersionLabel.Text = $"Autosplitter Version: {version}";
    }

    private void InitializeComponent()
    {
        ModeSelect = new GroupBox();
        ILModeButton = new RadioButton();
        FullGameModeButton = new RadioButton();
        DeathrunModeButton = new RadioButton();
        EnableAutoResetCheckbox = new CheckBox();
        GameVersionLabel = new Label();
        AutosplitterVersionLabel = new Label();
        _aslWarningLabel = new Label();
        ModeSelect.SuspendLayout();
        SuspendLayout();

        // _modeSelect
        ModeSelect.Controls.Add(FullGameModeButton);
        ModeSelect.Controls.Add(ILModeButton);
        ModeSelect.Controls.Add(DeathrunModeButton);
        ModeSelect.Location = new Point(10, 10);
        ModeSelect.Name = "ModeSelect";
        ModeSelect.Size = new Size(300, 55);
        ModeSelect.TabIndex = 0;
        ModeSelect.TabStop = false;
        ModeSelect.Text = "Mode Selection";

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

        // GameVersionLabel
        GameVersionLabel.AutoSize = true;
        GameVersionLabel.Location = new Point(10, 150);
        GameVersionLabel.Name = "GameVersionLabel";
        GameVersionLabel.Size = new Size(200, 15);
        GameVersionLabel.TabIndex = 1;
        GameVersionLabel.Text = "Game Version: Unknown/Undetected";

        // AutosplitterVersionLabel
        AutosplitterVersionLabel.AutoSize = true;
        AutosplitterVersionLabel.Location = new Point(10, 170);
        AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
        AutosplitterVersionLabel.Size = new Size(200, 15);
        AutosplitterVersionLabel.TabIndex = 2;
        AutosplitterVersionLabel.Text = "Autosplitter Version: Uninitialized";

        // _aslWarningLabel
        _aslWarningLabel.AutoSize = true;
        _aslWarningLabel.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        _aslWarningLabel.ForeColor = Color.Crimson;
        _aslWarningLabel.Location = new Point(24, 210);
        _aslWarningLabel.Name = "_aslWarningLabel";
        _aslWarningLabel.Size = new Size(476, 20);
        _aslWarningLabel.TabStop = false;
        _aslWarningLabel.Text = "Scriptable Auto Splitter in Layout � Please Remove!";
        _aslWarningLabel.Visible = false;

        // ComponentSettings
        Controls.Add(_aslWarningLabel);
        Controls.Add(AutosplitterVersionLabel);
        Controls.Add(GameVersionLabel);
        Controls.Add(EnableAutoResetCheckbox);
        Controls.Add(ModeSelect);
        Name = "ClassicComponentSettings";
        Size = new Size(476, 250);
        ModeSelect.ResumeLayout(false);
        ModeSelect.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    public void SetAslWarningLabelVisibility(bool aslComponentIsPresent) => _aslWarningLabel.Visible = aslComponentIsPresent;

    public virtual void SetGameVersion(VersionDetectionResult result)
    {
        const string noneUndetected = "Game Version: None / Undetected";

        GameVersionLabel.Text = result switch
        {
            VersionDetectionResult.None            => noneUndetected,
            VersionDetectionResult.Unknown unknown => $"Found unknown version, MD5 hash: {unknown.Hash}",
            VersionDetectionResult.Found           => GameVersionLabel.Text,
            _                                      => throw new ArgumentOutOfRangeException(nameof(result)),
        };
    }

    private void FullGameModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = true;
        Deathrun = false;
    }

    private void ILModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = false;
        Deathrun = false;
    }

    private void DeathrunModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = false;
        Deathrun = true;
    }

    private void EnableAutoResetCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox) sender;
        EnableAutoReset = checkbox.Checked;
    }
}