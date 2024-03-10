using LiveSplit.Model; // LiveSplitState
using TRUtil;          // LaterClassicAutosplitter, LaterClassicComponent

namespace TR4;

/// <inheritdoc />
/// <summary>Implementation of <see cref="T:TRUtil.LaterClassicComponent" />.</summary>
/// <inheritdoc/>
internal sealed class Component(LaterClassicAutosplitter autosplitter, LiveSplitState state) : LaterClassicComponent(autosplitter, state)
{
    public override string ComponentName => "Tomb Raider IV and The Times Exclusive";
}