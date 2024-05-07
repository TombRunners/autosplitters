using LiveSplit.Model; // LiveSplitState
using TRUtil;          // LaterClassicAutosplitter, LaterClassicComponent

namespace TR5;

/// <summary>Implementation of <see cref="LaterClassicComponent{TData}" />.</summary>
/// <inheritdoc />
internal sealed class Component(LaterClassicAutosplitter<GameData> autosplitter, LiveSplitState state) : LaterClassicComponent<GameData>(autosplitter, state)
{
    public override string ComponentName => "Tomb Raider V";
}