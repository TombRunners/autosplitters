using System.Collections.Generic;
using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace TR2.UI
{
    class Component : MultiCounterComponent
    {
        private readonly Dictionary<int, List<SimpleCounterSettings>> Tr2CounterSettings = new Dictionary<int, List<SimpleCounterSettings>>
        {
            // Level 1: Great Wall
            {
                0, 
                new List<SimpleCounterSettings>
                {
                    new SimpleCounterSettings("Key", target: 2),
                    new SimpleCounterSettings("Flare", target: 1),
                    new SimpleCounterSettings("Small Med", target: 2),
                    new SimpleCounterSettings("Large Med", target: 1),
                    new SimpleCounterSettings("Shotgun", target: 1),
                    new SimpleCounterSettings("Autos", target: 1),
                    new SimpleCounterSettings("Grenades", target: 3),
                    new SimpleCounterSettings("Secrets", target: 3)
                }
            },
            // Level 2: Venice
            { 
                1,
                new List<SimpleCounterSettings>()
                {
                    new SimpleCounterSettings("Key", target: 3),
                    new SimpleCounterSettings("Flare", target: 2),
                    new SimpleCounterSettings("Small Med", target: 4),
                    new SimpleCounterSettings("Large Med", target: 2),
                    new SimpleCounterSettings("Shotgun", target: 3),
                    new SimpleCounterSettings("Autos", target: 9),
                    new SimpleCounterSettings("Uzi", target: 2),
                    new SimpleCounterSettings("M16", target: 2),
                    new SimpleCounterSettings("Secrets", target: 3)
                }
            }
        };
        
        public Component(LiveSplitState state) : base(state)
        {
            CounterSettings = Tr2CounterSettings;
            NumSplits = CounterSettings.Count;
            RebuildCounters();
        }
    }
}
