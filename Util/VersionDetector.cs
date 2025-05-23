using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Util;

public class VersionDetector
{
    public const uint None = 0xBEEF;
    public const uint Unknown = 0xDEADBEEF;

    /// <summary>Strings used when searching for a running game <see cref="Process"/>.</summary>
    internal readonly List<string> ProcessSearchNames = [];

    /// <summary>Used to reasonably assure a potential game process is a known, unmodified EXE.</summary>
    /// <remarks>Ideally, the <see cref="uint"/> will be converted from some <see cref="Enum"/> for clarity.</remarks>
    internal readonly Dictionary<string, uint> VersionHashes = [];

    /// <summary>
    ///     Detects the version of a running game process using <see cref="ProcessSearchNames" /> and <see cref="VersionHashes" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="VersionDetectionResult" /> indicating the detection result:
    ///     <list type="bullet">
    ///         <item><see cref="VersionDetectionResult.None" /> if no matching process is found.</item>
    ///         <item><see cref="VersionDetectionResult.Found" /> if a process with a known version hash is found.</item>
    ///         <item><see cref="VersionDetectionResult.Unknown" /> if a process is found but its version hash is not recognized.</item>
    ///     </list>
    /// </returns>
    public VersionDetectionResult DetectVersion()
    {
        var processes = ProcessSearchNames.SelectMany(Process.GetProcessesByName).ToList();
        if (processes.Count == 0)
            return new VersionDetectionResult.None();

        string hash = processes[0].GetMd5Hash() ?? string.Empty;
        if (VersionHashes.TryGetValue(hash, out uint version))
            return new VersionDetectionResult.Found(processes[0], hash, version);
        return new VersionDetectionResult.Unknown(processes[0], hash);
    }
}