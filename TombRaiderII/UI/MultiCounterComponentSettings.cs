using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class MultiCounterComponentSettings : UserControl
    {
        readonly EventLog log = new EventLog("Application")
        {
            Source = "LiveSplit"
        };

        public Font TextFont { get; set; }

        public string TextFontString => SettingsHelper.FormatFont(TextFont);

        public bool OverrideFont
        {
            get => CheckBoxFontOverride.Checked;
            set => CheckBoxFontOverride.Checked = value;
        }

        public Color NameColor
        {
            get => ButtonNameColor.BackColor;
            set => ButtonNameColor.BackColor = value;
        }

        public Color ValueColor
        {
            get => ButtonValueColor.BackColor;
            set => ButtonValueColor.BackColor = value;
        }

        public bool OverrideTextColor
        {
            get => CheckBoxForegroundColorOverride.Checked;
            set => CheckBoxForegroundColorOverride.Checked = value;
        }

        public bool UseThinSeparators
        {
            get => CheckBoxUseThinSeparators.Checked;
            set => CheckBoxUseThinSeparators.Checked = value;
        }

        /// <remarks>
        ///     If <see cref="BackgroundGradient"/> != <see cref="ExtendedGradientType.Plain"/>, <see cref="ButtonBackgroundColor1"/>
        ///     will be hidden, so <see cref="ButtonBackgroundColor2"/> should be used.
        /// </remarks>
        public Color BackgroundColor1
        {
            get => BackgroundGradient != ExtendedGradientType.Plain ? ButtonBackgroundColor1.BackColor : ButtonBackgroundColor2.BackColor;
            set
            {
                ButtonBackgroundColor1.BackColor = value;
                if (BackgroundGradient != ExtendedGradientType.Plain)
                    BackgroundColor2 = value;
            }
        }

        /// <remarks>
        ///     <see cref="ButtonBackgroundColor2"/> is visible regardless of <see cref="BackgroundGradient"/>.
        /// </remarks>
        public Color BackgroundColor2
        {
            get => ButtonBackgroundColor2.BackColor;
            set => ButtonBackgroundColor2.BackColor = value;
        }

        public ExtendedGradientType BackgroundGradient { get; set; }

        public event EventHandler CounterLayoutChanged;

        public MultiCounterComponentSettings()
        {
            InitializeComponent();

            // Set default values.
            TextFont = DefaultFont;
            OverrideFont = false;
            NameColor = Color.FromArgb(255, 255, 255);
            ValueColor = Color.FromArgb(255, 255, 255);
            OverrideTextColor = false;
            BackgroundColor1 = Color.Transparent;
            BackgroundColor2 = Color.Transparent;
            BackgroundGradient = ExtendedGradientType.Plain;

            LabelFontSample.Font = OverrideFont ? TextFont : DefaultFont;
            LabelFontSample.Text = TextFontString;

            Load += CounterSettings_Load;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            BackgroundColor1 = SettingsHelper.ParseColor(element["BackgroundColor1"], Color.Transparent);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.Transparent);
            BackgroundGradient = SettingsHelper.ParseEnum(element["BackgroundGradient"], ExtendedGradientType.Plain);
            UseThinSeparators = SettingsHelper.ParseBool(element["UseThinSeparators"], true);
            OverrideFont = SettingsHelper.ParseBool(element["OverrideTextFont"], false);
            TextFont = SettingsHelper.GetFontFromElement(element["TextFont"]);
            OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"], false);
            NameColor = SettingsHelper.ParseColor(element["NameColor"], Color.FromArgb(255, 255, 255));
            ValueColor = SettingsHelper.ParseColor(element["ValueColor"], Color.FromArgb(255, 255, 255));
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent) =>
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor1", BackgroundColor1) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "UseThinSeparators", UseThinSeparators) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTextFont", OverrideFont) ^
            SettingsHelper.CreateSetting(document, parent, "TextFont", TextFont) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
            SettingsHelper.CreateSetting(document, parent, "NameColor", NameColor) ^
            SettingsHelper.CreateSetting(document, parent, "ValueColor", ValueColor);

        private TableLayoutPanel MainTableLayoutPanel;
        private Label LabelBackgroundColor;
        private Button ButtonBackgroundColor1;
        private Button ButtonBackgroundColor2;
        private ComboBox ComboBoxGradientType;
        private CheckBox CheckBoxUseThinSeparators;
        private GroupBox GroupBoxFont;
        private TableLayoutPanel TableLayoutPanelFont;
        private CheckBox CheckBoxFontOverride;
        private Label LabelFont;
        private Label LabelFontSample;
        private Button ButtonFont;
        private GroupBox GroupBoxForegroundColor;
        private TableLayoutPanel TableLayoutPanelForegroundColor;
        private CheckBox CheckBoxForegroundColorOverride;
        private Label LabelNameColor;
        private Button ButtonNameColor;
        private Label LabelValueColor;
        private Button ButtonValueColor;
        private Label AutoMultiCounterVersionLabel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            MainTableLayoutPanel = new TableLayoutPanel();
            GroupBoxForegroundColor = new GroupBox();
            AutoMultiCounterVersionLabel = new Label();
            TableLayoutPanelForegroundColor = new TableLayoutPanel();
            CheckBoxForegroundColorOverride = new CheckBox();
            ButtonNameColor = new Button();
            LabelNameColor = new Label();
            LabelValueColor = new Label();
            ButtonValueColor = new Button();
            GroupBoxFont = new GroupBox();
            TableLayoutPanelFont = new TableLayoutPanel();
            LabelFontSample = new Label();
            ButtonFont = new Button();
            CheckBoxFontOverride = new CheckBox();
            LabelFont = new Label();
            CheckBoxUseThinSeparators = new CheckBox();
            ComboBoxGradientType = new ComboBox();
            LabelBackgroundColor = new Label();
            ButtonBackgroundColor1 = new Button();
            ButtonBackgroundColor2 = new Button();
            MainTableLayoutPanel.SuspendLayout();
            GroupBoxForegroundColor.SuspendLayout();
            TableLayoutPanelForegroundColor.SuspendLayout();
            GroupBoxFont.SuspendLayout();
            TableLayoutPanelFont.SuspendLayout();
            SuspendLayout();

            // MainTableLayoutPanel
            MainTableLayoutPanel.ColumnCount = 4;
            MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 151F));
            MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 29F));
            MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 29F));
            MainTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MainTableLayoutPanel.Controls.Add(LabelBackgroundColor, 0, 0);
            MainTableLayoutPanel.Controls.Add(ButtonBackgroundColor1, 1, 0);
            MainTableLayoutPanel.Controls.Add(ButtonBackgroundColor2, 2, 0);
            MainTableLayoutPanel.Controls.Add(ComboBoxGradientType, 3, 0);
            MainTableLayoutPanel.Controls.Add(CheckBoxUseThinSeparators, 0, 1);
            MainTableLayoutPanel.Controls.Add(GroupBoxFont, 0, 2);
            MainTableLayoutPanel.Controls.Add(GroupBoxForegroundColor, 0, 3);
            MainTableLayoutPanel.Controls.Add(AutoMultiCounterVersionLabel, 0, 4);
            MainTableLayoutPanel.Dock = DockStyle.Fill;
            MainTableLayoutPanel.Location = new Point(7, 7);
            MainTableLayoutPanel.Name = "MainTableLayoutPanel";
            MainTableLayoutPanel.RowCount = 5;
            MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 82F));
            MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
            MainTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            MainTableLayoutPanel.Size = new Size(442, 282);
            MainTableLayoutPanel.TabIndex = 0;

            // GroupBoxColor
            MainTableLayoutPanel.SetColumnSpan(GroupBoxForegroundColor, 4);
            GroupBoxForegroundColor.Controls.Add(TableLayoutPanelForegroundColor);
            GroupBoxForegroundColor.Dock = DockStyle.Fill;
            GroupBoxForegroundColor.Location = new Point(3, 114);
            GroupBoxForegroundColor.Name = "GroupBoxColor";
            GroupBoxForegroundColor.Size = new Size(436, 106);
            GroupBoxForegroundColor.TabIndex = 4;
            GroupBoxForegroundColor.TabStop = false;
            GroupBoxForegroundColor.Text = "Counter Colors";

            // TableLayoutPanelColor
            TableLayoutPanelForegroundColor.ColumnCount = 2;
            TableLayoutPanelForegroundColor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            TableLayoutPanelForegroundColor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TableLayoutPanelForegroundColor.Controls.Add(CheckBoxForegroundColorOverride, 0, 0);
            TableLayoutPanelForegroundColor.Controls.Add(ButtonNameColor, 1, 1);
            TableLayoutPanelForegroundColor.Controls.Add(LabelNameColor, 0, 1);
            TableLayoutPanelForegroundColor.Controls.Add(LabelValueColor, 0, 2);
            TableLayoutPanelForegroundColor.Controls.Add(ButtonValueColor, 1, 2);
            TableLayoutPanelForegroundColor.Dock = DockStyle.Fill;
            TableLayoutPanelForegroundColor.Location = new Point(3, 16);
            TableLayoutPanelForegroundColor.Name = "TableLayoutPanelColor";
            TableLayoutPanelForegroundColor.RowCount = 3;
            TableLayoutPanelForegroundColor.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            TableLayoutPanelForegroundColor.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            TableLayoutPanelForegroundColor.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            TableLayoutPanelForegroundColor.Size = new Size(430, 87);
            TableLayoutPanelForegroundColor.TabIndex = 0;

            // CheckBoxForegroundColorOverride
            CheckBoxForegroundColorOverride.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CheckBoxForegroundColorOverride.AutoSize = true;
            TableLayoutPanelForegroundColor.SetColumnSpan(CheckBoxForegroundColorOverride, 2);
            CheckBoxForegroundColorOverride.Location = new Point(7, 6);
            CheckBoxForegroundColorOverride.Margin = new Padding(7, 3, 3, 3);
            CheckBoxForegroundColorOverride.Name = "CheckBoxForegroundColorOverride";
            CheckBoxForegroundColorOverride.Size = new Size(420, 17);
            CheckBoxForegroundColorOverride.TabIndex = 0;
            CheckBoxForegroundColorOverride.Text = "Override Layout Settings";
            CheckBoxForegroundColorOverride.UseVisualStyleBackColor = true;
            CheckBoxForegroundColorOverride.CheckedChanged += CheckBoxColor_CheckedChanged;

            // ButtonNameColor
            ButtonNameColor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ButtonNameColor.FlatStyle = FlatStyle.Popup;
            ButtonNameColor.Location = new Point(148, 32);
            ButtonNameColor.Name = "ButtonNameColor";
            ButtonNameColor.Size = new Size(23, 23);
            ButtonNameColor.TabIndex = 1;
            ButtonNameColor.UseVisualStyleBackColor = false;
            ButtonNameColor.Click += ColorButton_Click;

            // LabelNameColor
            LabelNameColor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            LabelNameColor.AutoSize = true;
            LabelNameColor.Location = new Point(3, 37);
            LabelNameColor.Name = "LabelNameColor";
            LabelNameColor.Size = new Size(139, 13);
            LabelNameColor.TabIndex = 11;
            LabelNameColor.Text = "Text Color:";

            // LabelValueColor
            LabelValueColor.Anchor = AnchorStyles.Left;
            LabelValueColor.AutoSize = true;
            LabelValueColor.Location = new Point(3, 66);
            LabelValueColor.Name = "LabelValueColor";
            LabelValueColor.Size = new Size(64, 13);
            LabelValueColor.TabIndex = 12;
            LabelValueColor.Text = "Value Color:";

            // ButtonValueColor
            ButtonValueColor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ButtonValueColor.FlatStyle = FlatStyle.Popup;
            ButtonValueColor.Location = new Point(148, 61);
            ButtonValueColor.Name = "ButtonValueColor";
            ButtonValueColor.Size = new Size(23, 23);
            ButtonValueColor.TabIndex = 13;
            ButtonValueColor.UseVisualStyleBackColor = false;
            ButtonValueColor.Click += ColorButton_Click;

            // GroupBoxFont
            MainTableLayoutPanel.SetColumnSpan(GroupBoxFont, 4);
            GroupBoxFont.Controls.Add(TableLayoutPanelFont);
            GroupBoxFont.Dock = DockStyle.Fill;
            GroupBoxFont.Location = new Point(3, 32);
            GroupBoxFont.Name = "GroupBoxFont";
            GroupBoxFont.Size = new Size(436, 76);
            GroupBoxFont.TabIndex = 3;
            GroupBoxFont.TabStop = false;
            GroupBoxFont.Text = "Counter Font";

            // TableLayoutPanelFont
            TableLayoutPanelFont.ColumnCount = 3;
            TableLayoutPanelFont.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            TableLayoutPanelFont.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TableLayoutPanelFont.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 81F));
            TableLayoutPanelFont.Controls.Add(LabelFontSample, 1, 1);
            TableLayoutPanelFont.Controls.Add(ButtonFont, 2, 1);
            TableLayoutPanelFont.Controls.Add(CheckBoxFontOverride, 0, 0);
            TableLayoutPanelFont.Controls.Add(LabelFont, 0, 1);
            TableLayoutPanelFont.Dock = DockStyle.Fill;
            TableLayoutPanelFont.Location = new Point(3, 16);
            TableLayoutPanelFont.Name = "TableLayoutPanelFont";
            TableLayoutPanelFont.RowCount = 2;
            TableLayoutPanelFont.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            TableLayoutPanelFont.RowStyles.Add(new RowStyle(SizeType.Absolute, 29F));
            TableLayoutPanelFont.Size = new Size(430, 57);
            TableLayoutPanelFont.TabIndex = 0;

            // LabelFontSample
            LabelFontSample.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            LabelFontSample.AutoSize = true;
            LabelFontSample.Location = new Point(148, 37);
            LabelFontSample.Name = "LabelFontSample";
            LabelFontSample.Size = new Size(198, 13);
            LabelFontSample.TabIndex = 4;
            LabelFontSample.Text = "Font";

            // ButtonFont
            ButtonFont.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            ButtonFont.Location = new Point(352, 32);
            ButtonFont.Name = "ButtonFont";
            ButtonFont.Size = new Size(75, 23);
            ButtonFont.TabIndex = 1;
            ButtonFont.Text = "Choose...";
            ButtonFont.UseVisualStyleBackColor = true;
            ButtonFont.Click += FontButton_Click;

            // CheckBoxFontOverride
            CheckBoxFontOverride.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CheckBoxFontOverride.AutoSize = true;
            TableLayoutPanelFont.SetColumnSpan(CheckBoxFontOverride, 2);
            CheckBoxFontOverride.Location = new Point(7, 6);
            CheckBoxFontOverride.Margin = new Padding(7, 3, 3, 3);
            CheckBoxFontOverride.Name = "CheckBoxFontOverride";
            CheckBoxFontOverride.Size = new Size(339, 17);
            CheckBoxFontOverride.TabIndex = 0;
            CheckBoxFontOverride.Text = "Override Layout Settings";
            CheckBoxFontOverride.UseVisualStyleBackColor = true;
            CheckBoxFontOverride.CheckedChanged += CheckBoxFont_CheckedChanged;

            // LabelFont
            LabelFont.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            LabelFont.AutoSize = true;
            LabelFont.Location = new Point(3, 37);
            LabelFont.Name = "LabelFont";
            LabelFont.Size = new Size(139, 13);
            LabelFont.TabIndex = 5;
            LabelFont.Text = "Font:";

            // CheckBoxUseThinSeparators
            CheckBoxUseThinSeparators.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            CheckBoxUseThinSeparators.AutoSize = true;
            TableLayoutPanelForegroundColor.SetColumnSpan(CheckBoxUseThinSeparators, 2);           
            CheckBoxUseThinSeparators.Location = new Point(7, 6);
            CheckBoxUseThinSeparators.Margin = new Padding(7, 3, 3, 3);
            CheckBoxUseThinSeparators.Name = "CheckBoxUseThinSeparators";
            CheckBoxUseThinSeparators.Size = new Size(420, 17);
            CheckBoxUseThinSeparators.TabIndex = 0;
            CheckBoxUseThinSeparators.Text = "Use Thin Separators";
            CheckBoxUseThinSeparators.UseVisualStyleBackColor = true;
            CheckBoxUseThinSeparators.Checked = true;
            CheckBoxUseThinSeparators.CheckedChanged += CheckBoxUseThinSeparators_CheckedChanged;

            // ComboBoxGradientType
            ComboBoxGradientType.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            ComboBoxGradientType.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxGradientType.FormattingEnabled = true;
            ComboBoxGradientType.Items.AddRange(new object[] { "Plain", "Vertical", "Horizontal", "Alternating" });
            ComboBoxGradientType.SelectedIndex = 0;
            ComboBoxGradientType.Location = new Point(212, 4);
            ComboBoxGradientType.Name = "ComboBoxGradientType";
            ComboBoxGradientType.Size = new Size(227, 21);
            ComboBoxGradientType.TabIndex = 2;
            ComboBoxGradientType.SelectedIndexChanged += ComboBoxGradientType_SelectedIndexChanged;

            // LabelBackgroundColor
            LabelBackgroundColor.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            LabelBackgroundColor.AutoSize = true;
            LabelBackgroundColor.Location = new Point(3, 8);
            LabelBackgroundColor.Name = "LabelBackgroundColor";
            LabelBackgroundColor.Size = new Size(145, 13);
            LabelBackgroundColor.TabIndex = 39;
            LabelBackgroundColor.Text = "Background Color:";

            // ButtonBackgroundColor1
            ButtonBackgroundColor1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ButtonBackgroundColor1.FlatStyle = FlatStyle.Popup;
            ButtonBackgroundColor1.Location = new Point(154, 3);
            ButtonBackgroundColor1.Name = "ButtonBackgroundColor1";
            ButtonBackgroundColor1.Size = new Size(23, 23);
            ButtonBackgroundColor1.TabIndex = 0;
            ButtonBackgroundColor1.UseVisualStyleBackColor = false;
            ButtonBackgroundColor1.Click += ColorButton_Click;

            // ButtonBackgroundColor2
            ButtonBackgroundColor2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ButtonBackgroundColor2.FlatStyle = FlatStyle.Popup;
            ButtonBackgroundColor2.Location = new Point(183, 3);
            ButtonBackgroundColor2.Name = "ButtonBackgroundColor2";
            ButtonBackgroundColor2.Size = new Size(23, 23);
            ButtonBackgroundColor2.TabIndex = 1;
            ButtonBackgroundColor2.UseVisualStyleBackColor = false;
            ButtonBackgroundColor2.Click += ColorButton_Click;

            // AutoMultiCounterVersionLabel
            TableLayoutPanelFont.SetColumnSpan(AutoMultiCounterVersionLabel, 4);
            AutoMultiCounterVersionLabel.AutoSize = true;
            AutoMultiCounterVersionLabel.Location = new Point(3, 223);
            AutoMultiCounterVersionLabel.Name = "AutosplitterVersionLabel";
            AutoMultiCounterVersionLabel.Size = new Size(35, 30);
            AutoMultiCounterVersionLabel.TabIndex = 40;
            AutoMultiCounterVersionLabel.Text = "Auto Multi-Counter Version: " + Assembly.GetExecutingAssembly().GetName().Version;

            // MultiCounterComponentSettings
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(MainTableLayoutPanel);
            Name = "MultiCounterComponentSettings";
            Padding = new Padding(7);
            Size = new Size(456, 300);
            MainTableLayoutPanel.ResumeLayout(false);
            MainTableLayoutPanel.PerformLayout();
            GroupBoxForegroundColor.ResumeLayout(false);
            TableLayoutPanelForegroundColor.ResumeLayout(false);
            TableLayoutPanelForegroundColor.PerformLayout();
            GroupBoxFont.ResumeLayout(false);
            TableLayoutPanelFont.ResumeLayout(false);
            TableLayoutPanelFont.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private void ColorButton_Click(object sender, EventArgs e)
            => SettingsHelper.ColorButtonClick((Button)sender, this);

        private void ComboBoxGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var gradientText = ComboBoxGradientType.SelectedItem.ToString();
            BackgroundGradient = (ExtendedGradientType)Enum.Parse(typeof(ExtendedGradientType), gradientText);
            ButtonBackgroundColor1.Visible = gradientText != "Plain";
        }
        
        private void CheckBoxUseThinSeparators_CheckedChanged(object sender, EventArgs e)
        {
            UseThinSeparators = CheckBoxUseThinSeparators.Checked;
            CounterLayoutChanged(this, null);
        }

        private void FontButton_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(TextFont, 7, 20);
            dialog.FontChanged += (s, ev) => TextFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            LabelFontSample.Text = TextFontString;
        }

        private void CheckBoxFont_CheckedChanged(object sender, EventArgs e)
            => LabelFont.Enabled = LabelFontSample.Enabled = ButtonFont.Enabled = CheckBoxFontOverride.Checked;

        private void CheckBoxColor_CheckedChanged(object sender, EventArgs e)
            => LabelNameColor.Enabled = ButtonNameColor.Enabled = LabelValueColor.Enabled = ButtonValueColor.Enabled = CheckBoxForegroundColorOverride.Checked;

        private void CounterSettings_Load(object sender, EventArgs e)
        {
            LabelFontSample.Text = TextFontString;
            CheckBoxUseThinSeparators.Checked = UseThinSeparators;

            // Force a check on background color elements' visibility.
            ComboBoxGradientType.SelectedItem = BackgroundGradient.ToString();
            // Force enable/disable of foreground color elements.
            CheckBoxForegroundColorOverride.Checked = OverrideTextColor;
            CheckBoxColor_CheckedChanged(null, null);
            // Force enable/disable of font elements.
            CheckBoxFontOverride.Checked = OverrideFont;
            CheckBoxFont_CheckedChanged(null, null);
        }
    }
}
