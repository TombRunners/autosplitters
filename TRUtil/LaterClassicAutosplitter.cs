using System;
using System.Collections.Generic;
using LiveSplit.Model;

namespace TRUtil
{
    public abstract class LaterClassicAutosplitter : BaseAutosplitter
    {      
        protected internal LaterClassicComponentSettings Settings = new LaterClassicComponentSettings();
        public LaterClassicGameData Data;

        protected static readonly LaterClassicProgressTracker ProgressTracker = new LaterClassicProgressTracker();

        protected internal static LaterClassicProgressEntry CurrentProgressEntry = new LaterClassicProgressEntry();

        public override TimeSpan? GetGameTime(LiveSplitState state)
            => null;

        public override bool ShouldReset(LiveSplitState state) 
            => LaterClassicGameData.GfLevelComplete.Current == 0 && LaterClassicGameData.Loading.Current;

        public override bool ShouldStart(LiveSplitState state) 
            => BaseGameData.Level.Current == 1 && !LaterClassicGameData.Loading.Current && LaterClassicGameData.Loading.Old;
        
        /// <summary>On <see cref="LiveSplitState.OnStart"/>, updates values.</summary>
        public virtual void OnStart() => ProgressTracker.Clear();

        /// <summary>On <see cref="LiveSplitState.OnSplit"/>, updates values.</summary>
        /// <param name="entry">Progress value(s) to add to <see cref="ProgressTracker"/></param>
        public virtual void OnSplit(LaterClassicProgressEntry entry) => ProgressTracker.Push(entry);

        /// <summary>On <see cref="LiveSplitState.OnUndoSplit"/>, updates values.</summary>
        public abstract void OnUndoSplit();

        public override void Dispose()
        {
            Data.OnGameFound -= Settings.SetGameVersion;
            Data = null;
        }
    }
}
