using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace TR456;

internal static class VersionDetector
{
    /// <summary>Strings used when searching for a running game <see cref="Process" />.</summary>
    private static readonly ImmutableList<string> ProcessSearchNames = ["tomb456"];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>The uint will be converted from <see cref="GameVersion" />.</remarks>
    private static readonly ImmutableDictionary<string, GameVersion> VersionHashes = new Dictionary<string, GameVersion>
    {
        { "CA258829147BD3BF932152BFFABBE4A1".ToLowerInvariant(), GameVersion.PublicV10 }, // EGS
        { "25FEE8EBB2FAE95BF13CABE151CB7A9F".ToLowerInvariant(), GameVersion.PublicV10 }, // GOG
        { "14479C2B293FAC5A8E175D0D540B7C77".ToLowerInvariant(), GameVersion.PublicV10 }, // Steam
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