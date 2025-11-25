using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LaterClassicUtil;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.UI;

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
        XmlElement settingsNode = document.CreateElement("Settings");
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.FullGame), Splitter.Settings.FullGame));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.Deathrun), Splitter.Settings.Deathrun));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.LegacyGlitchless), Splitter.Settings.LegacyGlitchless));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.EnableAutoReset), Splitter.Settings.EnableAutoReset));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.SplitSecrets), Splitter.Settings.SplitSecrets));

        AppendTransitionSettings(document, settingsNode, nameof(Splitter.Settings.Tr4LevelTransitions), Splitter.Settings.Tr4LevelTransitions);
        AppendTransitionSettings(document, settingsNode, nameof(Splitter.Settings.TteLevelTransitions), Splitter.Settings.TteLevelTransitions);

        return settingsNode;
    }

    private static void AppendTransitionSettings<TLevel>(XmlDocument doc, XmlNode settingsNode, string elementName, IEnumerable<TransitionSetting<TLevel>> transitions)
        where TLevel : Enum
    {
        XmlElement transitionsNode = doc.CreateElement(elementName);
        foreach (var transition in transitions)
            transitionsNode.AppendChild(transition.ToXmlElement(doc));

        settingsNode.AppendChild(transitionsNode);
    }

    /// <inheritdoc />
    /// <param name="settings"><see cref="XmlNode" /> passed by LiveSplit</param>
    /// <remarks>
    ///     This might happen more than once (e.g., when the settings dialog is canceled, to restore previous settings).
    ///     The XML file is the <c>[game - category].lss</c> file in your LiveSplit folder.
    ///     <see href="https://github.com/LiveSplit/LiveSplit.ScriptableAutoSplit/blob/7e5a6cbe91569e7688fdb37446d32326b4b14b1c/ComponentSettings.cs#L70" />
    ///     <see href="https://github.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/blob/master/LiveSplit.DarkSoulsIGT/UI/DSSettings.cs#L25" />
    /// </remarks>
    public override void SetSettings(XmlNode settings)
    {
        // Read serialized values, or keep defaults if they are not yet serialized.
        Splitter.Settings.FullGame = SettingsHelper.ParseBool(settings["FullGame"], Splitter.Settings.FullGame);
        Splitter.Settings.Deathrun = SettingsHelper.ParseBool(settings["Deathrun"], Splitter.Settings.Deathrun);
        Splitter.Settings.LegacyGlitchless = SettingsHelper.ParseBool(settings["LegacyGlitchless"], Splitter.Settings.LegacyGlitchless);
        Splitter.Settings.EnableAutoReset = SettingsHelper.ParseBool(settings["EnableAutoReset"], Splitter.Settings.EnableAutoReset);
        Splitter.Settings.SplitSecrets = SettingsHelper.ParseBool(settings["SplitSecrets"], Splitter.Settings.SplitSecrets);

        // Assign values to Settings.
        if (Splitter.Settings.FullGame) // Grouped RadioButtons
            Splitter.Settings.FullGameModeButton.Checked = true;
        else if (Splitter.Settings.Deathrun)
            Splitter.Settings.DeathrunModeButton.Checked = true;
        else
            Splitter.Settings.ILModeButton.Checked = true;

        Splitter.Settings.LegacyGlitchlessCheckbox.Checked = Splitter.Settings.LegacyGlitchless; // CheckBox
        Splitter.Settings.EnableAutoResetCheckbox.Checked = Splitter.Settings.EnableAutoReset;   // CheckBox
        Splitter.Settings.SplitSecretsCheckbox.Checked = Splitter.Settings.SplitSecrets;         // CheckBox

        // Update transition settings lists.
        ProcessTransitionSettings(settings, nameof(Splitter.Settings.Tr4LevelTransitions), Splitter.Settings.Tr4LevelTransitions, "TR4");
        ProcessTransitionSettings(settings, nameof(Splitter.Settings.TteLevelTransitions), Splitter.Settings.TteLevelTransitions, "TTE");
    }

    private static void ProcessTransitionSettings<TLevel>(
        XmlNode settings, string nodeName, List<TransitionSetting<TLevel>> settingsList, string settingsPrefix
    )
        where TLevel : Enum
    {
        XmlElement transitionsNode = settings[nodeName];
        if (transitionsNode == null)
            return;

        int transitionsCount = settingsList.Count;
        int xmlTransitionsCount = transitionsNode.ChildNodes.Count;
        if (xmlTransitionsCount != transitionsCount)
        {
            Log.Error(
                $"Refusing to apply {settingsPrefix} level transition settings due to a mismatched count. " +
                $"{xmlTransitionsCount} found in XML, expected {transitionsCount}. Reverting to default/existing."
            );
            return;
        }

        // Create a copy of the existing settings for potential reversion.
        var defaultOrExistingTransitions = settingsList.ToArray();
        var encounteredSettingIds = new HashSet<ulong>(transitionsCount);

        foreach (XmlNode transitionNode in transitionsNode.ChildNodes)
        {
            TransitionSetting<TLevel> settingFromXml;
            try
            {
                settingFromXml = TransitionSetting<TLevel>.FromXmlElement(transitionNode);
            }
            catch (Exception ex)
            {
                Log.Error($"{settingsPrefix} level transition deserialization failed: {ex.Message}\n\n{ex.StackTrace}");
                RevertSettings(settingsList, defaultOrExistingTransitions);
                break;
            }

            var settingsNeedReversion = false;
            var existingSettings = settingsList.Where(t => t.Id == settingFromXml.Id).ToList();
            if (existingSettings.Count != 1)
            {
                settingsNeedReversion = true;
                Log.Error(
                    $"Found unexpected amount of matches ({existingSettings.Count}) for {settingsPrefix} level transition " +
                    $"from XML with ID {settingFromXml.Id}. Reverting to default/existing."
                );
            }

            if (!encounteredSettingIds.Add(settingFromXml.Id))
            {
                settingsNeedReversion = true;
                Log.Error(
                    $"Encountered {settingsPrefix} level transition setting more than once " +
                    $"from XML with ID {settingFromXml.Id}. Reverting to default/existing."
                );
            }

            if (settingsNeedReversion)
            {
                RevertSettings(settingsList, defaultOrExistingTransitions);
                break;
            }

            existingSettings[0].UpdateActive(settingFromXml.Active);
            existingSettings[0].SelectedDirectionality = settingFromXml.SelectedDirectionality;
        }
    }

    private static void RevertSettings<TLevel>(List<TransitionSetting<TLevel>> settings, TransitionSetting<TLevel>[] defaultOrExistingSettings)
        where TLevel : Enum
    {
        settings.Clear();
        settings.AddRange(defaultOrExistingSettings);
    }
}