using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TRUtil;

namespace TR4;

public sealed class ComponentSettings : LaterClassicComponentSettings
{
    private GroupBox _modeSelect;
    public RadioButton ILModeButton;
    public RadioButton FullGameModeButton;
    public RadioButton DeathrunModeButton;
    public CheckBox GlitchlessCheckbox;
    public CheckBox EnableAutoResetCheckbox;
    public CheckBox SplitSecretsCheckbox;
    public bool Glitchless;

    private Panel _levelTransitionSettingsPanel;
    private GroupBox _levelTransitionSettings;
    private Button _selectAllButton;
    private Button _unselectAllButton;

    private readonly List<TransitionSetting> _levelTransitions =
        [
            // Cambodia
            new(Tr4Level.AngkorWat, Tr4Level.RaceForTheIris, TransitionDirection.OneWayFromLower),     // 01  -> 02
            new(Tr4Level.RaceForTheIris, Tr4Level.TheTombOfSeth, TransitionDirection.OneWayFromLower), // 02  -> 03
            // Valley of the Kings
            new(Tr4Level.TheTombOfSeth, Tr4Level.BurialChambers, TransitionDirection.OneWayFromLower),    // 03  -> 04
            new(Tr4Level.BurialChambers, Tr4Level.ValleyOfTheKings, TransitionDirection.OneWayFromLower), // 04  -> 05
            new(Tr4Level.ValleyOfTheKings, Tr4Level.Kv5, TransitionDirection.OneWayFromLower),            // 05  -> 06
            new(Tr4Level.Kv5, Tr4Level.TempleOfKarnak, TransitionDirection.OneWayFromLower),              // 06  -> 07
            // Karnak
            new(Tr4Level.TempleOfKarnak, Tr4Level.GreatHypostyleHall, TransitionDirection.OneWayFromLower),                 // 07  -> 08
            new(Tr4Level.GreatHypostyleHall, Tr4Level.SacredLake, TransitionDirection.OneWayFromLower),                     // 08  -> 09
            new(Tr4Level.TempleOfKarnak, Tr4Level.SacredLake, TransitionDirection.OneWayFromHigher),                        // 09  -> 07
            new(Tr4Level.SacredLake, Tr4Level.TombOfSemerkhet, TransitionDirection.OneWayFromLower, unusedLevelNumber: 10), // 09  -> 11
            new(Tr4Level.TombOfSemerkhet, Tr4Level.GuardianOfSemerkhet, TransitionDirection.OneWayFromLower),               // 11  -> 12
            new(Tr4Level.GuardianOfSemerkhet, Tr4Level.DesertRailroad, TransitionDirection.OneWayFromLower),                // 12  -> 13
            // Eastern Desert
            new(Tr4Level.DesertRailroad, Tr4Level.Alexandria, TransitionDirection.OneWayFromLower), // 13  -> 14
            // Alexandria
            new(Tr4Level.Alexandria, Tr4Level.CoastalRuins, TransitionDirection.TwoWay), // 14 <-> 15

            new(Tr4Level.CoastalRuins, Tr4Level.Catacombs, TransitionDirection.TwoWay, lowerRoomNumber: 153, higherRoomNumber: 2, note: "Setup"), // 15 <-> 18
            new(Tr4Level.CoastalRuins, Tr4Level.Catacombs, TransitionDirection.TwoWay, lowerRoomNumber: 154, higherRoomNumber: 6, note: "Start"), // 15 <-> 18 (Catacombs start)
            new(Tr4Level.CoastalRuins, Tr4Level.Catacombs, TransitionDirection.TwoWay, lowerRoomNumber: 158, higherRoomNumber: 9, note: "End"),   // 15 <-> 18 (Catacombs end)

            new(Tr4Level.CoastalRuins, Tr4Level.TempleOfPoseidon, TransitionDirection.TwoWay),          // 15 <-> 19
            new(Tr4Level.Catacombs, Tr4Level.TempleOfPoseidon, TransitionDirection.TwoWay),             // 18 <-> 19
            new(Tr4Level.TempleOfPoseidon, Tr4Level.TheLostLibrary, TransitionDirection.TwoWay),        // 19 <-> 20
            new(Tr4Level.TheLostLibrary, Tr4Level.HallOfDemetrius, TransitionDirection.TwoWay),         // 20 <-> 21
            new(Tr4Level.CoastalRuins, Tr4Level.HallOfDemetrius, TransitionDirection.OneWayFromHigher), // 21  -> 15

            new(Tr4Level.CoastalRuins, Tr4Level.PharosTempleOfIsis, TransitionDirection.OneWayFromLower, lowerRoomNumber: 104),      // 15  -> 16
            new(Tr4Level.CoastalRuins, Tr4Level.PharosTempleOfIsis, TransitionDirection.OneWayFromHigher, higherRoomNumber: 77),     // 16  -> 15
            new(Tr4Level.PharosTempleOfIsis, Tr4Level.CleopatrasPalaces, TransitionDirection.TwoWay),                                // 16 <-> 17
            new(Tr4Level.CleopatrasPalaces, Tr4Level.CityOfTheDead, TransitionDirection.OneWayFromLower),                            // 17  -> 22
            // Cairo
            new(Tr4Level.CityOfTheDead, Tr4Level.ChambersOfTulun, TransitionDirection.TwoWay), // 22 <-> 24
            new(Tr4Level.Trenches, Tr4Level.ChambersOfTulun, TransitionDirection.TwoWay),      // 23 <-> 24
            new(Tr4Level.Trenches, Tr4Level.CitadelGate, TransitionDirection.TwoWay),          // 23 <-> 24
            new(Tr4Level.ChambersOfTulun, Tr4Level.CitadelGate, TransitionDirection.TwoWay),   // 24 <-> 26

            new(Tr4Level.Trenches, Tr4Level.StreetBazaar, TransitionDirection.TwoWay, lowerRoomNumber: 23, higherRoomNumber: 26, note: "Minefield"), // 23 <-> 25
            new(Tr4Level.Trenches, Tr4Level.StreetBazaar, TransitionDirection.TwoWay, lowerRoomNumber: 44, higherRoomNumber: 08, note: "Garage"),    // 23 <-> 25
            new(Tr4Level.Trenches, Tr4Level.StreetBazaar, TransitionDirection.OneWayFromHigher, higherRoomNumber: 71, note: "Chute After Guardian"), // 25  -> 23

            new(Tr4Level.CitadelGate, Tr4Level.Citadel, TransitionDirection.OneWayFromLower, note: "Dragon Cutscene"),   // 26  -> 27 (dragon cutscene)
            new(Tr4Level.Citadel, Tr4Level.SphinxComplex, TransitionDirection.OneWayFromLower), // 27  -> 28
            // Giza
            new(Tr4Level.SphinxComplex, Tr4Level.UnderneathTheSphinx, TransitionDirection.TwoWay),              // 28 <-> 30
            new(Tr4Level.UnderneathTheSphinx, Tr4Level.MenkauresPyramid, TransitionDirection.TwoWay),           // 30 <-> 31
            new(Tr4Level.MenkauresPyramid, Tr4Level.InsideMenkauresPyramid, TransitionDirection.TwoWay),        // 31 <-> 32
            new(Tr4Level.SphinxComplex, Tr4Level.InsideMenkauresPyramid, TransitionDirection.OneWayFromHigher), // 32  -> 28

            new(Tr4Level.SphinxComplex, Tr4Level.TheMastabas, TransitionDirection.TwoWay),                                               // 28 <-> 33
            new(Tr4Level.TheMastabas, Tr4Level.TheGreatPyramid, TransitionDirection.OneWayFromLower),                                    // 33  -> 34
            new(Tr4Level.TheGreatPyramid, Tr4Level.KhufusQueensPyramid, TransitionDirection.OneWayFromLower),                            // 34  -> 35
            new(Tr4Level.KhufusQueensPyramid, Tr4Level.InsideTheGreatPyramid, TransitionDirection.TwoWay),                               // 35 <-> 36
            new(Tr4Level.InsideTheGreatPyramid, Tr4Level.TempleOfHorus, TransitionDirection.OneWayFromLower),                            // 36  -> 37
            new(Tr4Level.TempleOfHorus, Tr4Level.HorusBoss, TransitionDirection.OneWayFromLower),                                        // 37  -> 38
            new(Tr4Level.HorusBoss, Tr4Level.MainMenu, TransitionDirection.OneWayFromLower, unusedLevelNumber: 39, note: "End of Game"), // 38  -> End
        ];

