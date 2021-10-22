using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;

namespace TR2.UI
{
    internal class AutoMultiCounter : IAutoMultiCounter, IDisposable
    {
        private int _split = 0;
        
        private bool RunIsActive(LiveSplitState state) => state.CurrentPhase == TimerPhase.Running;

        internal readonly ComponentSettings Settings = new ComponentSettings();
        internal GameMemory GameMemory = new GameMemory();

        public AutoMultiCounter() => GameMemory.OnGameFound += Settings.SetGameVersion;

        public bool ShouldAdvance(LiveSplitState state)
        {
            if (_split == state.CurrentSplitIndex)
                return false;

            _split = state.CurrentSplitIndex;
            return true;
        }

        public HashSet<int> ShouldDecrement(LiveSplitState state) => new HashSet<int>();
        
        public HashSet<int> ShouldIncrement(LiveSplitState state)
        {
            if (GameMemory.Data.Level.Current == Level.GreatWall && GameMemory.Data.Health.Old > 0 && GameMemory.Data.Health.Current == 0)
                return new HashSet<int>() ;
            else
                return new HashSet<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
        }

        public HashSet<int> ShouldReset(LiveSplitState state) 
        {
            if (RunIsActive(state))
            {
                return new HashSet<int>();
            }
            if (state.CurrentSplitIndex == 0)
            {
                return new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
            }
            return new HashSet<int>();
        }

        public Dictionary<int, int> ShouldSet(LiveSplitState state) => new Dictionary<int, int>();

        public void Dispose()
        {
            GameMemory.OnGameFound -= Settings.SetGameVersion;
            GameMemory = null;
        }
    }
}
