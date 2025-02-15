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

    /// <summary>Gives the value of the active game, where TR4 is 0, TR5 is 1, TR6 is 2.</summary>
    /// <remarks>The value should be converted to <see cref="GameVersion" />.</remarks>
    internal MemoryWatcher<int> ActiveGame => (MemoryWatcher<int>)Watchers?["ActiveGame"];

    #endregion

    /// <summary>Sets addresses based on <paramref name="version" />.</summary>
    /// <param name="version">Game version to base addresses on</param>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    internal void InitializeMemoryWatchers(GameVersion version, Process gameProcess)
    {
        Watchers.Clear();

        var scanner = CreateSignatureScanner(gameProcess);

        switch (version)
        {
            case GameVersion.PublicV10:
                // Base game EXE (tomb456.exe)
                var gameAddressBytes = new byte[] { 0x83, 0xE1, 0x03, 0x25, 0xFF, 0xFF, 0xE7, 0xFF };
                var activeGameAddress = GetAddressFromSignatureWithOffsetToWriteInstruction(scanner, gameAddressBytes, 0x8A);
                Watchers.Add(new MemoryWatcher<int>(activeGameAddress) { Name = "ActiveGame" });
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

    private SignatureScanner CreateSignatureScanner(Process gameProcess)
    {
        var module = gameProcess.MainModuleWow64Safe();
        var address = module.BaseAddress;
        int size = module.ModuleMemorySize;

        return new SignatureScanner(gameProcess, address, size);
    }

    private static DeepPointer GetAddressFromSignatureWithOffsetToWriteInstruction(SignatureScanner scanner, byte[] signature, int offsetFromSignature = 0)
    {
        LiveSplit.Options.Log.Warning($"Attempting scan for bytes {string.Join(" ", signature)}");
        var target = new SigScanTarget(signature);
        var signatureAddress = scanner.Scan(target);
        if (signatureAddress == IntPtr.Zero)
            throw new Exception("Signature not found.");
        LiveSplit.Options.Log.Warning($"Found bytes at address {signatureAddress.ToString("X")}");

        var writeInstructionAddress = signatureAddress + offsetFromSignature;
        const int instructionLength = 6;
        byte[] instructionBytes = scanner.Process.ReadBytes(writeInstructionAddress, instructionLength);
        if (instructionBytes is not { Length: instructionLength })
            throw new Exception("Failed to read process memory at the write instruction.");
        LiveSplit.Options.Log.Warning($"At address {writeInstructionAddress.ToString("X")}, found bytes {string.Join(" ", instructionBytes)}");

        var extractedOffset = BitConverter.ToInt32(instructionBytes, 2);
        LiveSplit.Options.Log.Warning($"Extracted offset {extractedOffset:X}");

        var addressAfterWriteInstruction = writeInstructionAddress + instructionLength;
        var effectiveAddress = addressAfterWriteInstruction + extractedOffset;
        LiveSplit.Options.Log.Warning($"Extracted address {effectiveAddress.ToString("X")}");

        return new DeepPointer(effectiveAddress);
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
