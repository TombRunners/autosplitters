using LiveSplit.Model; // LiveSplitState
using TRUtil;          // ClassicAutosplitter, ClassicComponent

namespace TR3;

/// <inheritdoc />
/// <summary>Implementation of <see cref="T:TRUtil.ClassicComponent" />.</summary>
internal sealed class Component : ClassicComponent
{
    /// <inheritdoc/>
    public Component(ClassicAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
    {
    }

    public override string ComponentName => "Tomb Raider III and The Lost Artifact";
}