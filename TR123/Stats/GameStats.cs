using System.Collections.Generic;
using System.Linq;

namespace TR123;

/// <inheritdoc />
/// <summary>Stats for a game with <paramref name="LevelCount" /> levels.</summary>
/// <param name="LevelCount">Number of levels in the game</param>
public readonly record struct GameStats(int LevelCount)
{
    private readonly Stack<LevelStats> _levelStatistics = new(LevelCount);

    /// <summary><see langword="true" /> if all levels have been added into the backing stack; <see langword="false" /> otherwise.</summary>
    public bool GameComplete => _levelStatistics.Count == LevelCount;

    /// <summary>Accessor for the backing stack.</summary>
    public IEnumerable<LevelStats> LevelStats => _levelStatistics;

    /// <summary>Pushes <paramref name="stats" /> to the backing stack.</summary>
    /// <param name="stats">Stats to add</param>
    public bool AddLevelStats(LevelStats stats)
    {
        if (GameComplete || LevelAlreadyComplete(stats.LevelNumber))
            return false;

        _levelStatistics.Push(stats);
        return true;
    }

    /// <summary>Pops the most recently added <see cref="LevelStats" /> from the backing stack.</summary>
    public LevelStats PopLevelStats() => _levelStatistics.Pop();

    /// <summary>Clears the backing stack of all <see cref="LevelStats" />.</summary>
    public void Clear() => _levelStatistics.Clear();

    /// <summary>Determines if the stats for <paramref name="levelNumber" /> are already present.</summary>
    /// <param name="levelNumber">Level number</param>
    /// <returns><see langword="true" /> if the level is present in the backing stack; <see langword="false" /> otherwise</returns>
    public bool LevelAlreadyComplete(uint levelNumber) => LevelStats.Any(stats => stats.LevelNumber == levelNumber);
}