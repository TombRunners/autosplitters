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
    internal static readonly ImmutableDictionary<Game, string> GameModules = new Dictionary<Game, string>(3)
    {
        { Game.Tr4, "tomb4.dll" },
        { Game.Tr5, "tomb5.dll" },
        { Game.Tr6, "tomb6.dll" },
    }.ToImmutableDictionary();

    /// <summary>Address signature information for the game's EXE.</summary>
    internal static readonly ImmutableHashSet<AddressSignatureInfo> WatcherExeSignatureInfos =
        new HashSet<AddressSignatureInfo>()
        {
            new()
            {
                Name = Constants.WatcherActiveGameName,
                DataType = typeof(int),
                Signature = [0x83, 0xE1, 0x03, 0x25, 0xFF, 0xFF, 0xE7, 0xFF],
                OffsetToWriteInstruction = 0x8A,
                WriteInstructionLength = 6,
                EffectiveAddressOffset = 0,
            },
            new()
            {
                Name = Constants.WatcherGFrameIndexName,
                DataType = typeof(int),
                Signature = [0x66, 0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8D, 0x84, 0x24, 0xB0, 0x00, 0x00, 0x00],
                OffsetToWriteInstruction = 0x20,
                WriteInstructionLength = 7,
                EffectiveAddressOffset = 0x208,
            },
        }.ToImmutableHashSet();

    /// <summary>Address signature information for the game's DLLs.</summary>
    internal static readonly ImmutableDictionary<Game[], AddressSignatureInfo> WatcherDllSignatureInfos =
        new Dictionary<Game[], AddressSignatureInfo>
        {
            {
                [Game.Tr4, Game.Tr5],
                new AddressSignatureInfo
                {
                    Name = Constants.WatcherIsLoadingName,
                    DataType = typeof(bool),
                    Signature = [0x45, 0x33, 0xDB, 0x48, 0xB8, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x45, 0x8B, 0xD3],
                    OffsetToWriteInstruction = 0x10,
                    WriteInstructionLength = 7,
                    EffectiveAddressOffset = 0,
                }
            },
        }.ToImmutableDictionary();

    #region EXE MemoryWatcher Definitions

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic related to the game's executable.</summary>
    internal readonly MemoryWatcherList WatchersExe = [];

    /// <summary>Gives the value of the active game, where TR4 is 0, TR5 is 1, TR6 is 2.</summary>
    /// <remarks>The value should be converted to <see cref="GameVersion" />.</remarks>
    internal MemoryWatcher<int> ActiveGame => (MemoryWatcher<int>)WatchersExe?[Constants.WatcherActiveGameName];

    internal MemoryWatcher<int> GFrameIndex => (MemoryWatcher<int>)WatchersExe?[Constants.WatcherGFrameIndexName];

    #endregion

    #region DLL MemoryWatcher Definitions

    /// <summary>Contains memory addresses related to the TR4R game DLL, accessible by named members, used in auto-splitting logic.</summary>
    internal readonly MemoryWatcherList WatchersTR4R = [];

    /// <summary>Contains memory addresses related to the TR5R game DLL, accessible by named members, used in auto-splitting logic.</summary>
    internal readonly MemoryWatcherList WatchersTR5R = [];

    /// <summary>Contains memory addresses related to the TR6R game DLL, accessible by named members, used in auto-splitting logic.</summary>
    internal readonly MemoryWatcherList WatchersTR6R = [];

    private MemoryWatcherList GetMemoryWatcherListForGame(Game game) =>
        game switch
        {
            Game.Tr4 or Game.Tr4NgPlus or Game.Tr4TheTimesExclusive => WatchersTR4R,
            Game.Tr5 or Game.Tr5NgPlus => WatchersTR5R,
            Game.Tr6 or Game.Tr6NgPlus => WatchersTR6R,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "Invalid game")
        };

    internal MemoryWatcher<bool> IsLoading(Game game)
    {
        MemoryWatcherList list = GetMemoryWatcherListForGame(game);
        return (MemoryWatcher<bool>)list?[Constants.WatcherIsLoadingName];
    }

    #endregion

    /// <summary>Sets addresses based on <paramref name="version" />.</summary>
    /// <param name="version">Game version to base addresses on</param>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    internal void InitializeMemoryWatchers(GameVersion version, Process gameProcess)
    {
        WatchersExe.Clear();
        WatchersTR4R.Clear();
        WatchersTR5R.Clear();
        WatchersTR6R.Clear();

        SignatureScanner scannerExe = CreateScannerForExe(gameProcess);
        var scannersDlls = CreateScannersForDlls(gameProcess);

        switch (version)
        {
            case GameVersion.PublicV10:
                AddWatchersExe(scannerExe); // tomb456.exe
                AddWatchersDlls(scannersDlls); // tomb4.dll, tomb5.dll, tomb6.dll
                break;

            case GameVersion.None:
            case GameVersion.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(version), version, null);
        }

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

    internal void UpdateMemoryWatchers(Process gameProcess)
    {
        WatchersExe.UpdateAll(gameProcess);
        WatchersTR4R.UpdateAll(gameProcess);
        WatchersTR5R.UpdateAll(gameProcess);
        WatchersTR6R.UpdateAll(gameProcess);
    }

    private static IntPtr GetAddressFromSignature(SignatureScanner scanner, AddressSignatureInfo sigInfo)
    {
        // Find bytes signature.
        var target = new SigScanTarget(sigInfo.Signature);
        IntPtr signatureAddress = scanner.Scan(target);
        if (signatureAddress == IntPtr.Zero)
            throw new Exception("Signature not found.");
        LiveSplit.Options.Log.Warning($"Found signature {string.Join(" ", sigInfo.Signature.Select(static b => b.ToString("X2")))} at address {signatureAddress.ToString("X2")}");

        // Find the write instruction using the offset argument.
        IntPtr writeInstructionAddress = signatureAddress + sigInfo.OffsetToWriteInstruction;
        byte[] instructionBytes = scanner.Process.ReadBytes(writeInstructionAddress, sigInfo.WriteInstructionLength);
        if (instructionBytes is null || instructionBytes.Length != sigInfo.WriteInstructionLength)
            throw new Exception("Failed to read process memory at the write instruction.");
        LiveSplit.Options.Log.Warning($"At address {writeInstructionAddress.ToString("X2")}, found bytes {string.Join(" ", instructionBytes.Select(static b => b.ToString("X2")))}");

        // Find the target address using the write instruction's offset.
        var extractedOffset = BitConverter.ToInt32(instructionBytes, sigInfo.WriteInstructionLength - 4);
        IntPtr addressAfterWriteInstruction = writeInstructionAddress + sigInfo.WriteInstructionLength;
        IntPtr effectiveAddress = addressAfterWriteInstruction + extractedOffset + sigInfo.EffectiveAddressOffset;
        LiveSplit.Options.Log.Warning($"Extracted address {effectiveAddress.ToString("X2")} using extracted offset {extractedOffset:X2} and effective address offset {sigInfo.EffectiveAddressOffset:X2}");

        // Return the address.
        return effectiveAddress;
    }

    private static SignatureScanner CreateScannerForExe(Process gameProcess)
    {
        ProcessModuleWow64Safe module = gameProcess.MainModuleWow64Safe();
        IntPtr address = module.BaseAddress;
        int size = module.ModuleMemorySize;

        return new SignatureScanner(gameProcess, address, size);
    }

    private void AddWatchersExe(SignatureScanner scannerExe)
    {
        foreach (AddressSignatureInfo info in WatcherExeSignatureInfos)
        {
            IntPtr address = GetAddressFromSignature(scannerExe, info);
            CreateWatcherExe(info.Name, address);
        }
    }

    private void CreateWatcherExe(string name, IntPtr address)
    {
        switch (name)
        {
            case Constants.WatcherActiveGameName:
            case Constants.WatcherGFrameIndexName:
                WatchersExe.Add(new MemoryWatcher<int>(new DeepPointer(address)) { Name = name });
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static Dictionary<Game, SignatureScanner> CreateScannersForDlls(Process gameProcess)
    {
        var scanners = new Dictionary<Game, SignatureScanner>(3);

        foreach (ProcessModuleWow64Safe module in gameProcess.ModulesWow64Safe())
        {
            Game game;
            switch (module.ModuleName)
            {
                case "tomb4.dll":
                    game = Game.Tr4;
                    break;
                case "tomb5.dll":
                    game = Game.Tr5;
                    break;
                case "tomb6.dll":
                    game = Game.Tr6;
                    break;
                default:
                    continue;
            }

            IntPtr address = module.BaseAddress;
            int size = module.ModuleMemorySize;
            var scanner = new SignatureScanner(gameProcess, address, size);
            scanners.Add(game, scanner);
        }

        if (scanners.Count != 3)
            throw new Exception("Did not successfully scan all modules");

        return scanners;
    }

    /// <summary>Adds MemoryWatchers which are common across all games.</summary>
    private void AddWatchersDlls(Dictionary<Game, SignatureScanner> scanners)
    {
        var signatureInfos = WatcherDllSignatureInfos;
        foreach (var gameSignatureInfo in signatureInfos)
        {
            foreach (Game game in gameSignatureInfo.Key)
            {
                if (!BaseGames.Contains(game))
                    throw new ArgumentException("Base game required", nameof(game));

                SignatureScanner scanner = scanners[game];
                IntPtr address = GetAddressFromSignature(scanner, gameSignatureInfo.Value);
                CreateWatchersForDlls(gameSignatureInfo.Value.Name, game, address);
            }
        }
    }

    private void CreateWatchersForDlls(string name, Game game, IntPtr address)
    {
        MemoryWatcherList targetList = GetMemoryWatcherListForGame(game);
        switch (name)
        {
            case Constants.WatcherIsLoadingName:
                targetList.Add(new MemoryWatcher<bool>(new DeepPointer(address)) { Name = name });
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
