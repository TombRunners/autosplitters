using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TR123;

public static class RunStats
{
    /// <summary>Used to size new entries to AllGameStats.</summary>
    private static readonly ImmutableDictionary<Game, int> LevelCount = new Dictionary<Game, int>(9)
    {
        { Game.Tr1,                   15 },
        { Game.Tr1NgPlus,             15 },
        { Game.Tr1UnfinishedBusiness, 04 },
        { Game.Tr2,                   18 },
        { Game.Tr2NgPlus,             18 },
        { Game.Tr2GoldenMask,         05 },
        { Game.Tr3,                   20 },
        { Game.Tr3NgPlus,             20 },
        { Game.Tr3TheLostArtifact,    06 },
    }.ToImmutableDictionary();

    /// <summary>Used to decide when to split and which level time addresses should be read from memory.</summary>
    private static readonly ImmutableDictionary<Game, GameStats> GameStats = new Dictionary<Game, GameStats>(9)
    {
        { Game.Tr1,                   new GameStats(LevelCount[Game.Tr1]) },
        { Game.Tr1NgPlus,             new GameStats(LevelCount[Game.Tr1NgPlus]) },
        { Game.Tr1UnfinishedBusiness, new GameStats(LevelCount[Game.Tr1UnfinishedBusiness]) },
        { Game.Tr2,                   new GameStats(LevelCount[Game.Tr2]) },
        { Game.Tr2NgPlus,             new GameStats(LevelCount[Game.Tr2NgPlus]) },
        { Game.Tr2GoldenMask,         new GameStats(LevelCount[Game.Tr2GoldenMask]) },
        { Game.Tr3,                   new GameStats(LevelCount[Game.Tr3]) },
        { Game.Tr3NgPlus,             new GameStats(LevelCount[Game.Tr3NgPlus]) },
        { Game.Tr3TheLostArtifact,    new GameStats(LevelCount[Game.Tr3TheLostArtifact]) },
    }.ToImmutableDictionary();

    /// <summary>Tracks completion of Lara's Home by <see cref="Game" />.</summary>
    private static readonly HashSet<Game> LarasHomeComplete = [];

    /// <summary>Tracks splits across all games; Lara's Home should be entered with the base <see cref="Game" /> value.</summary>
    private static readonly Stack<(Game game, uint levelNumber)> LevelsSplit = new();

    /// <summary>Sums IGT from completed levels.</summary>
    /// <returns>Total ticks from completed levels</returns>
    public static ulong GetCompletedLevelIgtTicks(Game currentActiveGame, uint currentLevel)
    {
        // Sum IGT from other games' completed levels from splitter memory.
        ulong finishedLevelsTicks = GameStats
            .Where(entry => entry.Key != currentActiveGame)
            .Select(static entry => entry.Value)
            .Aggregate<GameStats, ulong>(
                0, static (current1, gameStats) =>
                    gameStats
                        .LevelStats
                        .Aggregate(current1, static (current, levelStats) => current + levelStats.Igt)
            );

        // Sum IGT from current game.
        GameStats gameLevelStats = GameStats[currentActiveGame];
        if (gameLevelStats.GameComplete)
        {
            // Sum IGT from stored stats.
            finishedLevelsTicks = gameLevelStats
                .LevelStats
                .Aggregate(finishedLevelsTicks, static (current, levelStat) => current + levelStat.Igt);
        }
        else
        {
            // Sum IGT from game memory.
            // Because the level stats are stored in a stack, we need to reverse the enumerable to get the correct level completion order.
            var completedLevelsInOrder = gameLevelStats.LevelStats.Select(static stats => stats.LevelNumber).Reverse();
            finishedLevelsTicks += GameData.SumCompletedLevelTimesInMemory(completedLevelsInOrder, currentLevel);
        }

        return finishedLevelsTicks;
    }

    /// <summary>Adds level <paramref name="stats" /> to <paramref name="game"/>.</summary>
    /// <param name="game">The level's corresponding <see cref="Game" /></param>
    /// <param name="stats"><see cref="LevelStats" /> to add</param>
    public static void AddLevelStats(Game game, LevelStats stats)
    {
        // Handle Lara's Home.
        if (stats.LevelNumber == 0)
        {
            Game baseGame = GameData.CurrentActiveBaseGame;
            if (LarasHomeComplete.Add(baseGame))
                LevelsSplit.Push((baseGame, 0));
            return;
        }

        // Handle other levels.
        if (GameStats[game].AddLevelStats(stats))
            LevelsSplit.Push((game, stats.LevelNumber));
    }

    /// <summary>Removes the most recently added <see cref="LevelStats" /> or Lara's Home entry.</summary>
    public static void UndoLevelStats()
    {
        (Game game, uint levelNumber) = LevelsSplit.Pop();

        // Handle Lara's Home.
        if (levelNumber == 0)
        {
            _ = LarasHomeComplete.Remove(game);
            return;
        }

        // Handle other levels.
        _ = GameStats[game].PopLevelStats();
    }

    /// <summary>Clears all backing <see cref="TR123.GameStats" /> and Lara's Home entries.</summary>
    public static void Clear()
    {
        foreach (GameStats gameStats in GameStats.Values)
            gameStats.Clear();

        LarasHomeComplete.Clear();
        LevelsSplit.Clear();
    }

    public static bool LevelHasBeenSplit(Game currentActiveGame, uint currentLevel)
        => currentLevel == 0
            ? LarasHomeComplete.Contains(GameData.CurrentActiveBaseGame)
            : GameStats[currentActiveGame].LevelAlreadyComplete(currentLevel);
}