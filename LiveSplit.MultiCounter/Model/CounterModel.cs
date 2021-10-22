using LiveSplit.Model.Input;
using System;
using System.Collections.Generic;

namespace LiveSplit.Model
{
    public class CounterModel
    {
        public LiveSplitState State { get; set; }

        public event EventHandler OnSplit;
        public event EventHandler OnUndoSplit;
        public event EventHandler OnSkipSplit;
        public event EventHandler OnStart;
        public event EventHandlerT<TimerPhase> OnReset;
        public event EventHandler OnPause;
        public event EventHandler OnUndoAllPauses;
        public event EventHandler OnResume;
        public event EventHandler OnScrollUp;
        public event EventHandler OnScrollDown;

        public event EventHandler OnAdvance;
        public event EventHandlerT<HashSet<int>> OnDecrementCounters;
        public event EventHandlerT<HashSet<int>> OnIncrementCounters;
        public event EventHandlerT<HashSet<int>> OnResetCounters;
        public event EventHandlerT<Dictionary<int, int>> OnSetCounters;

        public virtual void Start() => OnStart?.Invoke(this, null);
        public virtual void Split() => OnSplit?.Invoke(this, null);
        public virtual void SkipSplit() => OnSkipSplit?.Invoke(this, null);
        public virtual void UndoSplit() => OnUndoSplit?.Invoke(this, null);
        public virtual void Reset(TimerPhase timerPhase) => OnReset?.Invoke(this, timerPhase);
        public virtual void Resume() => OnResume?.Invoke(this, null);
        public virtual void Pause() => OnPause?.Invoke(this, null);
        public virtual void UndoAllPauses() => OnUndoAllPauses?.Invoke(this, null);
        public virtual void ScrollUp() => OnScrollUp?.Invoke(this, null);
        public virtual void ScrollDown() => OnScrollDown?.Invoke(this, null);


        public virtual void Advance() => OnAdvance?.Invoke(this, null);
        public virtual void DecrementCounters(HashSet<int> counters) => OnDecrementCounters?.Invoke(this, counters);
        public virtual void IncrementCounters(HashSet<int> counters) => OnIncrementCounters?.Invoke(this, counters);
        public virtual void ResetCounters(HashSet<int> counters) => OnResetCounters?.Invoke(this, counters);
        public virtual void SetCounters(Dictionary<int, int> counters) => OnSetCounters?.Invoke(this, counters);
    }
}
