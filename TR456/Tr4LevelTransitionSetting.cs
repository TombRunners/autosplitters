using System;
using System.IO;
using System.Text;
using System.Xml;
using LiveSplit.UI;

namespace TR456;

public class Tr4LevelTransitionSetting(
    Tr4Level lowerLevel, Tr4Level higherLevel, TransitionDirection directionality, int? unusedLevelNumber = null,
    int? lowerRoomNumber = null, int? higherRoomNumber = null, byte? lowerTriggerTimer = null, byte? higherTriggerTimer = null,
    string note = null, string toolTip = null, string section = null, int? maxCount = null)
{
    private const string LowerSettingName = "Lower";
    private const string HigherSettingName = "Higher";
    private const string DirectionalitySettingName = "Directionality";
    private const string SelectedDirectionalitySettingName = "SelectedDirectionality";
    private const string MaxCountSettingName = "MaxCount";
    private const string SelectedCountSettingName = "SelectedCount";
    private const string UnusedLevelNumberSettingName = "UnusedLevelNumber";
    private const string LowerRoomNumberSettingName = "LowerRoomNumber";
    private const string HigherRoomNumberSettingName = "HigherRoomNumber";
    private const string ActiveSettingName = "Active";

    public readonly Tr4Level LowerLevel = lowerLevel;
    public readonly Tr4Level HigherLevel = higherLevel;
    public readonly TransitionDirection Directionality = directionality;
    public readonly int? MaxCount = maxCount;
    public readonly int? UnusedLevelNumber = unusedLevelNumber;
    public readonly string ToolTip = toolTip;
    public readonly string Section = section;

    public int? SelectedCount { get; set; } = maxCount;

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
        element.AppendChild(SettingsHelper.ToElement(document, LowerSettingName, Convert.ToInt32(LowerLevel)));
        element.AppendChild(SettingsHelper.ToElement(document, HigherSettingName, Convert.ToInt32(HigherLevel)));
        element.AppendChild(SettingsHelper.ToElement(document, DirectionalitySettingName, ((int)Directionality).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, SelectedDirectionalitySettingName, ((int)SelectedDirectionality).ToString()));
        element.AppendChild(SettingsHelper.ToElement(document, ActiveSettingName, Active));

        if (UnusedLevelNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, UnusedLevelNumberSettingName, UnusedLevelNumber.Value.ToString()));
        if (lowerRoomNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, LowerRoomNumberSettingName, lowerRoomNumber.Value.ToString()));
        if (higherRoomNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, HigherRoomNumberSettingName, higherRoomNumber.Value.ToString()));

        if (MaxCount.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, MaxCountSettingName, MaxCount.Value.ToString()));
        if (SelectedCount.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, SelectedCountSettingName, SelectedCount.Value.ToString()));

        return element;
    }

    public static Tr4LevelTransitionSetting FromXmlElement(XmlNode node)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        int lowerValue = int.Parse(node[LowerSettingName].InnerText);
        if (!Enum.IsDefined(typeof(Tr4Level), lowerValue))
            throw new InvalidDataException($"Invalid value '{lowerValue}' for enum type '{nameof(Tr4Level)}'.");
        var lower = (Tr4Level)Enum.ToObject(typeof(Tr4Level), lowerValue);

        int higherValue = int.Parse(node[HigherSettingName].InnerText);
        if (!Enum.IsDefined(typeof(Tr4Level), higherValue))
            throw new InvalidDataException($"Invalid value '{higherValue}' for enum type '{nameof(Tr4Level)}'.");
        var higher = (Tr4Level)Enum.ToObject(typeof(Tr4Level), higherValue);

        var directionality = (TransitionDirection)int.Parse(node[DirectionalitySettingName].InnerText);
        if (!Enum.IsDefined(typeof(TransitionDirection), directionality))
            throw new InvalidDataException($"Invalid value '{directionality}' for enum type '{nameof(TransitionDirection)}'.");

        var selectedDirectionality = (TransitionDirection)int.Parse(node[SelectedDirectionalitySettingName].InnerText);
        if (!Enum.IsDefined(typeof(TransitionDirection), selectedDirectionality))
            throw new InvalidDataException($"Invalid value '{selectedDirectionality}' for enum type '{nameof(TransitionDirection)}'.");

        bool active = bool.Parse(node["Active"].InnerText);

        int? unusedLevelNumber = ParseNullableInt(UnusedLevelNumberSettingName);
        int? lowerRoomNumber = ParseNullableInt(LowerRoomNumberSettingName);
        int? higherRoomNumber = ParseNullableInt(HigherRoomNumberSettingName);

        int? maxCount = ParseNullableInt(MaxCountSettingName);
        int? selectedCount = ParseNullableInt(SelectedCountSettingName);

        var setting = new Tr4LevelTransitionSetting(lower, higher, directionality, unusedLevelNumber, lowerRoomNumber, higherRoomNumber, maxCount: maxCount)
        {
            SelectedCount = selectedCount,
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
