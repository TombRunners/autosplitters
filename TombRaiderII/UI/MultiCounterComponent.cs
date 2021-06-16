using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace TR2.UI
{
    class MultiCounterComponent : IComponent
    {
        public ComponentRendererComponent InternalComponent { get; protected set; } = new ComponentRendererComponent();

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        protected IList<IComponent> Components { get; set; }
        protected IList<SimpleCounterComponent> CounterComponents { get; set; }
        protected int SplitCount { get; set; }
        protected int SplitCounterCount { get; set; }
        protected Dictionary<int, List<string>> CounterNames { get; set; }

        protected MultiCounterComponentSettings Settings { get; set; } = new MultiCounterComponentSettings();

        protected int ScrollOffset { get; set; }

        protected LiveSplitState State { get; set; }

        public string ComponentName => "Multi-Counter";

        public float VerticalHeight => InternalComponent.VerticalHeight;

        public float MinimumWidth => InternalComponent.MinimumWidth;

        public float HorizontalWidth => InternalComponent.HorizontalWidth;

        public float MinimumHeight => InternalComponent.MinimumHeight;

        public IDictionary<string, Action> ContextMenuControls => null;

        public MultiCounterComponent(LiveSplitState state)
        {
            State = state;
            state.OnStart += OnStart;
            state.OnReset += OnReset;
            state.OnSplit += OnSplit;
            RebuildCounters();
        }

        public void IncrementCounter(int counterIndex) => CounterComponents[counterIndex].Increment();

        private void RebuildCounters()
        {
            Components = new List<IComponent>();
            CounterComponents = new List<SimpleCounterComponent>();
            InternalComponent.VisibleComponents = Components;
            int currentSplit = State.CurrentSplitIndex;
            EventLog log = new EventLog("Application")
            {
                Source = "LiveSplit"
            };
            log.WriteEntry($"CurrentSplitIndex = {currentSplit}");
            if (currentSplit != -1)
            {
                
                var currentSplitCounterNames = CounterNames[currentSplit];
                for (var i = 0; i < SplitCounterCount; ++i)
                {
                    var counterComponent = new SimpleCounterComponent(Settings, currentSplitCounterNames[i]);
                    Components.Add(counterComponent);
                    CounterComponents.Add(counterComponent);
                    if (i < SplitCounterCount - 1)
                        Components.Add(new ThinSeparatorComponent());
                }
            }
        }

        private void Prepare(LiveSplitState state)
        {
            int currentSplit = state.CurrentSplitIndex;
            var currentSplitCounterNames = CounterNames[currentSplit];
            var splitCounterCount = currentSplitCounterNames.Count;
            if (SplitCount != CounterNames.Count || SplitCounterCount != splitCounterCount)
            {
                SplitCount = CounterNames.Count;
                SplitCounterCount = splitCounterCount;
                RebuildCounters();
            }
        }

        void OnSplit(object sender, EventArgs e) => RebuildCounters();

        void OnReset(object sender, TimerPhase e) => RebuildCounters();

        void OnStart(object sender, EventArgs e) => RebuildCounters();

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

        public void Dispose() { }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode;
    }
}
