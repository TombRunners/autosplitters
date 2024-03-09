using LiveSplit.Model; // LiveSplitState
using TRUtil;          // LaterClassicAutosplitter, LaterClassicComponent

namespace TR5;

/// <inheritdoc />
/// <summary>Implementation of <see cref="T:TRUtil.LaterClassicComponent" />.</summary>
internal sealed class Component : LaterClassicComponent
{
    /// <inheritdoc/>
    public Component(LaterClassicAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
    {
    }

    public override string ComponentName => "Tomb Raider V";
}