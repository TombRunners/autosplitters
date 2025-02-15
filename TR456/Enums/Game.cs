namespace TR456;

/// <summary>Different Games that can be played.</summary>
/// <remarks>
///     NG+ is considered a different game due to some major differences between the NG+ and NG experiences.
///     And the distinction here makes it easier to separate and form dependent logic.
/// </remarks>
public enum Game
{
    Tr4 = 0,
    Tr4NgPlus = 1,
    Tr4TheTimesExclusive = 2,
    Tr5 = 3,
    Tr5NgPlus = 4,
    Tr6 = 6,
    Tr6NgPlus = 7,
}