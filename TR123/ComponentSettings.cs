using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Util;

namespace TR123;

public sealed class ComponentSettings : UserControl
{
    internal GroupBox ModeSelect;
    public RadioButton ILModeButton;
    public RadioButton FullGameModeButton;
    public RadioButton DeathrunModeButton;
    private Label _gameTimeMethodLabel;
    public CheckBox EnableAutoResetCheckbox;
    public Label GameVersionLabel;
    public Label AutosplitterVersionLabel;
    private LinkLabel _guideLabel;
    private Label _aslWarningLabel;
    private Label _timerWarningLabel;
    public bool FullGame = true;
    public bool Deathrun;
    public GameTimeMethod GameTimeMethod;
    public bool EnableAutoReset;

    internal static bool GameVersionInitialized;

    public ComponentSettings()
    {
        InitializeComponent();
        SetGameTimeMethod(Deathrun ? GameTimeMethod.Igt : GameTimeMethod.RtaNoLoads);
        GameVersionInitialized = false;
    }

    [SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
    private void InitializeComponent()
    {
        ModeSelect = new GroupBox();
        FullGameModeButton = new RadioButton();
        ILModeButton = new RadioButton();
        DeathrunModeButton = new RadioButton();
        _gameTimeMethodLabel = new Label();
        EnableAutoResetCheckbox = new CheckBox();
        GameVersionLabel = new Label();
        AutosplitterVersionLabel = new Label();
        _guideLabel = new LinkLabel();
        _aslWarningLabel = new Label();
        _timerWarningLabel = new Label();
        ModeSelect.SuspendLayout();
        SuspendLayout();

        // _modeSelect
        ModeSelect.Controls.Add(FullGameModeButton);
        ModeSelect.Controls.Add(ILModeButton);
        ModeSelect.Controls.Add(DeathrunModeButton);
        ModeSelect.Location = new Point(10, 10);
        ModeSelect.Name = "ModeSelect";
        ModeSelect.Size = new Size(275, 55);
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
        ILModeButton.Size = new Size(100, 17);
        ILModeButton.TabIndex = 1;
        ILModeButton.Text = "IL or Section Run";
        ILModeButton.UseVisualStyleBackColor = true;
        ILModeButton.CheckedChanged += ILModeButtonCheckedChanged;

        // DeathrunModeButton
        DeathrunModeButton.AutoSize = true;
        DeathrunModeButton.Location = new Point(198, 20);
        DeathrunModeButton.Name = "DeathrunModeButton";
        DeathrunModeButton.Size = new Size(70, 17);
        DeathrunModeButton.TabIndex = 2;
        DeathrunModeButton.Text = "Deathrun";
        DeathrunModeButton.UseVisualStyleBackColor = true;
        DeathrunModeButton.CheckedChanged += DeathrunModeButtonCheckedChanged;

        // GameTimeMethodLabel
        _gameTimeMethodLabel.AutoSize = true;
        _gameTimeMethodLabel.Location = new Point(328, 25);
        _gameTimeMethodLabel.Name = "GameVersionLabel";
        _gameTimeMethodLabel.Size = new Size(200, 30);
        _gameTimeMethodLabel.TabIndex = 0;
        _gameTimeMethodLabel.Text = "Game Time Method:\nNot Initialized!";
        _gameTimeMethodLabel.TextAlign = ContentAlignment.MiddleCenter;

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
        GameVersionLabel.Text = "Game Version: None / Undetected";

        // AutosplitterVersionLabel
        AutosplitterVersionLabel.AutoSize = true;
        AutosplitterVersionLabel.Location = new Point(10, 170);
        AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
        AutosplitterVersionLabel.Size = new Size(200, 15);
        AutosplitterVersionLabel.TabIndex = 2;
        AutosplitterVersionLabel.Text = "Autosplitter Version: " + Assembly.GetCallingAssembly().GetName().Version;

        // _aslWarningLabel
        _aslWarningLabel.AutoSize = true;
        _aslWarningLabel.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        _aslWarningLabel.ForeColor = Color.Crimson;
        _aslWarningLabel.Location = new Point(24, 210);
        _aslWarningLabel.Name = "_aslWarningLabel";
        _aslWarningLabel.Size = new Size(476, 20);
        _aslWarningLabel.TabStop = false;
        _aslWarningLabel.Text = "Scriptable Auto Splitter in Layout — Please Remove!";
        _aslWarningLabel.Visible = false;

        // _timerWarningLabel
        _timerWarningLabel.AutoSize = true;
        _timerWarningLabel.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        _timerWarningLabel.ForeColor = Color.Crimson;
        _timerWarningLabel.Location = new Point(55, 240);
        _timerWarningLabel.Name = "_timerWarningLabel";
        _timerWarningLabel.Size = new Size(476, 20);
        _timerWarningLabel.TabStop = false;
        _timerWarningLabel.Text = "No Game Time Timer in Layout — Please Fix!";
        _timerWarningLabel.Visible = false;

        // _guideLabel
        _guideLabel.AutoSize = false;
        _guideLabel.Font = new Font(FontFamily.GenericSansSerif, 8.25f);
        _guideLabel.Location = new Point(10, 275);
        _guideLabel.LinkClicked += GuideLinkClicked;
        _guideLabel.Links.Add(11, 55, "https://www.speedrun.com/tr123_remastered/guides/afl7w");
        _guideLabel.Name = "_guideLabel";
        _guideLabel.Size = new Size(476, 15);
        _guideLabel.TabIndex = 3;
        _guideLabel.Text = "Need help? https://www.speedrun.com/tr123_remastered/guides/afl7w";

        // ComponentSettings
        Controls.Add(ModeSelect);
        Controls.Add(_gameTimeMethodLabel);
        Controls.Add(EnableAutoResetCheckbox);
        Controls.Add(GameVersionLabel);
        Controls.Add(AutosplitterVersionLabel);
        Controls.Add(_aslWarningLabel);
        Controls.Add(_timerWarningLabel);
        Controls.Add(_guideLabel);
        Name = "ComponentSettings";
        Size = new Size(476, 290);
        ModeSelect.ResumeLayout(false);
        ModeSelect.PerformLayout();

        ResumeLayout(false);
        PerformLayout();
    }

    private static void GuideLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (e.Link.LinkData is string target)
            System.Diagnostics.Process.Start(target);
    }

