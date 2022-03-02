using System;
using System.Collections.Generic;
using LiveSplit.Model;
using LiveSplit.UI.Components.AutoSplit;

namespace TRUtil
{
    public abstract class BaseAutosplitter : IAutoSplitter, IDisposable
    {
        /// <summary>Used to size CompletedLevels.</summary>
        protected int LevelCount = 0;
        
        /// <summary>Used to decide when to split and which level time addresses should be read from memory.</summary>
        protected readonly List<uint> CompletedLevels = new List<uint>();

        /// <summary>Determines the IGT.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns>IGT as a <see cref="TimeSpan"/> if available, otherwise <see langword="null"/></returns>
        public abstract TimeSpan? GetGameTime(LiveSplitState state);

        /// <summary>Determines if IGT pauses when the game is quit or <see cref="GetGameTime"/> returns <see langword="null"/></summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> when IGT should be paused during the conditions, <see langword="false"/> otherwise</returns>
        public bool IsGameTimePaused(LiveSplitState state) => true;

        /// <summary>Determines if the timer should split.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should split, <see langword="false"/> otherwise</returns>
        public abstract bool ShouldSplit(LiveSplitState state);

        /// <summary>Determines if the timer should reset.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should reset, <see langword="false"/> otherwise</returns>
        public abstract bool ShouldReset(LiveSplitState state);

        /// <summary>Determines if the timer should start.</summary>
        /// <param name="state"><see cref="LiveSplitState"/> passed by LiveSplit</param>
        /// <returns><see langword="true"/> if the timer should start, <see langword="false"/> otherwise</returns>
        public abstract bool ShouldStart(LiveSplitState state);

        /// <summary>On <see cref="LiveSplitState.OnStart"/>, updates values.</summary>
        public virtual void OnStart() => CompletedLevels.Clear();

        /// <summary>On <see cref="LiveSplitState.OnSplit"/>, updates values.</summary>
        /// <param name="completedLevel">What to add to <see cref="CompletedLevels"/></param>
        public virtual void OnSplit(uint completedLevel) => CompletedLevels.Add(completedLevel);

        /// <summary>On <see cref="LiveSplitState.OnUndoSplit"/>, updates values.</summary>
        public virtual void OnUndoSplit() => CompletedLevels.RemoveAt(CompletedLevels.Count - 1);

        public abstract void Dispose();
    }
}
