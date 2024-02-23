namespace TR123;

public readonly record struct GameAddresses
{
    public int FirstLevelTime { get; init; }
    public int Health { get; init; }
    public int Level { get; init; }
    public int LevelComplete { get; init; }
    public int LevelIgt { get; init; }
}