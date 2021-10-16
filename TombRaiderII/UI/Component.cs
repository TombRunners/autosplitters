using System.Collections.Generic;
using LiveSplit.Model;
using LiveSplit.UI.Components;

namespace TR2.UI
{
    class Component : MultiCounterComponent
    {
        private readonly Dictionary<int, List<NamedTargetCounterSettings>> Tr2CounterSettings = new Dictionary<int, List<NamedTargetCounterSettings>>
        {
            // Level 1: Great Wall
            {
                0, 
                new List<NamedTargetCounterSettings>
                {
                    new NamedTargetCounterSettings("Key", target: 2, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Flare", target: 1, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Small Med", target: 2, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Large Med", target: 1, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Shotgun", target: 1, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Autos", target: 1, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Grenades", target: 0, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Secrets", target: 3, targetCondition: TargetCondition.GE)
                }
            },
            // Level 2: Venice
            { 
                1,
                new List<NamedTargetCounterSettings>()
                {
                    new NamedTargetCounterSettings("Key", target: 3, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Flare", target: 2, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Small Med", target: 4, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Large Med", target: 2, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Shotgun", target: 3, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Autos", target: 9, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Uzi", target: 2, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("M16", target: 2, targetCondition: TargetCondition.GE),
                    new NamedTargetCounterSettings("Secrets", target: 3, targetCondition: TargetCondition.GE)
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
