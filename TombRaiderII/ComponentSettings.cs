using System;
using System.Windows.Forms;

namespace TR2
{
    public class ComponentSettings : UserControl
    {
        private GroupBox _modeSelect;
        public RadioButton ILModeButton;
        public RadioButton FullGameModeButton;
        public RadioButton DeathrunModeButton;
        public Label GameVersionLabel;
        public bool FullGame = true;
        public bool Deathrun = false;

        public ComponentSettings() => InitializeComponent();

        private void InitializeComponent()
        {
            this._modeSelect = new System.Windows.Forms.GroupBox();
            this.ILModeButton = new System.Windows.Forms.RadioButton();
            this.FullGameModeButton = new System.Windows.Forms.RadioButton();
            this.DeathrunModeButton = new System.Windows.Forms.RadioButton();
            this.GameVersionLabel = new System.Windows.Forms.Label();
            this._modeSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // _modeSelect
            // 
            this._modeSelect.Controls.Add(this.ILModeButton);
            this._modeSelect.Controls.Add(this.FullGameModeButton);
            this._modeSelect.Controls.Add(this.DeathrunModeButton);
            this._modeSelect.Location = new System.Drawing.Point(4, 4);
            this._modeSelect.Name = "_modeSelect";
            this._modeSelect.Size = new System.Drawing.Size(297, 53);
            this._modeSelect.TabIndex = 0;
            this._modeSelect.TabStop = false;
            this._modeSelect.Text = "Mode Selection";
            // 
            // ILModeButton
            // 
            this.ILModeButton.AutoSize = true;
            this.ILModeButton.Location = new System.Drawing.Point(84, 20);
            this.ILModeButton.Name = "ILModeButton";
            this.ILModeButton.Size = new System.Drawing.Size(191, 17);
            this.ILModeButton.TabIndex = 1;
            this.ILModeButton.Text = "IL or Section Run (RTA)";
            this.ILModeButton.UseVisualStyleBackColor = true;
            // 
            // FullGameModeButton
            // 
            this.FullGameModeButton.AutoSize = true;
            this.FullGameModeButton.Checked = true;
            this.FullGameModeButton.Location = new System.Drawing.Point(6, 20);
            this.FullGameModeButton.Name = "FullGameModeButton";
            this.FullGameModeButton.Size = new System.Drawing.Size(72, 17);
            this.FullGameModeButton.TabIndex = 0;
            this.FullGameModeButton.TabStop = true;
            this.FullGameModeButton.Text = "Full Game";
            this.FullGameModeButton.UseVisualStyleBackColor = true;
            // 
            // DeathrunModeButton
            // 
            this.DeathrunModeButton.AutoSize = true;
            this.DeathrunModeButton.Location = new System.Drawing.Point(225, 20);
            this.DeathrunModeButton.Name = "DeathrunModeButton";
            this.DeathrunModeButton.Size = new System.Drawing.Size(69, 17);
            this.DeathrunModeButton.TabIndex = 2;
            this.DeathrunModeButton.Text = "Deathrun";
            this.DeathrunModeButton.UseVisualStyleBackColor = true;
            // 
            // GameVersionLabel
            // 
            this.GameVersionLabel.AutoSize = true;
            this.GameVersionLabel.Location = new System.Drawing.Point(10, 64);
            this.GameVersionLabel.Name = "GameVersionLabel";
            this.GameVersionLabel.Size = new System.Drawing.Size(186, 13);
            this.GameVersionLabel.TabIndex = 1;
            this.GameVersionLabel.Text = "Game Version: Unknown/Undetected";
            // 
            // ComponentSettings
            // 
            this.Controls.Add(this.GameVersionLabel);
            this.Controls.Add(this._modeSelect);
            this.Name = "ComponentSettings";
            this.Size = new System.Drawing.Size(313, 90);
            this._modeSelect.ResumeLayout(false);
            this._modeSelect.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal void SetGameVersion(GameVersion? version)
        {
            string versionText = "";
            switch (version)
            {
                case GameVersion.MP:
                    versionText = "Multipatch";
                    break;
                case GameVersion.EPC:
                    versionText = "Eidos Premier Collection";
                    break;
                case GameVersion.P1:
                    versionText = "CORE's Patch 1";
                    break;
                case GameVersion.UKB:
                    versionText = "Eidos UK Box";
                    break;
            }
            GameVersionLabel.Text = string.IsNullOrEmpty(versionText) ? "Game Version: Unknown/Undetected" : "Game Version: " + versionText;
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
    }
}
