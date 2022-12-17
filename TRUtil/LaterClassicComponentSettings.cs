using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TRUtil;

public class LaterClassicComponentSettings : UserControl
{
    protected GroupBox ModeSelect;
    public RadioButton ILModeButton;
    public RadioButton FullGameModeButton;
    public RadioButton DeathrunModeButton;
    public CheckBox OptionCheckbox;
    public CheckBox DisableAutoResetCheckbox;
    public Label GameVersionLabel;
    public Label AutosplitterVersionLabel;
    public bool FullGame = true;
    public bool Deathrun;
    public bool Option;
    public bool DisableAutoReset = true;

    public LaterClassicComponentSettings() => InitializeComponent();

    private void InitializeComponent()
    {
        ModeSelect = new GroupBox();
        FullGameModeButton = new RadioButton();
        ILModeButton = new RadioButton();
        DeathrunModeButton = new RadioButton();
        OptionCheckbox = new CheckBox();
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
        Controls.Add(OptionCheckbox);
        Controls.Add(DisableAutoResetCheckbox);
        Controls.Add(ModeSelect);
        Name = "LaterClassicComponentSettings";
        Size = new Size(350, 145);
        ModeSelect.ResumeLayout(false);
        ModeSelect.PerformLayout();
            
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
            
        // OptionCheckbox
        OptionCheckbox.AutoSize = true;
        OptionCheckbox.Checked = false;
        OptionCheckbox.Location = new Point(130, 64);
        OptionCheckbox.Size = new Size(72, 17);
        OptionCheckbox.Name = "OptionCheckbox";
        OptionCheckbox.Text = "Option";
        OptionCheckbox.TabIndex = 0;
        OptionCheckbox.UseVisualStyleBackColor = true;
        OptionCheckbox.CheckedChanged += OptionCheckboxCheckedChanged;

        ResumeLayout(false);
        PerformLayout();
    }
        
    public virtual void SetGameVersion(uint version)
    {
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

    private void OptionCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox)sender;
        Option = checkbox.Checked;
    }
        
    private void DisableAutoResetCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox)sender;
        DisableAutoReset = checkbox.Checked;
    }
}