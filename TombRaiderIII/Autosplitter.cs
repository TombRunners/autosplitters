using System;
using TRUtil;

namespace TR3;

/// <summary>Implementation of <see cref="ClassicAutosplitter"/>.</summary>
internal sealed class Autosplitter : ClassicAutosplitter
{
    /// <summary>A constructor that primarily exists to handle events/delegations and set static values.</summary>
    public Autosplitter(Version version) : base(version)
    {
        Settings = new ComponentSettings(version);

        LevelCount = 20; // This is the highest between TR3 at 20 and TLA at 6.
        CompletedLevels.Capacity = LevelCount;

        Data = new GameData();
        Data.OnAslComponentChanged += Settings.SetAslWarningLabelVisibility;
        Data.OnGameFound += Settings.SetGameVersion;
    }
}