    public void SetWarningLabelVisibilities(bool aslComponentIsPresent, bool timerWithGameTimeIsPresent)
    {
        _aslWarningLabel.Visible = aslComponentIsPresent;
        _timerWarningLabel.Visible = !timerWithGameTimeIsPresent;
    }

    public void SetGameVersion(VersionDetectionResult result)
    {
        const string noneUndetected = "Game Version: None / Undetected";
        const string egsDebug = "EGS debug release (Unsupported)";
        const string gogV10 = "GOG v1.0";
        const string publicV101 = "GOG v1.01 / Steam 13430979";
        const string patch1 = "GOG v1.01 Patch 1 / Steam 13617493";
        const string patch2 = "GOG v1.01 Patch 2 / Steam 13946608";
        const string patch3 = "GOG v1.01 Patch 3 / Steam 14397396";
        const string patch4 = "GOG v1.01 Patch 4 / Steam 15795727";
        const string patch4Update1 = "Steam 19001004";
        const string patch4Update2 = "Steam 19617537";

        GameVersionLabel.Text = result switch
        {
            VersionDetectionResult.None => noneUndetected,
            VersionDetectionResult.Unknown unknown => $"Found unknown version, MD5 hash: {unknown.Hash}",
            VersionDetectionResult.Found found => GameVersionLabel.Text =
                "Game Version: " +
                (GameVersion)found.Version switch
                {
                    GameVersion.EgsDebug => egsDebug,
                    GameVersion.GogV10 => gogV10,
                    GameVersion.PublicV101 => publicV101,
                    GameVersion.Patch1 => patch1,
                    GameVersion.Patch2 => patch2,
                    GameVersion.Patch3 => patch3,
                    GameVersion.Patch4 => patch4,
                    GameVersion.Patch4Update1 => patch4Update1,
                    GameVersion.Patch4Update2 => patch4Update2,
                    _ => throw new ArgumentOutOfRangeException(nameof(found.Version)),
                },
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };

        GameVersionInitialized = true;
    }

    private void SetGameTimeMethod(GameTimeMethod method)
    {
        string methodText = method switch
        {
            GameTimeMethod.Igt => "IGT",
            GameTimeMethod.RtaNoLoads => "RTA w/o Loads",
            _ => "Unknown",
        };
        _gameTimeMethodLabel.Text = $"Game Time Method:\n{methodText}";

        GameTimeMethod = method;
    }

    private void FullGameModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = true;
        Deathrun = false;
        SetGameTimeMethod(GameTimeMethod.RtaNoLoads);
    }

    private void ILModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = false;
        Deathrun = false;
        SetGameTimeMethod(GameTimeMethod.RtaNoLoads);
    }

    private void DeathrunModeButtonCheckedChanged(object sender, EventArgs e)
    {
        FullGame = false;
        Deathrun = true;
        SetGameTimeMethod(GameTimeMethod.Igt);
    }

    private void EnableAutoResetCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox)sender;
        EnableAutoReset = checkbox.Checked;
    }
}