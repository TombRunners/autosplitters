using System.Diagnostics;

namespace Util;

public abstract record VersionDetectionResult
{
    public sealed record None : VersionDetectionResult;

    public sealed record Unknown(Process Process, string Hash) : VersionDetectionResult;

    public sealed record Found(Process Process, string Hash, uint Version) : VersionDetectionResult;
}