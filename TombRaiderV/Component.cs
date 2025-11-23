using System.Xml;
using LaterClassicUtil;
using LiveSplit.Model;
using LiveSplit.UI;

namespace TR5;

/// <summary>Implementation of <see cref="LaterClassicComponent{TData,TSettings}" />.</summary>
/// <inheritdoc />
internal sealed class Component(LaterClassicAutosplitter<GameData, ComponentSettings> autosplitter, LiveSplitState state)
    : LaterClassicComponent<GameData, ComponentSettings>(autosplitter, state)
{
    public override string ComponentName => "Tomb Raider V";

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
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.SplitSecurityBreach), Splitter.Settings.SplitSecurityBreach));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.EnableAutoReset), Splitter.Settings.EnableAutoReset));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(Splitter.Settings.SplitSecrets), Splitter.Settings.SplitSecrets));
        return settingsNode;
    }

    public override void SetSettings(XmlNode settings)
    {
        // Read serialized values, or keep defaults if they are not yet serialized.
        Splitter.Settings.FullGame = SettingsHelper.ParseBool(settings["FullGame"], Splitter.Settings.FullGame);
        Splitter.Settings.Deathrun = SettingsHelper.ParseBool(settings["Deathrun"], Splitter.Settings.Deathrun);
        Splitter.Settings.SplitSecurityBreach = SettingsHelper.ParseBool(settings["SplitSecurityBreach"], Splitter.Settings.SplitSecurityBreach);
        Splitter.Settings.EnableAutoReset = SettingsHelper.ParseBool(settings["EnableAutoReset"], Splitter.Settings.EnableAutoReset);
        Splitter.Settings.SplitSecrets = SettingsHelper.ParseBool(settings["SplitSecrets"], Splitter.Settings.SplitSecrets);

        // Assign values to Settings.
        if (Splitter.Settings.FullGame) // Grouped RadioButtons
            Splitter.Settings.FullGameModeButton.Checked = true;
        else if (Splitter.Settings.Deathrun)
            Splitter.Settings.DeathrunModeButton.Checked = true;
        else
            Splitter.Settings.ILModeButton.Checked = true;

        Splitter.Settings.SplitSecurityBreachCheckbox.Checked = Splitter.Settings.SplitSecurityBreach; // CheckBox
        Splitter.Settings.EnableAutoResetCheckbox.Checked = Splitter.Settings.EnableAutoReset;         // CheckBox
        Splitter.Settings.SplitSecretsCheckbox.Checked = Splitter.Settings.SplitSecrets;               // CheckBox
    }
}