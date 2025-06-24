using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Util;

namespace TR456;

public static class GameData
{
    private static readonly VersionDetector VersionDetector =
        new(
            ["tomb456"],
            new Dictionary<string, uint>
            {
                { "CA258829147BD3BF932152BFFABBE4A1".ToLowerInvariant(), (uint)TR456.GameVersion.PublicV10 }, // EGS
                { "25FEE8EBB2FAE95BF13CABE151CB7A9F".ToLowerInvariant(), (uint)TR456.GameVersion.PublicV10 }, // GOG
                { "14479C2B293FAC5A8E175D0D540B7C77".ToLowerInvariant(), (uint)TR456.GameVersion.PublicV10 }, // Steam
                { "CC6936505922BE1A29F12173BF1A3EB7".ToLowerInvariant(), (uint)TR456.GameVersion.PublicV10Patch1 }, // EGS
                { "9C191729BCAFE153BA74AD83D964D6EE".ToLowerInvariant(), (uint)TR456.GameVersion.PublicV10Patch1 }, // GOG / Steam
            }
        );

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
    internal static uint CurrentGameVersion;

    /// <summary>Allows creation of an event regarding when and what game version was found.</summary>
    public delegate void GameVersionChangedDelegate(VersionDetectionResult result);

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
        if (_retryTime.HasValue && DateTime.UtcNow < _retryTime.Value)
            return false; // Waiting to retry

        if (GameProcess is null || GameProcess.HasExited)
        {
            if (GameProcess is not null && GameProcess.HasExited)
            {
                SignatureScanInfo.ResetCount(); // Reset so searches can continue.
                SignatureScanStatus = SignatureScanStatus.NotTriedYet;
                _retryTime = DateTime.UtcNow.AddSeconds(3); // Hack to lessen weird code paths.
                GameProcess = null;
                return false;
            }

            try
            {
                if (!FindSupportedGame())
                    return false;

                GameMemory.InitializeMemoryWatchers(CurrentGameVersion, GameProcess);
                SignatureScanStatus = SignatureScanStatus.Success;
                _retryTime = null;
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException && e.Message.EndsWith("has exited.", StringComparison.Ordinal))
                {
                    // This code path can happen because GameProcess can become null before the process itself fully quits.
                    SignatureScanInfo.ResetCount();
                    SignatureScanStatus = SignatureScanStatus.NotTriedYet;
                    _retryTime = null;
                    return false;
                }

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

        try
        {
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
    ///     <see langword="true" /> if <see cref="GameProcess" /> and <see cref="CurrentGameVersion" /> were meaningfully set,
    ///     <see langword="false" /> otherwise
    /// </returns>
    private static bool FindSupportedGame()
    {
        uint previousVersion = CurrentGameVersion;

        VersionDetectionResult result = VersionDetector.DetectVersion();
        switch (result)
        {
            case VersionDetectionResult.Found found:
                CurrentGameVersion = found.Version;
                SetGameProcess(found.Process);
                break;

            case VersionDetectionResult.Unknown unknown:
                CurrentGameVersion = VersionDetector.Unknown;
                SetGameProcess(unknown.Process);
                break;

            case VersionDetectionResult.None:
                CurrentGameVersion = VersionDetector.None;
                GameProcess = null;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(result));
        }

        if (previousVersion != CurrentGameVersion) // This protects against spamming the event in the repeated None case.
            OnGameVersionChanged.Invoke(result);

        return result is VersionDetectionResult.Found;
    }

    /// <summary>Sets <see cref="GameProcess" /> and performs additional work to ensure the process's termination is handled.</summary>
    /// <param name="gameProcess">Game process</param>
    private static void SetGameProcess(Process gameProcess)
    {
        GameProcess = gameProcess;
        GameProcess.EnableRaisingEvents = true;
        GameProcess.Exited += static (_, _) => OnGameVersionChanged.Invoke(new VersionDetectionResult.None());
    }
}