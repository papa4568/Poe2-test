namespace AncientValueOverlay;

public sealed record RewardRead(
    string RawText,
    string NormalizedName,
    int CenterY,
    int Quantity = 1,
    decimal Confidence = 1m);

public sealed record PriceEntry(
    decimal DivineValue,
    decimal ExaltedValue,
    bool HasMarketData = true);

public sealed record RewardRow(
    RewardRead Read,
    string? MatchedKey,
    PriceEntry? Price,
    bool IsExactMatch)
{
    public bool HasPrice => Price is { HasMarketData: true };
    public int Quantity => Math.Max(1, Read.Quantity);
    public decimal TotalDivines => HasPrice ? Price!.DivineValue * Quantity : 0m;
}

public sealed record PanelAdvice(
    IReadOnlyList<RewardRow> Rows,
    RewardRow? BestRow,
    RewardRow? SecondBestRow,
    decimal TotalDivines,
    int UnknownRows)
{
    public decimal BestGapDivines => BestRow is null || SecondBestRow is null ? 0m : BestRow.TotalDivines - SecondBestRow.TotalDivines;
}

public enum PriceSourceStatus
{
    Unknown,
    Live,
    CacheOnly,
    RefreshFailedUsingCache,
    Failed
}

public sealed record PriceSourceHealth(
    PriceSourceStatus Status,
    DateTimeOffset? LastLiveFetchUtc,
    DateTimeOffset? LastCacheUtc,
    int ItemCount,
    string Message);

public sealed record PriceFetchResult(
    IReadOnlyDictionary<string, PriceEntry> Prices,
    PriceSourceHealth Health);
