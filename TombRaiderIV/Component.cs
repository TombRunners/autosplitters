using System.Xml;
using LiveSplit.Model; // LiveSplitState
using LiveSplit.UI;
using TRUtil;          // LaterClassicAutosplitter, LaterClassicComponent

namespace TR4;

/// <summary>Implementation of <see cref="LaterClassicComponent{TData,TSettings}" />.</summary>
/// <inheritdoc />
internal sealed class Component(LaterClassicAutosplitter<GameData, ComponentSettings> autosplitter, LiveSplitState state)
    : LaterClassicComponent<GameData, ComponentSettings>(autosplitter, state)
{
    public override string ComponentName => "Tomb Raider IV and The Times Exclusive";

    /// <inheritdoc />
    /// <param name="document"><see cref="XmlDocument" /> passed by LiveSplit</param>
    /// <remarks>
    ///     Even if you don't have any settings, you can't return with null.
    ///     If you do, LiveSplit spams the Event Viewer with error messages:
    ///     <c>Object reference not set to an instance of an object.</c>
    ///     <see href="https://github.com/LiveSplit/LiveSplit.ScriptableAutoSplit/blob/7e5a6cbe91569e7688fdb37446d32326b4b14b1c/ComponentSettings.cs#L70" />
    ///     <see href="https://github.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/blob/master/LiveSplit.DarkSoulsIGT/UI/DSSettings.cs#L25" />
    /// </remarks>
    public override XmlNode GetSettings(XmlDocument document)
    {
        var settingsNode = document.CreateElement("Settings");
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.FullGame), Splitter.Settings.FullGame));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.Deathrun), Splitter.Settings.Deathrun));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.LegacyGlitchless), Splitter.Settings.LegacyGlitchless));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.EnableAutoReset), Splitter.Settings.EnableAutoReset));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.SplitSecrets), Splitter.Settings.SplitSecrets));

        var tr4TransitionsNode = document.CreateElement("Tr4LevelTransitions");
        foreach (var transition in Splitter.Settings.Tr4LevelTransitions)
            tr4TransitionsNode.AppendChild(transition.ToXmlElement(document));
        settingsNode.AppendChild(tr4TransitionsNode);

        var tteTransitionsNode = document.CreateElement("TteLevelTransitions");
        foreach (var transition in Splitter.Settings.TteLevelTransitions)
            tteTransitionsNode.AppendChild(transition.ToXmlElement(document));
        settingsNode.AppendChild(tteTransitionsNode);

        return settingsNode;
    }

    public override void SetSettings(XmlNode settings)
    {
        // Read serialized values, or keep defaults if they are not yet serialized.
        Splitter.Settings.FullGame = SettingsHelper.ParseBool(settings["FullGame"], Splitter.Settings.FullGame);
        Splitter.Settings.Deathrun = SettingsHelper.ParseBool(settings["Deathrun"], Splitter.Settings.Deathrun);
        Splitter.Settings.LegacyGlitchless = SettingsHelper.ParseBool(settings["LegacyGlitchless"], Splitter.Settings.LegacyGlitchless);
        Splitter.Settings.EnableAutoReset = SettingsHelper.ParseBool(settings["EnableAutoReset"], Splitter.Settings.EnableAutoReset);
        Splitter.Settings.SplitSecrets = SettingsHelper.ParseBool(settings["SplitSecrets"], Splitter.Settings.SplitSecrets);

        var tr4TransitionsNode = settings["Tr4LevelTransitions"];
        if (tr4TransitionsNode != null)
        {
            Splitter.Settings.Tr4LevelTransitions.Clear();
            foreach (XmlNode transitionNode in tr4TransitionsNode.ChildNodes)
                Splitter.Settings.Tr4LevelTransitions.Add(TransitionSetting<Tr4Level>.Tr4FromXmlElement(transitionNode));
        }

        var tteTransitionsNode = settings["TteLevelTransitions"];
        if (tteTransitionsNode != null)
        {
            Splitter.Settings.TteLevelTransitions.Clear();
            foreach (XmlNode transitionNode in tteTransitionsNode.ChildNodes)
                Splitter.Settings.TteLevelTransitions.Add(TransitionSetting<TteLevel>.TteFromXmlElement(transitionNode));
        }

        // Assign values to Settings.
        if (Splitter.Settings.FullGame)
            Splitter.Settings.FullGameModeButton.Checked = true; // Grouped RadioButton
        else if (Splitter.Settings.Deathrun)
            Splitter.Settings.DeathrunModeButton.Checked = true; // Grouped RadioButton
        else
            Splitter.Settings.ILModeButton.Checked = true;       // Grouped RadioButton

        Splitter.Settings.LegacyGlitchlessCheckbox.Checked = Splitter.Settings.LegacyGlitchless; // CheckBox
        Splitter.Settings.EnableAutoResetCheckbox.Checked = Splitter.Settings.EnableAutoReset;   // CheckBox
        Splitter.Settings.SplitSecretsCheckbox.Checked = Splitter.Settings.SplitSecrets;         // CheckBox
    }
}