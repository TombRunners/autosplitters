namespace TR123;

public readonly record struct GameAddresses
{
    /// <summary>Address of BonusFlag.</summary>
    public int BonusFlag { get; init; }

    /// <summary>Address of Cine.</summary>
    public int Cine { get; init; }

    /// <summary>Address of FirstLevelTime.</summary>
    public int FirstLevelTime { get; init; }

    /// <summary>Address of Lara's Health.</summary>
    public int Health { get; init; }

    /// <summary>Address of InventoryChosen.</summary>
    public int InventoryChosen { get; init; }

    /// <summary>Address of InventoryMode.</summary>
    public int InventoryMode { get; init; }

    /// <summary>Address of the current Level.</summary>
    public int Level { get; init; }

    /// <summary>Address of the LevelComplete flag.</summary>
    public int LevelComplete { get; init; }

    /// <summary>Address of the current Level's IGT.</summary>
    public int LevelIgt { get; init; }

    /// <summary>Address of LoadFade.</summary>
    public int LoadFade { get; init; }

    /// <summary>Address of OverlayFlag.</summary>
    public int OverlayFlag { get; init; }

    /// <summary>Address of the TitleLoaded flag.</summary>
    public int TitleLoaded { get; init; }
}