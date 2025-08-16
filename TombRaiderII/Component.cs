using ClassicUtil;
using LiveSplit.Model;

namespace TR2;

/// <summary>Implementation of <see cref="ClassicComponent{TData}" />.</summary>
/// <inheritdoc />
internal sealed class Component(ClassicAutosplitter<GameData> autosplitter, LiveSplitState state) : ClassicComponent<GameData>(autosplitter, state)
{
    public override string ComponentName => "Tomb Raider II and Golden Mask";
}