using System;

namespace TR4;

public static class Tr4LevelExtensions
{
    public static string Name(this Tr4Level level)
        => level switch
        {
            Tr4Level.MainMenu => "Main Menu",
            Tr4Level.AngkorWat => "Angkor Wat",
            Tr4Level.RaceForTheIris => "Race for the Iris",
            Tr4Level.TheTombOfSeth => "The Tomb of Seth",
            Tr4Level.BurialChambers => "Burial Chambers",
            Tr4Level.ValleyOfTheKings => "Valley of the Kings",
            Tr4Level.Kv5 => "KV5",
            Tr4Level.TempleOfKarnak => "Temple of Karnak",
            Tr4Level.GreatHypostyleHall => "Great Hypostyle Hall",
            Tr4Level.SacredLake => "Sacred Lake",
            Tr4Level.TombOfSemerkhet => "Tomb of Semerkhet",
            Tr4Level.GuardianOfSemerkhet => "Guardian of Semerkhet",
            Tr4Level.DesertRailroad => "Desert Railroad",
            Tr4Level.Alexandria => "Alexandria",
            Tr4Level.CoastalRuins => "Coastal Ruins",
            Tr4Level.PharosTempleOfIsis => "Pharos, Temple of Isis",
            Tr4Level.CleopatrasPalaces => "Cleopatra's Palaces",
            Tr4Level.Catacombs => "Catacombs",
            Tr4Level.TempleOfPoseidon => "Temple of Poseidon",
            Tr4Level.TheLostLibrary => "The Lost Library",
            Tr4Level.HallOfDemetrius => "Hall of Demetrius",
            Tr4Level.CityOfTheDead => "City of the Dead",
            Tr4Level.Trenches => "Trenches",
            Tr4Level.ChambersOfTulun => "Chambers of Tulun",
            Tr4Level.StreetBazaar => "Street Bazaar",
            Tr4Level.CitadelGate => "Citadel Gate",
            Tr4Level.Citadel => "Citadel",
            Tr4Level.SphinxComplex => "Sphinx Complex",
            Tr4Level.UnderneathTheSphinx => "Underneath the Sphinx",
            Tr4Level.MenkauresPyramid => "Menkaure's Pyramid",
            Tr4Level.InsideMenkauresPyramid => "Inside Menkaure's Pyramid",
            Tr4Level.TheMastabas => "The Mastabas",
            Tr4Level.TheGreatPyramid => "The Great Pyramid",
            Tr4Level.KhufusQueensPyramid => "Khufu's Queen's Pyramids",
            Tr4Level.InsideTheGreatPyramid => "Inside the Great Pyramid",
            Tr4Level.TempleOfHorus => "Temple of Horus",
            Tr4Level.HorusBoss => "Horus Boss",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unknown level"),
        };
}