// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace TR4;

/// <summary>
///     The game uses an array of 8-bit char values. Indices are re-used, with items re-assigned by the game's script.
/// </summary>
internal readonly struct PuzzleItems(
    byte item01, byte item02, byte item03, byte item04,
    byte item05, byte item06, byte item07, byte item08,
    byte item09, byte item10, byte item11, byte item12
)
{
    private byte Item01 { get; } = item01;
    private byte Item02 { get; } = item02;
    private byte Item03 { get; } = item03;
    private byte Item04 { get; } = item04;
    private byte Item05 { get; } = item05;
    private byte Item06 { get; } = item06;
    private byte Item07 { get; } = item07;
    private byte Item08 { get; } = item08;
    private byte Item09 { get; } = item09;
    private byte Item10 { get; } = item10;
    private byte Item11 { get; } = item11;
    private byte Item12 { get; } = item12;

    /// <summary>Non-unique item active during <see cref="Tr4Level.AngkorWat" />.</summary>
    public byte GoldenSkull => Item01;

    /// <summary>Unique item active during <see cref="Tr4Level.TheTombOfSeth" />.</summary>
    public byte EyeOfHorus       => Item01;

    /// <summary>Unique item active during <see cref="Tr4Level.TheTombOfSeth" />.</summary>
    public byte TheTimelessSands => Item02;

    /// <summary>Unique item active during <see cref="Tr4Level.BurialChambers" />.</summary>
    public byte TheHandOfOrion   => Item01;

    /// <summary>Unique item active during <see cref="Tr4Level.BurialChambers" />.</summary>
    public byte TheHandOfSirius  => Item02;

    /// <summary>Unique item active during <see cref="Tr4Level.BurialChambers" />.</summary>
    public byte ScarabTalisman   => Item03;

    /// <summary>Unique item active during <see cref="Tr4Level.BurialChambers" />.</summary>
    public byte TheGoldenSerpent => Item04;

    /// <summary>Unique item active during <see cref="Tr4Level.ValleyOfTheKings" /> and <see cref="Tr4Level.Kv5" />.</summary>
    public byte IgnitionKey => Item01;

    /// <summary>Unique item active during <see cref="Tr4Level.GreatHypostyleHall" />, and <see cref="Tr4Level.SacredLake" />.</summary>
    public byte SunTalisman => Item01;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.TempleOfKarnak" />, <see cref="Tr4Level.GreatHypostyleHall" />,
    ///     and <see cref="Tr4Level.SacredLake" />.
    /// </summary>
    public byte CanopicJar1 => Item02;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.TempleOfKarnak" />, <see cref="Tr4Level.GreatHypostyleHall" />,
    ///     and <see cref="Tr4Level.SacredLake" />.
    /// </summary>
    public byte CanopicJar2 => Item03;

    /// <summary>Unique item active during <see cref="Tr4Level.TombOfSemerkhet" />.</summary>
    public byte BaCartouche => Item04;

    /// <summary>Unique item active during <see cref="Tr4Level.TombOfSemerkhet" />.</summary>
    public byte RaCartouche => Item05;

    /// <summary>Unique item active during <see cref="Tr4Level.GuardianOfSemerkhet" />.</summary>
    public byte GoldenVraeus => Item06;

    /// <summary>Unique item active during <see cref="Tr4Level.GuardianOfSemerkhet" />.</summary>
    public byte GuardianKey  => Item07;

    /// <summary>
    ///     Non-unique item active during <see cref="Tr4Level.Alexandria"/>, <see cref="Tr4Level.CoastalRuins"/>,
    ///     <see cref="Tr4Level.PharosTempleOfIsis"/>, <see cref="Tr4Level.Catacombs"/>, and <see cref="Tr4Level.TempleOfPoseidon"/>.
    /// </summary>
    public byte Trident        => Item01;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.Alexandria" />, <see cref="Tr4Level.CoastalRuins" />,
    ///     <see cref="Tr4Level.PharosTempleOfIsis" />,
    ///     <see cref="Tr4Level.Catacombs" />, <see cref="Tr4Level.TempleOfPoseidon" />, and
    ///     <see cref="Tr4Level.TheLostLibrary" />.
    /// </summary>
    public byte MusicScroll    => Item02;

    /// <summary>
    ///     Non-unique item active during <see cref="Tr4Level.Alexandria" />, <see cref="Tr4Level.CoastalRuins" />, and
    ///     <see cref="Tr4Level.PharosTempleOfIsis" />.
    /// </summary>
    public byte GoldenStar     => Item03;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.Alexandria" />, <see cref="Tr4Level.CoastalRuins" />,
    ///     <see cref="Tr4Level.PharosTempleOfIsis" />, <see cref="Tr4Level.Catacombs" />,
    ///     <see cref="Tr4Level.TempleOfPoseidon" />, and <see cref="Tr4Level.TheLostLibrary" />.
    /// </summary>
    public byte HookAndPole    => Item04;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.Alexandria" />, <see cref="Tr4Level.CoastalRuins" />,
    ///     <see cref="Tr4Level.PharosTempleOfIsis" />, <see cref="Tr4Level.CleopatrasPalaces" />,
    ///     <see cref="Tr4Level.Catacombs" />, <see cref="Tr4Level.TempleOfPoseidon" />,
    ///     <see cref="Tr4Level.TheLostLibrary" />, and <see cref="Tr4Level.HallOfDemetrius" />.
    /// </summary>
    public byte PortalGuardian => Item05;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.Alexandria" />, <see cref="Tr4Level.CoastalRuins" />,
    ///     <see cref="Tr4Level.Catacombs" />, <see cref="Tr4Level.TempleOfPoseidon" />, and <see cref="Tr4Level.TheLostLibrary" />.
    /// </summary>
    public byte HorsemansGem   => Item06;

    /// <summary>
    ///     Non-unique item active during <see cref="Tr4Level.Alexandria" />, <see cref="Tr4Level.CoastalRuins" />,
    ///     <see cref="Tr4Level.PharosTempleOfIsis" />, <see cref="Tr4Level.CleopatrasPalaces" />, <see cref="Tr4Level.Catacombs" />,
    ///     <see cref="Tr4Level.TempleOfPoseidon" />, <see cref="Tr4Level.TheLostLibrary" />, and <see cref="Tr4Level.HallOfDemetrius" />.
    /// </summary>
    public byte PharosKnot     => Item10;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.Alexandria" />, <see cref="Tr4Level.CoastalRuins" />,
    ///     <see cref="Tr4Level.PharosTempleOfIsis" />, <see cref="Tr4Level.Catacombs" />, <see cref="Tr4Level.TempleOfPoseidon" />,
    ///     <see cref="Tr4Level.TheLostLibrary" />, and <see cref="Tr4Level.HallOfDemetrius" />.
    /// </summary>
    public byte PharosPillar   => Item11;

    /// <summary>
    ///     Non-unique item active during <see cref="Tr4Level.Alexandria" />, <see cref="Tr4Level.CoastalRuins" />,
    ///     <see cref="Tr4Level.PharosTempleOfIsis" />, <see cref="Tr4Level.CleopatrasPalaces" />, <see cref="Tr4Level.Catacombs" />,
    ///     <see cref="Tr4Level.TempleOfPoseidon" />, <see cref="Tr4Level.TheLostLibrary" />, and <see cref="Tr4Level.HallOfDemetrius" />.
    /// </summary>
    public byte BlackBeetle    => Item12;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.CityOfTheDead" />, <see cref="Tr4Level.Trenches" />,
    ///     <see cref="Tr4Level.ChambersOfTulun" />, <see cref="Tr4Level.StreetBazaar" />, and <see cref="Tr4Level.CitadelGate" />.
    /// </summary>
    public byte NitrousOxideFeeder => Item01;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.CityOfTheDead" />, <see cref="Tr4Level.Trenches" />,
    ///     <see cref="Tr4Level.ChambersOfTulun" />, <see cref="Tr4Level.StreetBazaar" />, and <see cref="Tr4Level.CitadelGate" />.
    /// </summary>
    public byte CarJack            => Item02;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.CityOfTheDead" />, <see cref="Tr4Level.Trenches" />,
    ///     <see cref="Tr4Level.ChambersOfTulun" />, see cref="Tr4Level.StreetBazaar" />, and <see cref="Tr4Level.CitadelGate" />.
    /// </summary>
    public byte RoomKey            => Item04;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.CityOfTheDead" />, <see cref="Tr4Level.Trenches" />,
    ///     <see cref="Tr4Level.ChambersOfTulun" />, <see cref="Tr4Level.StreetBazaar" />, and <see cref="Tr4Level.CitadelGate" />.
    /// </summary>
    public byte WeaponCodeKey      => Item05;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.CityOfTheDead" />, <see cref="Tr4Level.Trenches" />,
    ///     <see cref="Tr4Level.ChambersOfTulun" />, <see cref="Tr4Level.StreetBazaar" />, and <see cref="Tr4Level.CitadelGate" />.
    /// </summary>
    public byte MineDetonator      => Item08;

    /// <summary>Unique item active during <see cref="Tr4Level.SphinxComplex" />.</summary>
    public byte Shovel        => Item01;

    /// <summary>Unique item active during <see cref="Tr4Level.UnderneathTheSphinx" />.</summary>
    public byte StoneOfMaat   => Item01;

    /// <summary>Unique item active during <see cref="Tr4Level.UnderneathTheSphinx" />.</summary>
    public byte StoneOfKhepri => Item02;

    /// <summary>Unique item active during <see cref="Tr4Level.UnderneathTheSphinx" />.</summary>
    public byte StoneOfAtum   => Item03;

    /// <summary>Unique item active during <see cref="Tr4Level.UnderneathTheSphinx" />.</summary>
    public byte StoneOfRe     => Item04;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.SphinxComplex" />, <see cref="Tr4Level.UnderneathTheSphinx" />,
    ///     <see cref="Tr4Level.MenkauresPyramid" />, <see cref="Tr4Level.InsideMenkauresPyramid" />, <see cref="Tr4Level.TheMastabas" />,
    ///     <see cref="Tr4Level.TheGreatPyramid" />, <see cref="Tr4Level.KhufusQueensPyramid" />,
    ///     <see cref="Tr4Level.InsideTheGreatPyramid" />, and <see cref="Tr4Level.TempleOfHorus" />.
    /// </summary>
    public byte HolyScripture    => Item05;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.SphinxComplex" />, <see cref="Tr4Level.UnderneathTheSphinx" />,
    ///     <see cref="Tr4Level.MenkauresPyramid" />, <see cref="Tr4Level.InsideMenkauresPyramid" />,
    ///     <see cref="Tr4Level.TheMastabas" />, <see cref="Tr4Level.TheGreatPyramid" />, <see cref="Tr4Level.KhufusQueensPyramid" />, and
    ///     <see cref="Tr4Level.InsideTheGreatPyramid" />.
    /// </summary>
    public byte WesternShaftKey  => Item06;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.SphinxComplex" />, <see cref="Tr4Level.UnderneathTheSphinx" />,
    ///     <see cref="Tr4Level.MenkauresPyramid" />, <see cref="Tr4Level.InsideMenkauresPyramid" />, <see cref="Tr4Level.TheMastabas" />,
    ///     <see cref="Tr4Level.TheGreatPyramid" />, <see cref="Tr4Level.KhufusQueensPyramid" />, and <see cref="Tr4Level.InsideTheGreatPyramid" />.
    /// </summary>
    public byte NorthernShaftKey => Item07;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.TheMastabas" />, <see cref="Tr4Level.TheGreatPyramid" />,
    ///     <see cref="Tr4Level.KhufusQueensPyramid" /> and <see cref="Tr4Level.InsideTheGreatPyramid" />.
    /// </summary>
    public byte SouthernShaftKey => Item08;

    /// <summary>
    ///     Unique item active during <see cref="Tr4Level.TheMastabas" />, <see cref="Tr4Level.TheGreatPyramid" />,
    ///     <see cref="Tr4Level.KhufusQueensPyramid" /> and <see cref="Tr4Level.InsideTheGreatPyramid" />.
    /// </summary>
    public byte EasternShaftKey  => Item09;
}