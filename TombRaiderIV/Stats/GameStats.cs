using System.Collections.Generic;
using System.Linq;

namespace TR4;

/// <summary>Collection of level stats for a game.</summary>
public record GameStats
{
    private readonly Stack<LevelStats> _levelStatistics = new();

    /// <summary>Accessor for the backing stack.</summary>
    private IEnumerable<LevelStats> LevelStats => _levelStatistics;

    /// <summary>Pushes <paramref name="stats" /> to the backing stack.</summary>
    /// <param name="stats">Stats to add</param>
    public bool AddLevelStats(LevelStats stats)
    {
        _levelStatistics.Push(stats);
        return true;
    }

    /// <summary>Pops the most recently added <see cref="LevelStats" /> from the backing stack.</summary>
    public LevelStats PopLevelStats()
    {
        LevelStats stats = _levelStatistics.Pop();
        return stats;
    }

    public int Count => _levelStatistics.Count;

    /// <summary>Clears the backing stack of all <see cref="LevelStats" />.</summary>
    public void Clear() => _levelStatistics.Clear();

    /// <summary>Determines if the stats for <paramref name="levelId" /> are already present.</summary>
    /// <param name="levelId">Level ID</param>
    /// <param name="direction">Direction of level transition</param>
    /// <returns><see langword="true" /> if the level is present in the backing stack; <see langword="false" /> otherwise</returns>
    public bool LevelWasSplit(ulong levelId, TransitionDirection direction)
        => LevelStats.Any(stats => stats.LevelId == levelId && stats.Direction == direction && !stats.Ignored);

    /// <summary>Determines if the stats for <paramref name="levelId" /> are already present.</summary>
    /// <param name="levelId">Level ID</param>
    /// <param name="direction">Direction of level transition</param>
    /// <returns>The number of times the level was completed</returns>
    public int LevelSplitCount(ulong levelId, TransitionDirection direction)
        => LevelStats.Count(stats => stats.LevelId == levelId && stats.Direction == direction && !stats.Ignored);

    /// <summary>Determines if the stats for <paramref name="levelId" /> are already present.</summary>
    /// <param name="levelId">Level ID</param>
    /// <param name="direction">Direction of level transition</param>
    /// <returns><see langword="true" /> if the level is present in the backing stack; <see langword="false" /> otherwise</returns>
    public bool LevelWasIgnored(ulong levelId, TransitionDirection direction)
        => LevelStats.Any(stats => stats.LevelId == levelId && stats.Direction == direction && stats.Ignored);
}