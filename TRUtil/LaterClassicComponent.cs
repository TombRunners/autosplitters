using LiveSplit.Model;                    // LiveSplitState
using LiveSplit.UI;                       // LayoutMode
using LiveSplit.UI.Components;            // IComponent, LogicComponent, SettingsHelper
using LiveSplit.UI.Components.AutoSplit;  // AutoSplitComponent, IAutoSplitter
using System;                             // EventArgs, IDisposable
using System.Windows.Forms;               // Control, TableLayoutPanel
using System.Xml;                         // XmlDocument, XmlElement, XmlNode

namespace TRUtil;

/// <summary>
///     Implementation of <see cref="AutoSplitComponent"/>.
/// </summary>
/// <remarks>
///     <see cref="AutoSplitComponent"/> is derived from <see cref="LogicComponent"/>,
///     which derives from <see cref="IComponent"/> and <see cref="IDisposable"/>.
/// </remarks>
public abstract class LaterClassicComponent : AutoSplitComponent
{
    private readonly LaterClassicAutosplitter _splitter;
    private readonly LiveSplitState _state;
    
    private void StateOnStart(object _0, EventArgs _1) => _splitter?.OnStart();
    private void StateOnSplit(object _0, EventArgs _1) => _splitter?.OnSplit();
    private void StateOnUndoSplit(object _0, EventArgs _1) => _splitter?.OnUndoSplit();

    protected LaterClassicComponent(LaterClassicAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
    {
        _splitter = autosplitter;
        _state = state;
        _state.OnSplit += StateOnSplit;
        _state.OnStart += StateOnStart;
        _state.OnUndoSplit += StateOnUndoSplit;
    }

    /// <inheritdoc/>
    /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
    /// <remarks>
    ///     Returned object must contain at least an empty <see cref="TableLayoutPanel"/>, otherwise the Layout Settings menu doesn't show up!
    /// </remarks>
    public override Control GetSettingsControl(LayoutMode mode) => _splitter.Settings;

    public override XmlNode GetSettings(XmlDocument document)
    {
        var settingsNode = document.CreateElement("Settings");
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.FullGame), _splitter.Settings.FullGame));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.Deathrun), _splitter.Settings.Deathrun));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.Option), _splitter.Settings.Option));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.DisableAutoResetCheckbox), _splitter.Settings.DisableAutoResetCheckbox));
        return settingsNode;
    }

    public override void SetSettings(XmlNode settings)
    {
        _splitter.Settings.FullGame = settings["FullGame"]?.InnerText == "True";
        _splitter.Settings.Deathrun = settings["Deathrun"]?.InnerText == "True";
        _splitter.Settings.Option = settings["Option"]?.InnerText == "True";
        _splitter.Settings.DisableAutoReset = settings["DisableAutoReset"]?.InnerText == "True";

        if (_splitter.Settings.FullGame)
            _splitter.Settings.FullGameModeButton.Checked = true;
        else if (_splitter.Settings.Deathrun)
            _splitter.Settings.DeathrunModeButton.Checked = true;
        else
            _splitter.Settings.ILModeButton.Checked = true;

        if (_splitter.Settings.Option)
            _splitter.Settings.OptionCheckbox.Checked = true;

        if (_splitter.Settings.DisableAutoReset)
            _splitter.Settings.DisableAutoResetCheckbox.Checked = true;
    }

    public override void Dispose()
    {
        _state.OnSplit -= StateOnSplit;
        _state.OnStart -= StateOnStart;
        _state.OnUndoSplit -= StateOnUndoSplit;
        _splitter?.Dispose();
    }

    public override string ComponentName => "Later Classic Tomb Raider Component";

    /// <summary>
    ///     Adds <see cref="LaterClassicGameData"/> and <see cref="LaterClassicAutosplitter"/> management to <see cref="AutoSplitComponent.Update"/>.
    /// </summary>
    /// <param name="invalidator"><see cref="IInvalidator"/> passed by LiveSplit</param>
    /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
    /// <param name="width">width passed by LiveSplit</param>
    /// <param name="height">height passed by LiveSplit</param>
    /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
    /// <remarks>
    ///     This override allows <see cref="LaterClassicAutosplitter"/> to use <see cref="ClassicGameData"/> in its logic.
    /// </remarks>
    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        if (_splitter.Data.Update())
            base.Update(invalidator, state, width, height, mode);
    }
}