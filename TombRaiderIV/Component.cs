using LiveSplit.Model; // LiveSplitState
using TRUtil;          // LaterClassicAutosplitter, LaterClassicComponent

namespace TR4
{
    /// <summary>Implementation of <see cref="LaterClassicComponent"/>.</summary>
    internal sealed class Component : LaterClassicComponent
    {
        /// <inheritdoc/>
        public Component(LaterClassicAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
        {
        }

        public override string ComponentName => "Tomb Raider IV and The Times Exclusive";
    }
}
