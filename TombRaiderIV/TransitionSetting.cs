using System;
using System.Text;
using System.Xml;
using LiveSplit.UI;

namespace TR4;

public class TransitionSetting<TLevel>(
    TLevel lower, TLevel higher, TransitionDirection directionality,
    int? unusedLevelNumber = null, int? lowerRoomNumber = null, int? higherRoomNumber = null, string note = null)
    where TLevel : Enum
{
    public readonly TLevel Lower = lower;
    public readonly TLevel Higher = higher;
    public readonly TransitionDirection Directionality = directionality;
    public readonly int? UnusedLevelNumber = unusedLevelNumber;
    public readonly int? LowerRoomNumber = lowerRoomNumber;
    public readonly int? HigherRoomNumber = higherRoomNumber;

    public ulong Id
    {
        get
        {
            var sb = new StringBuilder($"{Convert.ToInt32(Lower)}{(int)Directionality:D1}{Convert.ToInt32(Higher)}");
            sb.Append(UnusedLevelNumber is null ?  "000" : UnusedLevelNumber.Value.ToString("D3"));
            sb.Append(LowerRoomNumber is null ?  "000" : LowerRoomNumber.Value.ToString("D3"));
            sb.Append(HigherRoomNumber is null ?  "000" : HigherRoomNumber.Value.ToString("D3"));

            return ulong.Parse(sb.ToString());
        }
    }

    public bool Active { get; private set; } = true;
    public bool Enabled => UnusedLevelNumber is not 39;

    public void UpdateActive(bool active) => Active = active;

    public TransitionDirection SelectedDirectionality { get; set; } = directionality;

    public string DisplayName()
    {
        string lowerName = Lower.Description();
        string higherName = Higher.Description();
        string noteText = !string.IsNullOrEmpty(note) ? $" [{note}]" : string.Empty;

        if (LowerRoomNumber is null && HigherRoomNumber is null)
            return Directionality switch
            {
                TransitionDirection.TwoWay => $"{lowerName} ↔️ {higherName}{noteText}",
                TransitionDirection.OneWayFromLower => $"{lowerName} → {higherName}{noteText}",
                TransitionDirection.OneWayFromHigher => $"{higherName} → {lowerName}{noteText}",
                _ => throw new ArgumentOutOfRangeException(nameof(Directionality), Directionality, "Unknown directionality"),
            };

        return Directionality switch
        {
            TransitionDirection.TwoWay => $"{lowerName} (room {LowerRoomNumber}) ↔️ {higherName} (room {HigherRoomNumber}){noteText}",
            TransitionDirection.OneWayFromLower => $"{lowerName} (room {LowerRoomNumber}) → {higherName}{noteText}",
            TransitionDirection.OneWayFromHigher => $"{higherName} (room {HigherRoomNumber}) → {lowerName}{noteText}",
            _ => throw new ArgumentOutOfRangeException(nameof(Directionality), Directionality, "Unknown directionality"),
        };
    }

    public XmlNode ToXmlElement(XmlDocument document)
    {
        var element = document.CreateElement("TransitionSetting");
        element.AppendChild(SettingsHelper.ToElement(document, "Lower", Convert.ToInt32(Lower)));
        element.AppendChild(SettingsHelper.ToElement(document, "Higher", Convert.ToInt32(Higher)));
        element.AppendChild(SettingsHelper.ToElement(document, "Directionality", ((int)Directionality).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, "SelectedDirectionality", ((int)SelectedDirectionality).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, "Enabled", Active));
        if (UnusedLevelNumber.HasValue) element.AppendChild(SettingsHelper.ToElement(document, "UnusedLevelNumber", UnusedLevelNumber.Value.ToString()));
        if (LowerRoomNumber.HasValue) element.AppendChild(SettingsHelper.ToElement(document, "LowerRoomNumber", LowerRoomNumber.Value.ToString()));
        if (HigherRoomNumber.HasValue) element.AppendChild(SettingsHelper.ToElement(document, "HigherRoomNumber", HigherRoomNumber.Value.ToString()));
        return element;
    }

    public static TransitionSetting<Tr4Level> Tr4FromXmlElement(XmlNode node)
    {
        var lower = (Tr4Level)int.Parse(node["Lower"].InnerText);
        var higher = (Tr4Level)int.Parse(node["Higher"].InnerText);
        var directionality = (TransitionDirection)int.Parse(node["Directionality"].InnerText);
        var selectedDirectionality = (TransitionDirection)int.Parse(node["SelectedDirectionality"].InnerText);
        bool enabled = bool.Parse(node["Enabled"].InnerText);

        int? unusedLevelNumber = node["UnusedLevelNumber"] != null ? int.Parse(node["UnusedLevelNumber"].InnerText) : null;
        int? lowerRoomNumber = node["LowerRoomNumber"] != null ? int.Parse(node["LowerRoomNumber"].InnerText) : null;
        int? higherRoomNumber = node["HigherRoomNumber"] != null ? int.Parse(node["HigherRoomNumber"].InnerText) : null;

        var setting = new TransitionSetting<Tr4Level>(lower, higher, directionality, unusedLevelNumber, lowerRoomNumber, higherRoomNumber)
        {
            SelectedDirectionality = selectedDirectionality,
            Active = enabled,
        };

        return setting;
    }

    public static TransitionSetting<TteLevel> TteFromXmlElement(XmlNode node)
    {
        var lower = (TteLevel)int.Parse(node["Lower"].InnerText);
        var higher = (TteLevel)int.Parse(node["Higher"].InnerText);
        var directionality = (TransitionDirection)int.Parse(node["Directionality"].InnerText);
        var selectedDirectionality = (TransitionDirection)int.Parse(node["SelectedDirectionality"].InnerText);
        bool enabled = bool.Parse(node["Enabled"].InnerText);

        int? unusedLevelNumber = node["UnusedLevelNumber"] != null ? int.Parse(node["UnusedLevelNumber"].InnerText) : null;
        int? lowerRoomNumber = node["LowerRoomNumber"] != null ? int.Parse(node["LowerRoomNumber"].InnerText) : null;
        int? higherRoomNumber = node["HigherRoomNumber"] != null ? int.Parse(node["HigherRoomNumber"].InnerText) : null;

        var setting = new TransitionSetting<TteLevel>(lower, higher, directionality, unusedLevelNumber, lowerRoomNumber, higherRoomNumber)
        {
            SelectedDirectionality = selectedDirectionality,
            Active = enabled,
        };

        return setting;
    }
}
