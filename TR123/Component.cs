using System;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.UI.Components.AutoSplit;

namespace TR123;

/// <summary>
///     Implementation of <see cref="AutoSplitComponent" />.
/// </summary>
/// <remarks>
///     <see cref="AutoSplitComponent" /> is derived from <see cref="LogicComponent" />,
///     which derives from <see cref="IComponent" /> and <see cref="IDisposable" />.
/// </remarks>
public class Component : AutoSplitComponent
{
    private readonly Autosplitter _splitter;
    private readonly LiveSplitState _state;

    /// <inheritdoc />
    public Component(Autosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
    {
        _splitter = autosplitter;
        _state = state;
        _state.OnSplit += StateOnSplit;
        _state.OnStart += StateOnStart;
        _state.OnUndoSplit += StateOnUndoSplit;
    }

    public override string ComponentName => "Tomb Raider I-III Remastered";

    private void StateOnStart(object _0, EventArgs _1) => _splitter?.OnStart();

    private void StateOnSplit(object _0, EventArgs _1) =>
        _splitter?.OnSplit(GameData.CurrentLevel());

    private void StateOnUndoSplit(object _0, EventArgs _1) => _splitter?.OnUndoSplit();

    /// <inheritdoc />
    /// <param name="mode"><see cref="LayoutMode" /> passed by LiveSplit</param>
    /// <remarks>
    ///     The returned object must contain at least an empty <see cref="TableLayoutPanel" />,
    ///     otherwise the Layout Settings menu doesn't show up!
    /// </remarks>
    public override Control GetSettingsControl(LayoutMode mode) => _splitter.Settings;

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
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.DisableAutoReset),
            _splitter.Settings.DisableAutoReset));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.FullGame),
            _splitter.Settings.FullGame));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.Deathrun),
            _splitter.Settings.Deathrun));
        return settingsNode;
    }

    /// <inheritdoc />
    /// <param name="settings"><see cref="XmlNode" /> passed by LiveSplit</param>
    /// <remarks>
    ///     This might happen more than once (e.g., when the settings dialog is cancelled, to restore previous settings).
    ///     The XML file is the <c>[game - category].lss</c> file in your LiveSplit folder.
    ///     <see href="https://github.com/LiveSplit/LiveSplit.ScriptableAutoSplit/blob/7e5a6cbe91569e7688fdb37446d32326b4b14b1c/ComponentSettings.cs#L70" />
    ///     <see href="https://github.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/blob/master/LiveSplit.DarkSoulsIGT/UI/DSSettings.cs#L25" />
    /// </remarks>
    public override void SetSettings(XmlNode settings)
    {
        // Read serialized values, or keep defaults if they are not yet serialized.
        _splitter.Settings.DisableAutoReset = SettingsHelper.ParseBool(settings["DisableAutoReset"], _splitter.Settings.DisableAutoReset);
        _splitter.Settings.FullGame = SettingsHelper.ParseBool(settings["FullGame"], _splitter.Settings.FullGame);
        _splitter.Settings.Deathrun = SettingsHelper.ParseBool(settings["Deathrun"], _splitter.Settings.Deathrun);

        // Assign values to Settings.
        _splitter.Settings.DisableAutoResetCheckbox.Checked = _splitter.Settings.DisableAutoReset; // CheckBox

        if (_splitter.Settings.FullGame)
            _splitter.Settings.FullGameModeButton.Checked = true; // Grouped RadioButton
        else if (_splitter.Settings.Deathrun)
            _splitter.Settings.DeathrunModeButton.Checked = true; // Grouped RadioButton
        else
            _splitter.Settings.ILModeButton.Checked = true; // Grouped RadioButton
    }

    public override void Dispose()
    {
        _state.OnSplit -= StateOnSplit;
        _state.OnStart -= StateOnStart;
        _state.OnUndoSplit -= StateOnUndoSplit;
        _splitter?.Dispose();
    }

    /// <summary>
    ///     Adds <see cref="GameData" /> and <see cref="Autosplitter" /> management to <see cref="AutoSplitComponent.Update" />.
    /// </summary>
    /// <param name="invalidator"><see cref="IInvalidator" /> passed by LiveSplit</param>
    /// <param name="state"><see cref="LiveSplitState" /> passed by LiveSplit</param>
    /// <param name="width">Width passed by LiveSplit</param>
    /// <param name="height">Height passed by LiveSplit</param>
    /// <param name="mode"><see cref="LayoutMode" /> passed by LiveSplit</param>
    /// <remarks>
    ///     This override allows <see cref="Autosplitter" /> to use <see cref="GameData" /> in its logic.
    /// </remarks>
    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height,
        LayoutMode mode)
    {
        if (_splitter.Data.Update())
            base.Update(invalidator, state, width, height, mode);
    }
}