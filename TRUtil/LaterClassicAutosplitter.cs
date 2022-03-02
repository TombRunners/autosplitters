using System;
using LiveSplit.Model;

namespace TRUtil
{
    public abstract class LaterClassicAutosplitter : BaseAutosplitter
    {
        protected internal LaterClassicComponentSettings Settings = new LaterClassicComponentSettings();
        public LaterClassicGameData Data;

        public override TimeSpan? GetGameTime(LiveSplitState state)
            => null;

        public override bool ShouldReset(LiveSplitState state) 
            => LaterClassicGameData.GfLevelComplete.Current == 0 && LaterClassicGameData.Loading.Current;

        public override bool ShouldStart(LiveSplitState state) 
            => BaseGameData.Level.Current == 1 && !LaterClassicGameData.Loading.Current && LaterClassicGameData.Loading.Old;
        
        public override void Dispose()
        {
            Data.OnGameFound -= Settings.SetGameVersion;
            Data = null;
        }
    }
}
