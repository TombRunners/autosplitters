using LiveSplit.ComponentUtil;
using Util;

namespace LaterClassicUtil;

public abstract class LaterClassicGameData : BaseOgGameData
{
    #region MemoryWatcherList Items

    /// <summary>Used to determine when to reset many of the game's global variables.</summary>
    /// <remarks>
    ///     1 during the main menu and when loading/starting game from the main menu, and when loading/watching credits; otherwise, 0.
    /// </remarks>
    public MemoryWatcher<bool> GfInitializeGame => (MemoryWatcher<bool>)Watchers?["GfInitializeGame"];

    /// <summary>Indicates the next level to be loaded.</summary>
    /// <remarks>
    ///     During any gameplay, the value is 0.
    ///     When certain triggers are hit, it will be set to whatever level is loaded next.
    ///     The values 10 and 29 do not correspond to a level and will instead load the following real level.
    ///     The value 39 is coded to start the end-game credits.
    /// </remarks>
    public MemoryWatcher<uint> GfLevelComplete => (MemoryWatcher<uint>)Watchers?["GfLevelComplete"];

    /// <summary>Represents a "global" timer tracking ticks since New Game. It is saved/reloaded.</summary>
    public MemoryWatcher<uint> GameTimer => (MemoryWatcher<uint>)Watchers?["GameTimer"];

    /// <summary>Indicates that a menu is active outside the title screen.</summary>
    /// <remarks>0 during the main menu, credits, and gameplay; 1 when in a save, load, key, or inventory menu.</remarks>
    public MemoryWatcher<long> InventoryActive => (MemoryWatcher<long>)Watchers?["InventoryActive"];

    /// <summary>Indicates if a loading screen is active.</summary>
    public MemoryWatcher<bool> Loading => (MemoryWatcher<bool>)Watchers?["Loading"];

    /// <summary>Increments by 1 whenever a secret is triggered.</summary>
    public MemoryWatcher<byte> Secrets => (MemoryWatcher<byte>) Watchers?["Secrets"];

    #endregion
}