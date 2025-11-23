using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml;
using LiveSplit.UI;

namespace TR456;

public class Tr6LevelTransitionSetting(string name, string oldLevel, string nextLevel, string toolTip = null, string section = null)
{
    private const string NameName = "Name";
    private const string OldName = "Old";
    private const string NextName = "Next";
    private const string ActiveName = "Active";

    public readonly string OldLevel = oldLevel;
    public readonly string NextLevel = nextLevel;
    public readonly string Name = name;
    public readonly string ToolTip = toolTip;
    public readonly string Section = section;

    public bool Active { get; private set; } = true;
    public void UpdateActive(bool active) => Active = active;

    private static readonly ImmutableDictionary<string, Tr6Level> LevelNameMap = new Dictionary<string, Tr6Level>
    {
        { "FRONTEND.GMX", Tr6Level.MainMenu },

        // Paris
        { "PARIS1.GMX", Tr6Level.ParisianBackStreets }, { "PARIS1A.GMX", Tr6Level.DerelictApartmentBlock },
        { "PARIS1C.GMX", Tr6Level.IndustrialRooftops }, { "PARIS1B.GMX", Tr6Level.MargotCarviersApartment },
        { "PARIS2_1.GMX", Tr6Level.ParisianGhetto1 }, { "PARIS2_2.GMX", Tr6Level.ParisianGhetto2 },
        { "PARIS2_3.GMX", Tr6Level.ParisianGhetto3 }, { "PARIS2B.GMX", Tr6Level.TheSerpentRouge },
        { "PARIS2C.GMX", Tr6Level.RennesPawnshop }, { "PARIS2D.GMX", Tr6Level.WillowtreeHerbalist },
        { "PARIS2E.GMX", Tr6Level.StAicardsChurch }, { "PARIS2F.GMX", Tr6Level.CafeMetro }, { "PARIS2G.GMX", Tr6Level.StAicardsGraveyard },
        { "PARIS2H.GMX", Tr6Level.BouchardsHideout }, { "CUTSCENE\\CS_2_51A.GMX", Tr6Level.RennesPawnshopCutscene },
        { "PARIS3.GMX", Tr6Level.LouvreStormDrains }, { "PARIS4.GMX", Tr6Level.LouvreGalleries },
        { "PARIS5A.GMX", Tr6Level.ArchaeologicalDig }, { "PARIS5.GMX", Tr6Level.TombOfTheAncients },
        { "PARIS5_1.GMX", Tr6Level.TheHallOfSeasons }, { "PARIS5_2.GMX", Tr6Level.NeptunesHall },
        { "PARIS5_3.GMX", Tr6Level.WrathOfTheBeast }, { "PARIS5_4.GMX", Tr6Level.TheSanctuaryOfFlame },
        { "PARIS5_5.GMX", Tr6Level.TheBreathOfHades }, { "PARIS4A.GMX", Tr6Level.GalleriesUnderSiege },
        { "CUTSCENE\\CS_6_2.GMX", Tr6Level.GalleriesUnderSiegeCutscene1 },
        { "CUTSCENE\\CS_6_16.GMX", Tr6Level.GalleriesUnderSiegeCutscene2 },
        { "CUTSCENE\\CS_6_21A.GMX", Tr6Level.GalleriesUnderSiegeCutscene3 },
        { "CUTSCENE\\CS_6_21B.GMX", Tr6Level.GalleriesUnderSiegeCutscene4 }, { "PARIS6.GMX", Tr6Level.VonCroysApartment },
        { "CUTSCENE\\CS_7_19.GMX", Tr6Level.VonCroysApartmentCutscene },

        // Prague
        { "PRAGUE1.GMX", Tr6Level.TheMonstrumCrimeScene }, { "CUTSCENE\\CS_9_1.GMX", Tr6Level.TheMonstrumCrimeSceneCutscene },
        { "PRAGUE2.GMX", Tr6Level.TheStrahovFortress }, { "CUTSCENE\\CS_9_12.GMX", Tr6Level.TheStrahovFortressCutscene1 },
        { "CUTSCENE\\CS_9_15B.GMX", Tr6Level.TheStrahovFortressCutscene2 }, { "PRAGUE3.GMX", Tr6Level.BioResearchFacility },
        { "CUTSCENE\\CS_10_8.GMX", Tr6Level.BioResearchFacilityCutscene1 },
        { "CUTSCENE\\CS_10_14.GMX", Tr6Level.BioResearchFacilityCutscene2 }, { "PRAGUE4.GMX", Tr6Level.TheSanitarium },
        { "PRAGUE4A.GMX", Tr6Level.MaximumContainmentArea }, { "CUTSCENE\\CS_12_1.GMX", Tr6Level.MaximumContainmentAreaCutscene },
        { "PRAGUE3A.GMX", Tr6Level.AquaticResearchArea }, { "PRAGUE5.GMX", Tr6Level.TheVaultOfTrophies },
        { "CUTSCENE\\CS_13_9.GMX", Tr6Level.TheVaultOfTrophiesCutscene }, { "PRAGUE5A.GMX", Tr6Level.BoazReturns },
        { "CUTSCENE\\CS_14_4A.GMX", Tr6Level.BoazReturnsCutscene1 }, { "CUTSCENE\\CS_14_6.GMX", Tr6Level.BoazReturnsCutscene2 },
        { "PRAGUE6A.GMX", Tr6Level.TheLostDomain }, { "PRAGUE6.GMX", Tr6Level.EckhardtsLab },
        { "CUTSCENE\\CS_15_10.GMX", Tr6Level.EckhardtsLabCutscene1 }, { "CUTSCENE\\CS_15_14.GMX", Tr6Level.EckhardtsLabCutscene2 },
        { "CUTSCENE\\CS_15_24.GMX", Tr6Level.EckhardtsLabCutscene3 },
    }.ToImmutableDictionary();

    public ulong Id
    {
        get
        {
            uint oldLevelNumber = 111;
            if (LevelNameMap.TryGetValue(OldLevel, out Tr6Level oldLevel))
                oldLevelNumber = (uint) oldLevel;

            uint nextLevelNumber = 111;
            if (LevelNameMap.TryGetValue(NextLevel, out Tr6Level nextLevel))
                nextLevelNumber = (uint) nextLevel;

            return ulong.Parse($"{oldLevelNumber:D3}{nextLevelNumber:D3}");
        }
    }

    public XmlNode ToXmlElement(XmlDocument document)
    {
        XmlElement element = document.CreateElement("TransitionSetting");
        element.AppendChild(SettingsHelper.ToElement(document, NameName, Name));
        element.AppendChild(SettingsHelper.ToElement(document, OldName, OldLevel));
        element.AppendChild(SettingsHelper.ToElement(document, NextName, NextLevel));
        element.AppendChild(SettingsHelper.ToElement(document, ActiveName, Active));
        return element;
    }

    public static Tr6LevelTransitionSetting FromXmlElement(XmlNode node)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        string name = node[NameName].InnerText;
        string old = node[OldName].InnerText;
        string next = node[NextName].InnerText;
        bool active = bool.Parse(node[ActiveName].InnerText);

        var setting = new Tr6LevelTransitionSetting(name, old, next)
        {
            Active = active,
        };

        return setting;
    }
}