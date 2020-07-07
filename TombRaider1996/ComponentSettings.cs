using System;
using System.Windows.Forms;

namespace TR1
{
    public class ComponentSettings : UserControl
    {
        private GroupBox _modeSelect;
        public RadioButton ILModeButton;
        public RadioButton FullGameModeButton;
        public bool FullGame = true;

        public ComponentSettings() => InitializeComponent();

        private void InitializeComponent()
        {
            _modeSelect = new GroupBox();
            ILModeButton = new RadioButton();
            FullGameModeButton = new RadioButton();
            _modeSelect.SuspendLayout();
            SuspendLayout();

            // _modeSelect
            _modeSelect.Controls.Add(ILModeButton);
            _modeSelect.Controls.Add(FullGameModeButton);
            _modeSelect.Location = new System.Drawing.Point(4, 4);
            _modeSelect.Name = "_modeSelect";
            _modeSelect.Size = new System.Drawing.Size(225, 53);
            _modeSelect.TabIndex = 0;
            _modeSelect.TabStop = false;
            _modeSelect.Text = "Mode Selection";

            // ILModeButton
            ILModeButton.AutoSize = true;
            ILModeButton.Location = new System.Drawing.Point(84, 20);
            ILModeButton.Name = "ILModeButton";
            ILModeButton.Size = new System.Drawing.Size(135, 17);
            ILModeButton.TabIndex = 1;
            ILModeButton.Text = "Individual Levels (RTA)";
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

            // ComponentSettings
            Controls.Add(_modeSelect);
            Name = "ComponentSettings";
            Size = new System.Drawing.Size(232, 60);
            _modeSelect.ResumeLayout(false);
            _modeSelect.PerformLayout();
            ResumeLayout(false);
        }

        private void FullGameModeButtonCheckedChanged(object sender, EventArgs e) => FullGame = true;
        
        private void ILModeButtonCheckedChanged(object sender, EventArgs e) => FullGame = false;
    }
}
