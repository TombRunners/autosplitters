using LiveSplit.Model; // LiveSplitState
using TRUtil;          // LaterClassicAutosplitter, LaterClassicComponent

namespace TR5;

/// <summary>Implementation of <see cref="LaterClassicComponent" />.</summary>
/// <inheritdoc />
internal sealed class Component(LaterClassicAutosplitter autosplitter, LiveSplitState state) : LaterClassicComponent(autosplitter, state)
{
    public override string ComponentName => "Tomb Raider V";
}