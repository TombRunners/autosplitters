using LiveSplit.Model;                    // LiveSplitState
using LiveSplit.UI;                       // IInvalidator, LayoutMode, SettingsHelper
using LiveSplit.UI.Components;            // ASLComponent, IComponent, LogicComponent
using LiveSplit.UI.Components.AutoSplit;  // AutoSplitComponent, IAutoSplitter
using System;                             // EventArgs, IDisposable
using System.Linq;                        // Any
using System.Windows.Forms;               // Control, TableLayoutPanel

namespace TRUtil;

/// <summary>
///     Implementation of <see cref="AutoSplitComponent"/>.
/// </summary>
/// <remarks>
///     <see cref="AutoSplitComponent"/> is derived from <see cref="LogicComponent"/>,
///     which derives from <see cref="IComponent"/> and <see cref="IDisposable"/>.
/// </remarks>
public abstract class LaterClassicComponent<TData, TSettings> : AutoSplitComponent
    where TData : LaterClassicGameData
    where TSettings : LaterClassicComponentSettings
{
    protected readonly LaterClassicAutosplitter<TData, TSettings> Splitter;
    private readonly LiveSplitState _state;

    private bool? _aslComponentPresent;
    private int _layoutComponentCount;

    /// <summary>Allows creation of an event when an ASL Component was found in the LiveSplit layout.</summary>
    private delegate void AslComponentChangedDelegate(bool aslComponentIsPresent);

    /// <summary>Allows subscribers to know when an ASL Component was found in the LiveSplit layout.</summary>
    private AslComponentChangedDelegate _onAslComponentChanged;

    private void StateOnStart(object _0, EventArgs _1) => Splitter?.OnStart();
    private void StateOnSplit(object _0, EventArgs _1) => Splitter?.OnSplit();
    private void StateOnUndoSplit(object _0, EventArgs _1) => Splitter?.OnUndoSplit();

    protected LaterClassicComponent(LaterClassicAutosplitter<TData, TSettings> autosplitter, LiveSplitState state) : base(autosplitter, state)
    {
        Splitter = autosplitter;
        _onAslComponentChanged += Splitter.Settings.SetAslWarningLabelVisibility;

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
    public override Control GetSettingsControl(LayoutMode mode) => Splitter.Settings;

    public override string ComponentName => "Later Classic Tomb Raider Component";

    /// <summary>
    ///     Adds <see cref="LaterClassicGameData"/> and <see cref="LaterClassicAutosplitter{TData,TSettings}"/> management to <see cref="AutoSplitComponent.Update"/>.
    /// </summary>
    /// <param name="invalidator"><see cref="IInvalidator"/> passed by LiveSplit</param>
    /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
    /// <param name="width">Width passed by LiveSplit</param>
    /// <param name="height">Height passed by LiveSplit</param>
    /// <param name="mode"><see cref="LayoutMode"/> passed by LiveSplit</param>
    /// <remarks>
    ///     This override allows <see cref="LaterClassicAutosplitter{TData,TSettings}"/> to use <see cref="ClassicGameData"/> in its logic.
    /// </remarks>
    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
    {
        int layoutComponentsCount = state.Layout.LayoutComponents.Count;
        if (_aslComponentPresent is null || layoutComponentsCount != _layoutComponentCount)
        {
            _layoutComponentCount = layoutComponentsCount;
            HandleLayoutUpdates(state);
        }

        if (Splitter.Data.Update())
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
        _onAslComponentChanged += Splitter.Settings.SetAslWarningLabelVisibility;
        Splitter?.Dispose();
    }
}