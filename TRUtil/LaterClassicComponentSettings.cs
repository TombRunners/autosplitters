using System;
using System.Reflection;
using System.Windows.Forms;

namespace TRUtil
{
    public class LaterClassicComponentSettings : UserControl
    {
        protected GroupBox _modeSelect;
        public RadioButton ILModeButton;
        public RadioButton FullGameModeButton;
        public RadioButton DeathrunModeButton;
        public CheckBox OptionCheckbox;
        public Label GameVersionLabel;
        public Label AutosplitterVersionLabel;
        public bool FullGame = true;
        public bool Deathrun;
        public bool Option;

        public LaterClassicComponentSettings() => InitializeComponent();

        private void InitializeComponent()
        {
            _modeSelect = new GroupBox();
            ILModeButton = new RadioButton();
            FullGameModeButton = new RadioButton();
            DeathrunModeButton = new RadioButton();
            OptionCheckbox = new CheckBox();
            GameVersionLabel = new Label();
            AutosplitterVersionLabel = new Label();
            _modeSelect.SuspendLayout();
            SuspendLayout();

            // _modeSelect
            _modeSelect.Controls.Add(ILModeButton);
            _modeSelect.Controls.Add(FullGameModeButton);
            _modeSelect.Controls.Add(DeathrunModeButton);
            _modeSelect.Location = new System.Drawing.Point(4, 4);
            _modeSelect.Name = "_modeSelect";
            _modeSelect.Size = new System.Drawing.Size(297, 53);
            _modeSelect.TabIndex = 0;
            _modeSelect.TabStop = false;
            _modeSelect.Text = "Mode Selection";

            // ILModeButton
            ILModeButton.AutoSize = true;
            ILModeButton.Location = new System.Drawing.Point(84, 20);
            ILModeButton.Name = "ILModeButton";
            ILModeButton.Size = new System.Drawing.Size(139, 17);
            ILModeButton.TabIndex = 1;
            ILModeButton.Text = "IL or Section Run (RTA)";
            ILModeButton.UseVisualStyleBackColor = true;
            ILModeButton.CheckedChanged += ILModeButtonCheckedChanged;

            // FullGameModeButton
            FullGameModeButton.AutoSize = true;
            FullGameModeButton.Checked = true;
            FullGameModeButton.Location = new System.Drawing.Point(6, 20);
            FullGameModeButton.Name = "FullGameModeButton";
            FullGameModeButton.Size = new System.Drawing.Size(72, 17);
            FullGameModeButton.TabIndex = 0;
            FullGameModeButton.TabStop = true;
            FullGameModeButton.Text = "Full Game";
            FullGameModeButton.UseVisualStyleBackColor = true;
            FullGameModeButton.CheckedChanged += FullGameModeButtonCheckedChanged;

            // DeathrunModeButton
            DeathrunModeButton.AutoSize = true;
            DeathrunModeButton.Location = new System.Drawing.Point(225, 20);
            DeathrunModeButton.Name = "DeathrunModeButton";
            DeathrunModeButton.Size = new System.Drawing.Size(69, 17);
            DeathrunModeButton.TabIndex = 2;
            DeathrunModeButton.Text = "Deathrun";
            DeathrunModeButton.UseVisualStyleBackColor = true;
            DeathrunModeButton.CheckedChanged += DeathrunModeButtonCheckedChanged;

            // GameVersionLabel
            GameVersionLabel.AutoSize = true;
            GameVersionLabel.Location = new System.Drawing.Point(10, 92);
            GameVersionLabel.Name = "GameVersionLabel";
            GameVersionLabel.Size = new System.Drawing.Size(186, 13);
            GameVersionLabel.TabIndex = 1;
            GameVersionLabel.Text = "Game Version: Unknown/Undetected";

            // AutosplitterVersionLabel
            AutosplitterVersionLabel.AutoSize = true;
            AutosplitterVersionLabel.Location = new System.Drawing.Point(10, 118);
            AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
            AutosplitterVersionLabel.Size = new System.Drawing.Size(103, 110);
            AutosplitterVersionLabel.TabIndex = 2;
            AutosplitterVersionLabel.Text = "Autosplitter Version: " + Assembly.GetCallingAssembly().GetName().Version;

            // ComponentSettings
            Controls.Add(AutosplitterVersionLabel);
            Controls.Add(GameVersionLabel);
            Controls.Add(OptionCheckbox);
            Controls.Add(_modeSelect);
            Name = "LaterClassicComponentSettings";
            Size = new System.Drawing.Size(313, 145);
            _modeSelect.ResumeLayout(false);
            _modeSelect.PerformLayout();

            // OptionCheckbox
            OptionCheckbox.AutoSize = true;
            OptionCheckbox.Checked = false;
            OptionCheckbox.Location = new System.Drawing.Point(10, 64);
            OptionCheckbox.Size = new System.Drawing.Size(72, 17);
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
    }
}
