using System;
using System.Reflection;
using System.Windows.Forms;

namespace TRMarathon
{
    public class ComponentSettings : UserControl
    {
        public Label GameVersionLabel;
        public Label AutosplitterVersionLabel;
        public bool FullGame = true;
        public TableLayoutPanel GameSelectionTableLayout;
        public CheckBox Tr1CheckBox;
        public CheckBox TrUbCheckBox;
        public Label GameSelectionColumnLabel;
        public CheckBox Tr2CheckBox;
        public CheckBox Tr2GCheckBox;
        public Label AutoLaunchColumnLabel;
        public CheckBox Tr3CheckBox;
        public Label GameOptionColumnLabel;
        public Label GameLocationColumnLabel;
        public CheckBox TlaCheckBox;
        public CheckBox Tr4CheckBox;
        public CheckBox TteCheckBox;
        public CheckBox Tr5CheckBox;
        public CheckBox Tr1AutoLaunchCheckBox;
        public CheckBox TrUbAutoLaunchCheckBox;
        private OpenFileDialog openFileDialog;
        public CheckBox Tr2AutoLaunchCheckBox;
        public CheckBox Tr2GAutoLaunchCheckBox;
        public CheckBox Tr3AutoLaunchCheckBox;
        public CheckBox TlaAutoLaunchCheckBox;
        public CheckBox Tr4AutoLaunchCheckBox;
        public CheckBox TteAutoLaunchCheckBox;
        public CheckBox Tr5AutoLaunchCheckBox;
        public CheckBox Tr4GlitchlessCheckBox;
        public CheckBox Tr5SplitCutsceneCheckBox;
        private TableLayoutPanel Tr1FileTableLayout;
        public TextBox Tr1FileTextBox;
        private Button Tr1FileButton;
        private TableLayoutPanel TrUbFileTableLayout;
        public TextBox TrUbFileTextBox;
        private Button TrUbFileButton;
        private TableLayoutPanel Tr2FileTableLayout;
        public TextBox Tr2FileTextBox;
        private Button Tr2FileButton;
        private TableLayoutPanel Tr3FileTableLayout;
        public TextBox Tr3FileTextBox;
        private Button Tr3FileButton;
        private TableLayoutPanel Tr2GFileTableLayout;
        public TextBox Tr2GFileTextBox;
        private Button Tr2GFileButton;
        private TableLayoutPanel TlaFileTableLayout;
        public TextBox TlaFileTextBox;
        private Button TlaFileButton;
        private TableLayoutPanel Tr4FileTableLayout;
        public TextBox Tr4FileTextBox;
        private Button Tr4FileButton;
        private TableLayoutPanel TteFileTableLayout;
        public TextBox TteFileTextBox;
        private Button TteFileButton;
        private TableLayoutPanel Tr5FileTableLayout;
        public TextBox Tr5FileTextBox;
        private Button Tr5FileButton;
        public bool Deathrun;

        private void SetTableRows()
        {
            
        }
        
        public ComponentSettings() => InitializeComponent();