    public ComponentSettings(Version version)
    {
        InitializeComponent();
        AutosplitterVersionLabel.Text = $"Autosplitter Version: {version}";
    }

    private void InitializeComponent()
    {
        _modeSelect = new GroupBox();
        FullGameModeButton = new RadioButton();
        ILModeButton = new RadioButton();
        DeathrunModeButton = new RadioButton();
        GlitchlessCheckbox = new CheckBox();
        EnableAutoResetCheckbox = new CheckBox();
        SplitSecretsCheckbox = new CheckBox();
        _levelTransitionSettings = new GroupBox();
        GameVersionLabel = new Label();
        AutosplitterVersionLabel = new Label();
        AslWarningLabel = new Label();
        _modeSelect.SuspendLayout();
        _levelTransitionSettings.SuspendLayout();
        SuspendLayout();

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
        SplitSecretsCheckbox.Text = "Split When Secret is Triggered";
        SplitSecretsCheckbox.TabIndex = 0;
        SplitSecretsCheckbox.UseVisualStyleBackColor = true;
        SplitSecretsCheckbox.CheckedChanged += SplitSecretsCheckboxCheckedChanged;

        // GlitchlessCheckbox
        GlitchlessCheckbox.AutoSize = true;
        GlitchlessCheckbox.Checked = false;
        GlitchlessCheckbox.Location = new Point(12, 120);
        GlitchlessCheckbox.Name = "GlitchlessCheckbox";
        GlitchlessCheckbox.Text = "Glitchless";
        GlitchlessCheckbox.TabIndex = 0;
        GlitchlessCheckbox.UseVisualStyleBackColor = true;
        GlitchlessCheckbox.CheckedChanged += GlitchlessCheckboxCheckedChanged;

        // Level Transition Setting
        _levelTransitionSettings.Location = new Point(0, 150);
        _levelTransitionSettings.Name = "_levelTransitionSettings";
        _levelTransitionSettings.Size = new Size(476, 270);
        _levelTransitionSettings.Text = "Level Transition Settings";

        // GameVersionLabel
        GameVersionLabel.AutoSize = true;
        GameVersionLabel.Location = new Point(10, 440);
        GameVersionLabel.Name = "GameVersionLabel";
        GameVersionLabel.Size = new Size(200, 15);
        GameVersionLabel.TabIndex = 1;
        GameVersionLabel.Text = "Game Version: Unknown/Undetected";

        // AutosplitterVersionLabel
        AutosplitterVersionLabel.AutoSize = true;
        AutosplitterVersionLabel.Location = new Point(10, 460);
        AutosplitterVersionLabel.Name = "AutosplitterVersionLabel";
        AutosplitterVersionLabel.Size = new Size(200, 15);
        AutosplitterVersionLabel.TabIndex = 2;
        AutosplitterVersionLabel.Text = "Autosplitter Version: Uninitialized";

        // _aslWarningLabel
        AslWarningLabel.AutoSize = true;
        AslWarningLabel.Font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
        AslWarningLabel.ForeColor = Color.Crimson;
        AslWarningLabel.Location = new Point(24, 480);
        AslWarningLabel.Name = "AslWarningLabel";
        AslWarningLabel.Size = new Size(476, 20);
        AslWarningLabel.TabStop = false;
        AslWarningLabel.Text = "Scriptable Auto Splitter in Layout — Please Remove!";
        AslWarningLabel.Visible = false;

        // ComponentSettings
        Controls.Add(AslWarningLabel);
        Controls.Add(AutosplitterVersionLabel);
        Controls.Add(GameVersionLabel);
        Controls.Add(GlitchlessCheckbox);
        Controls.Add(EnableAutoResetCheckbox);
        Controls.Add(SplitSecretsCheckbox);
        Controls.Add(_modeSelect);
        Controls.Add(_levelTransitionSettings);
        Name = "ComponentSettings";
        Size = new Size(476, 500);
        _modeSelect.ResumeLayout(false);
        _modeSelect.PerformLayout();

        ResumeLayout(false);
        PerformLayout();

        AddLevelTransitionsSettings();
    }

