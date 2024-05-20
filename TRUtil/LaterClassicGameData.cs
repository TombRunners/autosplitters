using LiveSplit.ComponentUtil;

namespace TRUtil;

public abstract class LaterClassicGameData : BaseGameData
{
    #region MemoryWatcherList Items

    /// <summary>Indicates the next level to be loaded.</summary>
    /// <remarks>
    ///     During any gameplay, the value is 0.
    ///     When certain triggers are hit, it will be set to whatever level is loaded next.
    ///     The values 10 and 29 do not correspond to a level and will instead load the following real level.
    ///     The value 39 is coded to start the end-game credits.
    /// </remarks>
    public MemoryWatcher<uint> GfLevelComplete => (MemoryWatcher<uint>)Watchers?["GfLevelComplete"];

    /// <summary>Represents a "global" timer tracking ticks since New Game. It is saved/reloaded. </summary>
    public MemoryWatcher<uint> GameTimer => (MemoryWatcher<uint>)Watchers?["GameTimer"];

    /// <summary>Indicates if a loading screen is active.</summary>
    public MemoryWatcher<bool> Loading => (MemoryWatcher<bool>)Watchers?["Loading"];

    #endregion
}