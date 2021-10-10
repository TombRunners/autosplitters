using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;

namespace LiveSplit.UI.Components
{
    public class MultiCounterComponent : IComponent
    {
        readonly EventLog log = new EventLog("Application")
        {
            Source = "LiveSplit"
        };

        IAutoMultiCounter AutoMultiCounter { get; set; }

        #region IComponent implementations

        public string ComponentName => "Multi-Counter";
        public IDictionary<string, Action> ContextMenuControls => null;
        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumHeight => InternalComponent.MinimumHeight;

        public XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);

        public Control GetSettingsControl(LayoutMode mode) => Settings;

        public void SetSettings(XmlNode settings)
        {
            Settings.SetSettings(settings);
            RebuildCounters();
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            Prepare(state);
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            Prepare(state);
            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                var countersToIncrement = AutoMultiCounter.ShouldIncrement(state);
                foreach(int index in countersToIncrement)
                    IncrementCounter(index);
                
                var countersToSet = AutoMultiCounter.ShouldSet(state);
                foreach(KeyValuePair<int, int> entry in countersToSet)
                    SetCounter(entry.Key, entry.Value);

                var countersToReset = AutoMultiCounter.ShouldReset(state);
                foreach(int index in countersToReset)
                    ResetCounter(index);
            }

            if (invalidator != null)
                InternalComponent.Update(invalidator, state, width, height, mode);
        }

        #endregion IComponent implementations

        public ComponentRendererComponent InternalComponent { get; protected set; } = new ComponentRendererComponent();
        protected IList<IComponent> Components { get; set; }
        protected IList<SimpleCounterComponent> CounterComponents { get; set; }
        protected int NumSplits { get; set; }
        protected int CountersInSplit { get; set; }
        protected IDictionary<int, List<SimpleCounterSettings>> CounterSettings { get; set;}
        protected MultiCounterComponentSettings Settings { get; set; } = new MultiCounterComponentSettings();
        protected int ScrollOffset { get; set; }
        protected LiveSplitState State { get; set; }

        public MultiCounterComponent(LiveSplitState state)
        {
            State = state;
            state.OnStart += OnStart;
            state.OnReset += OnReset;
            state.OnSplit += OnSplit;
        }

        public void IncrementCounter(int counterIndex) => CounterComponents[counterIndex].Increment();
        public void SetCounter(int counterIndex, int value) => CounterComponents[counterIndex].Value = value;
        public void ResetCounter(int counterIndex) => CounterComponents[counterIndex].Value = 0;

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

            var currentSplitCounterSettings = CounterSettings[currentSplit]; 
            for (var i = 0; i < CountersInSplit; ++i)
            {
                var counterComponent = new SimpleCounterComponent(Settings, currentSplitCounterSettings[i]);
                Components.Add(counterComponent);
                CounterComponents.Add(counterComponent);
                if (i < CountersInSplit - 1)
                    Components.Add(new ThinSeparatorComponent());
            }
            log.WriteEntry($"Finished building {CounterComponents.Count} counters for split #{currentSplit}.");
        }

        private void Prepare(LiveSplitState state)
        {
            int currentSplit = state.CurrentSplitIndex;
            if (currentSplit == -1)  // Run hasn't started, show first level/split counters.
                currentSplit = 0;
            var currentSplitCounterSettings = CounterSettings[currentSplit];
            var splitCounterCount = currentSplitCounterSettings.Count;
            if (NumSplits != CounterSettings.Count || CountersInSplit != splitCounterCount)
            {
                NumSplits = CounterSettings.Count;
                CountersInSplit = splitCounterCount;
                RebuildCounters();
            }
        }

        void OnSplit(object sender, EventArgs e) => RebuildCounters();

        void OnReset(object sender, TimerPhase e) => RebuildCounters();

        void OnStart(object sender, EventArgs e) => RebuildCounters();

        public void Dispose() {}
    }
}
