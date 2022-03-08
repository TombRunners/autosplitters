using System;
using System.Collections.Generic;
using LiveSplit.Model;

namespace TRUtil
{
    public abstract class LaterClassicAutosplitter : BaseAutosplitter
    {      
        protected internal LaterClassicComponentSettings Settings = new LaterClassicComponentSettings();
        public LaterClassicGameData Data;

        /// <summary>
        ///     The default implementation of <see cref="OnSplit"/> pushes <see cref="CurrentProgressEntry"/> onto this stack,
        ///     while the default implementation of <see cref="OnStart"/> clears this stack.
        /// </summary>
        /// <remarks>
        ///     An implementation of <see cref="OnUndoSplit"/> can pop the stack, using the returned <see cref="LaterClassicProgressEntry"/> 
        ///     to revert progress-related variables; this may prove helpful and/or necessary for backtracking/section split logic.
        /// </remarks>
        protected static readonly LaterClassicProgressTracker ProgressTracker = new LaterClassicProgressTracker();

        /// <summary>Used by <see cref="OnSplit"/> to update <see cref="ProgressTracker"/> with the progress up to the point being split.</summary>
        /// <remarks>The default implementation of <see cref="OnSplit"/> handles pushing and clearing <see cref="CurrentProgressEntry"/>.</remarks>
        protected static readonly LaterClassicProgressEntry CurrentProgressEntry = new LaterClassicProgressEntry();

        /// <summary>Populated by the default implementation of <see cref="OnStart"/>.</summary>
        /// <remarks>
        ///     <see cref="TicksAtStartOfRun"/> is used in <see cref="GetGameTime(LiveSplitState)"/> to subtract from <see cref="LaterClassicGameData.GameTimer"/>.
        ///     The subtraction is necessary for ILs/section runs starting on any level besides the first to get an accurate IGT from the start of the run.
        /// </remarks>
        private static ulong TicksAtStartOfRun;

        public override TimeSpan? GetGameTime(LiveSplitState state) {
            if (!LaterClassicGameData.GameTimer.Changed)
                return null;
            
            return TimeSpan.FromSeconds(BaseGameData.LevelTimeAsDouble(LaterClassicGameData.GameTimer.Current - TicksAtStartOfRun));
        }

        public override bool ShouldReset(LiveSplitState state)
        {
            bool loadingIntoMainMenu = LaterClassicGameData.GfLevelComplete.Current == 0 && BaseGameData.Level.Current == 0 && LaterClassicGameData.Loading.Current;
            // Checking the old level number ensures that someone re-opening the game (perhaps after a crash or Alt+F4) does not Reset.
            // This works because when loading non-test/demo versions of the games, the level variable initializes as 0 before the main menu load is called.
            bool comingFromALevel = BaseGameData.Level.Old != 0;
            return loadingIntoMainMenu && comingFromALevel;
        }

        public override bool ShouldStart(LiveSplitState state)
        {
            bool loadingScreenJustEnded = !LaterClassicGameData.Loading.Current && LaterClassicGameData.Loading.Old;
            // GfLevelComplete will only be 1 if New Game is clicked; loading a save does not set GfLevelComplete to that level's value.
            // Thus, the GfLevelComplete check will prevent Starts from someone loading into the first level.
            bool loadedIntoFirstLevelFromTheMainMenu = LaterClassicGameData.GfLevelComplete.Old == 1;
            return loadingScreenJustEnded && loadedIntoFirstLevelFromTheMainMenu;
        }

        /// <summary>On <see cref="LiveSplitState.OnStart"/>, updates values.</summary>
        public virtual void OnStart() {
            ProgressTracker.Clear();
            TicksAtStartOfRun = (BaseGameData.Level.Current == 1) ? 0 : LaterClassicGameData.GameTimer.Old;
        }

        /// <summary>On <see cref="LiveSplitState.OnSplit"/>, updates values.</summary>
        /// <param name="entry">Progress value(s) to add to <see cref="ProgressTracker"/></param>
        public virtual void OnSplit() {
            ProgressTracker.Push(CurrentProgressEntry);
            CurrentProgressEntry.Clear();
        }

        /// <summary>On <see cref="LiveSplitState.OnUndoSplit"/>, updates values.</summary>
        public abstract void OnUndoSplit();

        public override void Dispose()
        {
            Data.OnGameFound -= Settings.SetGameVersion;
            Data = null;
        }
    }
}
