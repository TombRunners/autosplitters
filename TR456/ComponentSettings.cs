using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Util;

namespace TR456;

public sealed class ComponentSettings : UserControl
{
    #region UI Declarations

    private GroupBox _runTypeSelect;
    public RadioButton IlOrAreaButton;
    public RadioButton FullGameButton;
    public RadioButton DeathrunButton;
    private Label _gameTimeMethodLabel;
    public CheckBox EnableAutoResetCheckbox;
    private GroupBox _pickupSplitSelect;
    public RadioButton SplitNoPickupsButton;
    public RadioButton SplitAllPickupsButton;
    public RadioButton SplitSecretsOnlyButton;
    public CheckBox SplitSecurityBreachCheckbox;

    private GroupBox _levelTransitionSelect;
    private ToolTip _toolTip;
    private Label _levelTransitionActiveTabLabel;
    private Button _tr4LevelSettingsButton;
    private Button _tr5LevelSettingsButton;
    private Button _tr6LevelSettingsButton;
    private Panel _tr4LevelTransitionSettingsPanel;
    private Button _tr4SelectAllButton;
    private Button _tr4UnselectAllButton;
    private Panel _tr5LevelTransitionSettingsPanel;
    private Panel _tr6LevelTransitionSettingsPanel;
    private Button _tr6SelectAllButton;
    private Button _tr6UnselectAllButton;

    public Label GameVersionLabel;
    public Label AutosplitterVersionLabel;
    private LinkLabel _guideLabel;
    private Label _aslWarningLabel;
    private Label _timerWarningLabel;
    private Label _signatureScanStatusLabel;

    #endregion

    private const string LevelTransitionSettingsTextDefault = "Level Transition Settings";
    private const string PickupSplitSettingDefault = "Split Pickups";

    public RunType RunType;
    public GameTimeMethod GameTimeMethod;
    public bool EnableAutoReset;
    public PickupSplitSetting PickupSplitSetting;
    public bool SplitSecurityBreach;

    public bool FullGame => RunType == RunType.FullGame;
    public bool IlOrArea => RunType == RunType.IndividualLevelOrArea;
    public bool Deathrun => RunType == RunType.Deathrun;

    internal static bool GameVersionInitialized;

    private SignatureScanInfo _displayInfo;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SuspendLayout();

        Control parent = Parent;
        Control parentParent = parent?.Parent;
        Control componentSettingsDialog = parentParent?.Parent;
        if (componentSettingsDialog is not null)
        {
            componentSettingsDialog.SuspendLayout();

            componentSettingsDialog.Size = new Size(Width + 20, Height + 100);
            parentParent.Size            = new Size(Width + 10, Height + 50);
            parent.Size                  = new Size(Width + 10, Height + 50);

            componentSettingsDialog.ResumeLayout(false);
            componentSettingsDialog.PerformLayout();
        }

        PopulateLevelControls();

        if (GameData.GameProcess is not null && GameData.GameIsInitialized)
            ShowCorrectTab(GameData.CurrentActiveBaseGame);

        EnableControlsPerState();

