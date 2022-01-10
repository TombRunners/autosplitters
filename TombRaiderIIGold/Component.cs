using LiveSplit.Model;                    // LiveSplitState
using TRUtil;                             // ClassicAutosplitter, ClassicComponent

namespace TR2Gold
{
    /// <summary>Implementation of <see cref="ClassicComponent"/>.</summary>
    internal class Component : ClassicComponent
    {
        /// <inheritdoc/>
        public Component(ClassicAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
        {
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     This is the text that is displayed in the white area of LiveSplit's Layout Editor.
        /// </remarks>
        public override string ComponentName => "Tomb Raider II Gold";
    }
}
