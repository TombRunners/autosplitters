using System;
using System.Reflection;
using System.Windows.Forms;

namespace TRMarathon
{
    public class ComponentSettings : UserControl
    {        
        private OpenFileDialog _openFileDialog;
        private GroupBox _modeSelect;
        private RadioButton _fullGameModeButton;
        private RadioButton _ilModeButton;
        public bool FullGame = true;
        private TableLayoutPanel _gameSelectionTableLayout;
        private Label _gameSelectionColumnLabel;
        private Label _autoStartColumnLabel;
        private Label _gameLocationColumnLabel;
        private Label _gameOptionColumnLabel;
        private CheckBox _tr1CheckBox;
        private CheckBox _trUbCheckBox;
        private CheckBox _tr2CheckBox;
        private CheckBox _tr2GCheckBox;
        private CheckBox _tr3CheckBox;
        private CheckBox _tlaCheckBox;
        private CheckBox _tr4CheckBox;
        private CheckBox _tteCheckBox;
        private CheckBox _tr5CheckBox;
        private CheckBox _tr1AutoStartCheckBox;
        public bool Tr1AutoStart;
        private CheckBox _trUbAutoStartCheckBox;
        public bool TrUbAutoStart;
        private CheckBox _tr2AutoStartCheckBox;
        public bool Tr2AutoStart;
        private CheckBox _tr2GAutoStartCheckBox;
        public bool Tr2GAutoStart;
        private CheckBox _tr3AutoStartCheckBox;
        public bool Tr3AutoStart;
        private CheckBox _tlaAutoStartCheckBox;
        public bool TlaAutoStart;
        private CheckBox _tr4AutoStartCheckBox;
        public bool Tr4AutoStart;
        private CheckBox _tteAutoStartCheckBox;
        public bool TteAutoStart;
        private CheckBox _tr5AutoStartCheckBox;
        public bool Tr5AutoStart;
        private TableLayoutPanel _tr1FileTableLayout;
        private TextBox _tr1FileTextBox;
        public string Tr1FileLocation;
        private Button _tr1FileButton;
        private TableLayoutPanel _trUbFileTableLayout;
        private TextBox _trUbFileTextBox;
        public string TrUbFileLocation;
        private Button _trUbFileButton;
        private TableLayoutPanel _tr2FileTableLayout;
        private TextBox _tr2FileTextBox;
        public string Tr2FileLocation;
        private Button _tr2FileButton;
        private TableLayoutPanel _tr2GFileTableLayout;
        private TextBox _tr2GFileTextBox;
        public string Tr2GFileLocation;
        private Button _tr2GFileButton;
        private TableLayoutPanel _tr3FileTableLayout;
        private TextBox _tr3FileTextBox;
        public string Tr3FileLocation;
        private Button _tr3FileButton;
        private TableLayoutPanel _tlaFileTableLayout;
        private TextBox _tlaFileTextBox;
        public string TlaFileLocation;
        private Button _tlaFileButton;
        private TableLayoutPanel _tr4FileTableLayout;
        private TextBox _tr4FileTextBox;
        public string Tr4FileLocation;
        private Button _tr4FileButton;
        private TableLayoutPanel _tteFileTableLayout;
        private TextBox _tteFileTextBox;
        public string TteFileLocation;
        private Button _tteFileButton;
        private TableLayoutPanel _tr5FileTableLayout;
        private TextBox _tr5FileTextBox;
        public string Tr5FileLocation;
        private Button _tr5FileButton;
        private CheckBox _tr4GlitchlessCheckBox;
        public bool Tr4Glitchless;
        private CheckBox _tr5SplitCutsceneCheckBox;
        public bool Tr5SplitCutscene;
        private Label _gameVersionLabel;
        private Label _autosplitterVersionLabel;

        public ComponentSettings() => InitializeComponent();

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _openFileDialog = new OpenFileDialog();
            _modeSelect = new GroupBox();
            _fullGameModeButton = new RadioButton();
            _ilModeButton = new RadioButton();
            _gameSelectionTableLayout = new TableLayoutPanel();
            _gameSelectionColumnLabel = new Label();
            _autoStartColumnLabel = new Label();
            _gameLocationColumnLabel = new Label();
            _gameOptionColumnLabel = new Label();
            _tr1CheckBox = new CheckBox();
            _trUbCheckBox = new CheckBox();
            _tr2CheckBox = new CheckBox();
            _tr2GCheckBox = new CheckBox();
            _tr3CheckBox = new CheckBox();
            _tlaCheckBox = new CheckBox();
            _tr4CheckBox = new CheckBox();
            _tteCheckBox = new CheckBox();
            _tr5CheckBox = new CheckBox();
            _tr1AutoStartCheckBox = new CheckBox();
            _trUbAutoStartCheckBox = new CheckBox();
            _tr2AutoStartCheckBox = new CheckBox();
            _tr2GAutoStartCheckBox = new CheckBox();
            _tr3AutoStartCheckBox = new CheckBox();
            _tlaAutoStartCheckBox = new CheckBox();
            _tr4AutoStartCheckBox = new CheckBox();
            _tteAutoStartCheckBox = new CheckBox();
            _tr5AutoStartCheckBox = new CheckBox();
            _tr1FileTableLayout = new TableLayoutPanel();
            _tr1FileTextBox = new TextBox();
            _tr1FileButton = new Button();
            _trUbFileTableLayout = new TableLayoutPanel();
            _trUbFileTextBox = new TextBox();
            _trUbFileButton = new Button();
            _tr2FileTableLayout = new TableLayoutPanel();
            _tr2FileTextBox = new TextBox();
            _tr2FileButton = new Button();
            _tr2GFileTableLayout = new TableLayoutPanel();
            _tr2GFileTextBox = new TextBox();
            _tr2GFileButton = new Button();
            _tr3FileTableLayout = new TableLayoutPanel();
            _tr3FileTextBox = new TextBox();
            _tr3FileButton = new Button();
            _tlaFileTableLayout = new TableLayoutPanel();
            _tlaFileTextBox = new TextBox();
            _tlaFileButton = new Button();
            _tr4FileTableLayout = new TableLayoutPanel();
            _tr4FileTextBox = new TextBox();
            _tr4FileButton = new Button();
            _tteFileTableLayout = new TableLayoutPanel();
            _tteFileTextBox = new TextBox();
            _tteFileButton = new Button();
            _tr5FileTableLayout = new TableLayoutPanel();
            _tr5FileTextBox = new TextBox();
            _tr5FileButton = new Button();
            _tr4GlitchlessCheckBox = new CheckBox();
            _tr5SplitCutsceneCheckBox = new CheckBox();
            _gameVersionLabel = new Label();
            _autosplitterVersionLabel = new Label();
            _modeSelect.SuspendLayout();
            _gameSelectionTableLayout.SuspendLayout();
            _tr1FileTableLayout.SuspendLayout();
            _trUbFileTableLayout.SuspendLayout();
            _tr2FileTableLayout.SuspendLayout();
            _tr2GFileTableLayout.SuspendLayout();
            _tr3FileTableLayout.SuspendLayout();
            _tlaFileTableLayout.SuspendLayout();
            _tr4FileTableLayout.SuspendLayout();
            _tteFileTableLayout.SuspendLayout();
            _tr5FileTableLayout.SuspendLayout();
            SuspendLayout();

