﻿using System;
using System.Diagnostics;
using LiveSplit.ComponentUtil;

namespace TR456;

public static class GameData
{
    private static readonly GameMemory GameMemory = new ();

    private static readonly SignatureScanInfo SignatureScanInfo = new();

    private static SignatureScanStatus SignatureScanStatus
    {
        set
        {
            SignatureScanInfo.SetStatus(value);
            OnSignatureScanStatusChanged.Invoke(SignatureScanInfo);
        }
    }

    /// <summary>Sometimes directly read, especially for reading level times.</summary>
    internal static Process GameProcess;

    /// <summary>Used to determine which addresses to watch and what text to display in the settings menu.</summary>
    internal static GameVersion GameVersion;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    /// <param name="version">The new <see cref="GameVersion" /></param>
    /// <param name="hash">MD5 hash of the game EXE</param>
    public delegate void GameVersionChangedDelegate(GameVersion version, string hash);

    /// <summary>Allows subscribers to know when and what game version was found.</summary>
    public static GameVersionChangedDelegate OnGameVersionChanged;

    /// <summary>Allows creation of an event regarding the success of scanning game addresses.</summary>
    public delegate void SignatureScanStatusChangedDelegate(SignatureScanInfo info);

    /// <summary>Allows subscribers to know the success of scanning game addresses.</summary>
    public static SignatureScanStatusChangedDelegate OnSignatureScanStatusChanged;

    /// <summary>Reads the current active game or expansion, accounting for NG+ variations for base games.</summary>
    // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
    public static Game CurrentActiveGame => (Game)ActiveGame.Current switch
    {
        Game.Tr4 =>
            (Tr4Level)CurrentLevel >= Tr4Level.Office
                ? Game.Tr4TheTimesExclusive
                : BonusFlag.Current
                    ? Game.Tr4NgPlus : Game.Tr4,
        Game.Tr5 => BonusFlag.Current ? Game.Tr5NgPlus : Game.Tr5,
        Game.Tr6 => BonusFlag.Current ? Game.Tr6NgPlus : Game.Tr6,
        _ => throw new ArgumentOutOfRangeException(nameof(CurrentActiveBaseGame), "Unknown Base Game"),
    };

    /// <summary>Identifies the game without NG+ identification.</summary>
    public static Game CurrentActiveBaseGame => (Game)ActiveGame.Current;

    #region EXE Watcher Accessors

    /// <inheritdoc cref="GameMemory.IsLoading" />
    internal static MemoryWatcher<bool> IsLoading => GameMemory.IsLoading;

    /// <inheritdoc cref="GameMemory.ActiveGame" />
    internal static MemoryWatcher<int> ActiveGame => GameMemory.ActiveGame;

    /// <inheritdoc cref="GameMemory.GFrameIndex" />
    internal static MemoryWatcher<int> GFrameIndex => GameMemory.GFrameIndex;

    /// <inheritdoc cref="GameMemory.Fmv" />
    internal static StringWatcher Fmv => GameMemory.Fmv;

    #endregion

    #region DLL Watcher Accessors

