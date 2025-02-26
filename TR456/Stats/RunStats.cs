using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace TR456;

/// <summary>A collection and manager of games' stats for a run.</summary>
public static class RunStats
{
    /// <summary>Used to decide when to split and which level time addresses should be read from memory.</summary>
    private static readonly ImmutableDictionary<Game, GameStats> GameStats = new Dictionary<Game, GameStats>(7)
    {
        { Game.Tr4,                  new GameStats() },
        { Game.Tr4NgPlus,            new GameStats() },
        { Game.Tr4TheTimesExclusive, new GameStats() },
        { Game.Tr5,                  new GameStats() },
        { Game.Tr5NgPlus,            new GameStats() },
        { Game.Tr6,                  new GameStats() },
        { Game.Tr6NgPlus,            new GameStats() },
    }.ToImmutableDictionary();

    /// <summary>Tracks splits across all games; Lara's Home should be entered with the base <see cref="Game" /> value.</summary>
    private static readonly Stack<Game> LevelsSplit = new();

    /// <summary>Sums IGT from completed levels.</summary>
    /// <returns>Total ticks from completed levels</returns>
    public static long GetCompletedLevelIgtIn60FpsTicks(Game currentActiveGame)
    {
        Game[] thirtyFpsGames = [Game.Tr4, Game.Tr4NgPlus, Game.Tr4TheTimesExclusive, Game.Tr5, Game.Tr5NgPlus];
        long thirtyFpsTicks = thirtyFpsGames
            .Where(game => game != currentActiveGame)
            .Aggregate<Game, long>(0, static (current, game) => current + GameStats[game].IgtTicks);

        Game[] sixtyFpsGames = [Game.Tr6, Game.Tr6NgPlus];
        long sixtyFpsTicks = sixtyFpsGames
            .Where(game => game != currentActiveGame)
            .Aggregate<Game, long>(0, static (current, game) => current + GameStats[game].IgtTicks);

        long finishedLevelsTicks = thirtyFpsTicks * 2 + sixtyFpsTicks;

        // Add IGT from current game.
        finishedLevelsTicks += currentActiveGame is not Game.Tr6 and not Game.Tr6NgPlus
            ? GameData.Igt.Current * 2
            : GameData.Igt.Current;

        return finishedLevelsTicks;
    }

    /// <summary>Adds level <paramref name="stats" /> to <paramref name="game"/>.</summary>
    /// <param name="game">The level's corresponding <see cref="Game" /></param>
    /// <param name="stats"><see cref="LevelStats" /> to add</param>
    public static void AddLevelStats(Game game, LevelStats stats)
    {
        // Correct TR4 and TR5 IGTs by subtracting previous levels.
        if (game is not Game.Tr6 and not Game.Tr6NgPlus)
            stats = stats with { Igt = stats.Igt - GameStats[game].IgtTicks };

        if (!GameStats[game].AddLevelStats(stats))
            return;

        LevelsSplit.Push(game);
    }

    /// <summary>Removes the most recently added <see cref="LevelStats" /> or Lara's Home entry.</summary>
    public static void UndoLevelStats()
    {
        Game game = LevelsSplit.Pop();

        _ = GameStats[game].PopLevelStats();
    }

    /// <summary>Clears all backing <see cref="TR456.GameStats" /> and Lara's Home entries.</summary>
    public static void Clear()
    {
        foreach (GameStats gameStats in GameStats.Values)
            gameStats.Clear();

        LevelsSplit.Clear();
    }

    /// <summary>Checks how many times the given <paramref name="game" />'s level with <paramref name="levelId" /> was split.</summary>
    /// <param name="game"><see cref="Game" /> to check</param>
    /// <param name="levelId">Level ID to check</param>
    /// <returns>The number of times the game's level was split</returns>
    public static uint LevelSplitCount(Game game, string levelId) => GameStats[game].LevelCompletionCount(levelId);

    /// <summary>Checks if the given <paramref name="game" />'s level with <paramref name="levelId" /> was split.</summary>
    /// <param name="game"><see cref="Game" /> to check</param>
    /// <param name="levelId">Level ID to check</param>
    /// <returns></returns>
    public static bool LevelHasBeenSplit(Game game, string levelId) => GameStats[game].LevelCompleted(levelId);
}