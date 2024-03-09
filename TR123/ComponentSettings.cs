using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TR123;

public sealed class ComponentSettings : UserControl
{
    internal GroupBox ModeSelect;
    public RadioButton ILModeButton;
    public RadioButton FullGameModeButton;
    public RadioButton DeathrunModeButton;
    public Label GameTimeMethodLabel;
    public CheckBox EnableAutoResetCheckbox;
    public Label GameVersionLabel;
    public Label AutosplitterVersionLabel;
    public bool FullGame = true;
    public bool Deathrun;
    public GameTimeMethod GameTimeMethod;
    public bool EnableAutoReset;

    public ComponentSettings()
    {
        InitializeComponent();
        SetGameTimeMethod(Deathrun ? GameTimeMethod.Igt : GameTimeMethod.RtaNoLoads);
    }

    private void InitializeComponent()
    {
        ModeSelect = new GroupBox();
        FullGameModeButton = new RadioButton();
        ILModeButton = new RadioButton();
        DeathrunModeButton = new RadioButton();
        GameTimeMethodLabel = new Label();
        EnableAutoResetCheckbox = new CheckBox();
        GameVersionLabel = new Label();
        AutosplitterVersionLabel = new Label();
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
        GameTimeMethodLabel.AutoSize = true;
        GameTimeMethodLabel.Location = new Point(328, 25);
        GameTimeMethodLabel.Name = "GameVersionLabel";
        GameTimeMethodLabel.Size = new Size(200, 30);
        GameTimeMethodLabel.TabIndex = 0;
        GameTimeMethodLabel.Text = "Game Time Method:\nNot Initialized!";
        GameTimeMethodLabel.TextAlign = ContentAlignment.MiddleCenter;

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
        AutosplitterVersionLabel.Text = "Autosplitter Version: " + Assembly.GetCallingAssembly().GetName().Version.ToString(3);

        // ComponentSettings
        Controls.Add(AutosplitterVersionLabel);
        Controls.Add(GameVersionLabel);
        Controls.Add(GameTimeMethodLabel);
        Controls.Add(EnableAutoResetCheckbox);
        Controls.Add(ModeSelect);
        Name = "ComponentSettings";
        Size = new Size(476, 200);
        ModeSelect.ResumeLayout(false);
        ModeSelect.PerformLayout();

        ResumeLayout(false);
        PerformLayout();
    }

    public void SetGameVersion(uint version)
    {
        const string gogV10 = "GOG v1.0";
        const string gogV101 = "GOG v1.01 / Steam 13430979";
        const string unknownText = "Unknown/Undetected";

        string versionText = (GameVersion)version switch
        {
            GameVersion.PublicV10  => gogV10,
            GameVersion.PublicV101 => gogV101,
            _                      => unknownText,
        };

        GameVersionLabel.Text = $"Game Version: {versionText}";
    }

    public void SetGameTimeMethod(GameTimeMethod method)
    {
        string methodText = method switch
        {
            GameTimeMethod.Igt => "IGT",
            GameTimeMethod.RtaNoLoads => "RTA w/o Loads",
            _ => "Unknown",
        };
        GameTimeMethodLabel.Text = $"Game Time Method:\n{methodText}";

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