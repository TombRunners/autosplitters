using System;
using System.IO;
using System.Text;
using System.Xml;
using LiveSplit.UI;
using Util;

namespace TR4;

public class TransitionSetting<TLevel>(
    TLevel lowerLevel, TLevel higherLevel, TransitionDirection directionality, int? unusedLevelNumber = null,
    int? lowerRoomNumber = null, int? higherRoomNumber = null, byte? lowerTriggerTimer = null, byte? higherTriggerTimer = null,
    string note = null, string toolTip = null, string section = null, bool complexIgnore = false
)
    where TLevel : Enum
{
    private const string LowerSettingName = "Lower";
    private const string HigherSettingName = "Higher";
    private const string DirectionalitySettingName = "Directionality";
    private const string SelectedDirectionalitySettingName = "SelectedDirectionality";
    private const string UnusedLevelNumberSettingName = "UnusedLevelNumber";
    private const string LowerRoomNumberSettingName = "LowerRoomNumber";
    private const string HigherRoomNumberSettingName = "HigherRoomNumber";
    private const string ActiveSettingName = "Active";

    public readonly TLevel LowerLevel = lowerLevel;
    public readonly TLevel HigherLevel = higherLevel;
    public readonly TransitionDirection Directionality = directionality;
    public readonly bool ComplexIgnore = complexIgnore;
    public readonly int? UnusedLevelNumber = unusedLevelNumber;
    public readonly string ToolTip = toolTip;
    public readonly string Section = section;

    public ulong Id
    {
        get
        {
            var sb = new StringBuilder($"{Convert.ToInt32(LowerLevel)}{(int) Directionality:D1}{Convert.ToInt32(HigherLevel)}");
            sb.Append(UnusedLevelNumber is null ? "000" : UnusedLevelNumber.Value.ToString("D3"));
            sb.Append(lowerRoomNumber is null ? "000" : lowerRoomNumber.Value.ToString("D3"));
            sb.Append(higherRoomNumber is null ? "000" : higherRoomNumber.Value.ToString("D3"));

            return ulong.Parse(sb.ToString());
        }
    }

    public ActiveSetting Active { get; private set; } = ActiveSetting.Active;

    public bool CanBeConfigured => UnusedLevelNumber is not 39;

    public void UpdateActive(ActiveSetting active)
    {
        if (CanBeConfigured)
            Active = active;
    }

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
        element.AppendChild(SettingsHelper.ToElement(document, ActiveSettingName, Convert.ToInt32(Active)));

        if (UnusedLevelNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, UnusedLevelNumberSettingName, UnusedLevelNumber.Value.ToString()));
        if (lowerRoomNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, LowerRoomNumberSettingName, lowerRoomNumber.Value.ToString()));
        if (higherRoomNumber.HasValue)
            element.AppendChild(SettingsHelper.ToElement(document, HigherRoomNumberSettingName, higherRoomNumber.Value.ToString()));

        return element;
    }

    public static TransitionSetting<TLevel> FromXmlElement(XmlNode node)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        int lowerValue = int.Parse(node[LowerSettingName].InnerText);
        if (!Enum.IsDefined(typeof(TLevel), lowerValue))
            throw new InvalidDataException($"Invalid value '{lowerValue}' for enum type '{nameof(TLevel)}'.");

        var lower = (TLevel) Enum.ToObject(typeof(TLevel), lowerValue);

        int higherValue = int.Parse(node[HigherSettingName].InnerText);
        if (!Enum.IsDefined(typeof(TLevel), higherValue))
            throw new InvalidDataException($"Invalid value '{higherValue}' for enum type '{nameof(TLevel)}'.");

        var higher = (TLevel) Enum.ToObject(typeof(TLevel), higherValue);

        var directionality = (TransitionDirection) int.Parse(node[DirectionalitySettingName].InnerText);
        if (!Enum.IsDefined(typeof(TransitionDirection), directionality))
            throw new InvalidDataException($"Invalid value '{directionality}' for enum type '{nameof(TransitionDirection)}'.");

        var selectedDirectionality = (TransitionDirection) int.Parse(node[SelectedDirectionalitySettingName].InnerText);
        if (!Enum.IsDefined(typeof(TransitionDirection), selectedDirectionality))
            throw new InvalidDataException($"Invalid value '{selectedDirectionality}' for enum type '{nameof(TransitionDirection)}'.");

        var active = (ActiveSetting) int.Parse(node[ActiveSettingName].InnerText);
        if (!Enum.IsDefined(typeof(ActiveSetting), active))
            throw new InvalidDataException($"Invalid value '{active}' for enum type '{nameof(ActiveSetting)}'.");

        int? unusedLevelNumber = ParseNullableInt(UnusedLevelNumberSettingName);
        int? lowerRoomNumber = ParseNullableInt(LowerRoomNumberSettingName);
        int? higherRoomNumber = ParseNullableInt(HigherRoomNumberSettingName);

        var setting = new TransitionSetting<TLevel>(lower, higher, directionality, unusedLevelNumber, lowerRoomNumber, higherRoomNumber)
        {
            Active = active,
            SelectedDirectionality = selectedDirectionality,
        };

        return setting;

        int? ParseNullableInt(string elementName)
        {
            XmlElement element = node[elementName];
            return element != null ? int.Parse(element.InnerText) : null;
        }
    }
}