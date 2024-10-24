using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using TRUtil;

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

        AppendTransitionSettings(document, settingsNode, nameof(Splitter.Settings.Tr4LevelTransitions), Splitter.Settings.Tr4LevelTransitions);
        AppendTransitionSettings(document, settingsNode, nameof(Splitter.Settings.TteLevelTransitions), Splitter.Settings.TteLevelTransitions);

        return settingsNode;
    }

    private static void AppendTransitionSettings<TLevel>(
        XmlDocument document, XmlNode settingsNode, string elementName, IEnumerable<TransitionSetting<TLevel>> transitions)
        where TLevel : Enum
    {
        var transitionsNode = document.CreateElement(elementName);
        foreach (var transition in transitions)
            transitionsNode.AppendChild(transition.ToXmlElement(document));

        settingsNode.AppendChild(transitionsNode);
    }

    public override void SetSettings(XmlNode settings)
    {
        // Read serialized values, or keep defaults if they are not yet serialized.
        Splitter.Settings.FullGame = SettingsHelper.ParseBool(settings["FullGame"], Splitter.Settings.FullGame);
        Splitter.Settings.Deathrun = SettingsHelper.ParseBool(settings["Deathrun"], Splitter.Settings.Deathrun);
        Splitter.Settings.LegacyGlitchless = SettingsHelper.ParseBool(settings["LegacyGlitchless"], Splitter.Settings.LegacyGlitchless);
        Splitter.Settings.EnableAutoReset = SettingsHelper.ParseBool(settings["EnableAutoReset"], Splitter.Settings.EnableAutoReset);
        Splitter.Settings.SplitSecrets = SettingsHelper.ParseBool(settings["SplitSecrets"], Splitter.Settings.SplitSecrets);

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

        // Update transition settings lists.
        ProcessTransitionSettings(settings, nameof(Splitter.Settings.Tr4LevelTransitions), Splitter.Settings.Tr4LevelTransitions);
        ProcessTransitionSettings(settings, nameof(Splitter.Settings.TteLevelTransitions), Splitter.Settings.TteLevelTransitions);
    }

    private static void ProcessTransitionSettings<TLevel>(XmlNode settings, string transitionsNodeName, List<TransitionSetting<TLevel>> settingsList)
        where TLevel : Enum
    {
        var transitionsNode = settings[transitionsNodeName];
        int transitionsCount = settingsList.Count;
        if (transitionsNode == null || transitionsNode.ChildNodes.Count != transitionsCount)
            return;

        var defaultOrExistingTransitions = new TransitionSetting<TLevel>[transitionsCount];
        var encounteredSettingIds = new HashSet<ulong>(transitionsCount);
        settingsList.CopyTo(defaultOrExistingTransitions);

        foreach (XmlNode transitionNode in transitionsNode.ChildNodes)
        {
            TransitionSetting<TLevel> settingFromXml;
            try
            {
                settingFromXml = TransitionSetting<TLevel>.FromXmlElement(transitionNode);
            }
            catch (Exception ex)
            {
                LiveSplit.Options.Log.Error($"{typeof(TLevel).Name} deserialization failed: {ex.Message}\n\n{ex.StackTrace}");
                RevertSettings(settingsList, defaultOrExistingTransitions);
                break;
            }

            var settingsNeedReversion = false;
            var existingSetting = settingsList.Where(t => t.Id == settingFromXml.Id).ToList();
            if (existingSetting.Count != 1)
            {
                settingsNeedReversion = true;
                LiveSplit.Options.Log.Error($"Found unexpected amount of matches ({existingSetting.Count}) for {typeof(TLevel).Name} from XML with ID {settingFromXml.Id}. Reverting to default/existing.");
            }

            if (!encounteredSettingIds.Add(settingFromXml.Id))
            {
                settingsNeedReversion = true;
                LiveSplit.Options.Log.Error($"Encountered {typeof(TLevel).Name} setting more than once from XML with ID {settingFromXml.Id}. Reverting to default/existing.");
            }

            if (settingsNeedReversion)
            {
                RevertSettings(settingsList, defaultOrExistingTransitions);
                break;
            }

            existingSetting[0].UpdateActive(settingFromXml.Active);
            existingSetting[0].SelectedDirectionality = settingFromXml.SelectedDirectionality;
        }
    }

    private static void RevertSettings<TLevel>(List<TransitionSetting<TLevel>> settings, TransitionSetting<TLevel>[] defaultOrExistingSettings)
        where TLevel : Enum
    {
        settings.Clear();
        settings.AddRange(defaultOrExistingSettings);
    }
}