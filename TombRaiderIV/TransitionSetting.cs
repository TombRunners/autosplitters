using System;
using System.IO;
using System.Text;
using System.Xml;
using LiveSplit.UI;

namespace TR4;

public class TransitionSetting<TLevel>(
    TLevel lowerLevel, TLevel higherLevel, TransitionDirection directionality, int? unusedLevelNumber = null,
    int? lowerRoomNumber = null, int? higherRoomNumber = null, byte? lowerTriggerTimer = null, byte? higherTriggerTimer = null, string note = null)
    where TLevel : Enum
{
    public readonly TLevel LowerLevel = lowerLevel;
    public readonly TLevel HigherLevel = higherLevel;
    public readonly TransitionDirection Directionality = directionality;
    public readonly int? UnusedLevelNumber = unusedLevelNumber;

    public ulong Id
    {
        get
        {
            var sb = new StringBuilder($"{Convert.ToInt32(LowerLevel)}{(int)Directionality:D1}{Convert.ToInt32(HigherLevel)}");
            sb.Append(UnusedLevelNumber is null ?  "000" : UnusedLevelNumber.Value.ToString("D3"));
            sb.Append(lowerRoomNumber is null ?  "000" : lowerRoomNumber.Value.ToString("D3"));
            sb.Append(higherRoomNumber is null ?  "000" : higherRoomNumber.Value.ToString("D3"));

            return ulong.Parse(sb.ToString());
        }
    }

    public bool Active { get; private set; } = true;
    public bool CanBeConfigured => UnusedLevelNumber is not 39;

    public void UpdateActive(bool active) => Active = CanBeConfigured && active;

    public TransitionDirection SelectedDirectionality { get; set; } = directionality;

    public string DisplayName()
    {
        string lowerName = LowerLevel.Description();
        string higherName = HigherLevel.Description();
        string noteText = !string.IsNullOrEmpty(note) ? $" [{note}]" : string.Empty;

        if (lowerRoomNumber is null && higherRoomNumber is null)
            return Directionality switch
            {
                TransitionDirection.TwoWay => $"{lowerName} ↔️ {higherName}{noteText}",
                TransitionDirection.OneWayFromLower => $"{lowerName} → {higherName}{noteText}",
                TransitionDirection.OneWayFromHigher => $"{higherName} → {lowerName}{noteText}",
                _ => throw new ArgumentOutOfRangeException(nameof(Directionality), Directionality, "Unknown directionality"),
            };

        return Directionality switch
        {
            TransitionDirection.TwoWay => $"{lowerName} (room {lowerRoomNumber}) ↔️ {higherName} (room {higherRoomNumber}){noteText}",
            TransitionDirection.OneWayFromLower => $"{lowerName} (room {lowerRoomNumber}) → {higherName}{noteText}",
            TransitionDirection.OneWayFromHigher => $"{higherName} (room {higherRoomNumber}) → {lowerName}{noteText}",
            _ => throw new ArgumentOutOfRangeException(nameof(Directionality), Directionality, "Unknown directionality"),
        };
    }

    public bool TriggerMatchedOrNotRequired(byte triggerTimer, bool laraIsInLowerLevel)
    {
        bool triggerTimerNotRequired = lowerTriggerTimer is null && higherTriggerTimer is null;
        if (triggerTimerNotRequired)
            return true;

        byte? triggerTimerToMatch = laraIsInLowerLevel ? lowerTriggerTimer : higherTriggerTimer;
        return triggerTimer == triggerTimerToMatch;
    }

    public XmlNode ToXmlElement(XmlDocument document)
    {
        XmlElement element = document.CreateElement("TransitionSetting");
        element.AppendChild(SettingsHelper.ToElement(document, "Lower", Convert.ToInt32(LowerLevel)));
        element.AppendChild(SettingsHelper.ToElement(document, "Higher", Convert.ToInt32(HigherLevel)));
        element.AppendChild(SettingsHelper.ToElement(document, "Directionality", ((int)Directionality).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, "SelectedDirectionality", ((int)SelectedDirectionality).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, "Active", Active));

        if (UnusedLevelNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, "UnusedLevelNumber", UnusedLevelNumber.Value.ToString()));
        if (lowerRoomNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, "LowerRoomNumber", lowerRoomNumber.Value.ToString()));
        if (higherRoomNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, "HigherRoomNumber", higherRoomNumber.Value.ToString()));

        return element;
    }

    public static TransitionSetting<TLevel> FromXmlElement(XmlNode node)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        int lowerValue = int.Parse(node["Lower"].InnerText);
        if (!Enum.IsDefined(typeof(TLevel), lowerValue))
            throw new InvalidDataException($"Invalid value '{lowerValue}' for enum type '{typeof(TLevel).Name}'.");
        var lower = (TLevel)Enum.ToObject(typeof(TLevel), lowerValue);

        int higherValue = int.Parse(node["Higher"].InnerText);
        if (!Enum.IsDefined(typeof(TLevel), higherValue))
            throw new InvalidDataException($"Invalid value '{higherValue}' for enum type '{typeof(TLevel).Name}'.");
        var higher = (TLevel)Enum.ToObject(typeof(TLevel), higherValue);

        var directionality = (TransitionDirection)int.Parse(node["Directionality"].InnerText);
        if (!Enum.IsDefined(typeof(TransitionDirection), directionality))
            throw new InvalidDataException($"Invalid value '{directionality}' for enum type '{nameof(TransitionDirection)}'.");

        var selectedDirectionality = (TransitionDirection)int.Parse(node["SelectedDirectionality"].InnerText);
        if (!Enum.IsDefined(typeof(TransitionDirection), selectedDirectionality))
            throw new InvalidDataException($"Invalid value '{selectedDirectionality}' for enum type '{nameof(TransitionDirection)}'.");

        bool active = bool.Parse(node["Active"].InnerText);

        int? unusedLevelNumber = ParseNullableInt("UnusedLevelNumber");
        int? lowerRoomNumber = ParseNullableInt("LowerRoomNumber");
        int? higherRoomNumber = ParseNullableInt("HigherRoomNumber");

        var setting = new TransitionSetting<TLevel>(lower, higher, directionality, unusedLevelNumber, lowerRoomNumber, higherRoomNumber)
        {
            SelectedDirectionality = selectedDirectionality,
            Active = active,
        };

        return setting;

        int? ParseNullableInt(string elementName)
        {
            XmlElement element = node[elementName];
            return element != null ? int.Parse(element.InnerText) : null;
        }
    }
}
