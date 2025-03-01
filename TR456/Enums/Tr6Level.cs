using System.ComponentModel;

namespace TR456;

/// <summary>The game's level values.</summary>
public enum Tr6Level
{
    [Description("Main Menu / Credits")]
    MainMenu,

    // Paris
    [Description("Parisian Back Streets")]
    ParisianBackStreets,

    [Description("Derelict Apartment Block")]
    DerelictApartmentBlock,

    [Description("Industrial Rooftops")]
    IndustrialRooftops,

    [Description("Margot Carvier's Apartment")]
    MargotCarviersApartment,

    [Description("Parisian Ghetto, Lower")]
    ParisianGhetto1,

    [Description("Parisian Ghetto, Upper Cafe")]
    ParisianGhetto2,

    [Description("Parisian Ghetto, Willowtree and Church")]
    ParisianGhetto3,

    [Description("The Serpent Rouge")]
    TheSerpentRouge,

    [Description("Rennes' Pawnshop")]
    RennesPawnshop,

    [Description("Willowtree Herbalist")]
    WillowtreeHerbalist,

    [Description("St. Aicard's Church")]
    StAicardsChurch,

    [Description("Cafe Metro")]
    CafeMetro,

    [Description("St. Aicard's Graveyard")]
    StAicardsGraveyard,

    [Description("Bouchard's Hideout")]
    BouchardsHideout,

    [Description("Cutscene after Rennes' Pawnshop")]
    RennesPawnshopCutscene,

    [Description("Louvre Storm Drains")]
    LouvreStormDrains,

    [Description("Louvre Galleries")]
    LouvreGalleries,

    [Description("Archaeological Dig")]
    ArchaeologicalDig,

    [Description("Tomb of the Ancients")]
    TombOfTheAncients,

    [Description("The Hall of Seasons")]
    TheHallOfSeasons,

    [Description("Neptune's Hall")]
    NeptunesHall,

    [Description("Wrath of the Beast")]
    WrathOfTheBeast,

    [Description("The Sanctuary of Flame")]
    TheSanctuaryOfFlame,

    [Description("The Breath of Hades")]
    TheBreathOfHades,

    [Description("Galleries Under Siege")]
    GalleriesUnderSiege,

    [Description("Galleries Under Siege Cutscene, Staircase")]
    GalleriesUnderSiegeCutscene1,

    [Description("Galleries Under Siege Cutscene, Confiscation")]
    GalleriesUnderSiegeCutscene2,

    [Description("Galleries Under Siege Cutscene, Louvre Escape")]
    GalleriesUnderSiegeCutscene3,

    [Description("Galleries Under Siege Cutscene, Drive to Apartment")]
    GalleriesUnderSiegeCutscene4,

    [Description("Von Croy's Apartment")]
    VonCroysApartment,

    [Description("Von Croy's Apartment Cutscene, Janitor")]
    VonCroysApartmentCutscene,

    // Prague
    [Description("The Monstrum Crime Scene")]
    TheMonstrumCrimeScene,

    [Description("The Monstrum Crime Scene Cutscene")]
    TheMonstrumCrimeSceneCutscene,

    [Description("The Strahov Fortress")]
    TheStrahovFortress,

    [Description("The Strahov Fortress Cutscene, Luddick")]
    TheStrahovFortressCutscene1,

    [Description("The Strahov Fortress Cutscene, Control Room Power")]
    TheStrahovFortressCutscene2,

    [Description("Bio-Research Facility")]
    BioResearchFacility,

    [Description("Bio-Research Facility Cutscene, Boaz")]
    BioResearchFacilityCutscene1,

    [Description("Bio-Research Facility, Kurtis Meeting")]
    BioResearchFacilityCutscene2,

    [Description("The Sanitarium")]
    TheSanitarium,

    [Description("Maximum Containment Area")]
    MaximumContainmentArea,

    [Description("Maximum Containment Area Cutscene")]
    MaximumContainmentAreaCutscene,

    [Description("Aquatic Research Area")]
    AquaticResearchArea,

    [Description("The Vault of Trophies")]
    TheVaultOfTrophies,

    [Description("The Vault of Trophies Cutscene")]
    TheVaultOfTrophiesCutscene,

    [Description("Boaz Returns")]
    BoazReturns,

    [Description("Boaz Returns Cutscene, Transformation")]
    BoazReturnsCutscene1,

    [Description("Boaz Returns Cutscene, End")]
    BoazReturnsCutscene2,

    [Description("The Lost Domain")]
    TheLostDomain,

    [Description("Eckhardt's Lab")]
    EckhardtsLab,

    [Description("Eckhardt's Lab Cutscene, Eckhardt Start")]
    EckhardtsLabCutscene1,

    [Description("Eckhardt's Lab Cutscene, Eckhardt End")]
    EckhardtsLabCutscene2,

    [Description("Eckhardt's Lab Cutscene, After Flashblack")]
    EckhardtsLabCutscene3,
}