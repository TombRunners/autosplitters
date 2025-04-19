using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using LiveSplit.ComponentUtil;

namespace TR456;

internal class GameMemory
{
    /// <summary>Base games included within the remastered EXE.</summary>
    private static readonly ImmutableList<Game> BaseGames = [Game.Tr4, Game.Tr5, Game.Tr6];

    /// <summary>Contains the names of the modules (DLLs) for each <see cref="Game" />.</summary>
    private static readonly ImmutableDictionary<string, Game> GameModules = new Dictionary<string, Game>(3)
    {
        { Constants.DllTomb4, Game.Tr4 },
        { Constants.DllTomb5, Game.Tr5 },
        { Constants.DllTomb6, Game.Tr6 },
    }.ToImmutableDictionary();

    #region EXE Signatures and MemoryWatchers

    /// <summary>Address signature information for the game's EXE.</summary>
    private static readonly ImmutableHashSet<AddressSignatureInfo> WatcherExeSignatureInfos =
        new HashSet<AddressSignatureInfo>
        {
            // ActiveGame
            new()
            {
                Name = Constants.WatcherActiveGameName,
                MemoryWatcherFactory = static address => new MemoryWatcher<int>(address) { Name = Constants.WatcherActiveGameName },
                Signature = [0x83, 0xE1, 0x03, 0x25, 0xFF, 0xFF, 0xE7, 0xFF],
                OffsetsToWriteInstruction = [(null, 0x8A)],
                WriteInstructionLength = 6,
                EffectiveAddressOffset = 0,
            },
            // GFrameIndex
            new()
            {
                Name = Constants.WatcherGFrameIndexName,
                MemoryWatcherFactory = static address => new MemoryWatcher<int>(address) { Name = Constants.WatcherGFrameIndexName },
                Signature = [0x66, 0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x84, 0x24, 0xB0, 0x00, 0x00, 0x00],
                OffsetsToWriteInstruction = [(null, 0x20)],
                WriteInstructionLength = 7,
                EffectiveAddressOffset = 0x208,
            },
            // IsLoading
            new()
            {
                Name = Constants.WatcherIsLoadingName,
                MemoryWatcherFactory = static address => new MemoryWatcher<bool>(address) { Name = Constants.WatcherIsLoadingName },
                Signature = [0x48, 0x69, 0xC0, 0xE8, 0x03, 0x00, 0x00, 0xC7, 0x05],
                OffsetsToWriteInstruction = [(null, 7)],
                WriteInstructionLength = 6,
                EffectiveAddressOffset = 4,
            },
            // FMV
            new()
            {
                Name = Constants.WatcherFmvName,
                MemoryWatcherFactory = static address => new StringWatcher(address, 5) { Name = Constants.WatcherFmvName },
                Signature = [0x0F, 0x57, 0xC9, 0x48, 0x89, 0x05],
                OffsetsToWriteInstruction = [(null, 0xA)],
                WriteInstructionLength = 7,
                EffectiveAddressOffset = 0,
            },
        }.ToImmutableHashSet();

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic related to the game's executable.</summary>
    private readonly MemoryWatcherList _watchersExe = [];

    /// <summary>Gives the value of the active game, where TR4 is 0, TR5 is 1, TR6 is 2.</summary>
    /// <remarks>The value should be converted to <see cref="GameVersion" />.</remarks>
    internal MemoryWatcher<int> ActiveGame => (MemoryWatcher<int>)_watchersExe?[Constants.WatcherActiveGameName];

    /// <summary>
    ///     From when a load occurs (level, FMV, in-game cutscene, title screen),
    ///     resets to 0 and then increments at the rate of IGT ticks (30 per second).
    ///     During actual loading time (asset loading, etc.), freezes.
    /// </summary>
    internal MemoryWatcher<int> GFrameIndex => (MemoryWatcher<int>)_watchersExe?[Constants.WatcherGFrameIndexName];

