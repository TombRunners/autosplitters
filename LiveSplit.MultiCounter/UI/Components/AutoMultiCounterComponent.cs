using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public abstract class AutoMultiCounterComponent : LogicComponent
    {
        protected CounterModel Model { get; set; }
        protected IAutoMultiCounter AutoMultiCounter { get; set; }

        protected AutoMultiCounterComponent(IAutoMultiCounter autoMultiCounter, LiveSplitState state)
        {
            Model = new CounterModel() { State = state };
            AutoMultiCounter = autoMultiCounter;

            state.OnPause += OnPause;
            state.OnReset += OnReset;
            state.OnResume += OnResume;
            state.OnScrollDown += OnScrollDown;
            state.OnScrollUp += OnScrollUp;
            state.OnSkipSplit += OnSkipSplit;
            state.OnStart += OnStart;
            state.OnUndoAllPauses += OnUndoAllPauses;
            state.OnUndoSplit += OnUndoSplit;
        }

        void OnPause(object sender, EventArgs e) => Model.Pause();
        void OnReset(object sender, TimerPhase timerPhase) => Model.Reset(timerPhase);
        void OnResume(object sender, EventArgs e) => Model.Resume();
        void OnScrollDown(object sender, EventArgs e) => Model.ScrollDown();
        void OnScrollUp(object sender, EventArgs e) => Model.ScrollUp();
        void OnSkipSplit(object sender, EventArgs e) => Model.SkipSplit();
        void OnSplit(object sender, EventArgs e) => Model.Split();
        void OnStart(object sender, EventArgs e) => Model.Start();
        void OnUndoAllPauses(object sender, EventArgs e) => Model.UndoAllPauses();
        void OnUndoSplit(object sender, EventArgs e) => Model.UndoSplit();

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (state.CurrentPhase == TimerPhase.Running || state.CurrentPhase == TimerPhase.Paused)
            {
                if (AutoMultiCounter.ShouldAdvance(state))
                {
                    Model.Advance();
                    return;
                }

                var countersToDecrement = AutoMultiCounter.ShouldDecrement(state);
                Model.DecrementCounters(countersToDecrement);

                var countersToIncrement = AutoMultiCounter.ShouldIncrement(state);
                Model.IncrementCounters(countersToIncrement);

                var countersToReset = AutoMultiCounter.ShouldReset(state);
                Model.ResetCounters(countersToReset);

                var countersToSet = AutoMultiCounter.ShouldSet(state);
                Model.SetCounters(countersToSet);
            }
        }
    }
}
