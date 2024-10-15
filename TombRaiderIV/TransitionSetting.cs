using System;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.UI;

namespace TR4;

public class TransitionSetting(
    Tr4Level lower, Tr4Level higher, TransitionDirection directionality,
    int? unusedLevelNumber = null, int? lowerRoomNumber = null, int? higherRoomNumber = null, string note = null)
{
    public readonly Tr4Level Lower = lower;
    public readonly Tr4Level Higher = higher;
    public readonly TransitionDirection Directionality = directionality;
    public readonly int? UnusedLevelNumber = unusedLevelNumber;
    public readonly int? LowerRoomNumber = lowerRoomNumber;
    public readonly int? HigherRoomNumber = higherRoomNumber;

    public bool Enabled { get; private set; }

    public void UpdateEnabled() => Enabled = CheckBox.Checked;

    public TransitionDirection SelectedDirectionality { get; set; } = directionality;
    public CheckBox CheckBox { get; set; }

    public string DisplayName
    {
        get
        {
            string noteText = !string.IsNullOrEmpty(note) ? $" [{note}]" : string.Empty;
            if (LowerRoomNumber is null && HigherRoomNumber is null)
                return Directionality switch
                {
                    TransitionDirection.TwoWay => $"{Lower.Name()} ↔️ {Higher.Name()}{noteText}",
                    TransitionDirection.OneWayFromLower => $"{Lower.Name()} → {Higher.Name()}{noteText}",
                    TransitionDirection.OneWayFromHigher => $"{Higher.Name()} → {Lower.Name()}{noteText}",
                    _ => throw new ArgumentOutOfRangeException(nameof(Directionality), Directionality, "Unknown directionality"),
                };

            return Directionality switch
            {
                TransitionDirection.TwoWay => $"{Lower.Name()} (room {LowerRoomNumber}) ↔️ {Higher.Name()} (room {HigherRoomNumber}){noteText}",
                TransitionDirection.OneWayFromLower => $"{Lower.Name()} (room {LowerRoomNumber}) → {Higher.Name()}{noteText}",
                TransitionDirection.OneWayFromHigher => $"{Higher.Name()} (room {HigherRoomNumber}) → {Lower.Name()}{noteText}",
                _ => throw new ArgumentOutOfRangeException(nameof(Directionality), Directionality, "Unknown directionality"),
            };
        }
    }

    public XmlNode ToXmlElement(XmlDocument document)
    {
        var element = document.CreateElement("TransitionSetting");
        element.AppendChild(SettingsHelper.ToElement(document, "Lower", ((int)Lower).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, "Higher", ((int)Higher).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, "Directionality", ((int)Directionality).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, "SelectedDirectionality", ((int)SelectedDirectionality).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, "Enabled", Enabled));
        if (UnusedLevelNumber.HasValue) element.AppendChild(SettingsHelper.ToElement(document, "UnusedLevelNumber", UnusedLevelNumber.Value.ToString()));
        if (LowerRoomNumber.HasValue) element.AppendChild(SettingsHelper.ToElement(document, "LowerRoomNumber", LowerRoomNumber.Value.ToString()));
        if (HigherRoomNumber.HasValue) element.AppendChild(SettingsHelper.ToElement(document, "HigherRoomNumber", HigherRoomNumber.Value.ToString()));
        return element;
    }

    public static TransitionSetting FromXmlElement(XmlNode node)
    {
        var lower = (Tr4Level)int.Parse(node["Lower"].InnerText);
        var higher = (Tr4Level)int.Parse(node["Higher"].InnerText);
        var directionality = (TransitionDirection)int.Parse(node["Directionality"].InnerText);
        var selectedDirectionality = (TransitionDirection)int.Parse(node["SelectedDirectionality"].InnerText);
        bool enabled = bool.Parse(node["Enabled"].InnerText);

        int? unusedLevelNumber = node["UnusedLevelNumber"] != null ? int.Parse(node["UnusedLevelNumber"].InnerText) : null;
        int? lowerRoomNumber = node["LowerRoomNumber"] != null ? int.Parse(node["LowerRoomNumber"].InnerText) : null;
        int? higherRoomNumber = node["HigherRoomNumber"] != null ? int.Parse(node["HigherRoomNumber"].InnerText) : null;

        var setting = new TransitionSetting(lower, higher, directionality, unusedLevelNumber, lowerRoomNumber, higherRoomNumber)
        {
            SelectedDirectionality = selectedDirectionality,
            Enabled = enabled,
        };

        return setting;
    }
}