using LiveSplit.UI;
using System.Xml;
using System;
using System.IO;
using System.Text;

namespace TR456;

public class Tr6LevelTransitionSetting(string name, string oldLevel, string nextLevel, int maxCount = 1)
{
    private const string NameName = "Name";
    private const string OldName = "Old";
    private const string NextName = "Next";
    private const string MaxCountName = "MaxCount";
    private const string SelectedCountName = "SelectedCount";
    private const string ActiveName = "Active";

    public readonly string OldLevel = oldLevel;
    public readonly string NextLevel = nextLevel;
    public readonly int MaxCount = maxCount;
    public readonly string Name = name;

    public int SelectedCount { get; set; } = maxCount;

    public bool Active { get; private set; } = true;
    public void UpdateActive(bool active) => Active = active;

    public string Id => $"{OldLevel}{MaxCount:D1}{NextLevel}";

    public XmlNode ToXmlElement(XmlDocument document)
    {
        XmlElement element = document.CreateElement("TransitionSetting");
        element.AppendChild(SettingsHelper.ToElement(document, NameName, Name));
        element.AppendChild(SettingsHelper.ToElement(document, OldName, OldLevel));
        element.AppendChild(SettingsHelper.ToElement(document, NextName, NextLevel));
        element.AppendChild(SettingsHelper.ToElement(document, MaxCountName, MaxCount));
        element.AppendChild(SettingsHelper.ToElement(document, SelectedCountName, SelectedCount));
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
        int maxCount = int.Parse(node[MaxCountName].InnerText);
        int selectedCount = int.Parse(node[SelectedCountName].InnerText);
        bool active = bool.Parse(node[ActiveName].InnerText);

        var setting = new Tr6LevelTransitionSetting(name, old, next, maxCount)
        {
            SelectedCount = selectedCount,
            Active = active,
        };

        return setting;
    }
}