    private void AddLevelTransitionsSettings()
    {
        SuspendLayout();

        _levelTransitionSettingsPanel = new Panel();
        _selectAllButton = new Button();
        _unselectAllButton = new Button();

        // _levelTransitionSettingsPanel
        _levelTransitionSettingsPanel.Location = new Point(5, 20);
        _levelTransitionSettingsPanel.Size = new Size(470, 220);
        _levelTransitionSettingsPanel.Padding = _levelTransitionSettings.Padding with { Left = 0, Right = 0 };
        _levelTransitionSettingsPanel.AutoScroll = false;
        _levelTransitionSettingsPanel.AutoScrollMinSize = new Size(0, _levelTransitionSettingsPanel.Height);
        _levelTransitionSettingsPanel.VerticalScroll.Enabled = true;
        _levelTransitionSettingsPanel.VerticalScroll.Visible = true;
        _levelTransitionSettingsPanel.HorizontalScroll.Enabled = false;
        _levelTransitionSettingsPanel.HorizontalScroll.Visible = false;

        _levelTransitionSettings.Controls.Add(_selectAllButton);
        _levelTransitionSettings.Controls.Add(_unselectAllButton);
        _levelTransitionSettings.Controls.Add(_levelTransitionSettingsPanel);

        // _selectAllButton
        _selectAllButton.Location = new Point(10, 240);
        _selectAllButton.Size = new Size(100, 20);
        _selectAllButton.Text = "Select All";
        _selectAllButton.Click += SelectAllButton_Click;

        // _unselectAllButton
        _unselectAllButton.Location = new Point(350, 240);
        _unselectAllButton.Size = new Size(100, 20);
        _unselectAllButton.Text = "Unselect All";
        _unselectAllButton.Click += UnselectAllButton_Click;

        var yOffset = 0;
        foreach (var transition in _levelTransitions)
        {
            transition.CheckBox = new CheckBox
            {
                Text = transition.DisplayName,
                Location = new Point(0, yOffset),
                AutoSize = true,
                Height = 20,
                Padding = Padding with { Left = 0, Right = 0 },
            };
            _levelTransitionSettingsPanel.Controls.Add(transition.CheckBox);

            if (transition.Directionality == TransitionDirection.TwoWay)
            {
                const int maxWidth = 180;
                int availableWidth = 452 - transition.CheckBox.Width;
                int width = Math.Min(maxWidth, availableWidth);

                var font = new Font(transition.CheckBox.Font, FontStyle.Regular);
                var directionComboBox = new ComboBox
                {
                    Location = new Point(452 - width, yOffset - 2),
                    Size = new Size(width, 20),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = font,
                    Padding = Padding with { Left = 0, Right = 0 },
                };
                directionComboBox.Items.AddRange(["Two-Way", $"From {transition.Lower.Name()}", $"From {transition.Higher.Name()}"]);
                directionComboBox.SelectedIndex = 0;
                directionComboBox.SelectedIndexChanged += (_, _) =>
                {
                    transition.SelectedDirectionality = (TransitionDirection)directionComboBox.SelectedIndex;
                };

                _levelTransitionSettingsPanel.Controls.Add(directionComboBox);
            }

            yOffset += 22;
        }

        _levelTransitionSettings.ResumeLayout(false);
        _levelTransitionSettings.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private void SelectAllButton_Click(object sender, EventArgs e)
    {
        foreach (var transition in _levelTransitions)
            transition.CheckBox.Checked = true;
    }

    private void UnselectAllButton_Click(object sender, EventArgs e)
    {
        foreach (var transition in _levelTransitions)
            transition.CheckBox.Checked = false;
    }

    private void GlitchlessCheckboxCheckedChanged(object sender, EventArgs e)
    {
        var checkbox = (CheckBox)sender;
        Glitchless = checkbox.Checked;
    }

    public override void SetGameVersion(uint version, string hash)
    {
        const string digitalText = "Steam/GOG [TR4]";
        const string tteText = "The Times Exclusive [TTE]";

        string versionText;
        switch ((Tr4Version)version)
        {
            case Tr4Version.SteamOrGog:
                versionText = digitalText;
                break;

            case Tr4Version.TheTimesExclusive:
                versionText = tteText;
                break;

            case Tr4Version.None:
            default:
                base.SetGameVersion(version, hash);
                return;
        }

        GameVersionLabel.Text = "Game Version: " + versionText;
    }
}