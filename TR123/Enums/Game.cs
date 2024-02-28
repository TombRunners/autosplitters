namespace TR123;

/// <summary>Different Games that can be played.</summary>
/// <remarks>
///     NG+ is considered a different game due to some major differences between the NG+ and NG experiences.
///     And the distinction here makes it easier to separate and form dependent logic.
/// </remarks>
public enum Game
{
    Tr1 = 0,
    Tr1NgPlus = 1,
    Tr1Ub = 2,
    Tr2 = 3,
    Tr2NgPlus = 4,
    Tr2GoldenMask = 5,
    Tr3 = 6,
    Tr3NgPlus = 7,
    Tr3TheLostArtifact = 8,
}