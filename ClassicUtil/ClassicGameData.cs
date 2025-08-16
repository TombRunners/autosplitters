using System.Collections.Generic;
using LiveSplit.ComponentUtil;
using Util;

namespace ClassicUtil;

public abstract class ClassicGameData : BaseOgGameData
{
    /// <summary>Used to locate the first in-memory saved level time.</summary>
    protected uint FirstLevelTimeAddress;

    /// <summary>The memory struct size of save game info; used to find subsequent level time addresses.</summary>
    protected uint LevelSaveStructSize;

    /// <summary>Sums level times based on <paramref name="completedLevels" /> and <paramref name="currentLevel" />.</summary>
    /// <param name="completedLevels">Levels completed</param>
    /// <param name="currentLevel"></param>
    public delegate ulong SumCompletedLevelTimesDelegate(IEnumerable<uint> completedLevels, uint? currentLevel);

    /// <summary>Allows a specific method to be assigned for use in summing completed levels' times.</summary>
    public SumCompletedLevelTimesDelegate SumLevelTimes;

    #region MemoryWatcherList Items

    /// <summary>Indicates if the game is on the title screen (main menu).</summary>
    /// <remarks>Goes back to 0 during demos, if applicable to the game.</remarks>
    public MemoryWatcher<bool> TitleScreen => (MemoryWatcher<bool>)Watchers?["TitleScreen"];

    /// <summary>Indicates if the current level is finished.</summary>
    /// <remarks>
    ///     1 while an end-level stats screen is active.
    ///     Remains 1 through FMVs that immediately follow a stats screen, i.e., until the next level starts.
    ///     Before most end-level in-game cutscenes, the value changes from 0 to 1 then back to 0 immediately.
    ///     Otherwise, the value is 0.
    /// </remarks>
    public MemoryWatcher<bool> LevelComplete => (MemoryWatcher<bool>)Watchers?["LevelComplete"];

    /// <summary>Gives the IGT value for the current level.</summary>
    public MemoryWatcher<uint> LevelTime => (MemoryWatcher<uint>)Watchers?["LevelTime"];

    /// <summary>Indicates the passport function chosen by the user.</summary>
    /// <remarks>
    ///     0 if <c>Load Game</c> was picked.
    ///     Changes to 1 when choosing <c>New Game</c> or <c>Save Game</c>.
    ///     The value stays 1 until the inventory is reopened.
    ///     The value stays 1 through opening FMVs if you pick <c>New Game</c> from Lara's Home.
    ///     The value is always 2 when using the <c>Exit To Title</c> or <c>Exit Game</c> pages.
    ///     Elsewhere, the value is 0.
    /// </remarks>
    public MemoryWatcher<uint> PickedPassportFunction => (MemoryWatcher<uint>)Watchers?["PickedPassportFunction"];

    #endregion
}