using System;
using System.Windows.Forms;

namespace TR1
{
    public class ComponentSettings : UserControl
    {
        private GroupBox _modeSelect;
        public RadioButton _ilModeButton;
        public RadioButton _fullGameModeButton;
        public bool FullGame;

        public ComponentSettings()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this._modeSelect = new System.Windows.Forms.GroupBox();
            this._ilModeButton = new System.Windows.Forms.RadioButton();
            this._fullGameModeButton = new System.Windows.Forms.RadioButton();
            this._modeSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // _modeSelect
            // 
            this._modeSelect.Controls.Add(this._ilModeButton);
            this._modeSelect.Controls.Add(this._fullGameModeButton);
            this._modeSelect.Location = new System.Drawing.Point(4, 4);
            this._modeSelect.Name = "_modeSelect";
            this._modeSelect.Size = new System.Drawing.Size(225, 53);
            this._modeSelect.TabIndex = 0;
            this._modeSelect.TabStop = false;
            this._modeSelect.Text = "Mode Selection";
            // 
            // _ilModeButton
            // 
            this._ilModeButton.AutoSize = true;
            this._ilModeButton.Location = new System.Drawing.Point(84, 20);
            this._ilModeButton.Name = "_ilModeButton";
            this._ilModeButton.Size = new System.Drawing.Size(135, 17);
            this._ilModeButton.TabIndex = 1;
            this._ilModeButton.TabStop = true;
            this._ilModeButton.Text = "Individual Levels (RTA)";
            this._ilModeButton.UseVisualStyleBackColor = true;
            this._ilModeButton.CheckedChanged += new System.EventHandler(this.ILModeButtonCheckedChanged);
            // 
            // _fullGameModeButton
            // 
            this._fullGameModeButton.AutoSize = true;
            this._fullGameModeButton.Location = new System.Drawing.Point(6, 20);
            this._fullGameModeButton.Name = "_fullGameModeButton";
            this._fullGameModeButton.Size = new System.Drawing.Size(72, 17);
            this._fullGameModeButton.TabIndex = 0;
            this._fullGameModeButton.TabStop = true;
            this._fullGameModeButton.Text = "Full Game";
            this._fullGameModeButton.UseVisualStyleBackColor = true;
            this._fullGameModeButton.CheckedChanged += new System.EventHandler(this.FullGameModeButtonCheckedChanged);
            // 
            // ComponentSettings
            // 
            this.Controls.Add(this._modeSelect);
            this.Name = "ComponentSettings";
            this.Size = new System.Drawing.Size(232, 60);
            this._modeSelect.ResumeLayout(false);
            this._modeSelect.PerformLayout();
            this.ResumeLayout(false);

        }

        private void FullGameModeButtonCheckedChanged(object sender, EventArgs e)
        {
            FullGame = true;
        }

        private void ILModeButtonCheckedChanged(object sender, EventArgs e)
        {
            FullGame = false;
        }
    }
}
