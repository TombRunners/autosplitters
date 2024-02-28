using System.Collections.Generic;
using System.Linq;

namespace TR123;

/// <summary>Stats related to a game with <paramref name="levelCount" /> levels.</summary>
/// <param name="levelCount">Number of levels in the game</param>
public readonly struct GameStats(int levelCount)
{
    private List<LevelStats> LevelStatistics { get; } = new(levelCount);

    /// <summary>The number of levels in the game.</summary>
    public readonly int LevelCount = levelCount;

    /// <summary><see langword="true" /> if all levels have been added into the backing list; <see langword="false" /> otherwise.</summary>
    public bool GameComplete => LevelStatistics.Count == LevelCount;

    public IEnumerable<LevelStats> LevelStats => LevelStatistics;

    /// <summary>Adds <paramref name="stats" /> to the backing list.</summary>
    /// <param name="stats">Stats to add</param>
    public void AddLevelStats(LevelStats stats)
    {
        if (GameComplete)
            return;

        if (LevelStatistics.Select(static stats => stats.LevelNumber).Contains(stats.LevelNumber))
            return;

        LevelStatistics.Add(stats);
    }

    /// <summary>Removes the most recently added <see cref="LevelStats" /> from the backing list.</summary>
    public void RemoveLevelStats() => LevelStatistics.RemoveAt(LevelStatistics.Count - 1);

    /// <summary>Clears the backing list of all <see cref="LevelStats" />.</summary>
    public void Clear() => LevelStatistics.Clear();
}