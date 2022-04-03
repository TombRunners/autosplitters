using LiveSplit.Model; // LiveSplitState
using LiveSplit.UI;
using System.Xml;
using TRUtil;          // LaterClassicAutosplitter, LaterClassicComponent

namespace TR5
{
    /// <summary>Implementation of <see cref="LaterClassicComponent"/>.</summary>
    internal sealed class Component : LaterClassicComponent
    {
        /// <inheritdoc/>
        public Component(LaterClassicAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
        {
        }

        public override string ComponentName => "Tomb Raider V";
    }
}
