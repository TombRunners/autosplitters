using LiveSplit.Model;                    // LiveSplitState
using LiveSplit.UI;                       // IInvalidator, LayoutMode, SettingsHelper
using LiveSplit.UI.Components;            // ASLComponent, IComponent, LogicComponent
using LiveSplit.UI.Components.AutoSplit;  // AutoSplitComponent, IAutoSplitter
using System;                             // EventArgs, IDisposable
using System.Linq;                        // Any
using System.Windows.Forms;               // Control, TableLayoutPanel
using System.Xml;                         // XmlDocument, XmlNode

namespace TRUtil;

/// <summary>
///     Implementation of <see cref="AutoSplitComponent"/>.
/// </summary>
/// <remarks>
///     <see cref="AutoSplitComponent"/> is derived from <see cref="LogicComponent"/>,
///     which derives from <see cref="IComponent"/> and <see cref="IDisposable"/>.
/// </remarks>
public abstract class LaterClassicComponent<TData> : AutoSplitComponent
    where TData : LaterClassicGameData
{
    private readonly LaterClassicAutosplitter<TData> _splitter;
    private readonly LiveSplitState _state;

    private bool? _aslComponentPresent;
    private int _layoutComponentCount;

    /// <summary>Allows creation of an event when an ASL Component was found in the LiveSplit layout.</summary>
    private delegate void AslComponentChangedDelegate(bool aslComponentIsPresent);

    /// <summary>Allows subscribers to know when an ASL Component was found in the LiveSplit layout.</summary>
    private AslComponentChangedDelegate _onAslComponentChanged;

    private void StateOnStart(object _0, EventArgs _1) => _splitter?.OnStart();
    private void StateOnSplit(object _0, EventArgs _1) => _splitter?.OnSplit();
    private void StateOnUndoSplit(object _0, EventArgs _1) => _splitter?.OnUndoSplit();

    protected LaterClassicComponent(LaterClassicAutosplitter<TData> autosplitter, LiveSplitState state) : base(autosplitter, state)
    {
        _splitter = autosplitter;
        _onAslComponentChanged += _splitter.Settings.SetAslWarningLabelVisibility;

        _state = state;
        _state.OnSplit += StateOnSplit;
        _state.OnStart += StateOnStart;
        _state.OnUndoSplit += StateOnUndoSplit;
    }

    /// <inheritdoc />
    /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
    /// <remarks>
    ///     The returned object must contain at least an empty <see cref="TableLayoutPanel"/>,
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
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.FullGame), _splitter.Settings.FullGame));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.Deathrun), _splitter.Settings.Deathrun));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.Option), _splitter.Settings.Option));
        _ = settingsNode.AppendChild(SettingsHelper.ToElement(document, nameof(_splitter.Settings.EnableAutoReset), _splitter.Settings.EnableAutoReset));
        return settingsNode;
    }

    public override void SetSettings(XmlNode settings)
    {
        // Read serialized values, or keep defaults if they are not yet serialized.
        _splitter.Settings.FullGame = SettingsHelper.ParseBool(settings["FullGame"], _splitter.Settings.FullGame);
        _splitter.Settings.Deathrun = SettingsHelper.ParseBool(settings["Deathrun"], _splitter.Settings.Deathrun);
        _splitter.Settings.Option = SettingsHelper.ParseBool(settings["Option"], _splitter.Settings.Option);
        _splitter.Settings.EnableAutoReset = SettingsHelper.ParseBool(settings["EnableAutoReset"], _splitter.Settings.EnableAutoReset);

        // Assign values to Settings.
        if (_splitter.Settings.FullGame)
            _splitter.Settings.FullGameModeButton.Checked = true; // Grouped RadioButton
        else if (_splitter.Settings.Deathrun)
            _splitter.Settings.DeathrunModeButton.Checked = true; // Grouped RadioButton
        else
            _splitter.Settings.ILModeButton.Checked = true;       // Grouped RadioButton

        _splitter.Settings.OptionCheckbox.Checked = _splitter.Settings.Option;                   // CheckBox
        _splitter.Settings.EnableAutoResetCheckbox.Checked = _splitter.Settings.EnableAutoReset; // CheckBox
    }

    public override string ComponentName => "Later Classic Tomb Raider Component";

    /// <summary>
    ///     Adds <see cref="LaterClassicGameData"/> and <see cref="LaterClassicAutosplitter{TData}"/> management to <see cref="AutoSplitComponent.Update"/>.
    /// </summary>
    /// <param name="invalidator"><see cref="IInvalidator"/> passed by LiveSplit</param>
    /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
    /// <param name="width">Width passed by LiveSplit</param>
    /// <param name="height">Height passed by LiveSplit</param>
    /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
    /// <remarks>
    ///     This override allows <see cref="LaterClassicAutosplitter{TData}"/> to use <see cref="ClassicGameData"/> in its logic.
    /// </remarks>
    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        int layoutComponentsCount = state.Layout.LayoutComponents.Count;
        if (_aslComponentPresent is null || layoutComponentsCount != _layoutComponentCount)
        {
            _layoutComponentCount = layoutComponentsCount;
            HandleLayoutUpdates(state);
        }

        if (_splitter.Data.Update())
            base.Update(invalidator, state, width, height, mode);
    }

    private void HandleLayoutUpdates(LiveSplitState state)
    {
        bool aslInLayout = state.Layout.LayoutComponents.Any(static comp => comp.Component is ASLComponent);
        if (_aslComponentPresent == aslInLayout)
            return;

        _aslComponentPresent = aslInLayout;
        _onAslComponentChanged.Invoke(aslInLayout);
    }

    public override void Dispose()
    {
        _state.OnSplit -= StateOnSplit;
        _state.OnStart -= StateOnStart;
        _state.OnUndoSplit -= StateOnUndoSplit;
        _onAslComponentChanged += _splitter.Settings.SetAslWarningLabelVisibility;
        _splitter?.Dispose();
    }
}