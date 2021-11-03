using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using TRUtil;

namespace TR2.UI
{
    internal class AutoMultiCounter : IAutoMultiCounter, IDisposable
    {
        private int _split = 0;
        
        private bool RunIsActive(LiveSplitState state) => state.CurrentPhase == TimerPhase.Running;

        internal readonly ComponentSettings Settings = new ComponentSettings();
        internal GameData GameData = new GameData();

        public AutoMultiCounter() => GameData.OnGameFound += Settings.SetGameVersion;

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
            if (BaseGameData.Level.Current == (uint)Tr2Level.GreatWall && BaseGameData.Health.Old > 0 && BaseGameData.Health.Current == 0)
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

            return state.CurrentSplitIndex == 0
                ? new HashSet<int> { 0, 1, 2, 3, 4, 5, 6, 7 }
                : new HashSet<int>();
        }

        public Dictionary<int, int> ShouldSet(LiveSplitState state) => new Dictionary<int, int>();

        public void Dispose()
        {
            GameData.OnGameFound -= Settings.SetGameVersion;
            GameData = null;
        }
    }
}
