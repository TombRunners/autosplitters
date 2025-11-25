using System.Collections.Generic;
using System.Collections.Immutable;

namespace TR4;

/// <summary>A collection and manager of games' stats for a run.</summary>
public static class RunStats
{
    /// <summary>Used to decide when to split.</summary>
    private static readonly ImmutableDictionary<Tr4Version, GameStats> AllGameStats = new Dictionary<Tr4Version, GameStats>(2)
    {
        { Tr4Version.SteamOrGog,        new GameStats() },
        { Tr4Version.TheTimesExclusive, new GameStats() },
    }.ToImmutableDictionary();

    /// <summary>Tracks splits across all games.</summary>
    private static readonly Stack<Tr4Version> GamesSplitStack = new();

    /// <summary>Adds level <paramref name="stats" /> to <paramref name="game" />.</summary>
    /// <param name="game">The level's corresponding <see cref="Tr4Version" /></param>
    /// <param name="stats"><see cref="LevelStats" /> to add</param>
    public static void AddLevelStats(Tr4Version game, LevelStats stats)
    {
        if (!AllGameStats[game].AddLevelStats(stats))
            return;

        GamesSplitStack.Push(game);
    }

    /// <summary>Removes the most recently added <see cref="LevelStats" />.</summary>
    public static void UndoLevelStats()
    {
        Tr4Version game = GamesSplitStack.Pop();

        var continuePopping = true;
        while (continuePopping)
        {
            if (AllGameStats[game].Count == 0)
                break;

            LevelStats levelStats = AllGameStats[game].PopLevelStats();
            continuePopping = levelStats.Ignored;
        }
    }

    /// <summary>Clears all backing <see cref="GameStats" />.</summary>
    public static void Clear()
    {
        foreach (GameStats gameStats in AllGameStats.Values)
            gameStats.Clear();

        GamesSplitStack.Clear();
    }

    /// <summary>Checks if the given <paramref name="game" />'s level with <paramref name="levelId" /> was split.</summary>
    /// <param name="game"><see cref="Tr4Version" /> to check</param>
    /// <param name="levelId">Level ID to check</param>
    /// <param name="direction">Direction of level transition</param>
    public static bool LevelWasSplit(Tr4Version game, ulong levelId, TransitionDirection direction = TransitionDirection.OneWayFromLower)
        => AllGameStats[game].LevelWasSplit(levelId, direction);

    /// <summary>Checks if the given <paramref name="game" />'s level with <paramref name="levelId" /> was split.</summary>
    /// <param name="game"><see cref="Tr4Version" /> to check</param>
    /// <param name="levelId">Level ID to check</param>
    /// <param name="direction">Direction of level transition</param>
    public static int LevelSplitCount(Tr4Version game, ulong levelId, TransitionDirection direction = TransitionDirection.OneWayFromLower)
        => AllGameStats[game].LevelSplitCount(levelId, direction);

    /// <summary>Checks if the given <paramref name="game" />'s level with <paramref name="levelId" /> was ignored instead of split.</summary>
    /// <param name="game"><see cref="Tr4Version" /> to check</param>
    /// <param name="levelId">Level ID to check</param>
    /// <param name="direction">Direction of level transition</param>
    public static bool LevelWasIgnored(Tr4Version game, ulong levelId, TransitionDirection direction = TransitionDirection.OneWayFromLower)
        => AllGameStats[game].LevelWasIgnored(levelId, direction);
}