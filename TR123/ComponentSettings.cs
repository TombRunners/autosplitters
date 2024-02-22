using System.Reflection;
using System.Windows.Forms;
using System;
using System.Drawing;

namespace TR123;

public sealed class ComponentSettings : UserControl
{
    internal GroupBox ModeSelect;
    public RadioButton ILModeButton;
    public RadioButton FullGameModeButton;
    public RadioButton DeathrunModeButton;
    public CheckBox DisableAutoResetCheckbox;
    public Label GameVersionLabel;
    public Label AutosplitterVersionLabel;
    public bool FullGame = true;
    public bool Deathrun;
    public bool DisableAutoReset = true;

    public ComponentSettings() => InitializeComponent();

    private void InitializeComponent()
    {
        ModeSelect = new GroupBox();
        FullGameModeButton = new RadioButton();
        ILModeButton = new RadioButton();
        DeathrunModeButton = new RadioButton();
        DisableAutoResetCheckbox = new CheckBox();
        GameVersionLabel = new Label();
        AutosplitterVersionLabel = new Label();
        ModeSelect.SuspendLayout();
        SuspendLayout();

        // _modeSelect
        ModeSelect.Controls.Add(FullGameModeButton);
        ModeSelect.Controls.Add(ILModeButton);
        ModeSelect.Controls.Add(DeathrunModeButton);
        ModeSelect.Location = new Point(4, 4);
        ModeSelect.Name = "ModeSelect";
        ModeSelect.Size = new Size(297, 53);
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

        // DisableAutoResetCheckbox
        DisableAutoResetCheckbox.AutoSize = true;
        DisableAutoResetCheckbox.Checked = true;
        DisableAutoResetCheckbox.Location = new Point(10, 64);
        DisableAutoResetCheckbox.Size = new Size(72, 17);
        DisableAutoResetCheckbox.Name = "DisableAutoResetCheckbox";
        DisableAutoResetCheckbox.Text = "Disable Auto-Reset";
        DisableAutoResetCheckbox.TabIndex = 0;
        DisableAutoResetCheckbox.UseVisualStyleBackColor = true;
        DisableAutoResetCheckbox.CheckedChanged += DisableAutoResetCheckboxCheckedChanged;

        // GameVersionLabel
        GameVersionLabel.AutoSize = true;
        GameVersionLabel.Location = new Point(10, 92);
        GameVersionLabel.Name = "GameVersionLabel";
        GameVersionLabel.Size = new Size(186, 13);
        GameVersionLabel.TabIndex = 1;
        GameVersionLabel.Text = "Game Version: Unknown/Undetected";

        // AutosplitterVersionLabel
        AutosplitterVersionLabel.AutoSize = true;
        AutosplitterVersionLabel.Location = new Point(10, 118);
        AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
        AutosplitterVersionLabel.Size = new Size(103, 110);
        AutosplitterVersionLabel.TabIndex = 2;
        AutosplitterVersionLabel.Text = "Autosplitter Version: " + Assembly.GetCallingAssembly().GetName().Version;

        // ComponentSettings
        Controls.Add(AutosplitterVersionLabel);
        Controls.Add(GameVersionLabel);
        Controls.Add(DisableAutoResetCheckbox);
        Controls.Add(ModeSelect);
        Name = "ComponentSettings";
        Size = new Size(350, 145);
        ModeSelect.ResumeLayout(false);
        ModeSelect.PerformLayout();

        ResumeLayout(false);
        PerformLayout();
    }

    public void SetGameVersion(uint version)
    {
        string versionText = (GameVersion)version switch
        {
            GameVersion.InitialPublicRelease => "Initial Public Release",
            _ => "Unknown/Undetected",
        };
        GameVersionLabel.Text = "Game Version: " + versionText;
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
        
    private void DisableAutoResetCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox)sender;
        DisableAutoReset = checkbox.Checked;
    }
}