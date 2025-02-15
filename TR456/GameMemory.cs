using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using LiveSplit.ComponentUtil;

namespace TR456;

internal class GameMemory
{
    /// <summary>Base games included within the remastered EXE.</summary>
    private static readonly ImmutableList<Game> BaseGames = [Game.Tr4, Game.Tr5, Game.Tr6];

    /// <summary>Contains the names of the modules (DLLs) for each <see cref="Game" />.</summary>
    internal static readonly ImmutableDictionary<Game, string> GameModules = new Dictionary<Game, string>(3)
    {
        { Game.Tr4, "tomb4.dll" },
        { Game.Tr5, "tomb5.dll" },
        { Game.Tr6, "tomb6.dll" },
    }.ToImmutableDictionary();

    /// <summary>For each released remastered game version, contains each game's address offsets.</summary>
    internal static readonly ImmutableDictionary<GameVersion, Dictionary<Game, GameAddresses>> GameVersionAddresses =
        new Dictionary<GameVersion, Dictionary<Game, GameAddresses>>
        {
            {
                GameVersion.PublicV10,
                new Dictionary<Game, GameAddresses>()
            },
        }.ToImmutableDictionary();

    #region MemoryWatcher Definitions

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
    internal readonly MemoryWatcherList Watchers = [];

    #endregion

    /// <summary>Sets addresses based on <paramref name="version" />.</summary>
    /// <param name="version">Game version to base addresses on</param>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    internal void InitializeMemoryWatchers(GameVersion version, Process gameProcess)
    {
        Watchers.Clear();

        switch (version)
        {
            case GameVersion.PublicV10:
                // Base game EXE (tomb456.exe)

                // Common items for all game's DLLs
                AddCommonDllWatchers(GameVersion.PublicV10);
                break;

            case GameVersion.None:
            case GameVersion.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }

        PreLoadWatchers(gameProcess);
    }

    /// <summary>Adds MemoryWatchers which are common across all games.</summary>
    /// <param name="version">Game version</param>
    private void AddCommonDllWatchers(GameVersion version)
    {
        foreach (var game in BaseGames)
        {
            string moduleName = GameModules[game];
            var addresses = GameVersionAddresses[version][game];
        }
    }

    /// <summary>
    ///     This method should be called when initializing MemoryWatchers to ensure that they do not have
    ///     default / zeroed values on initialization, which complicates or ruins autosplitter logic.
    /// </summary>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    private void PreLoadWatchers(Process gameProcess)
    {
        Watchers.UpdateAll(gameProcess); // Loads Current values.
        Watchers.UpdateAll(gameProcess); // Moves Current to Old and loads new Current values.
    }
}