            // _openFileDialog
            _openFileDialog.Filter = "\"Executable Files|*.exe\"";

            // _modeSelect
            _modeSelect.Controls.Add(_fullGameModeButton);
            _modeSelect.Controls.Add(_ilModeButton);
            _modeSelect.Location = new System.Drawing.Point(4, 4);
            _modeSelect.Name = "_modeSelect";
            _modeSelect.Size = new System.Drawing.Size(473, 53);
            _modeSelect.TabIndex = 0;
            _modeSelect.TabStop = false;
            _modeSelect.Text = "Mode Selection";

            // _fullGameModeButton
            _fullGameModeButton.AutoSize = true;
            _fullGameModeButton.Checked = true;
            _fullGameModeButton.Location = new System.Drawing.Point(6, 20);
            _fullGameModeButton.Name = "_fullGameModeButton";
            _fullGameModeButton.Size = new System.Drawing.Size(72, 17);
            _fullGameModeButton.TabIndex = 0;
            _fullGameModeButton.TabStop = true;
            _fullGameModeButton.Text = "Full Game";
            _fullGameModeButton.UseVisualStyleBackColor = true;
            _fullGameModeButton.CheckedChanged += RunModeButtonCheckedChanged;

            // _ilModeButton
            _ilModeButton.AutoSize = true;
            _ilModeButton.Location = new System.Drawing.Point(84, 20);
            _ilModeButton.Name = "_ilModeButton";
            _ilModeButton.Size = new System.Drawing.Size(139, 17);
            _ilModeButton.TabIndex = 1;
            _ilModeButton.Text = "IL or Section Run (RTA)";
            _ilModeButton.UseVisualStyleBackColor = true;
            _ilModeButton.CheckedChanged += RunModeButtonCheckedChanged;

            // _gameVersionLabel
            _gameVersionLabel.AutoSize = true;
            _gameVersionLabel.Location = new System.Drawing.Point(3, 468);
            _gameVersionLabel.Name = "_gameVersionLabel";
            _gameVersionLabel.Size = new System.Drawing.Size(186, 13);
            _gameVersionLabel.TabIndex = 1;
            _gameVersionLabel.Text = "Game Version: Unknown/Undetected";

            // _autosplitterVersionLabel
            _autosplitterVersionLabel.AutoSize = true;
            _autosplitterVersionLabel.Location = new System.Drawing.Point(3, 491);
            _autosplitterVersionLabel.Name = "_autosplitterVersionLabel";
            _autosplitterVersionLabel.Size = new System.Drawing.Size(103, 13);
            _autosplitterVersionLabel.TabIndex = 2;
            _autosplitterVersionLabel.Text = "Autosplitter Version: " + Assembly.GetCallingAssembly().GetName().Version;