    /// <inheritdoc cref="GameMemory.LoadFade" />
    internal static MemoryWatcher<uint> LoadFade => GameMemory.LoadFade(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.Igt" />
    internal static MemoryWatcher<uint> Igt => GameMemory.Igt(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.LevelIgt" />
    internal static MemoryWatcher<uint> LevelIgt => GameMemory.LevelIgt(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.BonusFlag" />
    internal static MemoryWatcher<bool> BonusFlag => GameMemory.BonusFlag(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.GfInitializeGame" />
    internal static MemoryWatcher<bool> GfInitializeGame => GameMemory.GfInitializeGame(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.GfRequiredStartPosition" />
    internal static MemoryWatcher<byte> GfRequiredStartPosition => GameMemory.GfRequiredStartPosition(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.Room" />
    internal static MemoryWatcher<short> Room => GameMemory.Room(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.Level" />
    internal static MemoryWatcher<uint> Level => GameMemory.Level(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.Tr6LevelName" />
    internal static StringWatcher Tr6LevelName => GameMemory.Tr6LevelName;

    /// <inheritdoc cref="GameMemory.NextLevel" />
    internal static MemoryWatcher<uint> NextLevel => GameMemory.NextLevel(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.Tr45Health" />
    internal static MemoryWatcher<int> Tr45Health => GameMemory.Tr45Health(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.Tr6Health" />
    internal static MemoryWatcher<float> Tr6Health => GameMemory.Tr6Health;

    /// <inheritdoc cref="GameMemory.Pickups" />
    internal static MemoryWatcher<short> Pickups => GameMemory.Pickups(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.Secrets" />
    internal static MemoryWatcher<byte> Secrets => GameMemory.Secrets(CurrentActiveBaseGame);

    /// <inheritdoc cref="GameMemory.Tr6MenuTicker" />
    internal static MemoryWatcher<int> Tr6MenuTicker => GameMemory.Tr6MenuTicker;

    #endregion

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the current level.</summary>
    /// <returns>Current level of the game</returns>
    private static uint CurrentLevel => Level.Current;

    /// <summary>Based on <see cref="CurrentActiveBaseGame" />, determines the old level.</summary>
    /// <returns>Old level of the game</returns>
    private static uint OldLevel => Level.Old;

    public static ulong OldLevelId => OldLevel;

    /// <summary>Test that the game has fully initialized based on expected memory readings.</summary>
    internal static bool GameIsInitialized => GameMemory.ActiveGame.Old is >= 0 and <= 2;

    private static DateTime? _retryTime;

    /// <summary>Updates <see cref="GameData" /> implementation and its addresses' values.</summary>
    /// <returns><see langword="true" /> if game data was updated, <see langword="false" /> otherwise</returns>
    public static bool Update()
    {
        try
        {
            if (_retryTime.HasValue && DateTime.UtcNow < _retryTime.Value)
                return false; // Waiting to retry

            if (GameProcess is null || GameProcess.HasExited)
            {
                if (GameProcess is not null && GameProcess.HasExited)
                {
                    SignatureScanInfo.ResetCount(); // Reset so searches can continue.
                    SignatureScanStatus = SignatureScanStatus.NotTriedYet;
                }

                if (!FindSupportedGame())
                    return false;

                try
                {
                    GameMemory.InitializeMemoryWatchers(GameVersion, GameProcess);
                    SignatureScanStatus = SignatureScanStatus.Success;
                    _retryTime = null;
                }
                catch (Exception e)
                {
                    LiveSplit.Options.Log.Error(e);

                    // Sometimes the cause of the error is LS attempting to scan too quickly when the game opens, before modules are fully available to scan.
                    if (SignatureScanInfo.MaxRetriesReached)
                    {
                        SignatureScanStatus = SignatureScanStatus.Failure; // Update will not try again (unless GameProcess.HasExited).
                        _retryTime = null;
                    }
                    else
                    {
                        // Set retry state.
                        GameProcess = null;
                        SignatureScanInfo.AddRetry();
                        SignatureScanStatus = SignatureScanStatus.Retrying;
                        _retryTime = DateTime.UtcNow.AddSeconds(3);
                    }
                }

                return SignatureScanInfo.IsSuccess && GameIsInitialized;
            }

            if (!SignatureScanInfo.IsSuccess)
                return false;

            GameMemory.UpdateMemoryWatchers(GameProcess);
            return GameIsInitialized;
        }
        catch (Exception e)
        {
            LiveSplit.Options.Log.Error(e);
            return false;
        }
    }

    /// <summary>If applicable, finds a <see cref="Process" /> running an expected version of the game.</summary>
    /// <returns>
    ///     <see langword="true" /> if <see cref="GameProcess" /> and <see cref="GameVersion" /> were meaningfully set,
    ///     <see langword="false" /> otherwise
    /// </returns>
    private static bool FindSupportedGame()
    {
        GameVersion detectedVersion = VersionDetector.DetectVersion(out Process gameProcess, out string hash);
        if (GameVersion != detectedVersion)
        {
            GameVersion = detectedVersion;
            OnGameVersionChanged.Invoke(GameVersion, hash);
        }

        if (gameProcess is null)
            return false;

        SetGameProcess(gameProcess);
        return true;
    }

    /// <summary>Sets <see cref="GameProcess" /> and performs additional work to ensure the process's termination is handled.</summary>
    /// <param name="gameProcess">Game process</param>
    private static void SetGameProcess(Process gameProcess)
    {
        GameProcess = gameProcess;
        GameProcess.EnableRaisingEvents = true;
        GameProcess.Exited += static (_, _) => OnGameVersionChanged.Invoke(GameVersion.None, string.Empty);
    }
}