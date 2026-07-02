namespace AncientValueOverlay;

public sealed class AppConfig
{
    public string LeagueName { get; set; } = "Runes of Aldur";
    public string GameLanguage { get; set; } = "en";
    public bool SavePriceCache { get; set; } = true;
    public bool LoadCacheOnStartupFailure { get; set; } = true;
    public decimal MinimumHighlightDivines { get; set; } = 0.05m;
    public List<CalibrationProfile> CalibrationProfiles { get; set; } = [];
}

public sealed class CalibrationProfile
{
    public string Name { get; set; } = "Default";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int XOffset { get; set; } = 8;
    public bool IsValid => Width > 0 && Height > 0;
}
