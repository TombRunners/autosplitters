using System;
using System.Windows.Forms;

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
}