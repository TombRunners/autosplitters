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
        {
            bool loadingIntoMainMenu = LaterClassicGameData.GfLevelComplete.Current == 0 && LaterClassicGameData.Loading.Current;
            // Checking the old level number ensures that someone Alt+F4-ing or otherwise opening the game (perhaps after a crash)
            // will not cause ShouldReset to return true, losing their LiveSplit run progress.
            // This works because when loading non-test/demo games, the level value initializes as 0 before the main menu load is called.
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
