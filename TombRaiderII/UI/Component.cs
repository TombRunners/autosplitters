using System.Collections.Generic;
using LiveSplit.Model;

namespace TR2.UI
{
    class Component : MultiCounterComponent
    {
        private readonly Dictionary<int, List<string>> Names = new Dictionary<int, List<string>>()
        {
            { 0, new List<string> { "Small Med", "Large Med", "Shotgun" } },
            { 1, new List<string> { "Harpoons", "M16" } }
        };
        
        public Component(LiveSplitState state) : base(state)
        {
            CounterNames = Names;
            SplitCount = Names.Count;
        }
    }
}
