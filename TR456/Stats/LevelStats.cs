namespace TR456;

/// <inheritdoc />
/// <summary>Stats for a level, including its ID, IGT, and maximum completions.</summary>
public readonly record struct LevelStats()
{
    public ulong LevelId { get; init; } = 0;
    public uint Igt { get; init; } = 0;
    public uint MaxCompletions { get; init; } = 1;
}