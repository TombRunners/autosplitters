using System.ComponentModel;

namespace TR456;

/// <summary>Different Games that can be played.</summary>
/// <remarks>
///     NG+ is considered a different game due to some major differences between the NG+ and NG experiences.
///     And the distinction here makes it easier to separate and form dependent logic.
/// </remarks>
public enum Game
{
    [Description("TR4")]
    Tr4 = 0,

    [Description("TR5")]
    Tr5 = 1,

    [Description("TR6")]
    Tr6 = 2,

    [Description("TR4 New Game+")]
    Tr4NgPlus = 3,

    [Description("TR5 New Game+")]
    Tr5NgPlus = 4,

    [Description("TR6 New Game+")]
    Tr6NgPlus = 5,

    [Description("The Times Exclusive")]
    Tr4TheTimesExclusive = 6,
}