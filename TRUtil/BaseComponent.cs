using System;                             // IDisposable
using LiveSplit.Model;                    // LiveSplitState
using LiveSplit.UI.Components;            // IComponent, LogicComponent, SettingsHelper
using LiveSplit.UI.Components.AutoSplit;  // AutoSplitComponent, IAutoSplitter

namespace TRUtil
{
    /// <summary>
    ///     Implementation of <see cref="AutoSplitComponent"/>.
    /// </summary>
    /// <remarks>
    ///     <see cref="AutoSplitComponent"/> is derived from <see cref="LogicComponent"/>,
    ///     which derives from <see cref="IComponent"/> and <see cref="IDisposable"/>.
    /// </remarks>
    public abstract class BaseComponent : AutoSplitComponent
    {
        private readonly BaseAutosplitter _splitter;
        private readonly LiveSplitState _state;

        /// <summary>
        ///     Initializes the component.
        /// </summary>
        /// <param name="autosplitter"><see cref="IAutoSplitter"/> implementation</param>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        protected BaseComponent(BaseAutosplitter autosplitter, LiveSplitState state) : base(autosplitter, state)
        {
            _splitter = autosplitter;
            _state = state;
            _state.OnSplit += (s, e) => _splitter?.OnSplit(BaseGameData.Level.Current);
            _state.OnStart += (s, e) => _splitter?.OnStart();
            _state.OnUndoSplit += (s, e) => _splitter?.OnUndoSplit();
        }

        public override void Dispose()
        {
            _state.OnSplit -= (s, e) => _splitter?.OnSplit(BaseGameData.Level.Current);
            _state.OnStart -= (s, e) => _splitter?.OnStart();
            _state.OnUndoSplit -= (s, e) => _splitter?.OnUndoSplit();
            _splitter?.Dispose();
        }
    }
}
