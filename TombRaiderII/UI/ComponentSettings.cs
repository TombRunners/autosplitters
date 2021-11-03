using System;
using System.Reflection;
using System.Windows.Forms;

namespace TR2.UI
{
    public class ComponentSettings : UserControl
    {
        private GroupBox _modeSelect;
        
        public bool CountersAdvanceSeparately
        {
            get => CheckBoxCountersAdvanceSeparately.Checked;
            set => CheckBoxCountersAdvanceSeparately.Checked = value;
        }

        public CheckBox CheckBoxCountersAdvanceSeparately;
        public Label GameVersionLabel;
        public Label AutoMultiCounterVersionLabel;
        
        public ComponentSettings() => InitializeComponent();

        private void InitializeComponent()
        {
            _modeSelect = new GroupBox();
            CheckBoxCountersAdvanceSeparately = new CheckBox();
            GameVersionLabel = new Label();
            AutoMultiCounterVersionLabel = new Label();
            _modeSelect.SuspendLayout();
            SuspendLayout();

            // _modeSelect
            _modeSelect.Controls.Add(CheckBoxCountersAdvanceSeparately);
            _modeSelect.Location = new System.Drawing.Point(4, 4);
            _modeSelect.Name = "_modeSelect";
            _modeSelect.Size = new System.Drawing.Size(297, 53);
            _modeSelect.TabIndex = 0;
            _modeSelect.TabStop = false;
            _modeSelect.Text = "Mode Selection";

            // CheckBoxCountersAdvanceSeparately
            CheckBoxCountersAdvanceSeparately.AutoSize = true;
            CheckBoxCountersAdvanceSeparately.Location = new System.Drawing.Point(7, 20);
            CheckBoxCountersAdvanceSeparately.Name = "CheckBoxCountersAdvanceSeparately";
            CheckBoxCountersAdvanceSeparately.Size = new System.Drawing.Size(139, 17);
            CheckBoxCountersAdvanceSeparately.TabIndex = 1;
            CheckBoxCountersAdvanceSeparately.Text = "Counters advance separately from splits";
            CheckBoxCountersAdvanceSeparately.UseVisualStyleBackColor = true;

            // GameVersionLabel
            GameVersionLabel.AutoSize = true;
            GameVersionLabel.Location = new System.Drawing.Point(10, 64);
            GameVersionLabel.Name = "GameVersionLabel";
            GameVersionLabel.Size = new System.Drawing.Size(186, 13);
            GameVersionLabel.TabIndex = 1;
            GameVersionLabel.Text = "Game Version: Unknown/Undetected";

            // AutoMultiCounterVersionLabel
            AutoMultiCounterVersionLabel.AutoSize = true;
            AutoMultiCounterVersionLabel.Location = new System.Drawing.Point(10, 87);
            AutoMultiCounterVersionLabel.Name = "AutoMultiCounterVersionLabel";
            AutoMultiCounterVersionLabel.Size = new System.Drawing.Size(103, 13);
            AutoMultiCounterVersionLabel.TabIndex = 2;
            AutoMultiCounterVersionLabel.Text = "AutoMultiCounter Version: " + Assembly.GetExecutingAssembly().GetName().Version;

            // ComponentSettings
            Controls.Add(AutoMultiCounterVersionLabel);
            Controls.Add(GameVersionLabel);
            Controls.Add(_modeSelect);
            Name = "ComponentSettings";
            Size = new System.Drawing.Size(313, 110);
            _modeSelect.ResumeLayout(false);
            _modeSelect.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        internal void SetGameVersion(uint version)
        {
            var gameVersion = (GameVersion)version;

            var versionText = "";
            switch (gameVersion)
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
                case GameVersion.None:
                    break;
                case GameVersion.StellaGold:
                    versionText = "Stella's TR2 Gold";
                    break;
                case GameVersion.StellaGoldCracked:
                    versionText = "Stella's TR2";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameVersion), gameVersion, null);
            }

            GameVersionLabel.Text = string.IsNullOrEmpty(versionText) ? "Game Version: Unknown/Undetected" : "Game Version: " + versionText;
        }
    }
}