        ResumeLayout(false);
        PerformLayout();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        ResetScrolling();
        base.OnHandleDestroyed(e);
    }

    public ComponentSettings()
    {
        InitializeComponent();
        SetGameTimeMethod(RunType == RunType.Deathrun ? GameTimeMethod.Igt : GameTimeMethod.RtaNoLoads);
        GameVersionInitialized = false;
        EnableControlsPerState();
    }

    public void SetLayoutWarningLabelVisibilities(bool aslComponentIsPresent, bool timerWithGameTimeIsPresent)
    {
        _aslWarningLabel.Visible = aslComponentIsPresent;
        _timerWarningLabel.Visible = !timerWithGameTimeIsPresent;
    }

    public void SetSignatureScanStatusLabel(SignatureScanInfo info)
    {
        _displayInfo = info;
        if (IsHandleCreated)
            UpdateSignatureScanStatusLabel();
    }

    private void UpdateSignatureScanStatusLabel()
    {
        if (_displayInfo is null)
            return;

        SuspendLayout();

        switch (_displayInfo.Status)
        {
            case SignatureScanStatus.NotTriedYet:
            case SignatureScanStatus.Success:
                _signatureScanStatusLabel.Visible = false;
                break;

            case SignatureScanStatus.Retrying:
                _signatureScanStatusLabel.Text = $"Address scan failed! Retrying... ({_displayInfo.RetryCount} of {_displayInfo.MaxRetries})";
                _signatureScanStatusLabel.ForeColor = Color.DarkGoldenrod;
                _signatureScanStatusLabel.Visible = true;
                break;

            case SignatureScanStatus.Failure:
                _signatureScanStatusLabel.Text = $"Address scan failed! Max retries reached. ({_displayInfo.MaxRetries} of {_displayInfo.MaxRetries})";
                _signatureScanStatusLabel.ForeColor = Color.Crimson;
                _signatureScanStatusLabel.Visible = true;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(_displayInfo.Status), _displayInfo.Status, "Unknown signature scan status");
        }

        ResumeLayout();
        PerformLayout();
    }

    public void SetGameVersion(VersionDetectionResult result)
    {
        const string noneUndetected = "Game Version: None / Undetected";
        const string gogV10 = "GOG v1.0";
        const string publicV10 = "GOG v1.0.0 / Steam 17156603 / EGS TRX2_250128_19221_WIN";
        const string patch1 = "GOG v1.0.0_Patch_1 / Steam 17983102 / EGS TRX2_250404_20819_WIN";
        const string patch2 = "Steam 19062321 / EGS TRX2_250701_21868_WIN.7z";
        const string patch2Hotfix1 = "Steam 19923088 / EGS TRX2_250910_21914_WIN.7z";

        GameVersionLabel.Text = result switch
        {
            VersionDetectionResult.None            => noneUndetected,
            VersionDetectionResult.Unknown unknown => $"Found unknown version, MD5 hash: {unknown.Hash}",
            VersionDetectionResult.Found found     => GameVersionLabel.Text =
                "Game Version: " +
                (GameVersion)found.Version switch
                {
                    GameVersion.GogV10        => gogV10,
                    GameVersion.PublicV10     => publicV10,
                    GameVersion.Patch1        => patch1,
                    GameVersion.Patch2        => patch2,
                    GameVersion.Patch2Hotfix1 => patch2Hotfix1,
                    _ => throw new ArgumentOutOfRangeException(nameof(found.Version)),
                },
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };

        GameVersionInitialized = true;
    }

    private void SetGameTimeMethod(GameTimeMethod method)
    {
        string methodText = method switch
        {
            GameTimeMethod.Igt        => "IGT",
            GameTimeMethod.RtaNoLoads => "RTA w/o Loads",
            _                         => "Unknown",
        };

        _gameTimeMethodLabel.Text = $"Game Time Method:{Environment.NewLine}{methodText}";
        GameTimeMethod = method;
    }

    private void SetAllTr4CheckBoxes(bool check)
    {
        foreach (CheckBox checkBox in _tr4LevelTransitionSettingsPanel.Controls.OfType<CheckBox>().Where(static cb => cb.Enabled))
            checkBox.Checked = check; // This triggers checkBox.CheckedChanged.
    }

    private void SetAllTr6CheckBoxes(bool check)
    {
        foreach (CheckBox checkBox in _tr6LevelTransitionSettingsPanel.Controls.OfType<CheckBox>().Where(static cb => cb.Enabled))
            checkBox.Checked = check; // This triggers checkBox.CheckedChanged.
    }

    // ReSharper disable once FunctionComplexityOverflow
    private void InitializeComponent()
    {
        _runTypeSelect = new GroupBox();
        FullGameButton = new RadioButton();
        IlOrAreaButton = new RadioButton();
        DeathrunButton = new RadioButton();
        _gameTimeMethodLabel = new Label();
        EnableAutoResetCheckbox = new CheckBox();
        _pickupSplitSelect = new GroupBox();
        SplitNoPickupsButton = new RadioButton();
        SplitAllPickupsButton = new RadioButton();
        SplitSecretsOnlyButton = new RadioButton();
        SplitSecurityBreachCheckbox = new CheckBox();

        _levelTransitionSelect = new GroupBox();
        _toolTip = new ToolTip();
        _levelTransitionActiveTabLabel = new Label();
        _tr4LevelSettingsButton = new Button();
        _tr5LevelSettingsButton = new Button();
        _tr6LevelSettingsButton = new Button();
        _tr4LevelTransitionSettingsPanel = new Panel();
        _tr4SelectAllButton = new Button();
        _tr4UnselectAllButton = new Button();
        _tr5LevelTransitionSettingsPanel = new Panel();
        _tr6LevelTransitionSettingsPanel = new Panel();
        _tr6SelectAllButton = new Button();
        _tr6UnselectAllButton = new Button();

        GameVersionLabel = new Label();
        AutosplitterVersionLabel = new Label();
        _guideLabel = new LinkLabel();
        _aslWarningLabel = new Label();
        _timerWarningLabel = new Label();
        _signatureScanStatusLabel = new Label();
        _runTypeSelect.SuspendLayout();
        _pickupSplitSelect.SuspendLayout();
        _levelTransitionSelect.SuspendLayout();
        SuspendLayout();

        #region _runTypeSelect and Radio Buttons

        _runTypeSelect.Controls.Add(FullGameButton);
        _runTypeSelect.Controls.Add(IlOrAreaButton);
        _runTypeSelect.Controls.Add(DeathrunButton);
        _runTypeSelect.Location = new Point(10, 5);
        _runTypeSelect.Name = "_runTypeSelect";
        _runTypeSelect.Size = new Size(280, 45);
        _runTypeSelect.TabIndex = 0;
        _runTypeSelect.TabStop = false;
        _runTypeSelect.Text = "Run Type";

        // FullGameButton
        FullGameButton.AutoSize = true;
        FullGameButton.Checked = true;
        FullGameButton.Location = new Point(6, 20);
        FullGameButton.Name = "FullGameButton";
        FullGameButton.Size = new Size(85, 17);
        FullGameButton.TabIndex = 0;
        FullGameButton.TabStop = true;
        FullGameButton.Text = "Full Game";
        FullGameButton.UseVisualStyleBackColor = true;
        FullGameButton.CheckedChanged += FullGameButtonCheckedChanged;

        // IlOrAreaButton
        IlOrAreaButton.AutoSize = true;
        IlOrAreaButton.Location = new Point(96, 20);
        IlOrAreaButton.Name = "IlOrAreaButton";
        IlOrAreaButton.Size = new Size(91, 17);
        IlOrAreaButton.TabIndex = 1;
        IlOrAreaButton.Text = "IL or Area%";
        IlOrAreaButton.UseVisualStyleBackColor = true;
        IlOrAreaButton.CheckedChanged += IlOrAreaButtonCheckedChanged;

        // DeathrunButton
        DeathrunButton.AutoSize = true;
        DeathrunButton.Location = new Point(192, 20);
        DeathrunButton.Name = "DeathrunButton";
        DeathrunButton.Size = new Size(79, 17);
        DeathrunButton.TabIndex = 2;
        DeathrunButton.Text = "Deathrun";
        DeathrunButton.UseVisualStyleBackColor = true;
        DeathrunButton.CheckedChanged += DeathrunButtonCheckedChanged;

        #endregion

        // _gameTimeMethodLabel
        _gameTimeMethodLabel.AutoSize = true;
        _gameTimeMethodLabel.Location = new Point(335, 18);
        _gameTimeMethodLabel.Name = "_gameTimeMethodLabel";
        _gameTimeMethodLabel.Size = new Size(200, 30);
        _gameTimeMethodLabel.TabIndex = 0;
        _gameTimeMethodLabel.Text = "Game Time Method:" + Environment.NewLine + "Not Initialized!";
        _gameTimeMethodLabel.TextAlign = ContentAlignment.MiddleCenter;

        // EnableAutoResetCheckbox
        EnableAutoResetCheckbox.AutoSize = true;
        EnableAutoResetCheckbox.Checked = false;
        EnableAutoResetCheckbox.Location = new Point(12, 69);
        EnableAutoResetCheckbox.Size = new Size(72, 17);
        EnableAutoResetCheckbox.Name = "EnableAutoResetCheckbox";
        EnableAutoResetCheckbox.Text = "Enable Auto-Reset";
        EnableAutoResetCheckbox.TabIndex = 0;
        EnableAutoResetCheckbox.UseVisualStyleBackColor = true;
        EnableAutoResetCheckbox.CheckedChanged += EnableAutoResetCheckboxCheckedChanged;

        #region _pickupSplitSelect and Radio Buttons

        // PickupSplitSelect
        _pickupSplitSelect.Controls.Add(SplitNoPickupsButton);
        _pickupSplitSelect.Controls.Add(SplitAllPickupsButton);
        _pickupSplitSelect.Controls.Add(SplitSecretsOnlyButton);
        _pickupSplitSelect.Location = new Point(140, 55);
        _pickupSplitSelect.Name = "_pickupSplitSelect";
        _pickupSplitSelect.Size = new Size(336, 45);
        _pickupSplitSelect.TabIndex = 0;
        _pickupSplitSelect.TabStop = false;
        _pickupSplitSelect.Text = "Split Pickups";

        // SplitNoPickupsButton
        SplitNoPickupsButton.AutoSize = true;
        SplitNoPickupsButton.Checked = true;
        SplitNoPickupsButton.Location = new Point(6, 20);
        SplitNoPickupsButton.Name = "SplitNoPickupsButton";
        SplitNoPickupsButton.Size = new Size(58, 17);
        SplitNoPickupsButton.TabIndex = 0;
        SplitNoPickupsButton.TabStop = true;
        SplitNoPickupsButton.Text = "None";
        SplitNoPickupsButton.UseVisualStyleBackColor = true;
        SplitNoPickupsButton.CheckedChanged += SplitNoPickupsButtonCheckedChanged;

        // SplitAllPickupsButton
        SplitAllPickupsButton.AutoSize = true;
        SplitAllPickupsButton.Location = new Point(69, 20);
        SplitAllPickupsButton.Name = "SplitAllPickupsButton";
        SplitAllPickupsButton.Size = new Size(41, 17);
        SplitAllPickupsButton.TabIndex = 1;
        SplitAllPickupsButton.Text = "All";
        SplitAllPickupsButton.UseVisualStyleBackColor = true;
        SplitAllPickupsButton.CheckedChanged += SplitAllPickupsButtonCheckedChanged;

        // SplitSecretsOnlyButton
        SplitSecretsOnlyButton.AutoSize = true;
        SplitSecretsOnlyButton.Location = new Point(115, 20);
        SplitSecretsOnlyButton.Name = "SplitSecretsOnlyButton";
        SplitSecretsOnlyButton.Size = new Size(220, 17);
        SplitSecretsOnlyButton.TabIndex = 2;
        SplitSecretsOnlyButton.Text = "Secrets Only [TR6 Secrets = Chocobars]";
        SplitSecretsOnlyButton.UseVisualStyleBackColor = true;
        SplitSecretsOnlyButton.CheckedChanged += SplitSecretsOnlyButtonCheckedChanged;

        #endregion

        // _toolTip
        _toolTip.AutoPopDelay = 60000;
        _toolTip.InitialDelay = 500;
        _toolTip.ReshowDelay = 250;

        #region _levelTransitionSelect

        // _levelTransitionSelect
        _levelTransitionSelect.Controls.Add(_levelTransitionActiveTabLabel);
        _levelTransitionSelect.Controls.Add(_tr4LevelSettingsButton);
        _levelTransitionSelect.Controls.Add(_tr5LevelSettingsButton);
        _levelTransitionSelect.Controls.Add(_tr6LevelSettingsButton);
        _levelTransitionSelect.Controls.Add(_tr4LevelTransitionSettingsPanel);
        _levelTransitionSelect.Controls.Add(_tr4SelectAllButton);
        _levelTransitionSelect.Controls.Add(_tr4UnselectAllButton);
        _levelTransitionSelect.Controls.Add(_tr5LevelTransitionSettingsPanel);
        _levelTransitionSelect.Controls.Add(_tr6LevelTransitionSettingsPanel);
        _levelTransitionSelect.Controls.Add(_tr6SelectAllButton);
        _levelTransitionSelect.Controls.Add(_tr6UnselectAllButton);
        _levelTransitionSelect.Location = new Point(0, 105);
        _levelTransitionSelect.Name = "_levelTransitionSelect";
        _levelTransitionSelect.Size = new Size(476, 325);
        _levelTransitionSelect.Text = "Level Transition Settings";

        // _levelTransitionActiveTabLabel
        _levelTransitionActiveTabLabel.AutoSize = true;
        _levelTransitionActiveTabLabel.Location = new Point(180, 17);
        _levelTransitionActiveTabLabel.Name = "_levelTransitionActiveTabLabel";
        _levelTransitionActiveTabLabel.Size = new Size(150, 30);
        _levelTransitionActiveTabLabel.Text = "Currently showing: TR4";
        _levelTransitionActiveTabLabel.TextAlign = ContentAlignment.MiddleCenter;

        // _tr4LevelSettingsButton
        _tr4LevelSettingsButton.Location = new Point(320, 13);
        _tr4LevelSettingsButton.Size = new Size(45, 21);
        _tr4LevelSettingsButton.Margin = Padding.Empty;
        _tr4LevelSettingsButton.Padding = Padding.Empty;
        _tr4LevelSettingsButton.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
        _tr4LevelSettingsButton.Text = "TR4";
        _tr4LevelSettingsButton.Click += _tr4LevelSettingsButtonClicked;
        _tr4LevelSettingsButton.Enabled = true;
        _tr4LevelSettingsButton.Visible = true;

        // _tr5LevelSettingsButton
        _tr5LevelSettingsButton.Location = new Point(370, 13);
        _tr5LevelSettingsButton.Size = new Size(45, 21);
        _tr5LevelSettingsButton.Margin = Padding.Empty;
        _tr5LevelSettingsButton.Padding = Padding.Empty;
        _tr5LevelSettingsButton.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
        _tr5LevelSettingsButton.Text = "TR5";
        _tr5LevelSettingsButton.Click += _tr5LevelSettingsButtonClicked;
        _tr5LevelSettingsButton.Enabled = true;
        _tr5LevelSettingsButton.Visible = true;

        // _tr6LevelSettingsButton
        _tr6LevelSettingsButton.Location = new Point(420, 13);
        _tr6LevelSettingsButton.Size = new Size(45, 21);
        _tr6LevelSettingsButton.Margin = Padding.Empty;
        _tr6LevelSettingsButton.Padding = Padding.Empty;
        _tr6LevelSettingsButton.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
        _tr6LevelSettingsButton.Text = "TR6";
        _tr6LevelSettingsButton.Click += _tr6LevelSettingsButtonClicked;
        _tr6LevelSettingsButton.Enabled = true;
        _tr6LevelSettingsButton.Visible = true;

        // _tr4LevelTransitionSettingsPanel
        _tr4LevelTransitionSettingsPanel.Location = new Point(5, 35);
        _tr4LevelTransitionSettingsPanel.Size = new Size(466, 254);
        _tr4LevelTransitionSettingsPanel.AutoScroll = true;
        _tr4LevelTransitionSettingsPanel.AutoScrollMinSize = new Size(0, _levelTransitionSelect.Height);
        _tr4LevelTransitionSettingsPanel.VerticalScroll.Enabled = true;
        _tr4LevelTransitionSettingsPanel.VerticalScroll.Visible = true;
        _tr4LevelTransitionSettingsPanel.HorizontalScroll.Enabled = false;
        _tr4LevelTransitionSettingsPanel.HorizontalScroll.Visible = false;
        _tr4LevelTransitionSettingsPanel.Enabled = true;
        _tr4LevelTransitionSettingsPanel.Visible = true;
        _tr4LevelTransitionSettingsPanel.TabStop = true;

        // _tr4SelectAllButton
        _tr4SelectAllButton.Location = new Point(5, 294);
        _tr4SelectAllButton.Size = new Size(120, 25);
        _tr4SelectAllButton.Text = "Select All (TR4)";
        _tr4SelectAllButton.Click += _tr4SelectAllButtonClicked;
        _tr4SelectAllButton.Enabled = true;
        _tr4SelectAllButton.Visible = true;

        // _tr4UnselectAllButton
        _tr4UnselectAllButton.Location = new Point(331, 294);
        _tr4UnselectAllButton.Size = new Size(120, 25);
        _tr4UnselectAllButton.Text = "Unselect All (TR4)";
        _tr4UnselectAllButton.Click += _tr4UnselectAllButtonClicked;
        _tr4UnselectAllButton.Enabled = true;
        _tr4UnselectAllButton.Visible = true;

        // _tr5LevelTransitionSettingsPanel
        _tr5LevelTransitionSettingsPanel.Location = new Point(5, 35);
        _tr5LevelTransitionSettingsPanel.Size = new Size(466, 100);
        _tr5LevelTransitionSettingsPanel.AutoScroll = true;
        _tr5LevelTransitionSettingsPanel.VerticalScroll.Enabled = false;
        _tr5LevelTransitionSettingsPanel.VerticalScroll.Visible = false;
        _tr5LevelTransitionSettingsPanel.HorizontalScroll.Enabled = false;
        _tr5LevelTransitionSettingsPanel.HorizontalScroll.Visible = false;
        _tr5LevelTransitionSettingsPanel.Enabled = false;
        _tr5LevelTransitionSettingsPanel.Visible = false;
        _tr5LevelTransitionSettingsPanel.TabStop = false;

        // SplitSecurityBreachCheckbox
        SplitSecurityBreachCheckbox.AutoSize = true;
        SplitSecurityBreachCheckbox.Checked = false;
        SplitSecurityBreachCheckbox.Location = new Point(0, 5);
        SplitSecurityBreachCheckbox.Size = new Size(72, 17);
        SplitSecurityBreachCheckbox.Name = "SplitSecurityBreachCheckbox";
        SplitSecurityBreachCheckbox.Text = "Split Security Breach Cutscene\nThis applies to FG, IL, and Area% runs";
        SplitSecurityBreachCheckbox.TabIndex = 0;
        SplitSecurityBreachCheckbox.UseVisualStyleBackColor = true;
        SplitSecurityBreachCheckbox.CheckedChanged += SplitSecurityBreachCheckboxCheckedChanged;

        _tr5LevelTransitionSettingsPanel.Controls.Add(SplitSecurityBreachCheckbox);

        // _tr6LevelTransitionSettingsPanel
        _tr6LevelTransitionSettingsPanel.Location = new Point(5, 35);
        _tr6LevelTransitionSettingsPanel.Size = new Size(466, 254);
        _tr6LevelTransitionSettingsPanel.AutoScroll = true;
        _tr6LevelTransitionSettingsPanel.AutoScrollMinSize = new Size(0, _levelTransitionSelect.Height);
        _tr6LevelTransitionSettingsPanel.VerticalScroll.Enabled = true;
        _tr6LevelTransitionSettingsPanel.VerticalScroll.Visible = true;
        _tr6LevelTransitionSettingsPanel.HorizontalScroll.Enabled = false;
        _tr6LevelTransitionSettingsPanel.HorizontalScroll.Visible = false;
        _tr6LevelTransitionSettingsPanel.Enabled = false;
        _tr6LevelTransitionSettingsPanel.Visible = false;
        _tr6LevelTransitionSettingsPanel.TabStop = false;

        // _tr6SelectAllButton
        _tr6SelectAllButton.Location = new Point(5, 294);
        _tr6SelectAllButton.Size = new Size(120, 25);
        _tr6SelectAllButton.Text = "Select All (TR6)";
        _tr6SelectAllButton.Click += _tr6SelectAllButtonClicked;
        _tr6SelectAllButton.Enabled = false;
        _tr6SelectAllButton.Visible = false;

        // _tr6UnselectAllButton
        _tr6UnselectAllButton.Location = new Point(331, 294);
        _tr6UnselectAllButton.Size = new Size(120, 25);
        _tr6UnselectAllButton.Text = "Unselect All (TR6)";
        _tr6UnselectAllButton.Click += _tr6UnselectAllButtonClicked;
        _tr6UnselectAllButton.Enabled = false;
        _tr6UnselectAllButton.Visible = false;

        #endregion

        #region GameVersion and AutosplitterVersion Labels

        // GameVersionLabel
        GameVersionLabel.AutoSize = true;
        GameVersionLabel.Location = new Point(10, 434);
        GameVersionLabel.Name = "GameVersionLabel";
        GameVersionLabel.Size = new Size(200, 15);
        GameVersionLabel.TabIndex = 1;
        GameVersionLabel.Text = "Game Version: None / Undetected";

        // AutosplitterVersionLabel
        AutosplitterVersionLabel.AutoSize = true;
        AutosplitterVersionLabel.Location = new Point(10, 454);
        AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
        AutosplitterVersionLabel.Size = new Size(200, 15);
        AutosplitterVersionLabel.TabIndex = 2;
        AutosplitterVersionLabel.Text = "Autosplitter Version: " + Assembly.GetCallingAssembly().GetName().Version;

        #endregion

        #region Warning Labels

        // _aslWarningLabel
        _aslWarningLabel.AutoSize = false;
        _aslWarningLabel.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
        _aslWarningLabel.ForeColor = Color.Crimson;
        _aslWarningLabel.Location = new Point(0, 470);
        _aslWarningLabel.Name = "_aslWarningLabel";
        _aslWarningLabel.Size = new Size(476, 20);
        _aslWarningLabel.TabStop = false;
        _aslWarningLabel.Text = "Scriptable Auto Splitter in Layout — Please Remove!";
        _aslWarningLabel.TextAlign = ContentAlignment.MiddleCenter;
        _aslWarningLabel.Visible = false;

        // _timerWarningLabel
        _timerWarningLabel.AutoSize = false;
        _timerWarningLabel.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
        _timerWarningLabel.ForeColor = Color.Crimson;
        _timerWarningLabel.Location = new Point(0, 490);
        _timerWarningLabel.TextAlign = ContentAlignment.MiddleCenter;
        _timerWarningLabel.Name = "_timerWarningLabel";
        _timerWarningLabel.Size = new Size(476, 20);
        _timerWarningLabel.TabStop = false;
        _timerWarningLabel.Text = "No Game Time Timer in Layout — Please Fix!";
        _timerWarningLabel.Visible = false;

        // _signatureScanStatusLabel
        _signatureScanStatusLabel.AutoSize = false;
        _signatureScanStatusLabel.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold);
        _signatureScanStatusLabel.ForeColor = Color.Crimson;
        _signatureScanStatusLabel.Location = new Point(0, 510);
        _signatureScanStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
        _signatureScanStatusLabel.Name = "_signatureScanStatusLabel";
        _signatureScanStatusLabel.Size = new Size(476, 20);
        _signatureScanStatusLabel.TabStop = false;
        _signatureScanStatusLabel.Text = "Address scan failed!";
        _signatureScanStatusLabel.Visible = false;

        #endregion

        // _guideLabel
        _guideLabel.AutoSize = false;
        _guideLabel.Font = new Font(FontFamily.GenericSansSerif, 8.25f);
        _guideLabel.Location = new Point(0, 532);
        _guideLabel.TextAlign = ContentAlignment.MiddleCenter;
        _guideLabel.LinkClicked += GuideLinkClicked;
        _guideLabel.Links.Add(11, 55, "https://www.speedrun.com/tr456_remastered/guides/h548a");
        _guideLabel.Name = "_guideLabel";
        _guideLabel.Size = new Size(476, 15);
        _guideLabel.TabIndex = 3;
        _guideLabel.Text = "Need help? https://www.speedrun.com/tr456_remastered/guides/h548a";

        // ComponentSettings
        Controls.Add(_runTypeSelect);
        Controls.Add(_gameTimeMethodLabel);
        Controls.Add(EnableAutoResetCheckbox);
        Controls.Add(_pickupSplitSelect);
        Controls.Add(_levelTransitionSelect);
        Controls.Add(GameVersionLabel);
        Controls.Add(AutosplitterVersionLabel);
        Controls.Add(_aslWarningLabel);
        Controls.Add(_timerWarningLabel);
        Controls.Add(_signatureScanStatusLabel);
        Controls.Add(_guideLabel);
        Name = "ComponentSettings";
        Size = new Size(476, 550);

        _runTypeSelect.ResumeLayout(false);
        _pickupSplitSelect.ResumeLayout(false);
        _levelTransitionSelect.ResumeLayout(false);

        ResumeLayout(false);
        PerformLayout();
    }

    private void PopulateLevelControls()
    {
        _toolTip.RemoveAll();
        PopulateTr4LevelControls(Tr4LevelTransitions);
        PopulateTr6LevelControls(Tr6LevelTransitions);
        ResetScrolling();
    }

    private void PopulateTr4LevelControls(List<Tr4LevelTransitionSetting> referenceList)
    {
        const int rowHeight = 22;
        _tr4LevelTransitionSettingsPanel.Controls.Clear();

        var firstEntry = true;
        var yOffset = 0;
        var font = new Font(_levelTransitionSelect.Font, FontStyle.Regular);
        foreach (Tr4LevelTransitionSetting transition in referenceList)
        {
            // Section
            if (!string.IsNullOrEmpty(transition.Section))
            {
                yOffset += 3; // Magic number that makes it look even on the top and bottom.

                if (!firstEntry)
                    yOffset += 3; // Magic number that makes it look even on the top and bottom.

                var label = new Label
                {
                    Text = transition.Section,
                    Font = new Font(font, FontStyle.Bold),
                    Location = new Point(0, yOffset),
                    Size = new Size(400, 20),
                    Padding = Padding with { Left = 0, Right = 0 },
                };
                _tr4LevelTransitionSettingsPanel.Controls.Add(label);

                yOffset += rowHeight;
            }

            // CheckBox
            int widthNeeded = TextRenderer.MeasureText(transition.DisplayName(), font).Width + 20;
            const int checkBoxPadding = 1;
            CheckBox firstSplitCheckBox = null;
            CheckBox secondSplitCheckBox;
            if (transition.ComplexIgnore)
            {
                firstSplitCheckBox = new CheckBox
                {
                    Location = new Point(0, yOffset),
                    Size = new Size(20, 20),
                    Padding = Padding with { Left = 0, Right = 0 },
                    Checked = transition.UnusedLevelNumber is 39 || transition.Active is ActiveSetting.Active or ActiveSetting.IgnoreSecond,
                    Enabled = transition.CanBeConfigured,
                };
                _tr4LevelTransitionSettingsPanel.Controls.Add(firstSplitCheckBox);

                secondSplitCheckBox = new CheckBox
                {
                    Text = transition.DisplayName(),
                    Location = new Point(firstSplitCheckBox.Width + checkBoxPadding, yOffset),
                    Size = new Size(widthNeeded, 20),
                    Padding = Padding with { Left = 0, Right = 0 },
                    Checked = transition.UnusedLevelNumber is 39 || transition.Active is ActiveSetting.Active or ActiveSetting.IgnoreFirst,
                    Enabled = transition.CanBeConfigured,
                };
            }
            else
            {
                secondSplitCheckBox = new CheckBox
                {
                    Text = transition.DisplayName(),
                    Location = new Point(0, yOffset),
                    Size = new Size(widthNeeded, 20),
                    Padding = Padding with { Left = 0, Right = 0 },
                    Checked = transition.UnusedLevelNumber is 39 || transition.Active is ActiveSetting.Active,
                    Enabled = transition.CanBeConfigured,
                };
            }

            _tr4LevelTransitionSettingsPanel.Controls.Add(secondSplitCheckBox);

            // ToolTip
            if (!string.IsNullOrEmpty(transition.ToolTip))
            {
                if (firstSplitCheckBox is not null)
                    _toolTip.SetToolTip(firstSplitCheckBox, transition.ToolTip);

                secondSplitCheckBox.Text += " ℹ️";
                secondSplitCheckBox.Width += 20;
                _toolTip.SetToolTip(secondSplitCheckBox, transition.ToolTip);
            }

            // ComboBox
            if (transition.Directionality == TransitionDirection.TwoWay)
            {
                const int maxWidth = 180;
                int firstCheckWidthAndPadding = firstSplitCheckBox?.Width + checkBoxPadding ?? 0;
                int availableWidth = 448 - secondSplitCheckBox.Width - firstCheckWidthAndPadding;
                int width = Math.Min(maxWidth, availableWidth);

                var directionComboBox = new ComboBox
                {
                    Location = new Point(448 - width, yOffset - 2),
                    Size = new Size(width, 20),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = font,
                    Padding = Padding with { Left = 0, Right = 0 },
                };

                string lowerName = transition.LowerLevel.Description();
                string higherName = transition.HigherLevel.Description();
                directionComboBox.Items.AddRange(["Two-Way", $"From {lowerName}", $"From {higherName}"]);

                directionComboBox.SelectedIndex = (int)transition.SelectedDirectionality;
                directionComboBox.SelectedIndexChanged += (_, _) => { transition.SelectedDirectionality = (TransitionDirection)directionComboBox.SelectedIndex; };

                EventHandler handler = (_, _) =>
                {
                    ActiveSetting activeSetting;
                    if (firstSplitCheckBox is null)
                    {
                        activeSetting = secondSplitCheckBox.Checked
                            ? ActiveSetting.Active
                            : ActiveSetting.IgnoreAll;
                    }
                    else
                    {
                        bool firstEnabled = firstSplitCheckBox is not null && firstSplitCheckBox.Checked;
                        bool secondEnabled = secondSplitCheckBox.Checked;
                        activeSetting = (firstEnabled, secondEnabled) switch
                        {
                            (true, true)   => ActiveSetting.Active,
                            (false, true)  => ActiveSetting.IgnoreFirst,
                            (true, false)  => ActiveSetting.IgnoreSecond,
                            (false, false) => ActiveSetting.IgnoreAll,
                        };
                    }

                    transition.UpdateActive(activeSetting);
                    directionComboBox.Enabled = activeSetting is not ActiveSetting.IgnoreAll;
                };

                if (firstSplitCheckBox is not null)
                    firstSplitCheckBox.CheckedChanged += handler;
                secondSplitCheckBox.CheckedChanged += handler;

                _tr4LevelTransitionSettingsPanel.Controls.Add(directionComboBox);
            }
            else
            {
                EventHandler handler = (_, _) =>
                {
                    ActiveSetting activeSetting;
                    if (firstSplitCheckBox is null)
                    {
                        activeSetting = secondSplitCheckBox.Checked
                            ? ActiveSetting.Active
                            : ActiveSetting.IgnoreAll;
                    }
                    else
                    {
                        bool firstEnabled = firstSplitCheckBox is not null && firstSplitCheckBox.Checked;
                        bool secondEnabled = secondSplitCheckBox.Checked;
                        activeSetting = (firstEnabled, secondEnabled) switch
                        {
                            (true, true)   => ActiveSetting.Active,
                            (false, true)  => ActiveSetting.IgnoreFirst,
                            (true, false)  => ActiveSetting.IgnoreSecond,
                            (false, false) => ActiveSetting.IgnoreAll,
                        };
                    }

                    transition.UpdateActive(activeSetting);
                };

                if (firstSplitCheckBox is not null)
                    firstSplitCheckBox.CheckedChanged += handler;
                secondSplitCheckBox.CheckedChanged += handler;
            }

            yOffset += rowHeight;
            firstEntry = false;
        }
    }

    private void PopulateTr6LevelControls(List<Tr6LevelTransitionSetting> referenceList)
    {
        const int rowHeight = 22;
        _tr6LevelTransitionSettingsPanel.Controls.Clear();

        var firstEntry = true;
        var rowCount = 0;
        var yOffset = 0;
        var font = new Font(_levelTransitionSelect.Font, FontStyle.Regular);
        foreach (Tr6LevelTransitionSetting transition in referenceList)
        {
            // Section
            if (!string.IsNullOrEmpty(transition.Section))
            {
                if (rowCount != 0)
                {
                    rowCount = 0;
                    yOffset += rowHeight;
                }

                if (!firstEntry)
                    yOffset += 3; // Magic number that makes it look even on the top and bottom.

                var label = new Label
                {
                    Text = transition.Section,
                    Font = new Font(font, FontStyle.Bold),
                    Location = new Point(0, yOffset),
                    Size = new Size(400, 20),
                    Padding = Padding with { Left = 0, Right = 0 },
                };
                _tr6LevelTransitionSettingsPanel.Controls.Add(label);

                yOffset += rowHeight;
            }

            // Reset row count and adjust y for next row.
            if (rowCount >= 2)
            {
                rowCount = 0;
                yOffset += rowHeight;
            }

            // CheckBox
            int widthNeeded = TextRenderer.MeasureText(transition.Name, font).Width + 20;
            var checkBox = new CheckBox
            {
                Text = transition.Name,
                Location = new Point(rowCount * 224, yOffset),
                Size = new Size(widthNeeded, 20),
                Padding = Padding with { Left = 0, Right = 0 },
                Checked = transition.Active,
                Enabled = true,
            };

            // ToolTip
            if (!string.IsNullOrEmpty(transition.ToolTip))
            {
                checkBox.Text += " ℹ️";
                checkBox.Width += 20;
                _toolTip.SetToolTip(checkBox, transition.ToolTip);
            }

            checkBox.CheckedChanged += (sender, _) => transition.UpdateActive(((CheckBox) sender).Checked);

            firstEntry = false;
            rowCount += 1;
            _tr6LevelTransitionSettingsPanel.Controls.Add(checkBox);
        }
    }

    #region Form Event Handlers

    private void FullGameButtonCheckedChanged(object sender, EventArgs e)
    {
        RunType = RunType.FullGame;
        SetGameTimeMethod(GameTimeMethod.RtaNoLoads);
        EnableControlsPerState();
    }

    private void IlOrAreaButtonCheckedChanged(object sender, EventArgs e)
    {
        RunType = RunType.IndividualLevelOrArea;
        SetGameTimeMethod(GameTimeMethod.RtaNoLoads);
        EnableControlsPerState();
    }

    private void DeathrunButtonCheckedChanged(object sender, EventArgs e)
    {
        RunType = RunType.Deathrun;
        SetGameTimeMethod(GameTimeMethod.Igt);
        EnableControlsPerState();
    }

    private void EnableAutoResetCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox) sender;
        EnableAutoReset = checkbox.Checked;
    }

    private void SplitNoPickupsButtonCheckedChanged(object sender, EventArgs e) => PickupSplitSetting = PickupSplitSetting.None;

    private void SplitAllPickupsButtonCheckedChanged(object sender, EventArgs e) => PickupSplitSetting = PickupSplitSetting.All;

    private void SplitSecretsOnlyButtonCheckedChanged(object sender, EventArgs e) => PickupSplitSetting = PickupSplitSetting.SecretsOnly;

    private void SplitSecurityBreachCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox) sender;
        SplitSecurityBreach = checkbox.Checked;
    }

    private void _tr4LevelSettingsButtonClicked(object sender, EventArgs e) => ShowCorrectTab(Game.Tr4);
    private void _tr5LevelSettingsButtonClicked(object sender, EventArgs e) => ShowCorrectTab(Game.Tr5);
    private void _tr6LevelSettingsButtonClicked(object sender, EventArgs e) => ShowCorrectTab(Game.Tr6);

    private void _tr4SelectAllButtonClicked(object sender, EventArgs e) => SetAllTr4CheckBoxes(true);
    private void _tr4UnselectAllButtonClicked(object sender, EventArgs e) => SetAllTr4CheckBoxes(false);

    private void _tr6SelectAllButtonClicked(object sender, EventArgs e) => SetAllTr6CheckBoxes(true);
    private void _tr6UnselectAllButtonClicked(object sender, EventArgs e) => SetAllTr6CheckBoxes(false);

    private static void GuideLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        if (e.Link.LinkData is string target)
            Process.Start(target);
    }

    #endregion

    private void EnableControlsPerState()
    {
        AdjustSplitPickupsState();
        AdjustTransitionsGroupBoxState();
        UpdateSignatureScanStatusLabel();
    }

    private void AdjustSplitPickupsState()
    {
        if (RunType != RunType.Deathrun)
        {
            _pickupSplitSelect.Enabled = true;
            _pickupSplitSelect.Text = PickupSplitSettingDefault;
            return;
        }

        // Disable for Deathrun Mode.
        _pickupSplitSelect.Enabled = false;
        _pickupSplitSelect.Text = $"{PickupSplitSettingDefault} [Disabled: Deathrun overrides Split logic]";
    }

    private void AdjustTransitionsGroupBoxState()
    {
        ResetScrolling();

        // Enable or disable.
        bool tr4Tr6Enable = RunType is RunType.FullGame;
        EnableTr4Tr6LevelSelectControls(tr4Tr6Enable);

        bool tr5Enable = RunType is not RunType.Deathrun;
        EnableTr5LevelSelectControls(tr5Enable);

        // Set the text to reflect the state.
        string text = LevelTransitionSettingsTextDefault; // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (RunType is RunType.Deathrun)
            text += " [Disabled: Deathrun overrides Split logic]";
        else if (RunType is RunType.IndividualLevelOrArea) // TR4 IL
            text += " [TR4 + TR6 Disabled: All transitions split in IL / Area%]";

        _levelTransitionActiveTabLabel.Enabled = RunType is not RunType.Deathrun;
        _levelTransitionSelect.Text = text;
    }

    private void EnableTr4Tr6LevelSelectControls(bool enable) =>
        _tr4LevelTransitionSettingsPanel.Enabled = _tr6LevelTransitionSettingsPanel.Enabled =
        _tr4LevelSettingsButton.Enabled = _tr6LevelSettingsButton.Enabled =
        _tr4SelectAllButton.Enabled = _tr4UnselectAllButton.Enabled =
        _tr6SelectAllButton.Enabled = _tr6UnselectAllButton.Enabled = enable;

    private void EnableTr5LevelSelectControls(bool enable) =>
        _tr5LevelSettingsButton.Enabled =
        SplitSecurityBreachCheckbox.Enabled = enable;

    private void ShowCorrectTab(Game game)
    {
        _levelTransitionActiveTabLabel.Text = $"Currently showing: {game.Description()}";
        ResetScrolling();

        _tr4SelectAllButton.Enabled = _tr4SelectAllButton.Visible = _tr4UnselectAllButton.Enabled = _tr4UnselectAllButton.Visible = game is Game.Tr4;
        _tr4LevelTransitionSettingsPanel.Enabled = _tr4LevelTransitionSettingsPanel.Visible = game is Game.Tr4;

        _tr5LevelTransitionSettingsPanel.Enabled = _tr5LevelTransitionSettingsPanel.Visible = game is Game.Tr5;

        _tr6SelectAllButton.Enabled = _tr6SelectAllButton.Visible = _tr6UnselectAllButton.Enabled = _tr6UnselectAllButton.Visible = game is Game.Tr6;
        _tr6LevelTransitionSettingsPanel.Enabled = _tr6LevelTransitionSettingsPanel.Visible = game is Game.Tr6;
    }

    private void ResetScrolling()
        => _tr4LevelTransitionSettingsPanel.AutoScrollPosition = _tr5LevelTransitionSettingsPanel.AutoScrollPosition = _tr6LevelTransitionSettingsPanel.AutoScrollPosition = new Point(0, 0);

    // ReSharper disable ArgumentsStyleLiteral
    internal readonly List<Tr4LevelTransitionSetting> Tr4LevelTransitions =
    [
        // Cambodia
        new(Tr4Level.AngkorWat, Tr4Level.RaceForTheIris, TransitionDirection.OneWayFromLower,      // 01  -> 02
            section: "Cambodia"),
        new(Tr4Level.RaceForTheIris, Tr4Level.TheTombOfSeth, TransitionDirection.OneWayFromLower), // 02  -> 03

        // Valley of the Kings
        new(Tr4Level.TheTombOfSeth, Tr4Level.BurialChambers, TransitionDirection.OneWayFromLower,     // 03  -> 04
            section: "Valley of the Kings"),
        new(Tr4Level.BurialChambers, Tr4Level.ValleyOfTheKings, TransitionDirection.OneWayFromLower), // 04  -> 05
        new(Tr4Level.ValleyOfTheKings, Tr4Level.Kv5, TransitionDirection.OneWayFromLower),            // 05  -> 06
        new(Tr4Level.Kv5, Tr4Level.TempleOfKarnak, TransitionDirection.OneWayFromLower),              // 06  -> 07

        // Karnak
        new(Tr4Level.TempleOfKarnak, Tr4Level.GreatHypostyleHall, TransitionDirection.OneWayFromLower, complexIgnore: true, // 07  -> 08
            toolTip: $"You can disable the first, the second, or both splits. Runs that only visit the levels once can safely leave both boxes checked and have only one split.{Environment.NewLine}" +
                     "The left checkbox is for the first visit split; the right checkbox is for the second visit split.",
            section: "Karnak Temple Complex"),
        new(Tr4Level.GreatHypostyleHall, Tr4Level.SacredLake, TransitionDirection.OneWayFromLower, complexIgnore: true,     // 08  -> 09
            toolTip: $"You can disable the first, the second, or both splits. Runs that only visit the levels once can safely leave both boxes checked and have only one split.{Environment.NewLine}" +
                     "The left checkbox is for the first visit split; the right checkbox is for the second visit split."),
        new(Tr4Level.TempleOfKarnak, Tr4Level.SacredLake, TransitionDirection.OneWayFromHigher),                            // 09  -> 07
        new(Tr4Level.SacredLake, Tr4Level.TombOfSemerkhet, TransitionDirection.OneWayFromLower, unusedLevelNumber: 10),     // 09  -> 11

        // Semerkhet
        new(Tr4Level.TombOfSemerkhet, Tr4Level.GuardianOfSemerkhet, TransitionDirection.OneWayFromLower, // 11  -> 12
            section: "Semerkhet"),
        new(Tr4Level.GuardianOfSemerkhet, Tr4Level.DesertRailroad, TransitionDirection.OneWayFromLower), // 12  -> 13

        // Eastern Desert
        new(Tr4Level.DesertRailroad, Tr4Level.Alexandria, TransitionDirection.OneWayFromLower, // 13  -> 14
            section: "Eastern Desert"),

        // Alexandria
        new(Tr4Level.Alexandria, Tr4Level.CoastalRuins, TransitionDirection.TwoWay, // 14 <-> 15
            section: "Alexandria"),

        new(Tr4Level.CoastalRuins, Tr4Level.Catacombs, TransitionDirection.TwoWay,
            lowerRoomNumber: 153, lowerTriggerTimer: 1, higherRoomNumber: 2, higherTriggerTimer: 1, note: "Setup",  // 15 <-> 18
            toolTip: "To/from the isolated Catacombs room with the pillar that must be pulled to progress Catacombs."),
        new(Tr4Level.CoastalRuins, Tr4Level.Catacombs, TransitionDirection.TwoWay,
            lowerRoomNumber: 154, lowerTriggerTimer: 0, higherRoomNumber: 6, higherTriggerTimer: 2, note: "Start",  // 15 <-> 18
            complexIgnore: true,
            toolTip: $"To/from the starting Catacombs area with the door locked by the mechanism in the Setup room.{Environment.NewLine}" +
                     $"You can disable the first, the second, or both splits. Runs that only visit the levels once can safely leave both boxes checked and have only one split.{Environment.NewLine}" +
                     $"The left checkbox is for the first visit split; the right checkbox is for the second visit split.{Environment.NewLine}" +
                     "If Two-Way is selected, the checkboxes apply to both directions. If one way is selected, only that direction will be split accordingly."),
        new(Tr4Level.CoastalRuins, Tr4Level.Catacombs, TransitionDirection.TwoWay,
            lowerRoomNumber: 158, lowerTriggerTimer: 2, higherRoomNumber: 9, higherTriggerTimer: 4, note: "End",    // 15 <-> 18
            toolTip: "To/from the Catacombs exit which allows an alternate backtracking route to Coastal Ruins."),

        new(Tr4Level.CoastalRuins, Tr4Level.TempleOfPoseidon, TransitionDirection.TwoWay),               // 15 <-> 19
        new(Tr4Level.Catacombs, Tr4Level.TempleOfPoseidon, TransitionDirection.TwoWay),                  // 18 <-> 19
        new(Tr4Level.TempleOfPoseidon, Tr4Level.TheLostLibrary, TransitionDirection.TwoWay),             // 19 <-> 20
        new(Tr4Level.TheLostLibrary, Tr4Level.HallOfDemetrius, TransitionDirection.TwoWay),              // 20 <-> 21
        new(Tr4Level.CoastalRuins, Tr4Level.HallOfDemetrius, TransitionDirection.OneWayFromHigher),      // 21  -> 15

        new(Tr4Level.CoastalRuins, Tr4Level.PharosTempleOfIsis, TransitionDirection.OneWayFromLower,     // 15  -> 16
            lowerRoomNumber: 104, lowerTriggerTimer: 0, note: "Underwater Current"),
        new(Tr4Level.CoastalRuins, Tr4Level.PharosTempleOfIsis, TransitionDirection.OneWayFromHigher,    // 16  -> 15
            higherRoomNumber: 77, higherTriggerTimer: 3, note: "Chute After Surfacing"),
        new(Tr4Level.PharosTempleOfIsis, Tr4Level.CleopatrasPalaces, TransitionDirection.TwoWay,         // 16 <-> 17
            complexIgnore: true,
            toolTip: $"You can disable the first, the second, or both splits. Runs that only visit the levels once can safely leave both boxes checked and have only one split.{Environment.NewLine}" +
                     $"The left checkbox is for the first visit split; the right checkbox is for the second visit split.{Environment.NewLine}" +
                     "If Two-Way is selected, the checkboxes apply to both directions. If one way is selected, only that direction will be split accordingly."),
        new(Tr4Level.CleopatrasPalaces, Tr4Level.CityOfTheDead, TransitionDirection.OneWayFromLower),    // 17  -> 22

        // Cairo
        new(Tr4Level.CityOfTheDead, Tr4Level.ChambersOfTulun, TransitionDirection.TwoWay,  // 22 <-> 24
            section: "Cairo"),
        new(Tr4Level.Trenches, Tr4Level.ChambersOfTulun, TransitionDirection.TwoWay),      // 23 <-> 24
        new(Tr4Level.Trenches, Tr4Level.CitadelGate, TransitionDirection.TwoWay),          // 23 <-> 24
        new(Tr4Level.ChambersOfTulun, Tr4Level.CitadelGate, TransitionDirection.TwoWay),   // 24 <-> 26

        new(Tr4Level.Trenches, Tr4Level.StreetBazaar, TransitionDirection.TwoWay,
            lowerRoomNumber: 23, lowerTriggerTimer: 1, higherRoomNumber: 26, higherTriggerTimer: 2, note: "Minefield"), // 23 <-> 25
        new(Tr4Level.Trenches, Tr4Level.StreetBazaar, TransitionDirection.TwoWay,
            lowerRoomNumber: 44, lowerTriggerTimer: 0, higherRoomNumber: 08, higherTriggerTimer: 3, note: "Garage",    // 23 <-> 25
            complexIgnore: true,
            toolTip: $"You can disable the first, the second, or both splits. Runs that only visit the levels once can safely leave both boxes checked and have only one split.{Environment.NewLine}" +
                     $"The left checkbox is for the first visit split; the right checkbox is for the second visit split.{Environment.NewLine}" +
                     "If Two-Way is selected, the checkboxes apply to both directions. If one way is selected, only that direction will be split accordingly."),
        new(Tr4Level.Trenches, Tr4Level.StreetBazaar, TransitionDirection.OneWayFromHigher,
            higherRoomNumber: 71, higherTriggerTimer: 4, note: "Chute After Guardian"),                                 // 25  -> 23

        new(Tr4Level.CitadelGate, Tr4Level.Citadel, TransitionDirection.OneWayFromLower, note: "Dragon Cutscene"), // 26  -> 27
        new(Tr4Level.Citadel, Tr4Level.SphinxComplex, TransitionDirection.OneWayFromLower),                        // 27  -> 28

        // Giza
        new(Tr4Level.SphinxComplex, Tr4Level.UnderneathTheSphinx, TransitionDirection.TwoWay,               // 28 <-> 30
            section: "Giza"),
        new(Tr4Level.UnderneathTheSphinx, Tr4Level.MenkauresPyramid, TransitionDirection.TwoWay),           // 30 <-> 31
        new(Tr4Level.MenkauresPyramid, Tr4Level.InsideMenkauresPyramid, TransitionDirection.TwoWay),        // 31 <-> 32
        new(Tr4Level.SphinxComplex, Tr4Level.InsideMenkauresPyramid, TransitionDirection.OneWayFromHigher), // 32  -> 28

        new(Tr4Level.SphinxComplex, Tr4Level.TheMastabas, TransitionDirection.TwoWay),                    // 28 <-> 33
        new(Tr4Level.TheMastabas, Tr4Level.TheGreatPyramid, TransitionDirection.OneWayFromLower),         // 33  -> 34
        new(Tr4Level.TheGreatPyramid, Tr4Level.KhufusQueensPyramid, TransitionDirection.OneWayFromLower), // 34  -> 35
        new(Tr4Level.KhufusQueensPyramid, Tr4Level.InsideTheGreatPyramid, TransitionDirection.TwoWay),    // 35 <-> 36
        new(Tr4Level.InsideTheGreatPyramid, Tr4Level.TempleOfHorus, TransitionDirection.OneWayFromLower), // 36  -> 37
        new(Tr4Level.TempleOfHorus, Tr4Level.HorusBoss, TransitionDirection.OneWayFromLower),             // 37  -> 38
        new(Tr4Level.HorusBoss, Tr4Level.MainMenu, TransitionDirection.OneWayFromLower,                   // 38  -> End
            unusedLevelNumber: 39, note: "End of Game"),
    ];

    internal readonly List<Tr6LevelTransitionSetting> Tr6LevelTransitions =
    [
        new("Parisian Back Streets", "PARIS1.GMX", "PARIS1A.GMX", section: "Paris"),
        new("Derelict Apartment Block", "PARIS1A.GMX", "PARIS1C.GMX"),
        new("Industrial Roof Tops", "PARIS1C.GMX", "PARIS1B.GMX"),
        new("Margot Carvier's Apartment", "PARIS1B.GMX", "PARIS2_1.GMX"),
        new("The Serpent Rouge (Start)", "PARIS2_1.GMX", "PARIS2B.GMX",
            "Splits when you enter The Serpent Rouge from Parisian Ghetto."),
        new("The Serpent Rouge (End)", "PARIS2B.GMX", "PARIS2_1.GMX",
            "Splits when you exit The Serpent Rouge to Parisian Ghetto."),
        new("Parisian Ghetto: Pierre's Questline", "PARIS2_2.GMX", "PARIS2G.GMX",
            "Splits when you enter St. Aicard's Garveyard by entering through the apartment area."),
        new("Parisian Ghetto: Bernard's Questline", "PARIS2_3.GMX", "PARIS2G.GMX",
            "Splits when you enter St. Aicard's Garveyard by talking to the bodyguard in the church area."),
        new("St. Aicard's Graveyard", "PARIS2G.GMX", "PARIS2H.GMX"),
        new("Bouchard's Hideout", "PARIS2H.GMX", "PARIS2E.GMX"),
        new("Rennes' Pawnshop", "CUTSCENE\\CS_2_51A.GMX", "PARIS3.GMX"),
        new("Louvre Storm Drains", "PARIS3.GMX", "PARIS4.GMX"),
        new("Louvre Galleries", "PARIS4.GMX", "PARIS5A.GMX"),
        new("The Archaeological Dig, Part 1", "PARIS5A.GMX", "PARIS5.GMX",
            "Splits when you enter Tomb of Ancients."),
        new("Tomb of Ancients", "PARIS5.GMX", "PARIS5_1.GMX",
            "Splits when you enter The Hall of Seasons."),
        new("Neptune's Hall", "PARIS5_2.GMX", "PARIS5_1.GMX"),
        new("Wrath of the Beast", "PARIS5_3.GMX", "PARIS5_1.GMX"),
        new("The Sanctuary of Flame", "PARIS5_4.GMX", "PARIS5_1.GMX"),
        new("The Breath of Hades", "PARIS5_5.GMX", "PARIS5_1.GMX"),
        new("The Hall of Seasons", "PARIS5_1.GMX", "PARIS5.GMX"),
        new("Tomb of Ancients (flooded)", "PARIS5.GMX", "PARIS5A.GMX",
            "Splits when you return to The Archaeological Dig after completing The Hall of Seasons."),
        new("The Archaeological Dig, Part 2", "PARIS5A.GMX", "PARIS4A.GMX",
            "Splits when you enter Galleries Under Siege."),
        new("Galleries Under Siege", "CUTSCENE\\CS_6_21B.GMX", "PARIS6.GMX"),
        new("Von Croy's Apartment", "CUTSCENE\\CS_7_19.GMX", "PRAGUE1.GMX"),
        new("The Monstrum Crimescene", "CUTSCENE\\CS_9_1.GMX", "PRAGUE2.GMX", section: "Prague"),
        new("The Strahov Fortress", "PRAGUE2.GMX", "PRAGUE3.GMX"),
        new("The Bio-Research Facility", "CUTSCENE\\CS_10_14.GMX", "PRAGUE4.GMX"),
        new("The Sanitarium", "PRAGUE4.GMX", "PRAGUE4A.GMX"),
        new("Maximum Containment Area", "CUTSCENE\\CS_12_1.GMX", "PRAGUE3A.GMX"),
        new("Aquatic Research Area", "PRAGUE3A.GMX", "PRAGUE5.GMX"),
        new("Vault of Trophies", "CUTSCENE\\CS_13_9.GMX", "PRAGUE5A.GMX"),
        new("Boaz Returns", "CUTSCENE\\CS_14_6.GMX", "PRAGUE6A.GMX"),
        new("The Lost Domain", "PRAGUE6A.GMX", "PRAGUE6.GMX"),
    ];
    // ReSharper restore ArgumentsStyleLiteral
}