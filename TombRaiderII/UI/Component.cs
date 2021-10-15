using System.Collections.Generic;
using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace TR2.UI
{
    class Component : MultiCounterComponent
    {
        private readonly Dictionary<int, List<TargetCounterSettings>> Tr2CounterSettings = new Dictionary<int, List<TargetCounterSettings>>
        {
            // Level 1: Great Wall
            {
                0, 
                new List<TargetCounterSettings>
                {
                    new TargetCounterSettings("Key", target: 2),
                    new TargetCounterSettings("Flare", target: 1),
                    new TargetCounterSettings("Small Med", target: 2),
                    new TargetCounterSettings("Large Med", target: 1),
                    new TargetCounterSettings("Shotgun", target: 1),
                    new TargetCounterSettings("Autos", target: 1),
                    new TargetCounterSettings("Grenades", target: 3),
                    new TargetCounterSettings("Secrets", target: 3)
                }
            },
            // Level 2: Venice
            { 
                1,
                new List<TargetCounterSettings>()
                {
                    new TargetCounterSettings("Key", target: 3),
                    new TargetCounterSettings("Flare", target: 2),
                    new TargetCounterSettings("Small Med", target: 4),
                    new TargetCounterSettings("Large Med", target: 2),
                    new TargetCounterSettings("Shotgun", target: 3),
                    new TargetCounterSettings("Autos", target: 9),
                    new TargetCounterSettings("Uzi", target: 2),
                    new TargetCounterSettings("M16", target: 2),
                    new TargetCounterSettings("Secrets", target: 3)
                }
            }
        };

        internal GameMemory GameMemory = new GameMemory();
        
        public Component(LiveSplitState state) : base(state)
        {
            CounterSettings = Tr2CounterSettings;
            PreviousNumSplits = CounterSettings.Count;
        }
    }
}
