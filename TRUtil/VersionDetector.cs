using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TRUtil;

public class VersionDetector()
{
    internal const uint NoneOrUndetectedValue = 0;
    private const uint UnknownValue = 0xDEADBEEF;

    /// <summary>Strings used when searching for a running game <see cref="Process"/>.</summary>
    internal readonly List<string> ProcessSearchNames = [];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>Ideally, this will be converted from some <see cref="Enum"/> for clarity.</remarks>
    internal readonly Dictionary<string, uint> VersionHashes = [];

    public uint DetectVersion(out Process gameProcess, out string hash)
    {
        gameProcess = null;
        hash = string.Empty;

        // Find game Processes.
        var processes = ProcessSearchNames.SelectMany(Process.GetProcessesByName).ToList();
        if (processes.Count == 0)
            return NoneOrUndetectedValue;

        // Try finding a match from known version hashes.
        foreach (var p in processes)
        {
            string foundHash = p.GetMd5Hash();
            if (!VersionHashes.TryGetValue(foundHash, out uint version))
                continue;

            hash = foundHash;
            gameProcess = p;
            return version;
        }

        return UnknownValue;
    }
}