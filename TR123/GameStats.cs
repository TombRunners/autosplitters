using System.Collections.Generic;

namespace TR123;

public struct GameStats(int levelCount)
{
    private List<LevelStats> LevelStatistics { get; } = new(levelCount);

    public bool GameComplete { get; private set; } = false;
    public readonly int LevelCount = levelCount;

    public readonly IEnumerable<LevelStats> LevelStats => LevelStatistics;

    public void AddLevelStats(LevelStats stats)
    {
        if (GameComplete)
            return;

        LevelStatistics.Add(stats);
        if (LevelStatistics.Count == LevelCount)
            GameComplete = true;
    }

    public void RemoveLevelStats(LevelStats stats)
    {
        LevelStatistics.Remove(stats);
        GameComplete = false;
    }
}