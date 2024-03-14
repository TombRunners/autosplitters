using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace TR123;

internal static class VersionDetector
{
    /// <summary>Strings used when searching for a running game <see cref="Process" />.</summary>
    private static readonly ImmutableList<string> ProcessSearchNames = ["tomb123"];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>The uint will be converted from <see cref="GameVersion" />.</remarks>
    private static readonly ImmutableDictionary<string, GameVersion> VersionHashes = new Dictionary<string, GameVersion>
    {
        { "0C0C1C466DAE013ABBB976F11B52C726".ToLowerInvariant(), GameVersion.EgsDebug },
        { "0A937857C0AF755AEEAA98F4520CA0D2".ToLowerInvariant(), GameVersion.PublicV10 },
        { "769B1016F945167C48C6837505E37748".ToLowerInvariant(), GameVersion.PublicV101 },
        { "5B1644AFFD7BAD65B2AC5D76F15139C6".ToLowerInvariant(), GameVersion.PublicV102 },
    }.ToImmutableDictionary();

    public static GameVersion DetectVersion(out Process gameProcess, out string hash)
    {
        gameProcess = null;
        hash = string.Empty;

        // Find game Processes.
        var processes = ProcessSearchNames.SelectMany(Process.GetProcessesByName).ToList();
        if (processes.Count == 0)
            return GameVersion.None;

        // Try finding a match from known version hashes.
        foreach (var p in processes)
        {
            string foundHash = p.GetMd5Hash();
            if (!VersionHashes.TryGetValue(foundHash, out var version))
                continue;

            hash = foundHash;
            gameProcess = p;
            return version;
        }

        return GameVersion.Unknown;
    }
}