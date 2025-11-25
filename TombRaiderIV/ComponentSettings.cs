using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LaterClassicUtil;
using Util;

namespace TR4;

public sealed class ComponentSettings : LaterClassicComponentSettings
{
    private const string LegacyGlitchlessSettingTextDefault = "Legacy Glitchless Splits (TR4 FG Only)";
    private const string LevelTransitionSettingsTextDefault = "Level Transition Settings";
    private const string SplitsSecretsSettingTextDefault = "Split When Secret is Triggered";

    public ComponentSettings(Version version)
    {
        InitializeComponent();
        AutosplitterVersionLabel.Text = $"Autosplitter Version: {version}";
        EnableControlsPerState();
    }

    private uint ActiveVersion { get; set; } = VersionDetector.None;

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

        if (ActiveVersion is not VersionDetector.None)
            ShowCorrectTab((Tr4Version) ActiveVersion);

        EnableControlsPerState();

        ResumeLayout(false);
        PerformLayout();
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        ResetScrolling();
        base.OnHandleDestroyed(e);
    }

    [SuppressMessage("ReSharper", "FunctionComplexityOverflow")]
    private void InitializeComponent()
    {
        _modeSelect = new GroupBox();
        FullGameModeButton = new RadioButton();
        ILModeButton = new RadioButton();
        DeathrunModeButton = new RadioButton();
        LegacyGlitchlessCheckbox = new CheckBox();
        EnableAutoResetCheckbox = new CheckBox();
        SplitSecretsCheckbox = new CheckBox();

        _levelTransitionSelect = new GroupBox();
        _toolTip = new ToolTip();
        _levelTransitionActiveTabLabel = new Label();
        _tr4LevelSettingsButton = new Button();
        _tteLevelSettingsButton = new Button();
        _tr4LevelTransitionSettingsPanel = new Panel();
        _tr4SelectAllButton = new Button();
        _tr4UnselectAllButton = new Button();
        _tteLevelTransitionSettingsPanel = new Panel();

        GameVersionLabel = new Label();
        AutosplitterVersionLabel = new Label();
        AslWarningLabel = new Label();
        _modeSelect.SuspendLayout();
        _levelTransitionSelect.SuspendLayout();
        SuspendLayout();

        #region _modeSelect and Radio Buttons

        // _modeSelect
        _modeSelect.Controls.Add(FullGameModeButton);
        _modeSelect.Controls.Add(ILModeButton);
        _modeSelect.Controls.Add(DeathrunModeButton);
        _modeSelect.Location = new Point(10, 10);
        _modeSelect.Name = "_modeSelect";
        _modeSelect.Size = new Size(300, 55);
        _modeSelect.TabIndex = 0;
        _modeSelect.TabStop = false;
        _modeSelect.Text = "Mode Selection";

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

        #endregion

        #region Checkboxes

        // EnableAutoResetCheckbox
        EnableAutoResetCheckbox.AutoSize = true;
        EnableAutoResetCheckbox.Checked = false;
        EnableAutoResetCheckbox.Location = new Point(12, 80);
        EnableAutoResetCheckbox.Name = "EnableAutoResetCheckbox";
        EnableAutoResetCheckbox.Text = "Enable Auto-Reset";
        EnableAutoResetCheckbox.TabIndex = 0;
        EnableAutoResetCheckbox.UseVisualStyleBackColor = true;
        EnableAutoResetCheckbox.CheckedChanged += EnableAutoResetCheckboxCheckedChanged;

        // SplitSecretsCheckbox
        SplitSecretsCheckbox.AutoSize = true;
        SplitSecretsCheckbox.Checked = false;
        SplitSecretsCheckbox.Location = new Point(12, 100);
        SplitSecretsCheckbox.Name = "SplitSecretsCheckbox";
        SplitSecretsCheckbox.Text = SplitsSecretsSettingTextDefault;
        SplitSecretsCheckbox.TabIndex = 0;
        SplitSecretsCheckbox.UseVisualStyleBackColor = true;
        SplitSecretsCheckbox.CheckedChanged += SplitSecretsCheckboxCheckedChanged;

        // LegacyGlitchlessCheckbox
        LegacyGlitchlessCheckbox.AutoSize = true;
        LegacyGlitchlessCheckbox.Checked = false;
        LegacyGlitchlessCheckbox.Location = new Point(12, 120);
        LegacyGlitchlessCheckbox.Name = "LegacyGlitchlessCheckbox";
        LegacyGlitchlessCheckbox.Text = LegacyGlitchlessSettingTextDefault;
        LegacyGlitchlessCheckbox.TabIndex = 0;
        LegacyGlitchlessCheckbox.UseVisualStyleBackColor = true;
        LegacyGlitchlessCheckbox.CheckedChanged += LegacyGlitchlessCheckboxCheckedChanged;

        #endregion

        // _toolTip
        _toolTip.AutoPopDelay = 60000;
        _toolTip.InitialDelay = 500;
        _toolTip.ReshowDelay = 250;

        #region _levelTransitionSelect

        // _levelTransitionSelect
        _levelTransitionSelect.Controls.Add(_tr4LevelSettingsButton);
        _levelTransitionSelect.Controls.Add(_tteLevelSettingsButton);
        _levelTransitionSelect.Controls.Add(_levelTransitionActiveTabLabel);
        _levelTransitionSelect.Controls.Add(_tr4LevelTransitionSettingsPanel);
        _levelTransitionSelect.Controls.Add(_tr4SelectAllButton);
        _levelTransitionSelect.Controls.Add(_tr4UnselectAllButton);
        _levelTransitionSelect.Controls.Add(_tteLevelTransitionSettingsPanel);
        _levelTransitionSelect.Location = new Point(0, 150);
        _levelTransitionSelect.Name = "_levelTransitionSelect";
        _levelTransitionSelect.Size = new Size(476, 325);
        _levelTransitionSelect.Text = LevelTransitionSettingsTextDefault;

        // _levelTransitionActiveTabLabel
        _levelTransitionActiveTabLabel.AutoSize = false;
        _levelTransitionActiveTabLabel.Location = new Point(5, 12);
        _levelTransitionActiveTabLabel.Name = "_levelTransitionActiveTabLabel";
        _levelTransitionActiveTabLabel.Size = new Size(_levelTransitionSelect.Width - 10, 20);
        _levelTransitionActiveTabLabel.Text = "Currently showing: The Last Revelation";
        _levelTransitionActiveTabLabel.TextAlign = ContentAlignment.MiddleCenter;

        // _tr4LevelSettingsButton
        _tr4LevelSettingsButton.Name = "_tr4LevelSettingsButton";
        _tr4LevelSettingsButton.Location = new Point(350, 13);
        _tr4LevelSettingsButton.Size = new Size(45, 21);
        _tr4LevelSettingsButton.Margin = Padding.Empty;
        _tr4LevelSettingsButton.Padding = Padding.Empty;
        _tr4LevelSettingsButton.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
        _tr4LevelSettingsButton.Text = "TR4";
        _tr4LevelSettingsButton.Click += _tr4LevelSettingsButtonClicked;
        _tr4LevelSettingsButton.Enabled = true;
        _tr4LevelSettingsButton.Visible = true;

        // _tteLevelSettingsButton
        _tteLevelSettingsButton.Name = "_tteLevelSettingsButton";
        _tteLevelSettingsButton.Location = new Point(400, 13);
        _tteLevelSettingsButton.Size = new Size(45, 21);
        _tteLevelSettingsButton.Margin = Padding.Empty;
        _tteLevelSettingsButton.Padding = Padding.Empty;
        _tteLevelSettingsButton.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
        _tteLevelSettingsButton.Text = "TTE";
        _tteLevelSettingsButton.Click += _tteLevelSettingsButtonClicked;
        _tteLevelSettingsButton.Enabled = true;
        _tteLevelSettingsButton.Visible = true;

        // _tr4LevelTransitionSettingsPanel
        _tr4LevelTransitionSettingsPanel.Name = "_tr4LevelTransitionSettingsPanel";
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
        _tr4SelectAllButton.Name = "_tr4SelectAllButton";
        _tr4SelectAllButton.Location = new Point(5, 294);
        _tr4SelectAllButton.Size = new Size(120, 25);
        _tr4SelectAllButton.Text = "Select All (TR4)";
        _tr4SelectAllButton.Click += _tr4SelectAllButtonClicked;
        _tr4SelectAllButton.Enabled = true;
        _tr4SelectAllButton.Visible = true;

        // _tr4UnselectAllButton
        _tr4UnselectAllButton.Name = "_tr4UnselectAllButton";
        _tr4UnselectAllButton.Location = new Point(331, 294);
        _tr4UnselectAllButton.Size = new Size(120, 25);
        _tr4UnselectAllButton.Text = "Unselect All (TR4)";
        _tr4UnselectAllButton.Click += _tr4UnselectAllButtonClicked;
        _tr4UnselectAllButton.Enabled = true;
        _tr4UnselectAllButton.Visible = true;

        // _tteLevelTransitionSettingsPanel
        _tteLevelTransitionSettingsPanel.Name = "_tteLevelTransitionSettingsPanel";
        _tteLevelTransitionSettingsPanel.Location = new Point(5, 35);
        _tteLevelTransitionSettingsPanel.Size = new Size(466, 100);
        _tteLevelTransitionSettingsPanel.AutoScroll = true;
        _tteLevelTransitionSettingsPanel.VerticalScroll.Enabled = false;
        _tteLevelTransitionSettingsPanel.VerticalScroll.Visible = false;
        _tteLevelTransitionSettingsPanel.HorizontalScroll.Enabled = false;
        _tteLevelTransitionSettingsPanel.HorizontalScroll.Visible = false;
        _tteLevelTransitionSettingsPanel.Enabled = false;
        _tteLevelTransitionSettingsPanel.Visible = false;
        _tteLevelTransitionSettingsPanel.TabStop = false;

        #endregion

        #region GameVersion and AutosplitterVersion Labels

        // GameVersionLabel
        GameVersionLabel.AutoSize = true;
        GameVersionLabel.Location = new Point(10, 485);
        GameVersionLabel.Name = "GameVersionLabel";
        GameVersionLabel.Size = new Size(200, 15);
        GameVersionLabel.TabIndex = 1;
        GameVersionLabel.Text = "Game Version: Unknown/Undetected";

        // AutosplitterVersionLabel
        AutosplitterVersionLabel.AutoSize = true;
        AutosplitterVersionLabel.Location = new Point(10, 505);
        AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
        AutosplitterVersionLabel.Size = new Size(200, 15);
        AutosplitterVersionLabel.TabIndex = 2;
        AutosplitterVersionLabel.Text = "Autosplitter Version: Uninitialized";

        #endregion

        #region Warning Labels

        // _aslWarningLabel
        AslWarningLabel.AutoSize = false;
        AslWarningLabel.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        AslWarningLabel.ForeColor = Color.Crimson;
        AslWarningLabel.Location = new Point(0, 530);
        AslWarningLabel.Name = "AslWarningLabel";
        AslWarningLabel.Size = new Size(476, 20);
        AslWarningLabel.TabStop = false;
        AslWarningLabel.Text = "Scriptable Auto Splitter in Layout — Please Remove!";
        AslWarningLabel.TextAlign = ContentAlignment.MiddleCenter;
        AslWarningLabel.Visible = false;

        #endregion

        // ComponentSettings
        Controls.Add(_modeSelect);
        Controls.Add(EnableAutoResetCheckbox);
        Controls.Add(SplitSecretsCheckbox);
        Controls.Add(LegacyGlitchlessCheckbox);
        Controls.Add(_levelTransitionSelect);
        Controls.Add(GameVersionLabel);
        Controls.Add(AutosplitterVersionLabel);
        Controls.Add(AslWarningLabel);
        Name = "ComponentSettings";
        Size = new Size(476, 550);

        _modeSelect.ResumeLayout(false);
        _levelTransitionSelect.ResumeLayout(false);

        ResumeLayout(false);
        PerformLayout();
    }

    private void PopulateLevelControls()
    {
        _toolTip.RemoveAll();
        PopulateTr4LevelControls();
        PopulateTteLevelControls();
        ResetScrolling();
    }

    private void PopulateTr4LevelControls() => PopulateControl(Tr4LevelTransitions, _tr4LevelTransitionSettingsPanel);

    private void PopulateTteLevelControls() => PopulateControl(TteLevelTransitions, _tteLevelTransitionSettingsPanel);

    private void PopulateControl<TLevel>(List<TransitionSetting<TLevel>> referenceList, Panel panel)
        where TLevel : Enum
    {
        const int rowHeight = 22;
        panel.Controls.Clear();

        var firstEntry = true;
        var yOffset = 0;
        var font = new Font(_levelTransitionSelect.Font, FontStyle.Regular);
        foreach (var transition in referenceList)
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
                panel.Controls.Add(label);

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
                    Checked = transition.UnusedLevelNumber is 39 ||
                              transition.Active is ActiveSetting.Active or ActiveSetting.IgnoreSecond,
                    Enabled = transition.CanBeConfigured,
                };
                panel.Controls.Add(firstSplitCheckBox);

                secondSplitCheckBox = new CheckBox
                {
                    Text = transition.DisplayName(),
                    Location = new Point(firstSplitCheckBox.Width + checkBoxPadding, yOffset),
                    Size = new Size(widthNeeded, 20),
                    Padding = Padding with { Left = 0, Right = 0 },
                    Checked = transition.UnusedLevelNumber is 39 ||
                              transition.Active is ActiveSetting.Active or ActiveSetting.IgnoreFirst,
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
                    Checked = transition.UnusedLevelNumber is 39 ||
                              transition.Active is ActiveSetting.Active,
                    Enabled = transition.CanBeConfigured,
                };
            }

            panel.Controls.Add(secondSplitCheckBox);

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

                directionComboBox.SelectedIndex = (int) transition.SelectedDirectionality;
                directionComboBox.SelectedIndexChanged += (_, _) =>
                {
                    transition.SelectedDirectionality = (TransitionDirection) directionComboBox.SelectedIndex;
                };

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

                panel.Controls.Add(directionComboBox);
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

    private void SetAllTr4CheckBoxes(bool check)
    {
        foreach (CheckBox checkBox in _tr4LevelTransitionSettingsPanel.Controls.OfType<CheckBox>().Where(static cb => cb.Enabled))
            checkBox.Checked = check; // This triggers checkBox.CheckedChanged.
    }

    private void EnableControlsPerState()
    {
        AdjustLegacyGlitchlessState();
        AdjustSplitSecretsState();
        AdjustTransitionsGroupBoxState();
    }

    private void AdjustLegacyGlitchlessState()
    {
        if (FullGame)
        {
            LegacyGlitchlessCheckbox.Enabled = true;
            LegacyGlitchlessCheckbox.Text = LegacyGlitchlessSettingTextDefault;
            return;
        }

        // Disable for every Mode besides Full Game.
        LegacyGlitchlessCheckbox.Checked = LegacyGlitchlessCheckbox.Enabled = false;

        if (Deathrun)
            LegacyGlitchlessCheckbox.Text = $"{LegacyGlitchlessSettingTextDefault} [Disabled: Deathrun overrides split logic]";
        else if (!FullGame) // IL
            LegacyGlitchlessCheckbox.Text = $"{LegacyGlitchlessSettingTextDefault} [Disabled: All transitions split in IL / Area%]";
    }

    private void AdjustSplitSecretsState()
    {
        if (!Deathrun)
        {
            SplitSecretsCheckbox.Enabled = true;
            SplitSecretsCheckbox.Text = SplitsSecretsSettingTextDefault;
            return;
        }

        // Disable for Deathrun Mode.
        SplitSecretsCheckbox.Checked = SplitSecretsCheckbox.Enabled = false;
        SplitSecretsCheckbox.Text = $"{SplitsSecretsSettingTextDefault} [Disabled: Deathrun overrides split logic]";
    }

    private void AdjustTransitionsGroupBoxState()
    {
        ResetScrolling();

        // Enable or disable.
        bool enableLevelSelection = FullGame && !LegacyGlitchless;
        _levelTransitionSelect.Enabled = enableLevelSelection;

        // Set the text to reflect the state.
        string text = LevelTransitionSettingsTextDefault;
        if (Deathrun)
            text += " [Disabled: Deathrun overrides split logic]";
        else if (LegacyGlitchless)
            text += " [Disabled: Legacy Glitchless overrides with preset transitions]";
        else if (!FullGame) // TR4 IL
            text += " [Disabled: All transitions split in IL / Area%]";

        _levelTransitionSelect.Text = text;
    }

    private void ShowCorrectTab(Tr4Version version)
    {
        bool isTr4 = version == Tr4Version.SteamOrGog;

        _levelTransitionActiveTabLabel.Text = $"Currently showing: {(isTr4 ? "The Last Revelation" : "The Times Exclusive")}";
        ResetScrolling();

        _tr4SelectAllButton.Enabled = _tr4SelectAllButton.Visible = _tr4UnselectAllButton.Enabled = _tr4UnselectAllButton.Visible = isTr4;
        _tr4LevelTransitionSettingsPanel.Enabled = _tr4LevelTransitionSettingsPanel.Visible = isTr4;

        _tteLevelTransitionSettingsPanel.Enabled = _tteLevelTransitionSettingsPanel.Visible = !isTr4;
    }

    private void ResetScrolling()
        => _tr4LevelTransitionSettingsPanel.AutoScrollPosition = _tteLevelTransitionSettingsPanel.AutoScrollPosition = new Point(0, 0);

    #region UI Declarations

    private GroupBox _modeSelect;
    public RadioButton ILModeButton;
    public RadioButton FullGameModeButton;
    public RadioButton DeathrunModeButton;
    public CheckBox LegacyGlitchlessCheckbox;
    public CheckBox EnableAutoResetCheckbox;
    public CheckBox SplitSecretsCheckbox;
    public bool LegacyGlitchless;

    private GroupBox _levelTransitionSelect;
    private ToolTip _toolTip;
    private Label _levelTransitionActiveTabLabel;
    private Button _tr4LevelSettingsButton;
    private Button _tteLevelSettingsButton;
    private Panel _tr4LevelTransitionSettingsPanel;
    private Button _tr4SelectAllButton;
    private Button _tr4UnselectAllButton;
    private Panel _tteLevelTransitionSettingsPanel;

    #endregion

    #region Form Event Handlers

    protected override void FullGameModeButtonCheckedChanged(object sender, EventArgs e)
    {
        base.FullGameModeButtonCheckedChanged(sender, e);
        EnableControlsPerState();
    }

    protected override void ILModeButtonCheckedChanged(object sender, EventArgs e)
    {
        base.ILModeButtonCheckedChanged(sender, e);
        EnableControlsPerState();
    }

    protected override void DeathrunModeButtonCheckedChanged(object sender, EventArgs e)
    {
        base.DeathrunModeButtonCheckedChanged(sender, e);
        EnableControlsPerState();
    }

    private void LegacyGlitchlessCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox) sender;
        LegacyGlitchless = checkbox.Checked;

        EnableControlsPerState();
    }

    public override void SetGameVersion(VersionDetectionResult result)
    {
        const string digitalText = "Steam/GOG [TR4]";
        const string tteText = "The Times Exclusive [TTE]";

        switch (result)
        {
            case VersionDetectionResult.None:
                ActiveVersion = VersionDetector.None;
                base.SetGameVersion(result);
                break;

            case VersionDetectionResult.Unknown:
                ActiveVersion = VersionDetector.Unknown;
                base.SetGameVersion(result);
                return;

            case VersionDetectionResult.Found found:
                ActiveVersion = found.Version;
                GameVersionLabel.Text = "Game Version: " +
                                        (Tr4Version) found.Version switch
                                        {
                                            Tr4Version.SteamOrGog        => digitalText,
                                            Tr4Version.TheTimesExclusive => tteText,
                                            _                            => throw new ArgumentOutOfRangeException(nameof(found.Version)),
                                        };
                return;

            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }
    }

    private void _tr4LevelSettingsButtonClicked(object sender, EventArgs e) => ShowCorrectTab(Tr4Version.SteamOrGog);
    private void _tteLevelSettingsButtonClicked(object sender, EventArgs e) => ShowCorrectTab(Tr4Version.TheTimesExclusive);

    private void _tr4SelectAllButtonClicked(object sender, EventArgs e) => SetAllTr4CheckBoxes(true);
    private void _tr4UnselectAllButtonClicked(object sender, EventArgs e) => SetAllTr4CheckBoxes(false);

    #endregion

    // ReSharper disable ArgumentsStyleLiteral
    internal readonly List<TransitionSetting<Tr4Level>> Tr4LevelTransitions =
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

    internal readonly List<TransitionSetting<TteLevel>> TteLevelTransitions =
    [
        new(TteLevel.Office, TteLevel.TheTimesExclusive, TransitionDirection.OneWayFromLower,
            note: "Cutscene"),    // 01  -> 02
        new(TteLevel.TheTimesExclusive, TteLevel.MainMenu, TransitionDirection.OneWayFromLower,
            unusedLevelNumber: 39,
            note: "End of Game"), // 02  -> End
    ];
    // ReSharper restore ArgumentsStyleLiteral
}