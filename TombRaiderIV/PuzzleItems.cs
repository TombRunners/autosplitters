namespace TR4
{
    /// <summary>
    ///     The game uses an array of 8-bit char values. Indices are re-used, with items re-assigned by the game's script.
    /// </summary>
    internal struct PuzzleItems
    {
        private byte Item01 { get; set; }
        private byte Item02 { get; set; }
        private byte Item03 { get; set; }
        private byte Item04 { get; set; }
        private byte Item05 { get; set; }
        private byte Item06 { get; set; }
        private byte Item07 { get; set; }
        private byte Item08 { get; set; }
        private byte Item09 { get; set; }
        private byte Item10 { get; set; }
        private byte Item11 { get; set; }
        private byte Item12 { get; set; }

        public PuzzleItems(
            byte item01, byte item02, byte item03, byte item04, 
            byte item05, byte item06, byte item07, byte item08, 
            byte item09, byte item10, byte item11, byte item12
        )
        {
            Item01 = item01;
            Item02 = item02;
            Item03 = item03;
            Item04 = item04;
            Item05 = item05;
            Item06 = item06;
            Item07 = item07;
            Item08 = item08;
            Item09 = item09;
            Item10 = item10;
            Item11 = item11;
            Item12 = item12;
        }

        /// <summary>Non-unique item active during <see cref="Level.AngkorWat"/>.</summary>
        public byte GoldenSkull => Item01;

        /// <summary>Unique item active during <see cref="Level.TheTombOfSeth"/>.</summary>
        public byte EyeOfHorus       => Item01;
        /// <summary>Unique item active during <see cref="Level.TheTombOfSeth"/>.</summary>
        public byte TheTimelessSands => Item02;

        /// <summary>Unique item active during <see cref="Level.BurialChambers"/>.</summary>
        public byte TheHandOfOrion   => Item01;                                  
        /// <summary>Unique item active during <see cref="Level.BurialChambers"/>.</summary>
        public byte TheHandOfSirius  => Item02;                                  
        /// <summary>Unique item active during <see cref="Level.BurialChambers"/>.</summary>
        public byte ScarabTalisman   => Item03;                                  
        /// <summary>Unique item active during <see cref="Level.BurialChambers"/>.</summary>
        public byte TheGoldenSerpent => Item04;

        /// <summary>Unique item active during <see cref="Level.ValleyOfTheKings"/> and <see cref="Level.KV5"/>.</summary>
        public byte IgnitionKey => Item01;

        /// <summary>Unique item active during <see cref="Level.GreatHypostyleHall"/>, and <see cref="Level.SacredLake"/>.</summary>
        public byte SunTalisman => Item01;
        /// <summary>Unique item active during <see cref="Level.TempleofKarnak"/>, <see cref="Level.GreatHypostyleHall"/>, and <see cref="Level.SacredLake"/>.</summary>
        public byte CanopicJar1 => Item02;
        /// <summary>Unique item active during <see cref="Level.TempleofKarnak"/>, <see cref="Level.GreatHypostyleHall"/>, and <see cref="Level.SacredLake"/>.</summary>
        public byte CanopicJar2 => Item03;

        /// <summary>Unique item active during <see cref="Level.TombofSemerkhet"/>.</summary>
        public byte BaCartouche => Item04;
        /// <summary>Unique item active during <see cref="Level.TombofSemerkhet"/>.</summary>
        public byte RaCartouche => Item05;

        /// <summary>Unique item active during <see cref="Level.GuardianofSemerkhet"/>.</summary>
        public byte GoldenVraeus => Item06;
        /// <summary>Unique item active during <see cref="Level.GuardianofSemerkhet"/>.</summary>
        public byte GuardianKey  => Item07;

        /// <summary>
        ///     Non-unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, <see cref="Level.PharosTempleOfIsis"/>, 
        ///     <see cref="Level.Catacombs"/>, and <see cref="Level.TempleOfPoseidon"/>.
        /// </summary>
        public byte Trident         => Item01;
        /// <summary>
        ///     Unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, <see cref="Level.PharosTempleOfIsis"/>, 
        ///     <see cref="Level.Catacombs"/>, <see cref="Level.TempleOfPoseidon"/>, and <see cref="Level.TheLostLibrary"/>.
        /// </summary>
        public byte MusicScroll     => Item02;
        /// <summary>Non-unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, and <see cref="Level.PharosTempleOfIsis"/>.</summary>
        public byte GoldenStar      => Item03;
        /// <summary>
        ///     Unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, <see cref="Level.PharosTempleOfIsis"/>, 
        ///     <see cref="Level.Catacombs"/>, <see cref="Level.TempleOfPoseidon"/>, and <see cref="Level.TheLostLibrary"/>.
        /// </summary>
        public byte HookAndPole     => Item04;
        /// <summary>
        ///     Unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, <see cref="Level.PharosTempleOfIsis"/>, 
        ///     <see cref="Level.CleopatrasPalaces"/>, <see cref="Level.Catacombs"/>, <see cref="Level.TempleOfPoseidon"/>,
        ///     <see cref="Level.TheLostLibrary"/>, and <see cref="Level.HallOfDemetrius"/>.
        /// </summary>
        public byte PortalGuardian  => Item05;
        /// <summary>
        ///     Unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, <see cref="Level.Catacombs"/>,
        ///     <see cref="Level.TempleOfPoseidon"/>, and <see cref="Level.TheLostLibrary"/>.
        /// </summary>
        public byte HorsemansGem    => Item06;
        /// <summary>
        ///     Non-unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, <see cref="Level.PharosTempleOfIsis"/>,
        ///     <see cref="Level.CleopatrasPalaces"/>, <see cref="Level.Catacombs"/>, <see cref="Level.TempleOfPoseidon"/>,
        ///     <see cref="Level.TheLostLibrary"/>, and <see cref="Level.HallOfDemetrius"/>.
        /// </summary>
        public byte PharosKnot      => Item10;
        /// <summary>
        ///     Unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, <see cref="Level.PharosTempleOfIsis"/>,
        ///     <see cref="Level.Catacombs"/>, <see cref="Level.TempleOfPoseidon"/>, <see cref="Level.TheLostLibrary"/>, and <see cref="Level.HallOfDemetrius"/>.
        /// </summary>
        public byte PharosPillar    => Item11;
        /// <summary>
        ///     Non-unique item active during <see cref="Level.Alexandria"/>, <see cref="Level.CoastalRuins"/>, <see cref="Level.PharosTempleOfIsis"/>,
        ///     <see cref="Level.CleopatrasPalaces"/>, <see cref="Level.Catacombs"/>, <see cref="Level.TempleOfPoseidon"/>,
        ///     <see cref="Level.TheLostLibrary"/>, and <see cref="Level.HallOfDemetrius"/>.
        /// </summary>
        public byte BlackBeetle     => Item12;

        /// <summary>
        ///     Unique item active during <see cref="Level.CityOfTheDead"/>, <see cref="Level.Trenches"/>, <see cref="Level.ChambersOfTulun"/>, 
        ///     <see cref="Level.StreetBazaar"/>, and <see cref="Level.CitadelGate"/>.
        /// </summary>
        public byte NitrousOxideFeeder => Item01;
        /// <summary>
        ///     Unique item active during <see cref="Level.CityOfTheDead"/>, <see cref="Level.Trenches"/>, <see cref="Level.ChambersOfTulun"/>, 
        ///     <see cref="Level.StreetBazaar"/>, and <see cref="Level.CitadelGate"/>.
        /// </summary>
        public byte CarJack            => Item02;
        /// <summary>
        ///     Unique item active during <see cref="Level.CityOfTheDead"/>, <see cref="Level.Trenches"/>, <see cref="Level.ChambersOfTulun"/>, 
        ///     <see cref="Level.StreetBazaar"/>, and <see cref="Level.CitadelGate"/>.
        /// </summary>
        public byte RoomKey            => Item04;
        /// <summary>
        ///     Unique item active during <see cref="Level.CityOfTheDead"/>, <see cref="Level.Trenches"/>, <see cref="Level.ChambersOfTulun"/>, 
        ///     <see cref="Level.StreetBazaar"/>, and <see cref="Level.CitadelGate"/>.
        /// </summary>
        public byte WeaponCodeKey      => Item05;
        /// <summary>
        ///     Unique item active during <see cref="Level.CityOfTheDead"/>, <see cref="Level.Trenches"/>, <see cref="Level.ChambersOfTulun"/>, 
        ///     <see cref="Level.StreetBazaar"/>, and <see cref="Level.CitadelGate"/>.
        /// </summary>
        public byte MineDetonator      => Item08;

        /// <summary>Unique item active during <see cref="Level.SphinxComplex"/>.</summary>
        public byte Shovel => Item01;
        /// <summary>Unique item active during <see cref="Level.UnderneathTheSphinx"/>.</summary>
        public byte StoneOfMaat   => Item01;
        /// <summary>Unique item active during <see cref="Level.UnderneathTheSphinx"/>.</summary>
        public byte StoneOfKhepri => Item02;
        /// <summary>Unique item active during <see cref="Level.UnderneathTheSphinx"/>.</summary>
        public byte StoneOfAtum   => Item03;
        /// <summary>Unique item active during <see cref="Level.UnderneathTheSphinx"/>.</summary>
        public byte StoneOfRe     => Item04;
        /// <summary>
        ///     Unique item active during <see cref="Level.SphinxComplex"/>, <see cref="Level.UnderneathTheSphinx"/>, <see cref="Level.MenkauresPyramid"/>,
        ///     <see cref="Level.InsideMenkauresPyramid"/>, <see cref="Level.TheMastabas"/>, <see cref="Level.TheGreatPyramid"/>,
        ///     <see cref="Level.KhufusQueensPyramid"/>, <see cref="Level.InsideTheGreatPyramid"/>, and <see cref="Level.TempleOfHorus"/>.
        /// </summary>
        public byte HolyScripture    => Item05;
        /// <summary>
        ///     Unique item active during <see cref="Level.SphinxComplex"/>, <see cref="Level.UnderneathTheSphinx"/>,
        ///     <see cref="Level.MenkauresPyramid"/>, <see cref="Level.InsideMenkauresPyramid"/>, <see cref="Level.TheMastabas"/>,
        ///     <see cref="Level.TheGreatPyramid"/>, <see cref="Level.KhufusQueensPyramid"/> and <see cref="Level.InsideTheGreatPyramid"/>.
        /// </summary>
        public byte WesternShaftKey  => Item06;
        /// <summary>
        ///     Unique item active during <see cref="Level.SphinxComplex"/>, <see cref="Level.UnderneathTheSphinx"/>,
        ///     <see cref="Level.MenkauresPyramid"/>, <see cref="Level.InsideMenkauresPyramid"/>, <see cref="Level.TheMastabas"/>,
        ///     <see cref="Level.TheGreatPyramid"/>, <see cref="Level.KhufusQueensPyramid"/> and <see cref="Level.InsideTheGreatPyramid"/>.
        /// </summary>
        public byte NorthernShaftKey => Item07;
        /// <summary>
        ///     Unique item active during <see cref="Level.TheMastabas"/>, <see cref="Level.TheGreatPyramid"/>, 
        ///     <see cref="Level.KhufusQueensPyramid"/> and <see cref="Level.InsideTheGreatPyramid"/>.
        /// </summary>
        public byte SouthernShaftKey => Item08;
        /// <summary>
        ///     Unique item active during <see cref="Level.TheMastabas"/>, <see cref="Level.TheGreatPyramid"/>, 
        ///     <see cref="Level.KhufusQueensPyramid"/> and <see cref="Level.InsideTheGreatPyramid"/>.
        /// </summary>
        public byte EasternShaftKey  => Item09;
    }
}