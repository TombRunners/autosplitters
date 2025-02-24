using System.ComponentModel;

namespace TR456;

/// <summary>The game's level values.</summary>
public enum Tr6Level
{
    [Description("Main Menu / Credits")]
    MainMenu = 00,

    // Paris
    [Description("Parisian Back Streets")]
    ParisianBackStreets = 01,

    [Description("Derelict Apartment Block")]
    DerelictApartmentBlock = 02,

    [Description("Industrial Rooftops")]
    IndustrialRooftops = 03,

    [Description("Margot Carvier's Apartment")]
    MargotCarviersApartment = 04,

    [Description("Parisian Ghetto")]
    ParisianGhetto = 05,

    [Description("The Serpent Rouge")]
    TheSerpentRouge = 06,

    [Description("St. Aicard's Graveyard")]
    StAicardsGraveyard = 07,

    [Description("Bouchard's Hideout")]
    BouchardsHideout = 08,

    [Description("Louvre Storm Drains")]
    LouvreStormDrains = 09,

    [Description("Louvre Galleries")]
    LouvreGalleries = 10,

    [Description("Archaeological Dig")]
    ArchaeologicalDig = 11,

    [Description("Tomb of the Ancients")]
    TombOfTheAncients = 12,

    [Description("The Hall of Seasons")]
    TheHallOfSeasons = 13,

    [Description("The Breath of Hades")]
    TheBreathOfHades = 14,

    [Description("Neptune's Hall")]
    NeptunesHall = 15,

    [Description("The Sanctuary of Flame")]
    TheSanctuaryOfFlame = 16,

    [Description("Wrath of the Beast")]
    WrathOfTheBeast = 17,

    [Description("Galleries Under Siege")]
    GalleriesUnderSiege = 18,

    [Description("Von Croy's Apartment")]
    VonCroysApartment = 19,

    // Prague
    [Description("The Monstrum Crime Scene")]
    TheMonstrumCrimeScene = 20,

    [Description("The Strahov Fortress")]
    TheStrahovFortress = 21,

    [Description("Bio-Research Facility")]
    BioResearchFacility = 22,

    [Description("The Sanitarium")]
    TheSanitarium = 23,

    [Description("Maximum Containment Area")]
    MaximumContainmentArea = 24,

    [Description("Aquatic Research Area")]
    AquaticResearchArea = 25,

    [Description("The Vault of Trophies")]
    TheVaultOfTrophies = 26,

    [Description("Boaz Returns")]
    BoazReturns = 27,

    [Description("The Lost Domain")]
    TheLostDomain = 28,

    [Description("Eckhardt's Lab")]
    EckhardtsLab = 29,
}