        private void InitializeComponent()
        {
            this.GameVersionLabel = new System.Windows.Forms.Label();
            this.AutosplitterVersionLabel = new System.Windows.Forms.Label();
            this.GameSelectionTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Tr1FileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Tr1FileTextBox = new System.Windows.Forms.TextBox();
            this.Tr1FileButton = new System.Windows.Forms.Button();
            this.GameOptionColumnLabel = new System.Windows.Forms.Label();
            this.GameLocationColumnLabel = new System.Windows.Forms.Label();
            this.AutoLaunchColumnLabel = new System.Windows.Forms.Label();
            this.TrUbCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr1CheckBox = new System.Windows.Forms.CheckBox();
            this.GameSelectionColumnLabel = new System.Windows.Forms.Label();
            this.Tr2CheckBox = new System.Windows.Forms.CheckBox();
            this.Tr2GCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr3CheckBox = new System.Windows.Forms.CheckBox();
            this.TlaCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr4CheckBox = new System.Windows.Forms.CheckBox();
            this.TteCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr5CheckBox = new System.Windows.Forms.CheckBox();
            this.Tr1AutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.TrUbAutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr2AutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr2GAutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr3AutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.TlaAutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr4AutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.TteAutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr5AutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr4GlitchlessCheckBox = new System.Windows.Forms.CheckBox();
            this.Tr5SplitCutsceneCheckBox = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.TrUbFileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.TrUbFileTextBox = new System.Windows.Forms.TextBox();
            this.TrUbFileButton = new System.Windows.Forms.Button();
            this.Tr2FileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Tr2FileTextBox = new System.Windows.Forms.TextBox();
            this.Tr2FileButton = new System.Windows.Forms.Button();
            this.Tr3FileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Tr3FileTextBox = new System.Windows.Forms.TextBox();
            this.Tr3FileButton = new System.Windows.Forms.Button();
            this.Tr2GFileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Tr2GFileTextBox = new System.Windows.Forms.TextBox();
            this.Tr2GFileButton = new System.Windows.Forms.Button();
            this.TlaFileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.TlaFileTextBox = new System.Windows.Forms.TextBox();
            this.TlaFileButton = new System.Windows.Forms.Button();
            this.Tr4FileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Tr4FileTextBox = new System.Windows.Forms.TextBox();
            this.Tr4FileButton = new System.Windows.Forms.Button();
            this.TteFileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.TteFileTextBox = new System.Windows.Forms.TextBox();
            this.TteFileButton = new System.Windows.Forms.Button();
            this.Tr5FileTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.Tr5FileTextBox = new System.Windows.Forms.TextBox();
            this.Tr5FileButton = new System.Windows.Forms.Button();
            this.GameSelectionTableLayout.SuspendLayout();
            this.Tr1FileTableLayout.SuspendLayout();
            this.TrUbFileTableLayout.SuspendLayout();
            this.Tr2FileTableLayout.SuspendLayout();
            this.Tr3FileTableLayout.SuspendLayout();
            this.Tr2GFileTableLayout.SuspendLayout();
            this.TlaFileTableLayout.SuspendLayout();
            this.Tr4FileTableLayout.SuspendLayout();
            this.TteFileTableLayout.SuspendLayout();
            this.Tr5FileTableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // GameVersionLabel
            // 
            this.GameVersionLabel.AutoSize = true;
            this.GameVersionLabel.Location = new System.Drawing.Point(3, 464);
            this.GameVersionLabel.Name = "GameVersionLabel";
            this.GameVersionLabel.Size = new System.Drawing.Size(186, 13);
            this.GameVersionLabel.TabIndex = 1;
            this.GameVersionLabel.Text = "Game Version: Unknown/Undetected";
            // 
            // AutosplitterVersionLabel
            // 
            this.AutosplitterVersionLabel.AutoSize = true;
            this.AutosplitterVersionLabel.Location = new System.Drawing.Point(3, 487);
            this.AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
            this.AutosplitterVersionLabel.Size = new System.Drawing.Size(103, 13);
            this.AutosplitterVersionLabel.TabIndex = 2;
            this.AutosplitterVersionLabel.Text = "Autosplitter Version: ";
            // 
            // GameSelectionTableLayout
            // 
            this.GameSelectionTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameSelectionTableLayout.ColumnCount = 4;
            this.GameSelectionTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29F));
            this.GameSelectionTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11F));
            this.GameSelectionTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.GameSelectionTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.GameSelectionTableLayout.Controls.Add(this.Tr1FileTableLayout, 2, 1);
            this.GameSelectionTableLayout.Controls.Add(this.GameOptionColumnLabel, 3, 0);
            this.GameSelectionTableLayout.Controls.Add(this.GameLocationColumnLabel, 2, 0);
            this.GameSelectionTableLayout.Controls.Add(this.AutoLaunchColumnLabel, 1, 0);
            this.GameSelectionTableLayout.Controls.Add(this.TrUbCheckBox, 0, 2);
            this.GameSelectionTableLayout.Controls.Add(this.Tr1CheckBox, 0, 1);
            this.GameSelectionTableLayout.Controls.Add(this.GameSelectionColumnLabel, 0, 0);
            this.GameSelectionTableLayout.Controls.Add(this.Tr2CheckBox, 0, 3);
            this.GameSelectionTableLayout.Controls.Add(this.Tr2GCheckBox, 0, 4);
            this.GameSelectionTableLayout.Controls.Add(this.Tr3CheckBox, 0, 5);
            this.GameSelectionTableLayout.Controls.Add(this.TlaCheckBox, 0, 6);
            this.GameSelectionTableLayout.Controls.Add(this.Tr4CheckBox, 0, 7);
            this.GameSelectionTableLayout.Controls.Add(this.TteCheckBox, 0, 8);
            this.GameSelectionTableLayout.Controls.Add(this.Tr5CheckBox, 0, 9);
            this.GameSelectionTableLayout.Controls.Add(this.Tr1AutoLaunchCheckBox, 1, 1);
            this.GameSelectionTableLayout.Controls.Add(this.TrUbAutoLaunchCheckBox, 1, 2);
            this.GameSelectionTableLayout.Controls.Add(this.Tr2AutoLaunchCheckBox, 1, 3);
            this.GameSelectionTableLayout.Controls.Add(this.Tr2GAutoLaunchCheckBox, 1, 4);
            this.GameSelectionTableLayout.Controls.Add(this.Tr3AutoLaunchCheckBox, 1, 5);
            this.GameSelectionTableLayout.Controls.Add(this.TlaAutoLaunchCheckBox, 1, 6);
            this.GameSelectionTableLayout.Controls.Add(this.Tr4AutoLaunchCheckBox, 1, 7);
            this.GameSelectionTableLayout.Controls.Add(this.TteAutoLaunchCheckBox, 1, 8);
            this.GameSelectionTableLayout.Controls.Add(this.Tr5AutoLaunchCheckBox, 1, 9);
            this.GameSelectionTableLayout.Controls.Add(this.Tr4GlitchlessCheckBox, 3, 7);
            this.GameSelectionTableLayout.Controls.Add(this.Tr5SplitCutsceneCheckBox, 3, 9);
            this.GameSelectionTableLayout.Controls.Add(this.TrUbFileTableLayout, 2, 2);
            this.GameSelectionTableLayout.Controls.Add(this.Tr2FileTableLayout, 2, 3);
            this.GameSelectionTableLayout.Controls.Add(this.Tr2GFileTableLayout, 2, 4);
            this.GameSelectionTableLayout.Controls.Add(this.Tr3FileTableLayout, 2, 5);
            this.GameSelectionTableLayout.Controls.Add(this.TlaFileTableLayout, 2, 6);
            this.GameSelectionTableLayout.Controls.Add(this.Tr4FileTableLayout, 2, 7);
            this.GameSelectionTableLayout.Controls.Add(this.TteFileTableLayout, 2, 8);
            this.GameSelectionTableLayout.Controls.Add(this.Tr5FileTableLayout, 2, 9);
            this.GameSelectionTableLayout.Location = new System.Drawing.Point(3, 4);
            this.GameSelectionTableLayout.Name = "GameSelectionTableLayout";
            this.GameSelectionTableLayout.RowCount = 10;
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.0001F));
            this.GameSelectionTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.999102F));
            this.GameSelectionTableLayout.Size = new System.Drawing.Size(474, 450);
            this.GameSelectionTableLayout.TabIndex = 3;
            // 
            // Tr1FileTableLayout
            // 
            this.Tr1FileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr1FileTableLayout.ColumnCount = 2;
            this.Tr1FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.Tr1FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.Tr1FileTableLayout.Controls.Add(this.Tr1FileTextBox, 1, 0);
            this.Tr1FileTableLayout.Controls.Add(this.Tr1FileButton, 0, 0);
            this.Tr1FileTableLayout.Location = new System.Drawing.Point(192, 48);
            this.Tr1FileTableLayout.Name = "Tr1FileTableLayout";
            this.Tr1FileTableLayout.RowCount = 1;
            this.Tr1FileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Tr1FileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.Tr1FileTableLayout.TabIndex = 26;
            // 
            // Tr1FileTextBox
            // 
            this.Tr1FileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr1FileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tr1FileTextBox.CausesValidation = false;
            this.Tr1FileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tr1FileTextBox.Enabled = false;
            this.Tr1FileTextBox.Location = new System.Drawing.Point(34, 9);
            this.Tr1FileTextBox.Name = "Tr1FileTextBox";
            this.Tr1FileTextBox.ShortcutsEnabled = false;
            this.Tr1FileTextBox.Size = new System.Drawing.Size(122, 20);
            this.Tr1FileTextBox.TabIndex = 0;
            this.Tr1FileTextBox.TabStop = false;
            // 
            // Tr1FileButton
            // 
            this.Tr1FileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr1FileButton.Enabled = false;
            this.Tr1FileButton.Location = new System.Drawing.Point(3, 8);
            this.Tr1FileButton.Name = "Tr1FileButton";
            this.Tr1FileButton.Size = new System.Drawing.Size(25, 23);
            this.Tr1FileButton.TabIndex = 0;
            this.Tr1FileButton.Text = "...";
            this.Tr1FileButton.UseVisualStyleBackColor = true;
            this.Tr1FileButton.Click += new System.EventHandler(this.FileButton_Click);
            // 
            // GameOptionColumnLabel
            // 
            this.GameOptionColumnLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameOptionColumnLabel.AutoSize = true;
            this.GameOptionColumnLabel.Location = new System.Drawing.Point(379, 16);
            this.GameOptionColumnLabel.Name = "GameOptionColumnLabel";
            this.GameOptionColumnLabel.Size = new System.Drawing.Size(69, 13);
            this.GameOptionColumnLabel.TabIndex = 8;
            this.GameOptionColumnLabel.Text = "Game Option";
            this.GameOptionColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GameLocationColumnLabel
            // 
            this.GameLocationColumnLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameLocationColumnLabel.AutoSize = true;
            this.GameLocationColumnLabel.Location = new System.Drawing.Point(232, 16);
            this.GameLocationColumnLabel.Name = "GameLocationColumnLabel";
            this.GameLocationColumnLabel.Size = new System.Drawing.Size(79, 13);
            this.GameLocationColumnLabel.TabIndex = 7;
            this.GameLocationColumnLabel.Text = "Game Location";
            this.GameLocationColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AutoLaunchColumnLabel
            // 
            this.AutoLaunchColumnLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AutoLaunchColumnLabel.AutoSize = true;
            this.AutoLaunchColumnLabel.Location = new System.Drawing.Point(141, 9);
            this.AutoLaunchColumnLabel.Name = "AutoLaunchColumnLabel";
            this.AutoLaunchColumnLabel.Size = new System.Drawing.Size(43, 26);
            this.AutoLaunchColumnLabel.TabIndex = 6;
            this.AutoLaunchColumnLabel.Text = "Auto Launch";
            this.AutoLaunchColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TrUbCheckBox
            // 
            this.TrUbCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TrUbCheckBox.AutoSize = true;
            this.TrUbCheckBox.Location = new System.Drawing.Point(3, 104);
            this.TrUbCheckBox.Name = "TrUbCheckBox";
            this.TrUbCheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.TrUbCheckBox.Size = new System.Drawing.Size(126, 17);
            this.TrUbCheckBox.TabIndex = 1;
            this.TrUbCheckBox.Text = "Unfinished Business";
            this.TrUbCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr1CheckBox
            // 
            this.Tr1CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Tr1CheckBox.AutoSize = true;
            this.Tr1CheckBox.Location = new System.Drawing.Point(3, 59);
            this.Tr1CheckBox.Name = "Tr1CheckBox";
            this.Tr1CheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Tr1CheckBox.Size = new System.Drawing.Size(131, 17);
            this.Tr1CheckBox.TabIndex = 0;
            this.Tr1CheckBox.Text = "Tomb Raider I (1996)";
            this.Tr1CheckBox.UseVisualStyleBackColor = true;
            // 
            // GameSelectionColumnLabel
            // 
            this.GameSelectionColumnLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameSelectionColumnLabel.AutoSize = true;
            this.GameSelectionColumnLabel.Location = new System.Drawing.Point(27, 16);
            this.GameSelectionColumnLabel.Name = "GameSelectionColumnLabel";
            this.GameSelectionColumnLabel.Size = new System.Drawing.Size(82, 13);
            this.GameSelectionColumnLabel.TabIndex = 2;
            this.GameSelectionColumnLabel.Text = "Game Selection";
            this.GameSelectionColumnLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Tr2CheckBox
            // 
            this.Tr2CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Tr2CheckBox.AutoSize = true;
            this.Tr2CheckBox.Location = new System.Drawing.Point(3, 149);
            this.Tr2CheckBox.Name = "Tr2CheckBox";
            this.Tr2CheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Tr2CheckBox.Size = new System.Drawing.Size(101, 17);
            this.Tr2CheckBox.TabIndex = 3;
            this.Tr2CheckBox.Text = "Tomb Raider II";
            this.Tr2CheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr2GCheckBox
            // 
            this.Tr2GCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Tr2GCheckBox.AutoSize = true;
            this.Tr2GCheckBox.Location = new System.Drawing.Point(3, 194);
            this.Tr2GCheckBox.Name = "Tr2GCheckBox";
            this.Tr2GCheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Tr2GCheckBox.Size = new System.Drawing.Size(94, 17);
            this.Tr2GCheckBox.TabIndex = 4;
            this.Tr2GCheckBox.Text = "Golden Mask";
            this.Tr2GCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr3CheckBox
            // 
            this.Tr3CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Tr3CheckBox.AutoSize = true;
            this.Tr3CheckBox.Location = new System.Drawing.Point(3, 239);
            this.Tr3CheckBox.Name = "Tr3CheckBox";
            this.Tr3CheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Tr3CheckBox.Size = new System.Drawing.Size(104, 17);
            this.Tr3CheckBox.TabIndex = 5;
            this.Tr3CheckBox.Text = "Tomb Raider III";
            this.Tr3CheckBox.UseVisualStyleBackColor = true;
            // 
            // TlaCheckBox
            // 
            this.TlaCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TlaCheckBox.AutoSize = true;
            this.TlaCheckBox.Location = new System.Drawing.Point(3, 284);
            this.TlaCheckBox.Name = "TlaCheckBox";
            this.TlaCheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.TlaCheckBox.Size = new System.Drawing.Size(109, 17);
            this.TlaCheckBox.TabIndex = 9;
            this.TlaCheckBox.Text = "The Lost Artifact";
            this.TlaCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr4CheckBox
            // 
            this.Tr4CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Tr4CheckBox.AutoSize = true;
            this.Tr4CheckBox.Location = new System.Drawing.Point(3, 329);
            this.Tr4CheckBox.Name = "Tr4CheckBox";
            this.Tr4CheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Tr4CheckBox.Size = new System.Drawing.Size(105, 17);
            this.Tr4CheckBox.TabIndex = 10;
            this.Tr4CheckBox.Text = "Tomb Raider IV";
            this.Tr4CheckBox.UseVisualStyleBackColor = true;
            // 
            // TteCheckBox
            // 
            this.TteCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.TteCheckBox.AutoSize = true;
            this.TteCheckBox.Location = new System.Drawing.Point(3, 374);
            this.TteCheckBox.Name = "TteCheckBox";
            this.TteCheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.TteCheckBox.Size = new System.Drawing.Size(129, 17);
            this.TteCheckBox.TabIndex = 11;
            this.TteCheckBox.Text = "The Times Exclusive";
            this.TteCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr5CheckBox
            // 
            this.Tr5CheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Tr5CheckBox.AutoSize = true;
            this.Tr5CheckBox.Location = new System.Drawing.Point(3, 419);
            this.Tr5CheckBox.Name = "Tr5CheckBox";
            this.Tr5CheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Tr5CheckBox.Size = new System.Drawing.Size(102, 17);
            this.Tr5CheckBox.TabIndex = 12;
            this.Tr5CheckBox.Text = "Tomb Raider V";
            this.Tr5CheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr1AutoLaunchCheckBox
            // 
            this.Tr1AutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr1AutoLaunchCheckBox.AutoSize = true;
            this.Tr1AutoLaunchCheckBox.Enabled = false;
            this.Tr1AutoLaunchCheckBox.Location = new System.Drawing.Point(155, 60);
            this.Tr1AutoLaunchCheckBox.Name = "Tr1AutoLaunchCheckBox";
            this.Tr1AutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.Tr1AutoLaunchCheckBox.TabIndex = 13;
            this.Tr1AutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // TrUbAutoLaunchCheckBox
            // 
            this.TrUbAutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TrUbAutoLaunchCheckBox.AutoSize = true;
            this.TrUbAutoLaunchCheckBox.Enabled = false;
            this.TrUbAutoLaunchCheckBox.Location = new System.Drawing.Point(155, 105);
            this.TrUbAutoLaunchCheckBox.Name = "TrUbAutoLaunchCheckBox";
            this.TrUbAutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.TrUbAutoLaunchCheckBox.TabIndex = 14;
            this.TrUbAutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr2AutoLaunchCheckBox
            // 
            this.Tr2AutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr2AutoLaunchCheckBox.AutoSize = true;
            this.Tr2AutoLaunchCheckBox.Enabled = false;
            this.Tr2AutoLaunchCheckBox.Location = new System.Drawing.Point(155, 150);
            this.Tr2AutoLaunchCheckBox.Name = "Tr2AutoLaunchCheckBox";
            this.Tr2AutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.Tr2AutoLaunchCheckBox.TabIndex = 15;
            this.Tr2AutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr2GAutoLaunchCheckBox
            // 
            this.Tr2GAutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr2GAutoLaunchCheckBox.AutoSize = true;
            this.Tr2GAutoLaunchCheckBox.Enabled = false;
            this.Tr2GAutoLaunchCheckBox.Location = new System.Drawing.Point(155, 195);
            this.Tr2GAutoLaunchCheckBox.Name = "Tr2GAutoLaunchCheckBox";
            this.Tr2GAutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.Tr2GAutoLaunchCheckBox.TabIndex = 16;
            this.Tr2GAutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr3AutoLaunchCheckBox
            // 
            this.Tr3AutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr3AutoLaunchCheckBox.AutoSize = true;
            this.Tr3AutoLaunchCheckBox.Enabled = false;
            this.Tr3AutoLaunchCheckBox.Location = new System.Drawing.Point(155, 240);
            this.Tr3AutoLaunchCheckBox.Name = "Tr3AutoLaunchCheckBox";
            this.Tr3AutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.Tr3AutoLaunchCheckBox.TabIndex = 17;
            this.Tr3AutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // TlaAutoLaunchCheckBox
            // 
            this.TlaAutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TlaAutoLaunchCheckBox.AutoSize = true;
            this.TlaAutoLaunchCheckBox.Enabled = false;
            this.TlaAutoLaunchCheckBox.Location = new System.Drawing.Point(155, 285);
            this.TlaAutoLaunchCheckBox.Name = "TlaAutoLaunchCheckBox";
            this.TlaAutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.TlaAutoLaunchCheckBox.TabIndex = 18;
            this.TlaAutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr4AutoLaunchCheckBox
            // 
            this.Tr4AutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr4AutoLaunchCheckBox.AutoSize = true;
            this.Tr4AutoLaunchCheckBox.Enabled = false;
            this.Tr4AutoLaunchCheckBox.Location = new System.Drawing.Point(155, 330);
            this.Tr4AutoLaunchCheckBox.Name = "Tr4AutoLaunchCheckBox";
            this.Tr4AutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.Tr4AutoLaunchCheckBox.TabIndex = 19;
            this.Tr4AutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // TteAutoLaunchCheckBox
            // 
            this.TteAutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TteAutoLaunchCheckBox.AutoSize = true;
            this.TteAutoLaunchCheckBox.Enabled = false;
            this.TteAutoLaunchCheckBox.Location = new System.Drawing.Point(155, 375);
            this.TteAutoLaunchCheckBox.Name = "TteAutoLaunchCheckBox";
            this.TteAutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.TteAutoLaunchCheckBox.TabIndex = 20;
            this.TteAutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr5AutoLaunchCheckBox
            // 
            this.Tr5AutoLaunchCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr5AutoLaunchCheckBox.AutoSize = true;
            this.Tr5AutoLaunchCheckBox.Enabled = false;
            this.Tr5AutoLaunchCheckBox.Location = new System.Drawing.Point(155, 420);
            this.Tr5AutoLaunchCheckBox.Name = "Tr5AutoLaunchCheckBox";
            this.Tr5AutoLaunchCheckBox.Size = new System.Drawing.Size(15, 14);
            this.Tr5AutoLaunchCheckBox.TabIndex = 21;
            this.Tr5AutoLaunchCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr4GlitchlessCheckBox
            // 
            this.Tr4GlitchlessCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Tr4GlitchlessCheckBox.AutoSize = true;
            this.Tr4GlitchlessCheckBox.Enabled = false;
            this.Tr4GlitchlessCheckBox.Location = new System.Drawing.Point(357, 329);
            this.Tr4GlitchlessCheckBox.Name = "Tr4GlitchlessCheckBox";
            this.Tr4GlitchlessCheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Tr4GlitchlessCheckBox.Size = new System.Drawing.Size(76, 17);
            this.Tr4GlitchlessCheckBox.TabIndex = 22;
            this.Tr4GlitchlessCheckBox.Text = "Glitchless";
            this.Tr4GlitchlessCheckBox.UseVisualStyleBackColor = true;
            // 
            // Tr5SplitCutsceneCheckBox
            // 
            this.Tr5SplitCutsceneCheckBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.Tr5SplitCutsceneCheckBox.AutoSize = true;
            this.Tr5SplitCutsceneCheckBox.Enabled = false;
            this.Tr5SplitCutsceneCheckBox.Location = new System.Drawing.Point(357, 419);
            this.Tr5SplitCutsceneCheckBox.Name = "Tr5SplitCutsceneCheckBox";
            this.Tr5SplitCutsceneCheckBox.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Tr5SplitCutsceneCheckBox.Size = new System.Drawing.Size(99, 17);
            this.Tr5SplitCutsceneCheckBox.TabIndex = 23;
            this.Tr5SplitCutsceneCheckBox.Text = "Split Cutscene";
            this.Tr5SplitCutsceneCheckBox.UseVisualStyleBackColor = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.Filter = "\"Executable Files|*.exe\"";
            // 
            // TrUbFileTableLayout
            // 
            this.TrUbFileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TrUbFileTableLayout.ColumnCount = 2;
            this.TrUbFileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.TrUbFileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.TrUbFileTableLayout.Controls.Add(this.TrUbFileTextBox, 1, 0);
            this.TrUbFileTableLayout.Controls.Add(this.TrUbFileButton, 0, 0);
            this.TrUbFileTableLayout.Location = new System.Drawing.Point(192, 93);
            this.TrUbFileTableLayout.Name = "TrUbFileTableLayout";
            this.TrUbFileTableLayout.RowCount = 1;
            this.TrUbFileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TrUbFileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.TrUbFileTableLayout.TabIndex = 27;
            // 
            // TrUbFileTextBox
            // 
            this.TrUbFileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TrUbFileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TrUbFileTextBox.CausesValidation = false;
            this.TrUbFileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.TrUbFileTextBox.Enabled = false;
            this.TrUbFileTextBox.Location = new System.Drawing.Point(34, 9);
            this.TrUbFileTextBox.Name = "TrUbFileTextBox";
            this.TrUbFileTextBox.ShortcutsEnabled = false;
            this.TrUbFileTextBox.Size = new System.Drawing.Size(122, 20);
            this.TrUbFileTextBox.TabIndex = 0;
            this.TrUbFileTextBox.TabStop = false;
            // 
            // TrUbFileButton
            // 
            this.TrUbFileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TrUbFileButton.Enabled = false;
            this.TrUbFileButton.Location = new System.Drawing.Point(3, 8);
            this.TrUbFileButton.Name = "TrUbFileButton";
            this.TrUbFileButton.Size = new System.Drawing.Size(25, 23);
            this.TrUbFileButton.TabIndex = 0;
            this.TrUbFileButton.Text = "...";
            this.TrUbFileButton.UseVisualStyleBackColor = true;
            // 
            // Tr2FileTableLayout
            // 
            this.Tr2FileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr2FileTableLayout.ColumnCount = 2;
            this.Tr2FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.Tr2FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.Tr2FileTableLayout.Controls.Add(this.Tr2FileTextBox, 1, 0);
            this.Tr2FileTableLayout.Controls.Add(this.Tr2FileButton, 0, 0);
            this.Tr2FileTableLayout.Location = new System.Drawing.Point(192, 138);
            this.Tr2FileTableLayout.Name = "Tr2FileTableLayout";
            this.Tr2FileTableLayout.RowCount = 1;
            this.Tr2FileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Tr2FileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.Tr2FileTableLayout.TabIndex = 28;
            // 
            // Tr2FileTextBox
            // 
            this.Tr2FileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr2FileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tr2FileTextBox.CausesValidation = false;
            this.Tr2FileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tr2FileTextBox.Enabled = false;
            this.Tr2FileTextBox.Location = new System.Drawing.Point(34, 9);
            this.Tr2FileTextBox.Name = "Tr2FileTextBox";
            this.Tr2FileTextBox.ShortcutsEnabled = false;
            this.Tr2FileTextBox.Size = new System.Drawing.Size(122, 20);
            this.Tr2FileTextBox.TabIndex = 0;
            this.Tr2FileTextBox.TabStop = false;
            // 
            // Tr2FileButton
            // 
            this.Tr2FileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr2FileButton.Enabled = false;
            this.Tr2FileButton.Location = new System.Drawing.Point(3, 8);
            this.Tr2FileButton.Name = "Tr2FileButton";
            this.Tr2FileButton.Size = new System.Drawing.Size(25, 23);
            this.Tr2FileButton.TabIndex = 0;
            this.Tr2FileButton.Text = "...";
            this.Tr2FileButton.UseVisualStyleBackColor = true;
            // 
            // Tr3FileTableLayout
            // 
            this.Tr3FileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr3FileTableLayout.ColumnCount = 2;
            this.Tr3FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.Tr3FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.Tr3FileTableLayout.Controls.Add(this.Tr3FileTextBox, 1, 0);
            this.Tr3FileTableLayout.Controls.Add(this.Tr3FileButton, 0, 0);
            this.Tr3FileTableLayout.Location = new System.Drawing.Point(192, 228);
            this.Tr3FileTableLayout.Name = "Tr3FileTableLayout";
            this.Tr3FileTableLayout.RowCount = 1;
            this.Tr3FileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Tr3FileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Tr3FileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.Tr3FileTableLayout.TabIndex = 30;
            // 
            // Tr3FileTextBox
            // 
            this.Tr3FileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr3FileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tr3FileTextBox.CausesValidation = false;
            this.Tr3FileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tr3FileTextBox.Enabled = false;
            this.Tr3FileTextBox.Location = new System.Drawing.Point(34, 9);
            this.Tr3FileTextBox.Name = "Tr3FileTextBox";
            this.Tr3FileTextBox.ShortcutsEnabled = false;
            this.Tr3FileTextBox.Size = new System.Drawing.Size(122, 20);
            this.Tr3FileTextBox.TabIndex = 0;
            this.Tr3FileTextBox.TabStop = false;
            // 
            // Tr3FileButton
            // 
            this.Tr3FileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr3FileButton.Enabled = false;
            this.Tr3FileButton.Location = new System.Drawing.Point(3, 8);
            this.Tr3FileButton.Name = "Tr3FileButton";
            this.Tr3FileButton.Size = new System.Drawing.Size(25, 23);
            this.Tr3FileButton.TabIndex = 0;
            this.Tr3FileButton.Text = "...";
            this.Tr3FileButton.UseVisualStyleBackColor = true;
            // 
            // Tr2GFileTableLayout
            // 
            this.Tr2GFileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr2GFileTableLayout.ColumnCount = 2;
            this.Tr2GFileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.Tr2GFileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.Tr2GFileTableLayout.Controls.Add(this.Tr2GFileTextBox, 1, 0);
            this.Tr2GFileTableLayout.Controls.Add(this.Tr2GFileButton, 0, 0);
            this.Tr2GFileTableLayout.Location = new System.Drawing.Point(192, 183);
            this.Tr2GFileTableLayout.Name = "Tr2GFileTableLayout";
            this.Tr2GFileTableLayout.RowCount = 1;
            this.Tr2GFileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Tr2GFileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.Tr2GFileTableLayout.TabIndex = 31;
            // 
            // Tr2GFileTextBox
            // 
            this.Tr2GFileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr2GFileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tr2GFileTextBox.CausesValidation = false;
            this.Tr2GFileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tr2GFileTextBox.Enabled = false;
            this.Tr2GFileTextBox.Location = new System.Drawing.Point(34, 9);
            this.Tr2GFileTextBox.Name = "Tr2GFileTextBox";
            this.Tr2GFileTextBox.ShortcutsEnabled = false;
            this.Tr2GFileTextBox.Size = new System.Drawing.Size(122, 20);
            this.Tr2GFileTextBox.TabIndex = 0;
            this.Tr2GFileTextBox.TabStop = false;
            // 
            // Tr2GFileButton
            // 
            this.Tr2GFileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr2GFileButton.Enabled = false;
            this.Tr2GFileButton.Location = new System.Drawing.Point(3, 8);
            this.Tr2GFileButton.Name = "Tr2GFileButton";
            this.Tr2GFileButton.Size = new System.Drawing.Size(25, 23);
            this.Tr2GFileButton.TabIndex = 0;
            this.Tr2GFileButton.Text = "...";
            this.Tr2GFileButton.UseVisualStyleBackColor = true;
            // 
            // TlaFileTableLayout
            // 
            this.TlaFileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TlaFileTableLayout.ColumnCount = 2;
            this.TlaFileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.TlaFileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.TlaFileTableLayout.Controls.Add(this.TlaFileTextBox, 1, 0);
            this.TlaFileTableLayout.Controls.Add(this.TlaFileButton, 0, 0);
            this.TlaFileTableLayout.Location = new System.Drawing.Point(192, 273);
            this.TlaFileTableLayout.Name = "TlaFileTableLayout";
            this.TlaFileTableLayout.RowCount = 1;
            this.TlaFileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TlaFileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TlaFileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.TlaFileTableLayout.TabIndex = 32;
            // 
            // TlaFileTextBox
            // 
            this.TlaFileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TlaFileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TlaFileTextBox.CausesValidation = false;
            this.TlaFileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.TlaFileTextBox.Enabled = false;
            this.TlaFileTextBox.Location = new System.Drawing.Point(34, 9);
            this.TlaFileTextBox.Name = "TlaFileTextBox";
            this.TlaFileTextBox.ShortcutsEnabled = false;
            this.TlaFileTextBox.Size = new System.Drawing.Size(122, 20);
            this.TlaFileTextBox.TabIndex = 0;
            this.TlaFileTextBox.TabStop = false;
            // 
            // TlaFileButton
            // 
            this.TlaFileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TlaFileButton.Enabled = false;
            this.TlaFileButton.Location = new System.Drawing.Point(3, 8);
            this.TlaFileButton.Name = "TlaFileButton";
            this.TlaFileButton.Size = new System.Drawing.Size(25, 23);
            this.TlaFileButton.TabIndex = 0;
            this.TlaFileButton.Text = "...";
            this.TlaFileButton.UseVisualStyleBackColor = true;
            // 
            // Tr4FileTableLayout
            // 
            this.Tr4FileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr4FileTableLayout.ColumnCount = 2;
            this.Tr4FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.Tr4FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.Tr4FileTableLayout.Controls.Add(this.Tr4FileTextBox, 1, 0);
            this.Tr4FileTableLayout.Controls.Add(this.Tr4FileButton, 0, 0);
            this.Tr4FileTableLayout.Location = new System.Drawing.Point(192, 318);
            this.Tr4FileTableLayout.Name = "Tr4FileTableLayout";
            this.Tr4FileTableLayout.RowCount = 1;
            this.Tr4FileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Tr4FileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Tr4FileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.Tr4FileTableLayout.TabIndex = 33;
            // 
            // Tr4FileTextBox
            // 
            this.Tr4FileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr4FileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tr4FileTextBox.CausesValidation = false;
            this.Tr4FileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tr4FileTextBox.Enabled = false;
            this.Tr4FileTextBox.Location = new System.Drawing.Point(34, 9);
            this.Tr4FileTextBox.Name = "Tr4FileTextBox";
            this.Tr4FileTextBox.ShortcutsEnabled = false;
            this.Tr4FileTextBox.Size = new System.Drawing.Size(122, 20);
            this.Tr4FileTextBox.TabIndex = 0;
            this.Tr4FileTextBox.TabStop = false;
            // 
            // Tr4FileButton
            // 
            this.Tr4FileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr4FileButton.Enabled = false;
            this.Tr4FileButton.Location = new System.Drawing.Point(3, 8);
            this.Tr4FileButton.Name = "Tr4FileButton";
            this.Tr4FileButton.Size = new System.Drawing.Size(25, 23);
            this.Tr4FileButton.TabIndex = 0;
            this.Tr4FileButton.Text = "...";
            this.Tr4FileButton.UseVisualStyleBackColor = true;
            // 
            // TteFileTableLayout
            // 
            this.TteFileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TteFileTableLayout.ColumnCount = 2;
            this.TteFileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.TteFileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.TteFileTableLayout.Controls.Add(this.TteFileTextBox, 1, 0);
            this.TteFileTableLayout.Controls.Add(this.TteFileButton, 0, 0);
            this.TteFileTableLayout.Location = new System.Drawing.Point(192, 363);
            this.TteFileTableLayout.Name = "TteFileTableLayout";
            this.TteFileTableLayout.RowCount = 1;
            this.TteFileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TteFileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TteFileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.TteFileTableLayout.TabIndex = 34;
            // 
            // TteFileTextBox
            // 
            this.TteFileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TteFileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TteFileTextBox.CausesValidation = false;
            this.TteFileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.TteFileTextBox.Enabled = false;
            this.TteFileTextBox.Location = new System.Drawing.Point(34, 9);
            this.TteFileTextBox.Name = "TteFileTextBox";
            this.TteFileTextBox.ShortcutsEnabled = false;
            this.TteFileTextBox.Size = new System.Drawing.Size(122, 20);
            this.TteFileTextBox.TabIndex = 0;
            this.TteFileTextBox.TabStop = false;
            // 
            // TteFileButton
            // 
            this.TteFileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TteFileButton.Enabled = false;
            this.TteFileButton.Location = new System.Drawing.Point(3, 8);
            this.TteFileButton.Name = "TteFileButton";
            this.TteFileButton.Size = new System.Drawing.Size(25, 23);
            this.TteFileButton.TabIndex = 0;
            this.TteFileButton.Text = "...";
            this.TteFileButton.UseVisualStyleBackColor = true;
            // 
            // Tr5FileTableLayout
            // 
            this.Tr5FileTableLayout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr5FileTableLayout.ColumnCount = 2;
            this.Tr5FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.52663F));
            this.Tr5FileTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.47337F));
            this.Tr5FileTableLayout.Controls.Add(this.Tr5FileTextBox, 1, 0);
            this.Tr5FileTableLayout.Controls.Add(this.Tr5FileButton, 0, 0);
            this.Tr5FileTableLayout.Location = new System.Drawing.Point(192, 408);
            this.Tr5FileTableLayout.Name = "Tr5FileTableLayout";
            this.Tr5FileTableLayout.RowCount = 1;
            this.Tr5FileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.Tr5FileTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Tr5FileTableLayout.Size = new System.Drawing.Size(159, 39);
            this.Tr5FileTableLayout.TabIndex = 35;
            // 
            // Tr5FileTextBox
            // 
            this.Tr5FileTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr5FileTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Tr5FileTextBox.CausesValidation = false;
            this.Tr5FileTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.Tr5FileTextBox.Enabled = false;
            this.Tr5FileTextBox.Location = new System.Drawing.Point(34, 9);
            this.Tr5FileTextBox.Name = "Tr5FileTextBox";
            this.Tr5FileTextBox.ShortcutsEnabled = false;
            this.Tr5FileTextBox.Size = new System.Drawing.Size(122, 20);
            this.Tr5FileTextBox.TabIndex = 0;
            this.Tr5FileTextBox.TabStop = false;
            // 
            // Tr5FileButton
            // 
            this.Tr5FileButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Tr5FileButton.Enabled = false;
            this.Tr5FileButton.Location = new System.Drawing.Point(3, 8);
            this.Tr5FileButton.Name = "Tr5FileButton";
            this.Tr5FileButton.Size = new System.Drawing.Size(25, 23);
            this.Tr5FileButton.TabIndex = 0;
            this.Tr5FileButton.Text = "...";
            this.Tr5FileButton.UseVisualStyleBackColor = true;
            // 
            // ComponentSettings
            // 
            this.Controls.Add(this.GameSelectionTableLayout);
            this.Controls.Add(this.AutosplitterVersionLabel);
            this.Controls.Add(this.GameVersionLabel);
            this.Name = "ComponentSettings";
            this.Size = new System.Drawing.Size(480, 510);
            this.GameSelectionTableLayout.ResumeLayout(false);
            this.GameSelectionTableLayout.PerformLayout();
            this.Tr1FileTableLayout.ResumeLayout(false);
            this.Tr1FileTableLayout.PerformLayout();
            this.TrUbFileTableLayout.ResumeLayout(false);
            this.TrUbFileTableLayout.PerformLayout();
            this.Tr2FileTableLayout.ResumeLayout(false);
            this.Tr2FileTableLayout.PerformLayout();
            this.Tr3FileTableLayout.ResumeLayout(false);
            this.Tr3FileTableLayout.PerformLayout();
            this.Tr2GFileTableLayout.ResumeLayout(false);
            this.Tr2GFileTableLayout.PerformLayout();
            this.TlaFileTableLayout.ResumeLayout(false);
            this.TlaFileTableLayout.PerformLayout();
            this.Tr4FileTableLayout.ResumeLayout(false);
            this.Tr4FileTableLayout.PerformLayout();
            this.TteFileTableLayout.ResumeLayout(false);
            this.TteFileTableLayout.PerformLayout();
            this.Tr5FileTableLayout.ResumeLayout(false);
            this.Tr5FileTableLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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

        private void FileButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                var button = (Button) sender;
                switch (button.Name.Replace("FileButton", string.Empty))
                {
                    case "Tr1":
                        Tr1FileTextBox.Text = fileName;
                        break;
                    case "TrUb":
                        TrUbFileTextBox.Text = fileName;
                        break;
                    case "Tr2":
                        Tr2FileTextBox.Text = fileName;
                        break;
                    case "Tr2G":
                        Tr2GFileTextBox.Text = fileName;
                        break;
                    case "Tr3":
                        Tr3FileTextBox.Text = fileName;
                        break;
                    case "Tla":
                        TlaFileTextBox.Text = fileName;
                        break;
                    case "Tr4":
                        Tr4FileTextBox.Text = fileName;
                        break;
                    case "Tte":
                        TteFileTextBox.Text = fileName;
                        break;
                    case "Tr5":
                        Tr5FileTextBox.Text = fileName;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
