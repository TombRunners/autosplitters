using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace TR2.UI
{
    public class MultiCounterComponent : IComponent
    {
        readonly EventLog log = new EventLog("Application")
        {
            Source = "LiveSplit"
        };

        public ComponentRendererComponent InternalComponent { get; protected set; } = new ComponentRendererComponent();
        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumHeight => InternalComponent.MinimumHeight;

        protected IList<IComponent> Components { get; set; }
        protected IList<SimpleCounterComponent> CounterComponents { get; set; }
        protected int Splits { get; set; }
        protected int CountersInSplit { get; set; }
        protected Dictionary<int, List<string>> CounterNames { get; set; }

        protected MultiCounterComponentSettings Settings { get; set; } = new MultiCounterComponentSettings();

        protected int ScrollOffset { get; set; }

        protected LiveSplitState State { get; set; }

        public string ComponentName => "Multi-Counter";

        public IDictionary<string, Action> ContextMenuControls => null;

        public MultiCounterComponent(LiveSplitState state)
        {
            State = state;
            state.OnStart += OnStart;
            state.OnReset += OnReset;
            state.OnSplit += OnSplit;
        }

        public void IncrementCounter(int counterIndex) => CounterComponents[counterIndex].Increment();

        public void RebuildCounters()
        {
            Components = new List<IComponent>();
            CounterComponents = new List<SimpleCounterComponent>();
            InternalComponent.VisibleComponents = Components;
                        
            int currentSplit = State.CurrentSplitIndex;
            if (currentSplit == -1)
            {
                // Run hasn't started, show first level/split counters.
                log.WriteEntry($"CurrentSplitIndex was -1, changing to 0.");
                currentSplit = 0;
            }

            List<string> currentSplitCounterNames = CounterNames[currentSplit];
            log.WriteEntry($"Building level/split #{currentSplit}: {string.Join(", ", currentSplitCounterNames)}");     
            for (var i = 0; i < CountersInSplit; ++i)
            {
                var counterComponent = new SimpleCounterComponent(Settings, currentSplitCounterNames[i]);
                Components.Add(counterComponent);
                CounterComponents.Add(counterComponent);
                if (i < CountersInSplit - 1)
                    Components.Add(new ThinSeparatorComponent());
            }
        }

        private void Prepare(LiveSplitState state)
        {
            int currentSplit = state.CurrentSplitIndex;
            if (currentSplit == -1)  // Run hasn't started, show first level/split counters.
                currentSplit = 0;
            log.WriteEntry($"In Prepare: CounterNames.Count = {CounterNames.Count}");
            var currentSplitCounterNames = CounterNames[currentSplit];
            var splitCounterCount = currentSplitCounterNames.Count;
            if (Splits != CounterNames.Count || CountersInSplit != splitCounterCount)
            {
                Splits = CounterNames.Count;
                CountersInSplit = splitCounterCount;
                RebuildCounters();
            }
        }

        void OnSplit(object sender, EventArgs e) => RebuildCounters();

        void OnReset(object sender, TimerPhase e) => RebuildCounters();

        void OnStart(object sender, EventArgs e) => RebuildCounters();

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            Prepare(state);
            log.WriteEntry("DrawVertical Prepare successful.");
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            Prepare(state);
            log.WriteEntry("DrawHorizontal Prepare successful.");
            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);

        public Control GetSettingsControl(LayoutMode mode) => Settings;

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
            RebuildCounters();
        }        

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (invalidator != null)
                InternalComponent.Update(invalidator, state, width, height, mode);
        }

        public void Dispose() {}
    }
}
