using LiveSplit.Model;                    // LiveSplitState
using TRUtil;                             // ClassicAutosplitter, ClassicComponent

namespace TR1
{
    /// <summary>Implementation of <see cref="ClassicComponent"/>.</summary>
    internal sealed class Component : ClassicComponent
    {
        /// <inheritdoc/>
        public Component(ClassicAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
        {
        }

        public override string ComponentName => "Tomb Raider (1996)";
    }
}
