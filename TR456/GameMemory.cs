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

    internal static readonly ImmutableHashSet<AddressSignatureInfo> WatcherSignatureInfos =
        new HashSet<AddressSignatureInfo>()
        {
            new()
            {
                Name = Constants.WatcherActiveGameName,
                DataType = typeof(int),
                Signature = [0x83, 0xE1, 0x03, 0x25, 0xFF, 0xFF, 0xE7, 0xFF],
                OffsetToWriteInstruction = 0x8A,
                WriteInstructionLength = 6,
            },
        }.ToImmutableHashSet();

    #region MemoryWatcher Definitions

    /// <summary>Contains memory addresses, accessible by named members, used in auto-splitting logic.</summary>
    internal readonly MemoryWatcherList Watchers = [];

    /// <summary>Gives the value of the active game, where TR4 is 0, TR5 is 1, TR6 is 2.</summary>
    /// <remarks>The value should be converted to <see cref="GameVersion" />.</remarks>
    internal MemoryWatcher<int> ActiveGame => (MemoryWatcher<int>)Watchers?[Constants.WatcherActiveGameName];

    #endregion

    /// <summary>Sets addresses based on <paramref name="version" />.</summary>
    /// <param name="version">Game version to base addresses on</param>
    /// <param name="gameProcess"><see cref="Process" /> of the game</param>
    internal void InitializeMemoryWatchers(GameVersion version, Process gameProcess)
    {
        Watchers.Clear();

        SignatureScanner scanner = CreateSignatureScanner(gameProcess);

        switch (version)
        {
            case GameVersion.PublicV10:
                // Base game EXE (tomb456.exe)
                foreach (AddressSignatureInfo info in WatcherSignatureInfos)
                {
                    IntPtr address = GetAddressFromSignature(scanner, info);
                    CreateWatcher(info.Name, address);
                }

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

    private static SignatureScanner CreateSignatureScanner(Process gameProcess)
    {
        ProcessModuleWow64Safe module = gameProcess.MainModuleWow64Safe();
        IntPtr address = module.BaseAddress;
        int size = module.ModuleMemorySize;

        return new SignatureScanner(gameProcess, address, size);
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
        IntPtr effectiveAddress = addressAfterWriteInstruction + extractedOffset;
        LiveSplit.Options.Log.Warning($"Extracted address {effectiveAddress.ToString("X2")} using extracted offset {extractedOffset:X2}");

        // Return the address.
        return effectiveAddress;
    }

    private void CreateWatcher(string name, IntPtr address)
    {
        switch (name)
        {
            case Constants.WatcherActiveGameName:
                Watchers.Add(new MemoryWatcher<int>(new DeepPointer(address)) { Name = name });
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>Adds MemoryWatchers which are common across all games.</summary>
    /// <param name="version">Game version</param>
    private void AddCommonDllWatchers(GameVersion version)
    {
        foreach (Game game in BaseGames)
        {
            string moduleName = GameModules[game];
            GameAddresses addresses = GameVersionAddresses[version][game];
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
