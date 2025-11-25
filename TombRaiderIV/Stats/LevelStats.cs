namespace TR4;

/// <inheritdoc />
/// <summary>Stats for a level, including its ID, IGT, and maximum completions.</summary>
public readonly record struct LevelStats()
{
    public ulong LevelId { get; init; } = 0;
    public bool Ignored { get; init; } = false;
    public TransitionDirection Direction { get; init; } = TransitionDirection.OneWayFromLower;
}