    /// <summary>Found to be more reliable than the corresponding IsLoading in the tomb6.dll for TR6R.</summary>
    internal MemoryWatcher<bool> IsLoading => (MemoryWatcher<bool>)_watchersExe?[Constants.WatcherIsLoadingName];

    /// <summary>Gives the value of the active FMV, especially reliable for TR6R.</summary>
    internal StringWatcher Fmv => (StringWatcher)_watchersExe?[Constants.WatcherFmvName];

    #endregion

    #region DLL Signatures and MemoryWatchers

    /// <summary>Address signature information for the game's DLLs.</summary>
    private static readonly ImmutableDictionary<Game[], AddressSignatureInfo> WatcherDllSignatureInfos =
        new Dictionary<Game[], AddressSignatureInfo>
        {
            // LoadFade
            {
                [Game.Tr4, Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherLoadFadeName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherLoadFadeName },
                    Signature = [0xC1, 0xE0, 0x04, 0x03, 0xD0],
                    OffsetsToWriteInstruction = [(null, 0x11)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            // IGT (global)
            {
                [Game.Tr4],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherIgtName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherIgtName },
                    Signature = [0x85, 0xC0, 0x7E, 0x0A, 0xFF, 0xC8, 0x89, 0x05],
                    OffsetsToWriteInstruction = [(null, 0x6F)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherIgtName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherIgtName },
                    Signature = [0x85, 0xC0, 0x7E, 0x0A, 0xFF, 0xC8, 0x89, 0x05],
                    OffsetsToWriteInstruction = [(null, 0x6F)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr6],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherIgtName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherIgtName },
                    Signature = [0xA9, 0xF7, 0xFF, 0xFF, 0xFF, 0x74, 0x25],
                    OffsetsToWriteInstruction = [(null, 0x20)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            // LevelIgt (TR6R)
            {
                [Game.Tr6],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherLevelIgtName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherLevelIgtName },
                    Signature = [0xA9, 0xF7, 0xFF, 0xFF, 0xFF, 0x74, 0x25],
                    OffsetsToWriteInstruction = [(null, 0x26)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            // BonusFlag
            {
                [Game.Tr4, Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherBonusFlagName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<bool>(address) { Name = Constants.WatcherBonusFlagName },
                    Signature = [0x01, 0x00, 0x00, 0x00, 0x85, 0xC9, 0x78, 0x0A],
                    OffsetsToWriteInstruction = [(null, -54)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr6],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherBonusFlagName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<bool>(address) { Name = Constants.WatcherBonusFlagName },
                    Signature = [0x19, 0x00, 0x00, 0x0F, 0xB6, 0x05],
                    OffsetsToWriteInstruction = [(null, -0x4A)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            // GfInitializeGame
            {
                [Game.Tr4, Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherGfInitializeGameName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<bool>(address) { Name = Constants.WatcherGfInitializeGameName },
                    Signature = [0xEB, 0x0A, 0x45, 0x32, 0xC9],
                    OffsetsToWriteInstruction = [(null, 0x5)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 1,
                }
            },
            // GfRequiredStartPosition
            {
                [Game.Tr4],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherGfRequiredStartPositionName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<byte>(address) { Name = Constants.WatcherGfRequiredStartPositionName },
                    Signature = [0x44, 0x0F, 0xB6, 0x5C, 0x24, 0x28],
                    OffsetsToWriteInstruction = [(null, 0x6)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
            // Room
            {
                [Game.Tr4, Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherRoomName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<short>(address) { Name = Constants.WatcherRoomName },
                    Signature = [0x0F, 0xBF, 0x48, 0x06, 0x0F, 0xBF, 0x40, 0x04, 0x03, 0xC8, 0x89, 0x15],
                    OffsetsToWriteInstruction = [(null, 0x29), (GameVersion.PublicV10Patch1, 0x36)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
            // LevelName (TR6R)
            {
                [Game.Tr6],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherLevelNameName,
                    MemoryWatcherFactory = static address => new StringWatcher(new DeepPointer(address, 0x572), 21) { Name = Constants.WatcherLevelNameName },
                    Signature = [0x48, 0x81, 0xC1, 0xA0, 0x00, 0x00, 0x00, 0x4D, 0x8B, 0xCE, 0x4D, 0x8B, 0xC7],
                    OffsetsToWriteInstruction = [(null, 0x25)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                    IsPointer = true,
                    OffsetAfterPointerResolution = 0x572,
                }
            },
            // CurrentLevel
            {
                [Game.Tr4],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherCurrentLevelName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherCurrentLevelName },
                    Signature = [0x3C, 0x29, 0x44, 0x8B, 0xC8, 0x44, 0x0F, 0x47, 0xCE],
                    OffsetsToWriteInstruction = [(null, 0x26)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherCurrentLevelName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherCurrentLevelName },
                    Signature = [0x40, 0x80, 0xFF, 0x0F, 0x44, 0x0F, 0xB6, 0xCF, 0x44, 0x0F, 0x47, 0xCD],
                    OffsetsToWriteInstruction = [(null, 0x29)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
            // NextLevel
            {
                [Game.Tr4],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherNextLevelName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherNextLevelName },
                    SignatureWithMasks = ["40 84 ?? ?? ?? 00 00 0F 84 ?? ?? 00 00 48 8B ?? ?? ?? 00 00 39 30"],
                    OffsetsToWriteInstruction = [(null, -7)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherNextLevelName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<uint>(address) { Name = Constants.WatcherNextLevelName },
                    Signature = [0x2B, 0xC8, 0x0F, 0x48, 0xCF],
                    OffsetsToWriteInstruction = [(null, 0x2B)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
            // Health
            {
                [Game.Tr4],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherHealthName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<int>(address) { Name = Constants.WatcherHealthName },
                    SignatureWithMasks = ["79 04 ?? ?? EB 0A B8 E8 03 00 00"],
                    OffsetsToWriteInstruction = [(null, 0x10), (GameVersion.PublicV10Patch1, 0x29)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherHealthName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<int>(address) { Name = Constants.WatcherHealthName },
                    SignatureWithMasks = ["39 ?? ?? ?? 16 00 74 ?? 89 ?? ?? ?? 16 00"],
                    OffsetsToWriteInstruction = [(null, 0)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr6],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherHealthName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<float>(new DeepPointer(address, 0x1F0, 0xD64)) { Name = Constants.WatcherHealthName },
                    Signature = [0x79, 0x06, 0x33, 0xC0, 0x89, 0x44, 0x24, 0x70],
                    OffsetsToWriteInstruction = [(null, 8)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
            // Pickups
            {
                [Game.Tr4],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherPickupsName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<short>(address) { Name = Constants.WatcherPickupsName },
                    Signature = [0x75, 0x0C, 0x66, 0x42, 0x89, 0x6C],
                    OffsetsToWriteInstruction = [(null, 8)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherPickupsName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<short>(address) { Name = Constants.WatcherPickupsName },
                    Signature = [0xB8, 0x00, 0x01, 0x00, 0x00, 0x66, 0x85, 0x43, 0x30],
                    OffsetsToWriteInstruction = [(null, -15)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr6],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherPickupsName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<short>(address) { Name = Constants.WatcherPickupsName },
                    Signature = [0x74, 0x34, 0x66, 0x83, 0xF9, 0x51, 0x74, 0x2E],
                    OffsetsToWriteInstruction = [(null, 0xF)],
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
            // Secrets
            {
                [Game.Tr4],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherSecretsName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<byte>(address) { Name = Constants.WatcherSecretsName },
                    SignatureWithMasks = ["B9 05 00 00 00 E8 ?? ?? ?? 00 0F B6 05 ?? ?? ?? 00 FE 05 ?? ?? ?? 00 0F AB D8"],
                    OffsetsToWriteInstruction = [(null, 0x11)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherSecretsName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<byte>(address) { Name = Constants.WatcherSecretsName },
                    Signature = [0x8D, 0x41, 0xFF, 0x3C, 0x02, 0x77, 0x09],
                    OffsetsToWriteInstruction = [(null, -12)]   ,
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            {
                [Game.Tr6],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherSecretsName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<byte>(address) { Name = Constants.WatcherSecretsName },
                    Signature = [0x74, 0x34, 0x66, 0x83, 0xF9, 0x51, 0x74, 0x2E],
                    OffsetsToWriteInstruction = [(null, 0x1B)],
                    WriteInstructionLength = 6,
                    EffectiveAddressOffset = 0,
                }
            },
            // Menu Ticker (TR6R)
            {
                [Game.Tr6],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherMenuTickerName,
                    MemoryWatcherFactory = static address => new MemoryWatcher<int>(address) { Name = Constants.WatcherMenuTickerName },
                    Signature = [0xC7, 0x40, 0x08, 0x01, 0x00, 0x03, 0x00],
                    OffsetsToWriteInstruction = [(null, 7)],
                    WriteInstructionLength = 8,
                    EffectiveAddressOffset = 0,
                }
            },
        }.ToImmutableDictionary();

    /// <summary>Contains memory addresses related to the TR4R game DLL, accessible as named members, used in auto-splitting logic.</summary>
    private readonly MemoryWatcherList _watchersTR4R = [];

    /// <summary>Contains memory addresses related to the TR5R game DLL, accessible as named members, used in auto-splitting logic.</summary>
    private readonly MemoryWatcherList _watchersTR5R = [];

    /// <summary>Contains memory addresses related to the TR6R game DLL, accessible as named members, used in auto-splitting logic.</summary>
    private readonly MemoryWatcherList _watchersTR6R = [];

    private MemoryWatcherList GetMemoryWatcherListForGame(Game game) =>
        game switch
        {
            Game.Tr4 or Game.Tr4NgPlus or Game.Tr4TheTimesExclusive => _watchersTR4R,
            Game.Tr5 or Game.Tr5NgPlus                              => _watchersTR5R,
            Game.Tr6 or Game.Tr6NgPlus                              => _watchersTR6R,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "Invalid game"),
        };

    private MemoryWatcher<T> GetMemoryWatcherForGame<T>(string name, Game game) where T : struct
    {
        MemoryWatcherList list = GetMemoryWatcherListForGame(game);
        return (MemoryWatcher<T>)list?[name];
    }

    internal MemoryWatcher<uint> LoadFade(Game game) => GetMemoryWatcherForGame<uint>(Constants.WatcherLoadFadeName, game);

    internal MemoryWatcher<uint> Igt(Game game) => GetMemoryWatcherForGame<uint>(Constants.WatcherIgtName, game);

    internal MemoryWatcher<uint> LevelIgt(Game game) => GetMemoryWatcherForGame<uint>(Constants.WatcherLevelIgtName, game);

    internal MemoryWatcher<bool> BonusFlag(Game game) => GetMemoryWatcherForGame<bool>(Constants.WatcherBonusFlagName, game);

    internal MemoryWatcher<bool> GfInitializeGame(Game game) => GetMemoryWatcherForGame<bool>(Constants.WatcherGfInitializeGameName, game);

    internal MemoryWatcher<byte> GfRequiredStartPosition(Game game) => GetMemoryWatcherForGame<byte>(Constants.WatcherGfRequiredStartPositionName, game);

    internal MemoryWatcher<short> Room(Game game) => GetMemoryWatcherForGame<short>(Constants.WatcherRoomName, game);

    internal MemoryWatcher<uint> Level(Game game) => GetMemoryWatcherForGame<uint>(Constants.WatcherCurrentLevelName, game);

    internal MemoryWatcher<uint> NextLevel(Game game) => GetMemoryWatcherForGame<uint>(Constants.WatcherNextLevelName, game);

    internal StringWatcher Tr6LevelName => _watchersTR6R?[Constants.WatcherLevelNameName] as StringWatcher;

    internal MemoryWatcher<int> Tr45Health(Game game)
        => game is Game.Tr6 or Game.Tr6NgPlus
            ? throw new ArgumentOutOfRangeException(nameof(game), "TR6 not accepted")
            : GetMemoryWatcherForGame<int>(Constants.WatcherHealthName, game);

    internal MemoryWatcher<float> Tr6Health => _watchersTR6R?[Constants.WatcherHealthName] as MemoryWatcher<float>;

    internal MemoryWatcher<short> Pickups(Game game) => GetMemoryWatcherForGame<short>(Constants.WatcherPickupsName, game);

    internal MemoryWatcher<byte> Secrets(Game game) => GetMemoryWatcherForGame<byte>(Constants.WatcherSecretsName, game);

    internal MemoryWatcher<int> Tr6MenuTicker => _watchersTR6R?[Constants.WatcherMenuTickerName] as MemoryWatcher<int>;

    #endregion

    #region MemoryWatcher Initialization

    /// <summary>Sets addresses based on <paramref name="version" />.</summary>
    /// <param name="version">Game version to base addresses on</param>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    internal void InitializeMemoryWatchers(GameVersion version, Process gameProcess)
    {
        _watchersExe.Clear();
        _watchersTR4R.Clear();
        _watchersTR5R.Clear();
        _watchersTR6R.Clear();

        // Guard against an improper call.
        if (version is GameVersion.None)
            throw new ArgumentOutOfRangeException(nameof(version), version, null);

        // tomb456.exe
        SignatureScanner scannerExe = CreateScannerForExe(gameProcess);
        AddWatchersExe(scannerExe);

        // tomb4.dll, tomb5.dll, tomb6.dll
        var scannersDlls = CreateScannersForDlls(gameProcess);
        AddWatchersDlls(scannersDlls);

        PreLoadWatchers(gameProcess);
    }

    /// <summary>
    ///     This method should be called when initializing MemoryWatchers to ensure that they do not have
    ///     default / zeroed values on initialization, which complicates or ruins autosplitter logic.
    /// </summary>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    private void PreLoadWatchers(Process gameProcess)
    {
        UpdateMemoryWatchers(gameProcess); // Loads Current values.
        UpdateMemoryWatchers(gameProcess); // Moves Current to Old and loads new Current values.
    }

    private static IntPtr GetAddressFromSignature(SignatureScanner scanner, AddressSignatureInfo sigInfo)
    {
        bool useMask = sigInfo.SignatureWithMasks is not null && sigInfo.SignatureWithMasks.Any();

        // Find bytes signature.
        SigScanTarget target = useMask
            ? new SigScanTarget(sigInfo.SignatureWithMasks)
            : new SigScanTarget(sigInfo.Signature);
        IntPtr signatureAddress = scanner.Scan(target);
        if (signatureAddress == IntPtr.Zero)
            throw new Exception($"Signature not found for {sigInfo.Name}: {string.Join(" ", useMask ? sigInfo.SignatureWithMasks : sigInfo.Signature.Select(static b => b.ToString("X2")))}");

        // Find the write instruction using the offset argument.
        if (sigInfo.OffsetsToWriteInstruction.Length <= 0)
            throw new Exception($"Signature info not coded properly for {sigInfo.Name}: missing offset.");

        int sigOffset = sigInfo.OffsetsToWriteInstruction[0].offset;
        if (GameData.GameVersion is not GameVersion.Unknown && sigInfo.OffsetsToWriteInstruction.Length > 1) // Overwrite offsets if different for a later known version.
            foreach ((_, int offset) in sigInfo.OffsetsToWriteInstruction.Where(static tuple => tuple.version is not null && (int)GameData.GameVersion >= (int)tuple.version))
                sigOffset = offset;

        IntPtr writeInstructionAddress = signatureAddress + sigOffset;
        byte[] instructionBytes = scanner.Process.ReadBytes(writeInstructionAddress, sigInfo.WriteInstructionLength);
        if (instructionBytes is null || instructionBytes.Length != sigInfo.WriteInstructionLength)
            throw new Exception($"Failed to read process memory at the write instruction at {writeInstructionAddress.ToString("X2")} for {sigInfo.Name}.");

        // Find the target address using the write instruction's offset.
        var extractedOffset = BitConverter.ToInt32(instructionBytes, sigInfo.WriteInstructionLength - 4);
        IntPtr addressAfterWriteInstruction = writeInstructionAddress + sigInfo.WriteInstructionLength;
        IntPtr effectiveAddress = addressAfterWriteInstruction + extractedOffset + sigInfo.EffectiveAddressOffset;

#if DEBUG
        long moduleOffset = effectiveAddress.ToInt64() - scanner.Address.ToInt64();
        LiveSplit.Options.Log.Warning($"Found signature for {sigInfo.Name} {string.Join(" ", useMask ? sigInfo.SignatureWithMasks : sigInfo.Signature.Select(static b => b.ToString("X2")))} at address {signatureAddress.ToString("X2")}.\n" +
                                      $"At address {writeInstructionAddress.ToString("X2")}, found bytes {string.Join(" ", instructionBytes.Select(static b => b.ToString("X2")))}.\n" +
                                      $"Extracted address {effectiveAddress.ToString("X2")} ({scanner.Address.ToString("X2")} + 0x{moduleOffset:X8})\n" +
                                      $"using extracted offset {extractedOffset:X2} and effective address offset {sigInfo.EffectiveAddressOffset:X2}.");
#endif

        // Return the address.
        return effectiveAddress;
    }

    /// <summary>Creates a <see cref="SignatureScanner" /> for the game's EXE.</summary>
    private static SignatureScanner CreateScannerForExe(Process gameProcess)
    {
        ProcessModuleWow64Safe module = gameProcess.MainModuleWow64Safe();
        IntPtr address = module.BaseAddress;
        int size = module.ModuleMemorySize;

        return new SignatureScanner(gameProcess, address, size);
    }

    /// <summary>Adds MemoryWatchers related to the game's EXE.</summary>
    private void AddWatchersExe(SignatureScanner scannerExe)
    {
        foreach (AddressSignatureInfo info in WatcherExeSignatureInfos)
        {
            IntPtr address = GetAddressFromSignature(scannerExe, info);
            _watchersExe.Add(info.MemoryWatcherFactory(address));
        }
    }

    /// <summary>Creates <see cref="SignatureScanner" />s for the game's DLLs.</summary>
    private static Dictionary<Game, SignatureScanner> CreateScannersForDlls(Process gameProcess)
    {
        var scanners = new Dictionary<Game, SignatureScanner>(3);

        foreach (ProcessModuleWow64Safe module in gameProcess.ModulesWow64Safe())
        {
            if (!GameModules.TryGetValue(module.ModuleName, out Game game))
                continue;

            IntPtr address = module.BaseAddress;
            int size = module.ModuleMemorySize;
            var scanner = new SignatureScanner(gameProcess, address, size);
            scanners.Add(game, scanner);
        }

        if (scanners.Count != 3)
            throw new Exception("Did not successfully scan all modules");

        return scanners;
    }

    /// <summary>Adds MemoryWatchers related to game DLLs.</summary>
    private void AddWatchersDlls(Dictionary<Game, SignatureScanner> scanners)
    {
        var signatureInfos = WatcherDllSignatureInfos;
        foreach (var gameSignatureInfo in signatureInfos)
        {
            AddressSignatureInfo info = gameSignatureInfo.Value;
            foreach (Game game in gameSignatureInfo.Key)
            {
                if (!BaseGames.Contains(game))
                    throw new ArgumentException("Base game required", nameof(game));

                SignatureScanner scanner = scanners[game];
                IntPtr address = GetAddressFromSignature(scanner, gameSignatureInfo.Value);
                MemoryWatcherList list = GetMemoryWatcherListForGame(game);

                list.Add(info.MemoryWatcherFactory(address));
            }
        }
    }

    #endregion

    internal void UpdateMemoryWatchers(Process gameProcess)
    {
        _watchersExe.UpdateAll(gameProcess);
        _watchersTR4R.UpdateAll(gameProcess);
        _watchersTR5R.UpdateAll(gameProcess);
        _watchersTR6R.UpdateAll(gameProcess);
    }
}