            // _gameSelectionTableLayout
            _gameSelectionTableLayout.Anchor = AnchorStyles.None;
            _gameSelectionTableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _gameSelectionTableLayout.ColumnCount = 4;
            _gameSelectionTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            _gameSelectionTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8F));
            _gameSelectionTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            _gameSelectionTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            _gameSelectionTableLayout.Controls.Add(_gameSelectionColumnLabel, 0, 0);
            _gameSelectionTableLayout.Controls.Add(_autoStartColumnLabel, 1, 0);
            _gameSelectionTableLayout.Controls.Add(_gameLocationColumnLabel, 2, 0);
            _gameSelectionTableLayout.Controls.Add(_gameOptionColumnLabel, 3, 0);
            _gameSelectionTableLayout.Controls.Add(_trUbCheckBox, 0, 2);
            _gameSelectionTableLayout.Controls.Add(_tr1CheckBox, 0, 1);
            _gameSelectionTableLayout.Controls.Add(_tr2CheckBox, 0, 3);
            _gameSelectionTableLayout.Controls.Add(_tr2GCheckBox, 0, 4);
            _gameSelectionTableLayout.Controls.Add(_tr3CheckBox, 0, 5);
            _gameSelectionTableLayout.Controls.Add(_tlaCheckBox, 0, 6);
            _gameSelectionTableLayout.Controls.Add(_tr4CheckBox, 0, 7);
            _gameSelectionTableLayout.Controls.Add(_tteCheckBox, 0, 8);
            _gameSelectionTableLayout.Controls.Add(_tr5CheckBox, 0, 9);
            _gameSelectionTableLayout.Controls.Add(_tr1AutoStartCheckBox, 1, 1);
            _gameSelectionTableLayout.Controls.Add(_trUbAutoStartCheckBox, 1, 2);
            _gameSelectionTableLayout.Controls.Add(_tr2AutoStartCheckBox, 1, 3);
            _gameSelectionTableLayout.Controls.Add(_tr2GAutoStartCheckBox, 1, 4);
            _gameSelectionTableLayout.Controls.Add(_tr3AutoStartCheckBox, 1, 5);
            _gameSelectionTableLayout.Controls.Add(_tlaAutoStartCheckBox, 1, 6);
            _gameSelectionTableLayout.Controls.Add(_tr4AutoStartCheckBox, 1, 7);
            _gameSelectionTableLayout.Controls.Add(_tteAutoStartCheckBox, 1, 8);
            _gameSelectionTableLayout.Controls.Add(_tr5AutoStartCheckBox, 1, 9);
            _gameSelectionTableLayout.Controls.Add(_tr1FileTableLayout, 2, 1);
            _gameSelectionTableLayout.Controls.Add(_trUbFileTableLayout, 2, 2);
            _gameSelectionTableLayout.Controls.Add(_tr2FileTableLayout, 2, 3);
            _gameSelectionTableLayout.Controls.Add(_tr2GFileTableLayout, 2, 4);
            _gameSelectionTableLayout.Controls.Add(_tr3FileTableLayout, 2, 5);
            _gameSelectionTableLayout.Controls.Add(_tlaFileTableLayout, 2, 6);
            _gameSelectionTableLayout.Controls.Add(_tr4FileTableLayout, 2, 7);
            _gameSelectionTableLayout.Controls.Add(_tteFileTableLayout, 2, 8);
            _gameSelectionTableLayout.Controls.Add(_tr5FileTableLayout, 2, 9);
            _gameSelectionTableLayout.Controls.Add(_tr4GlitchlessCheckBox, 3, 7);
            _gameSelectionTableLayout.Controls.Add(_tr5SplitCutsceneCheckBox, 3, 9);
            _gameSelectionTableLayout.Location = new System.Drawing.Point(0, 62);
            _gameSelectionTableLayout.Name = "_gameSelectionTableLayout";
            _gameSelectionTableLayout.RowCount = 10;
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            _gameSelectionTableLayout.Size = new System.Drawing.Size(480, 400);
            _gameSelectionTableLayout.TabIndex = 3;

            // _gameSelectionColumnLabel
            _gameSelectionColumnLabel.Anchor = AnchorStyles.None;
            _gameSelectionColumnLabel.AutoSize = true;
            _gameSelectionColumnLabel.Location = new System.Drawing.Point(26, 13);
            _gameSelectionColumnLabel.Name = "_gameSelectionColumnLabel";
            _gameSelectionColumnLabel.Size = new System.Drawing.Size(82, 13);
            _gameSelectionColumnLabel.TabIndex = 2;
            _gameSelectionColumnLabel.Text = "Game Selection";
            _gameSelectionColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // _autoStartColumnLabel
            _autoStartColumnLabel.Anchor = AnchorStyles.None;
            _autoStartColumnLabel.AutoSize = true;
            _autoStartColumnLabel.Location = new System.Drawing.Point(139, 7);
            _autoStartColumnLabel.Name = "_autoStartColumnLabel";
            _autoStartColumnLabel.Size = new System.Drawing.Size(29, 26);
            _autoStartColumnLabel.TabIndex = 6;
            _autoStartColumnLabel.Text = "Auto Start";
            _autoStartColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // _gameLocationColumnLabel
            _gameLocationColumnLabel.Anchor = AnchorStyles.None;
            _gameLocationColumnLabel.AutoSize = true;
            _gameLocationColumnLabel.Location = new System.Drawing.Point(234, 13);
            _gameLocationColumnLabel.Name = "_gameLocationColumnLabel";
            _gameLocationColumnLabel.Size = new System.Drawing.Size(79, 13);
            _gameLocationColumnLabel.TabIndex = 7;
            _gameLocationColumnLabel.Text = "Game Location";
            _gameLocationColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // _gameOptionColumnLabel
            _gameOptionColumnLabel.Anchor = AnchorStyles.None;
            _gameOptionColumnLabel.AutoSize = true;
            _gameOptionColumnLabel.Location = new System.Drawing.Point(392, 13);
            _gameOptionColumnLabel.Name = "_gameOptionColumnLabel";
            _gameOptionColumnLabel.Size = new System.Drawing.Size(69, 13);
            _gameOptionColumnLabel.TabIndex = 8;
            _gameOptionColumnLabel.Text = "Game Option";
            _gameOptionColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // _tr1CheckBox
            _tr1CheckBox.Anchor = AnchorStyles.Left;
            _tr1CheckBox.AutoSize = true;
            _tr1CheckBox.Location = new System.Drawing.Point(4, 50);
            _tr1CheckBox.Name = "_tr1CheckBox";
            _tr1CheckBox.Padding = new Padding(5, 0, 0, 0);
            _tr1CheckBox.Size = new System.Drawing.Size(98, 17);
            _tr1CheckBox.TabIndex = 0;
            _tr1CheckBox.Tag = "Tr1";
            _tr1CheckBox.Text = "Tomb Raider I";
            _tr1CheckBox.UseVisualStyleBackColor = true;
            _tr1CheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _trUbCheckBox
            _trUbCheckBox.Anchor = AnchorStyles.Left;
            _trUbCheckBox.AutoSize = true;
            _trUbCheckBox.Location = new System.Drawing.Point(4, 89);
            _trUbCheckBox.Name = "_trUbCheckBox";
            _trUbCheckBox.Padding = new Padding(5, 0, 0, 0);
            _trUbCheckBox.Size = new System.Drawing.Size(126, 17);
            _trUbCheckBox.TabIndex = 1;
            _trUbCheckBox.Tag = "TrUb";
            _trUbCheckBox.Text = "Unfinished Business";
            _trUbCheckBox.UseVisualStyleBackColor = true;
            _trUbCheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _tr2CheckBox
            _tr2CheckBox.Anchor = AnchorStyles.Left;
            _tr2CheckBox.AutoSize = true;
            _tr2CheckBox.Location = new System.Drawing.Point(4, 128);
            _tr2CheckBox.Name = "_tr2CheckBox";
            _tr2CheckBox.Padding = new Padding(5, 0, 0, 0);
            _tr2CheckBox.Size = new System.Drawing.Size(101, 17);
            _tr2CheckBox.TabIndex = 3;
            _tr2CheckBox.Tag = "Tr2";
            _tr2CheckBox.Text = "Tomb Raider II";
            _tr2CheckBox.UseVisualStyleBackColor = true;
            _tr2CheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _tr2GCheckBox
            _tr2GCheckBox.Anchor = AnchorStyles.Left;
            _tr2GCheckBox.AutoSize = true;
            _tr2GCheckBox.Location = new System.Drawing.Point(4, 167);
            _tr2GCheckBox.Name = "_tr2GCheckBox";
            _tr2GCheckBox.Padding = new Padding(5, 0, 0, 0);
            _tr2GCheckBox.Size = new System.Drawing.Size(94, 17);
            _tr2GCheckBox.TabIndex = 4;
            _tr2GCheckBox.Tag = "Tr2G";
            _tr2GCheckBox.Text = "Golden Mask";
            _tr2GCheckBox.UseVisualStyleBackColor = true;
            _tr2GCheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _tr3CheckBox
            _tr3CheckBox.Anchor = AnchorStyles.Left;
            _tr3CheckBox.AutoSize = true;
            _tr3CheckBox.Location = new System.Drawing.Point(4, 206);
            _tr3CheckBox.Name = "_tr3CheckBox";
            _tr3CheckBox.Padding = new Padding(5, 0, 0, 0);
            _tr3CheckBox.Size = new System.Drawing.Size(104, 17);
            _tr3CheckBox.TabIndex = 5;
            _tr3CheckBox.Tag = "Tr3";
            _tr3CheckBox.Text = "Tomb Raider III";
            _tr3CheckBox.UseVisualStyleBackColor = true;
            _tr3CheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _tlaCheckBox
            _tlaCheckBox.Anchor = AnchorStyles.Left;
            _tlaCheckBox.AutoSize = true;
            _tlaCheckBox.Location = new System.Drawing.Point(4, 245);
            _tlaCheckBox.Name = "_tlaCheckBox";
            _tlaCheckBox.Padding = new Padding(5, 0, 0, 0);
            _tlaCheckBox.Size = new System.Drawing.Size(87, 17);
            _tlaCheckBox.TabIndex = 9;
            _tlaCheckBox.Tag = "Tla";
            _tlaCheckBox.Text = "Lost Artifact";
            _tlaCheckBox.UseVisualStyleBackColor = true;
            _tlaCheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _tr4CheckBox
            _tr4CheckBox.Anchor = AnchorStyles.Left;
            _tr4CheckBox.AutoSize = true;
            _tr4CheckBox.Location = new System.Drawing.Point(4, 284);
            _tr4CheckBox.Name = "_tr4CheckBox";
            _tr4CheckBox.Padding = new Padding(5, 0, 0, 0);
            _tr4CheckBox.Size = new System.Drawing.Size(105, 17);
            _tr4CheckBox.TabIndex = 10;
            _tr4CheckBox.Tag = "Tr4";
            _tr4CheckBox.Text = "Tomb Raider IV";
            _tr4CheckBox.UseVisualStyleBackColor = true;
            _tr4CheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _tteCheckBox
            _tteCheckBox.Anchor = AnchorStyles.Left;
            _tteCheckBox.AutoSize = true;
            _tteCheckBox.Location = new System.Drawing.Point(4, 323);
            _tteCheckBox.Name = "_tteCheckBox";
            _tteCheckBox.Padding = new Padding(5, 0, 0, 0);
            _tteCheckBox.Size = new System.Drawing.Size(107, 17);
            _tteCheckBox.TabIndex = 11;
            _tteCheckBox.Tag = "Tte";
            _tteCheckBox.Text = "Times Exclusive";
            _tteCheckBox.UseVisualStyleBackColor = true;
            _tteCheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _tr5CheckBox
            _tr5CheckBox.Anchor = AnchorStyles.Left;
            _tr5CheckBox.AutoSize = true;
            _tr5CheckBox.Location = new System.Drawing.Point(4, 365);
            _tr5CheckBox.Name = "_tr5CheckBox";
            _tr5CheckBox.Padding = new Padding(5, 0, 0, 0);
            _tr5CheckBox.Size = new System.Drawing.Size(102, 17);
            _tr5CheckBox.TabIndex = 12;
            _tr5CheckBox.Tag = "Tr5";
            _tr5CheckBox.Text = "Tomb Raider V";
            _tr5CheckBox.UseVisualStyleBackColor = true;
            _tr5CheckBox.CheckedChanged += GameCheckBoxCheckedChanged;

            // _tr1AutoStartCheckBox
            _tr1AutoStartCheckBox.Anchor = AnchorStyles.None;
            _tr1AutoStartCheckBox.AutoSize = true;
            _tr1AutoStartCheckBox.Enabled = false;
            _tr1AutoStartCheckBox.Location = new System.Drawing.Point(146, 52);
            _tr1AutoStartCheckBox.Name = "_tr1AutoStartCheckBox";
            _tr1AutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _tr1AutoStartCheckBox.TabIndex = 13;
            _tr1AutoStartCheckBox.Tag = "Tr1";
            _tr1AutoStartCheckBox.UseVisualStyleBackColor = true;
            _tr1AutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;

            // _trUbAutoStartCheckBox
            _trUbAutoStartCheckBox.Anchor = AnchorStyles.None;
            _trUbAutoStartCheckBox.AutoSize = true;
            _trUbAutoStartCheckBox.Enabled = false;
            _trUbAutoStartCheckBox.Location = new System.Drawing.Point(146, 91);
            _trUbAutoStartCheckBox.Name = "_trUbAutoStartCheckBox";
            _trUbAutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _trUbAutoStartCheckBox.TabIndex = 14;
            _trUbAutoStartCheckBox.Tag = "TrUb";
            _trUbAutoStartCheckBox.UseVisualStyleBackColor = true;
            _trUbAutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;

            // _tr2AutoStartCheckBox
            _tr2AutoStartCheckBox.Anchor = AnchorStyles.None;
            _tr2AutoStartCheckBox.AutoSize = true;
            _tr2AutoStartCheckBox.Enabled = false;
            _tr2AutoStartCheckBox.Location = new System.Drawing.Point(146, 130);
            _tr2AutoStartCheckBox.Name = "_tr2AutoStartCheckBox";
            _tr2AutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _tr2AutoStartCheckBox.TabIndex = 15;
            _tr2AutoStartCheckBox.Tag = "Tr2";
            _tr2AutoStartCheckBox.UseVisualStyleBackColor = true;
            _tr2AutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;

            // _tr2GAutoStartCheckBox
            _tr2GAutoStartCheckBox.Anchor = AnchorStyles.None;
            _tr2GAutoStartCheckBox.AutoSize = true;
            _tr2GAutoStartCheckBox.Enabled = false;
            _tr2GAutoStartCheckBox.Location = new System.Drawing.Point(146, 169);
            _tr2GAutoStartCheckBox.Name = "_tr2GAutoStartCheckBox";
            _tr2GAutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _tr2GAutoStartCheckBox.TabIndex = 16;
            _tr2GAutoStartCheckBox.Tag = "Tr2G";
            _tr2GAutoStartCheckBox.UseVisualStyleBackColor = true;
            _tr2GAutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;

            // _tr3AutoStartCheckBox
            _tr3AutoStartCheckBox.Anchor = AnchorStyles.None;
            _tr3AutoStartCheckBox.AutoSize = true;
            _tr3AutoStartCheckBox.Enabled = false;
            _tr3AutoStartCheckBox.Location = new System.Drawing.Point(146, 208);
            _tr3AutoStartCheckBox.Name = "_tr3AutoStartCheckBox";
            _tr3AutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _tr3AutoStartCheckBox.TabIndex = 17;
            _tr3AutoStartCheckBox.Tag = "Tr3";
            _tr3AutoStartCheckBox.UseVisualStyleBackColor = true;
            _tr3AutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;

            // _tlaAutoStartCheckBox
            _tlaAutoStartCheckBox.Anchor = AnchorStyles.None;
            _tlaAutoStartCheckBox.AutoSize = true;
            _tlaAutoStartCheckBox.Enabled = false;
            _tlaAutoStartCheckBox.Location = new System.Drawing.Point(146, 247);
            _tlaAutoStartCheckBox.Name = "_tlaAutoStartCheckBox";
            _tlaAutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _tlaAutoStartCheckBox.TabIndex = 18;
            _tlaAutoStartCheckBox.Tag = "Tla";
            _tlaAutoStartCheckBox.UseVisualStyleBackColor = true;
            _tlaAutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;

            // _tr4AutoStartCheckBox
            _tr4AutoStartCheckBox.Anchor = AnchorStyles.None;
            _tr4AutoStartCheckBox.AutoSize = true;
            _tr4AutoStartCheckBox.Enabled = false;
            _tr4AutoStartCheckBox.Location = new System.Drawing.Point(146, 286);
            _tr4AutoStartCheckBox.Name = "_tr4AutoStartCheckBox";
            _tr4AutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _tr4AutoStartCheckBox.TabIndex = 19;
            _tr4AutoStartCheckBox.Tag = "Tr4";
            _tr4AutoStartCheckBox.UseVisualStyleBackColor = true;
            _tr4AutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;

            // _tteAutoStartCheckBox
            _tteAutoStartCheckBox.Anchor = AnchorStyles.None;
            _tteAutoStartCheckBox.AutoSize = true;
            _tteAutoStartCheckBox.Enabled = false;
            _tteAutoStartCheckBox.Location = new System.Drawing.Point(146, 325);
            _tteAutoStartCheckBox.Name = "_tteAutoStartCheckBox";
            _tteAutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _tteAutoStartCheckBox.TabIndex = 20;
            _tteAutoStartCheckBox.Tag = "Tte";
            _tteAutoStartCheckBox.UseVisualStyleBackColor = true;
            _tteAutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;

            // _tr5AutoStartCheckBox
            _tr5AutoStartCheckBox.Anchor = AnchorStyles.None;
            _tr5AutoStartCheckBox.AutoSize = true;
            _tr5AutoStartCheckBox.Enabled = false;
            _tr5AutoStartCheckBox.Location = new System.Drawing.Point(146, 367);
            _tr5AutoStartCheckBox.Name = "_tr5AutoStartCheckBox";
            _tr5AutoStartCheckBox.Size = new System.Drawing.Size(15, 14);
            _tr5AutoStartCheckBox.TabIndex = 21;
            _tr5AutoStartCheckBox.Tag = "Tr5";
            _tr5AutoStartCheckBox.UseVisualStyleBackColor = true;
            _tr5AutoStartCheckBox.Click += AutoStartCheckBoxCheckedChanged;
            
            // _tr1FileTableLayout
            _tr1FileTableLayout.Anchor = AnchorStyles.None;
            _tr1FileTableLayout.ColumnCount = 2;
            _tr1FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _tr1FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _tr1FileTableLayout.Controls.Add(_tr1FileTextBox, 1, 0);
            _tr1FileTableLayout.Controls.Add(_tr1FileButton, 0, 0);
            _tr1FileTableLayout.Location = new System.Drawing.Point(177, 43);
            _tr1FileTableLayout.Name = "_tr1FileTableLayout";
            _tr1FileTableLayout.RowCount = 1;
            _tr1FileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tr1FileTableLayout.Size = new System.Drawing.Size(193, 32);
            _tr1FileTableLayout.TabIndex = 26;

            // _tr1FileTextBox
            _tr1FileTextBox.Anchor = AnchorStyles.None;
            _tr1FileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _tr1FileTextBox.CausesValidation = false;
            _tr1FileTextBox.Cursor = Cursors.Default;
            _tr1FileTextBox.Enabled = false;
            _tr1FileTextBox.Location = new System.Drawing.Point(33, 6);
            _tr1FileTextBox.Name = "_tr1FileTextBox";
            _tr1FileTextBox.ShortcutsEnabled = false;
            _tr1FileTextBox.Size = new System.Drawing.Size(157, 20);
            _tr1FileTextBox.TabIndex = 0;
            _tr1FileTextBox.TabStop = false;

            // _tr1FileButton
            _tr1FileButton.Anchor = AnchorStyles.None;
            _tr1FileButton.Enabled = false;
            _tr1FileButton.Location = new System.Drawing.Point(3, 4);
            _tr1FileButton.Name = "_tr1FileButton";
            _tr1FileButton.Size = new System.Drawing.Size(24, 23);
            _tr1FileButton.TabIndex = 0;
            _tr1FileButton.Tag = "Tr1";
            _tr1FileButton.Text = "...";
            _tr1FileButton.UseVisualStyleBackColor = true;
            _tr1FileButton.Click += FileButton_Click;

            // _trUbFileTableLayout
            _trUbFileTableLayout.Anchor = AnchorStyles.None;
            _trUbFileTableLayout.ColumnCount = 2;
            _trUbFileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _trUbFileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _trUbFileTableLayout.Controls.Add(_trUbFileTextBox, 1, 0);
            _trUbFileTableLayout.Controls.Add(_trUbFileButton, 0, 0);
            _trUbFileTableLayout.Location = new System.Drawing.Point(177, 82);
            _trUbFileTableLayout.Name = "_trUbFileTableLayout";
            _trUbFileTableLayout.RowCount = 1;
            _trUbFileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _trUbFileTableLayout.Size = new System.Drawing.Size(193, 32);
            _trUbFileTableLayout.TabIndex = 27;

            // _trUbFileTextBox
            _trUbFileTextBox.Anchor = AnchorStyles.None;
            _trUbFileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _trUbFileTextBox.CausesValidation = false;
            _trUbFileTextBox.Cursor = Cursors.Default;
            _trUbFileTextBox.Enabled = false;
            _trUbFileTextBox.Location = new System.Drawing.Point(33, 6);
            _trUbFileTextBox.Name = "_trUbFileTextBox";
            _trUbFileTextBox.ShortcutsEnabled = false;
            _trUbFileTextBox.Size = new System.Drawing.Size(157, 20);
            _trUbFileTextBox.TabIndex = 0;
            _trUbFileTextBox.TabStop = false;

            // _trUbFileButton
            _trUbFileButton.Anchor = AnchorStyles.None;
            _trUbFileButton.Enabled = false;
            _trUbFileButton.Location = new System.Drawing.Point(3, 4);
            _trUbFileButton.Name = "_trUbFileButton";
            _trUbFileButton.Size = new System.Drawing.Size(24, 23);
            _trUbFileButton.TabIndex = 0;
            _trUbFileButton.Tag = "TrUb";
            _trUbFileButton.Text = "...";
            _trUbFileButton.UseVisualStyleBackColor = true;
            _trUbFileButton.Click += FileButton_Click;

            // _tr2FileTableLayout
            _tr2FileTableLayout.Anchor = AnchorStyles.None;
            _tr2FileTableLayout.ColumnCount = 2;
            _tr2FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _tr2FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _tr2FileTableLayout.Controls.Add(_tr2FileTextBox, 1, 0);
            _tr2FileTableLayout.Controls.Add(_tr2FileButton, 0, 0);
            _tr2FileTableLayout.Location = new System.Drawing.Point(177, 121);
            _tr2FileTableLayout.Name = "_tr2FileTableLayout";
            _tr2FileTableLayout.RowCount = 1;
            _tr2FileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tr2FileTableLayout.Size = new System.Drawing.Size(193, 32);
            _tr2FileTableLayout.TabIndex = 28;

            // _tr2FileTextBox
            _tr2FileTextBox.Anchor = AnchorStyles.None;
            _tr2FileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _tr2FileTextBox.CausesValidation = false;
            _tr2FileTextBox.Cursor = Cursors.Default;
            _tr2FileTextBox.Enabled = false;
            _tr2FileTextBox.Location = new System.Drawing.Point(33, 6);
            _tr2FileTextBox.Name = "_tr2FileTextBox";
            _tr2FileTextBox.ShortcutsEnabled = false;
            _tr2FileTextBox.Size = new System.Drawing.Size(157, 20);
            _tr2FileTextBox.TabIndex = 0;
            _tr2FileTextBox.TabStop = false;

            // _tr2FileButton
            _tr2FileButton.Anchor = AnchorStyles.None;
            _tr2FileButton.Enabled = false;
            _tr2FileButton.Location = new System.Drawing.Point(3, 4);
            _tr2FileButton.Name = "_tr2FileButton";
            _tr2FileButton.Size = new System.Drawing.Size(24, 23);
            _tr2FileButton.TabIndex = 0;
            _tr2FileButton.Tag = "Tr2";
            _tr2FileButton.Text = "...";
            _tr2FileButton.UseVisualStyleBackColor = true;
            _tr2FileButton.Click += FileButton_Click;

            // _tr2GFileTableLayout
            _tr2GFileTableLayout.Anchor = AnchorStyles.None;
            _tr2GFileTableLayout.ColumnCount = 2;
            _tr2GFileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _tr2GFileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _tr2GFileTableLayout.Controls.Add(_tr2GFileTextBox, 1, 0);
            _tr2GFileTableLayout.Controls.Add(_tr2GFileButton, 0, 0);
            _tr2GFileTableLayout.Location = new System.Drawing.Point(177, 160);
            _tr2GFileTableLayout.Name = "_tr2GFileTableLayout";
            _tr2GFileTableLayout.RowCount = 1;
            _tr2GFileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tr2GFileTableLayout.Size = new System.Drawing.Size(193, 32);
            _tr2GFileTableLayout.TabIndex = 31;

            // _tr2GFileTextBox
            _tr2GFileTextBox.Anchor = AnchorStyles.None;
            _tr2GFileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _tr2GFileTextBox.CausesValidation = false;
            _tr2GFileTextBox.Cursor = Cursors.Default;
            _tr2GFileTextBox.Enabled = false;
            _tr2GFileTextBox.Location = new System.Drawing.Point(33, 6);
            _tr2GFileTextBox.Name = "_tr2GFileTextBox";
            _tr2GFileTextBox.ShortcutsEnabled = false;
            _tr2GFileTextBox.Size = new System.Drawing.Size(157, 20);
            _tr2GFileTextBox.TabIndex = 0;
            _tr2GFileTextBox.TabStop = false;

            // _tr2GFileButton
            _tr2GFileButton.Anchor = AnchorStyles.None;
            _tr2GFileButton.Enabled = false;
            _tr2GFileButton.Location = new System.Drawing.Point(3, 4);
            _tr2GFileButton.Name = "_tr2GFileButton";
            _tr2GFileButton.Size = new System.Drawing.Size(24, 23);
            _tr2GFileButton.TabIndex = 0;
            _tr2GFileButton.Tag = "Tr2G";
            _tr2GFileButton.Text = "...";
            _tr2GFileButton.UseVisualStyleBackColor = true;
            _tr2GFileButton.Click += FileButton_Click;

            // _tr3FileTableLayout
            _tr3FileTableLayout.Anchor = AnchorStyles.None;
            _tr3FileTableLayout.ColumnCount = 2;
            _tr3FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _tr3FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _tr3FileTableLayout.Controls.Add(_tr3FileTextBox, 1, 0);
            _tr3FileTableLayout.Controls.Add(_tr3FileButton, 0, 0);
            _tr3FileTableLayout.Location = new System.Drawing.Point(177, 199);
            _tr3FileTableLayout.Name = "_tr3FileTableLayout";
            _tr3FileTableLayout.RowCount = 1;
            _tr3FileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tr3FileTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _tr3FileTableLayout.Size = new System.Drawing.Size(193, 32);
            _tr3FileTableLayout.TabIndex = 30;

            // _tr3FileTextBox
            _tr3FileTextBox.Anchor = AnchorStyles.None;
            _tr3FileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _tr3FileTextBox.CausesValidation = false;
            _tr3FileTextBox.Cursor = Cursors.Default;
            _tr3FileTextBox.Enabled = false;
            _tr3FileTextBox.Location = new System.Drawing.Point(33, 6);
            _tr3FileTextBox.Name = "_tr3FileTextBox";
            _tr3FileTextBox.ShortcutsEnabled = false;
            _tr3FileTextBox.Size = new System.Drawing.Size(157, 20);
            _tr3FileTextBox.TabIndex = 0;
            _tr3FileTextBox.TabStop = false;

            // _tr3FileButton
            _tr3FileButton.Anchor = AnchorStyles.None;
            _tr3FileButton.Enabled = false;
            _tr3FileButton.Location = new System.Drawing.Point(3, 4);
            _tr3FileButton.Name = "_tr3FileButton";
            _tr3FileButton.Size = new System.Drawing.Size(24, 23);
            _tr3FileButton.TabIndex = 0;
            _tr3FileButton.Tag = "Tr3";
            _tr3FileButton.Text = "...";
            _tr3FileButton.UseVisualStyleBackColor = true;
            _tr3FileButton.Click += FileButton_Click;

            // _tlaFileTableLayout
            _tlaFileTableLayout.Anchor = AnchorStyles.None;
            _tlaFileTableLayout.ColumnCount = 2;
            _tlaFileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _tlaFileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _tlaFileTableLayout.Controls.Add(_tlaFileTextBox, 1, 0);
            _tlaFileTableLayout.Controls.Add(_tlaFileButton, 0, 0);
            _tlaFileTableLayout.Location = new System.Drawing.Point(177, 238);
            _tlaFileTableLayout.Name = "_tlaFileTableLayout";
            _tlaFileTableLayout.RowCount = 1;
            _tlaFileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tlaFileTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _tlaFileTableLayout.Size = new System.Drawing.Size(193, 32);
            _tlaFileTableLayout.TabIndex = 32;

            // _tlaFileTextBox
            _tlaFileTextBox.Anchor = AnchorStyles.None;
            _tlaFileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _tlaFileTextBox.CausesValidation = false;
            _tlaFileTextBox.Cursor = Cursors.Default;
            _tlaFileTextBox.Enabled = false;
            _tlaFileTextBox.Location = new System.Drawing.Point(33, 6);
            _tlaFileTextBox.Name = "_tlaFileTextBox";
            _tlaFileTextBox.ShortcutsEnabled = false;
            _tlaFileTextBox.Size = new System.Drawing.Size(157, 20);
            _tlaFileTextBox.TabIndex = 0;
            _tlaFileTextBox.TabStop = false;

            // _tlaFileButton
            _tlaFileButton.Anchor = AnchorStyles.None;
            _tlaFileButton.Enabled = false;
            _tlaFileButton.Location = new System.Drawing.Point(3, 4);
            _tlaFileButton.Name = "_tlaFileButton";
            _tlaFileButton.Size = new System.Drawing.Size(24, 23);
            _tlaFileButton.TabIndex = 0;
            _tlaFileButton.Tag = "Tla";
            _tlaFileButton.Text = "...";
            _tlaFileButton.UseVisualStyleBackColor = true;
            _tlaFileButton.Click += FileButton_Click;

            // _tr4FileTableLayout
            _tr4FileTableLayout.Anchor = AnchorStyles.None;
            _tr4FileTableLayout.ColumnCount = 2;
            _tr4FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _tr4FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _tr4FileTableLayout.Controls.Add(_tr4FileTextBox, 1, 0);
            _tr4FileTableLayout.Controls.Add(_tr4FileButton, 0, 0);
            _tr4FileTableLayout.Location = new System.Drawing.Point(177, 277);
            _tr4FileTableLayout.Name = "_tr4FileTableLayout";
            _tr4FileTableLayout.RowCount = 1;
            _tr4FileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tr4FileTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _tr4FileTableLayout.Size = new System.Drawing.Size(193, 32);
            _tr4FileTableLayout.TabIndex = 33;

            // _tr4FileTextBox
            _tr4FileTextBox.Anchor = AnchorStyles.None;
            _tr4FileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _tr4FileTextBox.CausesValidation = false;
            _tr4FileTextBox.Cursor = Cursors.Default;
            _tr4FileTextBox.Enabled = false;
            _tr4FileTextBox.Location = new System.Drawing.Point(33, 6);
            _tr4FileTextBox.Name = "_tr4FileTextBox";
            _tr4FileTextBox.ShortcutsEnabled = false;
            _tr4FileTextBox.Size = new System.Drawing.Size(157, 20);
            _tr4FileTextBox.TabIndex = 0;
            _tr4FileTextBox.TabStop = false;

            // _tr4FileButton
            _tr4FileButton.Anchor = AnchorStyles.None;
            _tr4FileButton.Enabled = false;
            _tr4FileButton.Location = new System.Drawing.Point(3, 4);
            _tr4FileButton.Name = "_tr4FileButton";
            _tr4FileButton.Size = new System.Drawing.Size(24, 23);
            _tr4FileButton.TabIndex = 0;
            _tr4FileButton.Tag = "Tr4";
            _tr4FileButton.Text = "...";
            _tr4FileButton.UseVisualStyleBackColor = true;
            _tr4FileButton.Click += FileButton_Click;

            // _tteFileTableLayout
            _tteFileTableLayout.Anchor = AnchorStyles.None;
            _tteFileTableLayout.ColumnCount = 2;
            _tteFileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _tteFileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _tteFileTableLayout.Controls.Add(_tteFileTextBox, 1, 0);
            _tteFileTableLayout.Controls.Add(_tteFileButton, 0, 0);
            _tteFileTableLayout.Location = new System.Drawing.Point(177, 316);
            _tteFileTableLayout.Name = "_tteFileTableLayout";
            _tteFileTableLayout.RowCount = 1;
            _tteFileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tteFileTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _tteFileTableLayout.Size = new System.Drawing.Size(193, 32);
            _tteFileTableLayout.TabIndex = 34;

            // _tteFileTextBox
            _tteFileTextBox.Anchor = AnchorStyles.None;
            _tteFileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _tteFileTextBox.CausesValidation = false;
            _tteFileTextBox.Cursor = Cursors.Default;
            _tteFileTextBox.Enabled = false;
            _tteFileTextBox.Location = new System.Drawing.Point(33, 6);
            _tteFileTextBox.Name = "_tteFileTextBox";
            _tteFileTextBox.ShortcutsEnabled = false;
            _tteFileTextBox.Size = new System.Drawing.Size(157, 20);
            _tteFileTextBox.TabIndex = 0;
            _tteFileTextBox.TabStop = false;

            // _tteFileButton
            _tteFileButton.Anchor = AnchorStyles.None;
            _tteFileButton.Enabled = false;
            _tteFileButton.Location = new System.Drawing.Point(3, 4);
            _tteFileButton.Name = "_tteFileButton";
            _tteFileButton.Size = new System.Drawing.Size(24, 23);
            _tteFileButton.TabIndex = 0;
            _tteFileButton.Tag = "Tte";
            _tteFileButton.Text = "...";
            _tteFileButton.UseVisualStyleBackColor = true;
            _tteFileButton.Click += FileButton_Click;

            // _tr5FileTableLayout
            _tr5FileTableLayout.Anchor = AnchorStyles.None;
            _tr5FileTableLayout.ColumnCount = 2;
            _tr5FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.95745F));
            _tr5FileTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 84.04256F));
            _tr5FileTableLayout.Controls.Add(_tr5FileTextBox, 1, 0);
            _tr5FileTableLayout.Controls.Add(_tr5FileButton, 0, 0);
            _tr5FileTableLayout.Location = new System.Drawing.Point(177, 355);
            _tr5FileTableLayout.Name = "_tr5FileTableLayout";
            _tr5FileTableLayout.RowCount = 1;
            _tr5FileTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _tr5FileTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            _tr5FileTableLayout.Size = new System.Drawing.Size(193, 38);
            _tr5FileTableLayout.TabIndex = 35;

            // _tr5FileTextBox
            _tr5FileTextBox.Anchor = AnchorStyles.None;
            _tr5FileTextBox.BorderStyle = BorderStyle.FixedSingle;
            _tr5FileTextBox.CausesValidation = false;
            _tr5FileTextBox.Cursor = Cursors.Default;
            _tr5FileTextBox.Enabled = false;
            _tr5FileTextBox.Location = new System.Drawing.Point(33, 9);
            _tr5FileTextBox.Name = "_tr5FileTextBox";
            _tr5FileTextBox.ShortcutsEnabled = false;
            _tr5FileTextBox.Size = new System.Drawing.Size(157, 20);
            _tr5FileTextBox.TabIndex = 0;
            _tr5FileTextBox.TabStop = false;

            // _tr5FileButton
            _tr5FileButton.Anchor = AnchorStyles.None;
            _tr5FileButton.Enabled = false;
            _tr5FileButton.Location = new System.Drawing.Point(3, 7);
            _tr5FileButton.Name = "_tr5FileButton";
            _tr5FileButton.Size = new System.Drawing.Size(24, 23);
            _tr5FileButton.TabIndex = 0;
            _tr5FileButton.Tag = "Tr5";
            _tr5FileButton.Text = "...";
            _tr5FileButton.UseVisualStyleBackColor = true;
            _tr5FileButton.Click += FileButton_Click;
            
            // _tr4GlitchlessCheckBox
            _tr4GlitchlessCheckBox.Anchor = AnchorStyles.Left;
            _tr4GlitchlessCheckBox.AutoSize = true;
            _tr4GlitchlessCheckBox.Enabled = false;
            _tr4GlitchlessCheckBox.Location = new System.Drawing.Point(377, 284);
            _tr4GlitchlessCheckBox.Name = "_tr4GlitchlessCheckBox";
            _tr4GlitchlessCheckBox.Padding = new Padding(5, 0, 0, 0);
            _tr4GlitchlessCheckBox.Size = new System.Drawing.Size(76, 17);
            _tr4GlitchlessCheckBox.TabIndex = 22;
            _tr4GlitchlessCheckBox.Tag = "Tr4";
            _tr4GlitchlessCheckBox.Text = "Glitchless";
            _tr4GlitchlessCheckBox.UseVisualStyleBackColor = true;
            _tr4GlitchlessCheckBox.CheckedChanged += GameOptionCheckBoxCheckedChanged;

            // _tr5SplitCutsceneCheckBox
            _tr5SplitCutsceneCheckBox.Anchor = AnchorStyles.Left;
            _tr5SplitCutsceneCheckBox.AutoSize = true;
            _tr5SplitCutsceneCheckBox.Enabled = false;
            _tr5SplitCutsceneCheckBox.Location = new System.Drawing.Point(377, 365);
            _tr5SplitCutsceneCheckBox.Name = "_tr5SplitCutsceneCheckBox";
            _tr5SplitCutsceneCheckBox.Padding = new Padding(5, 0, 0, 0);
            _tr5SplitCutsceneCheckBox.Size = new System.Drawing.Size(99, 17);
            _tr5SplitCutsceneCheckBox.TabIndex = 23;
            _tr5SplitCutsceneCheckBox.Tag = "Tr5";
            _tr5SplitCutsceneCheckBox.Text = "Split Cutscene";
            _tr5SplitCutsceneCheckBox.UseVisualStyleBackColor = true;
            _tr5SplitCutsceneCheckBox.CheckedChanged += GameOptionCheckBoxCheckedChanged;

            // ComponentSettings
            Controls.Add(_modeSelect);
            Controls.Add(_gameSelectionTableLayout);
            Controls.Add(_autosplitterVersionLabel);
            Controls.Add(_gameVersionLabel);
            Name = "ComponentSettings";
            Size = new System.Drawing.Size(480, 510);
            _modeSelect.ResumeLayout(false);
            _modeSelect.PerformLayout();
            _gameSelectionTableLayout.ResumeLayout(false);
            _gameSelectionTableLayout.PerformLayout();
            _tr1FileTableLayout.ResumeLayout(false);
            _tr1FileTableLayout.PerformLayout();
            _trUbFileTableLayout.ResumeLayout(false);
            _trUbFileTableLayout.PerformLayout();
            _tr2FileTableLayout.ResumeLayout(false);
            _tr2FileTableLayout.PerformLayout();
            _tr2GFileTableLayout.ResumeLayout(false);
            _tr2GFileTableLayout.PerformLayout();
            _tr3FileTableLayout.ResumeLayout(false);
            _tr3FileTableLayout.PerformLayout();
            _tlaFileTableLayout.ResumeLayout(false);
            _tlaFileTableLayout.PerformLayout();
            _tr4FileTableLayout.ResumeLayout(false);
            _tr4FileTableLayout.PerformLayout();
            _tteFileTableLayout.ResumeLayout(false);
            _tteFileTableLayout.PerformLayout();
            _tr5FileTableLayout.ResumeLayout(false);
            _tr5FileTableLayout.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        public virtual void SetGameVersion(uint version)
        {
        }

        private void RunModeButtonCheckedChanged(object sender, EventArgs e) => FullGame = _fullGameModeButton.Checked;

        private void GameCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            bool boxChecked = checkBox.Checked;
            switch (checkBox.Tag.ToString())
            {
                case "Tr1":
                    _tr1AutoStartCheckBox.Enabled = boxChecked;
                    break;
                case "TrUb":
                    _trUbAutoStartCheckBox.Enabled = boxChecked;
                    break;
                case "Tr2":
                    _tr2AutoStartCheckBox.Enabled = boxChecked;
                    break;
                case "Tr2G":
                    _tr2GAutoStartCheckBox.Enabled =  boxChecked;
                    break;
                case "Tr3":
                    _tr3AutoStartCheckBox.Enabled = boxChecked;
                    break;
                case "Tla":
                    _tlaAutoStartCheckBox.Enabled = boxChecked;
                    break;
                case "Tr4":
                    _tr4AutoStartCheckBox.Enabled = _tr4GlitchlessCheckBox.Checked = boxChecked;
                    break;
                case "Tte":
                    _tteAutoStartCheckBox.Enabled = boxChecked;
                    break;
                case "Tr5":
                    _tr5AutoStartCheckBox.Enabled = _tr5SplitCutsceneCheckBox.Enabled = boxChecked;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AutoStartCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            bool boxChecked = checkBox.Checked;
            switch (checkBox.Tag.ToString())
            {
                case "Tr1":
                    _tr1AutoStartCheckBox.Enabled = Tr1AutoStart = boxChecked;
                    break;
                case "TrUb":
                    _trUbAutoStartCheckBox.Enabled = TrUbAutoStart = boxChecked;
                    break;
                case "Tr2":
                    _tr2AutoStartCheckBox.Enabled = Tr2AutoStart = boxChecked;
                    break;
                case "Tr2G":
                    _tr2GAutoStartCheckBox.Enabled = Tr2GAutoStart = boxChecked;
                    break;
                case "Tr3":
                    _tr3AutoStartCheckBox.Enabled = Tr3AutoStart = boxChecked;
                    break;
                case "Tla":
                    _tlaAutoStartCheckBox.Enabled = TlaAutoStart = boxChecked;
                    break;
                case "Tr4":
                    _tr4AutoStartCheckBox.Enabled = _tr4GlitchlessCheckBox.Checked = Tr4AutoStart = boxChecked;
                    break;
                case "Tte":
                    _tteAutoStartCheckBox.Enabled = TteAutoStart = boxChecked;
                    break;
                case "Tr5":
                    _tr5AutoStartCheckBox.Enabled = _tr5SplitCutsceneCheckBox.Enabled = Tr5AutoStart = boxChecked;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void FileButton_Click(object sender, EventArgs e)
        {
            if (_openFileDialog.ShowDialog() != DialogResult.OK) 
                return;
            
            string fileName = _openFileDialog.FileName;
            var button = (Button) sender;
            switch (button.Tag.ToString())
            {
                case "Tr1":
                    _tr1FileTextBox.Text = Tr1FileLocation = fileName;
                    break;
                case "TrUb":
                    _trUbFileTextBox.Text = TrUbFileLocation = fileName;
                    break;
                case "Tr2":
                    _tr2FileTextBox.Text = Tr2FileLocation = fileName;
                    break;
                case "Tr2G":
                    _tr2GFileTextBox.Text = Tr2GFileLocation = fileName;
                    break;
                case "Tr3":
                    _tr3FileTextBox.Text = Tr3FileLocation = fileName;
                    break;
                case "Tla":
                    _tlaFileTextBox.Text = TlaFileLocation = fileName;
                    break;
                case "Tr4":
                    _tr4FileTextBox.Text = Tr4FileLocation = fileName;
                    break;
                case "Tte":
                    _tteFileTextBox.Text = TteFileLocation = fileName;
                    break;
                case "Tr5":
                    _tr5FileTextBox.Text = Tr5FileLocation = fileName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GameOptionCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var checkBox = (CheckBox) sender;
            bool boxChecked = checkBox.Checked;
            switch (checkBox.Tag.ToString())
            {
                case "Tr4":
                    Tr4Glitchless = boxChecked;
                    break;
                case "Tr5":
                    Tr5SplitCutscene = boxChecked;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
