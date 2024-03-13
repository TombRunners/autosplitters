using LiveSplit.Model; // LiveSplitState
using TRUtil;          // ClassicAutosplitter, ClassicComponent

namespace TR2;

/// <summary>Implementation of <see cref="ClassicComponent" />.</summary>
/// <inheritdoc />
internal sealed class Component(ClassicAutosplitter autosplitter, LiveSplitState state) : ClassicComponent(autosplitter, state)
{
    public override string ComponentName => "Tomb Raider II and Golden Mask";
}