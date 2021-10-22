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

        public virtual void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (invalidator != null)
                InternalComponent.Update(invalidator, state, width, height, mode);
        }

        #endregion IComponent implementations

        IAutoMultiCounter AutoMultiCounter { get; set; }
        public ComponentRendererComponent InternalComponent { get; protected set; } = new ComponentRendererComponent();
        protected IList<IComponent> Components { get; set; }
        protected IList<NamedTargetCounterComponent> CounterComponents { get; set; }
        protected int CurrentCountersIndex { get; set; } = 0;
        protected int PreviousNumSplits { get; set; }
        protected int CountersInSplit { get; set; }
        protected IDictionary<int, List<NamedTargetCounterSettings>> CounterSettings { get; set;}
        protected MultiCounterComponentSettings Settings { get; set; } = new MultiCounterComponentSettings();
        protected int ScrollOffset { get; set; }
        protected LiveSplitState State { get; set; }

        public MultiCounterComponent(LiveSplitState state)
        {
            State = state;
            state.OnStart += OnStart;
            state.OnReset += OnReset;
            state.OnSplit += OnSplit;

            Settings.CounterLayoutChanged += OnCounterLayoutChanged;
        }

        public void DecrementCounter(int counterIndex) => CounterComponents[counterIndex].Counter.Decrement();
        public void IncrementCounter(int counterIndex) => CounterComponents[counterIndex].Counter.Increment();
        public void SetCounter(int counterIndex, int value) => CounterComponents[counterIndex].Counter.SetCount(value);
        public void ResetCounter(int counterIndex) => CounterComponents[counterIndex].Counter.Reset();

        void OnCounterLayoutChanged(object sender, EventArgs e) => RebuildCounters();
        void OnSplit(object sender, EventArgs e) => RebuildCounters();
        void OnReset(object sender, TimerPhase e) => RebuildCounters();
        void OnStart(object sender, EventArgs e) => RebuildCounters();

        public void RebuildCounters()
        {
            Components = new List<IComponent>();
            CounterComponents = new List<NamedTargetCounterComponent>();
            InternalComponent.VisibleComponents = Components;
                        
            int currentSplit = State.CurrentSplitIndex;
            if (currentSplit == -1)
                currentSplit = 0;  // Run hasn't started, show first level/split counters.

            var currentSplitCounterSettings = CounterSettings[currentSplit]; 
            for (var i = 0; i < CountersInSplit; ++i)
            {
                var counterComponent = new NamedTargetCounterComponent(Settings, currentSplitCounterSettings[i], i);
                Components.Add(counterComponent);
                CounterComponents.Add(counterComponent);
                if (i < CountersInSplit - 1)
                {
                    if (Settings.UseThinSeparators)
                        Components.Add(new ThinSeparatorComponent());
                    else
                        Components.Add(new SeparatorComponent());
                }
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
            if (PreviousNumSplits != CounterSettings.Count || CountersInSplit != splitCounterCount)
            {
                PreviousNumSplits = CounterSettings.Count;
                CountersInSplit = splitCounterCount;
                RebuildCounters();
            }
        }

        public void Dispose() {}
    }
}
