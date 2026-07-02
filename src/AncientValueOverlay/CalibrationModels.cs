namespace AncientValueOverlay;

public sealed record CalibrationPreview(
    CalibrationProfile Profile,
    IReadOnlyList<CalibrationPreviewRow> Rows,
    string Summary)
{
    public bool LooksGood => Rows.Count > 0 && Rows.Count(r => r.HasMatchedPrice) >= Math.Max(1, Rows.Count / 2);
}

public sealed record CalibrationPreviewRow(
    string RawText,
    string NormalizedName,
    string? MatchedPriceKey,
    int CenterY,
    decimal Confidence)
{
    public bool HasMatchedPrice => !string.IsNullOrWhiteSpace(MatchedPriceKey);
}

public sealed class CalibrationPreviewBuilder
{
    public CalibrationPreview Build(CalibrationProfile profile, IReadOnlyList<RewardRead> reads, IReadOnlyDictionary<string, PriceEntry> prices)
    {
        var rows = reads.Select(r => new CalibrationPreviewRow(
            r.RawText,
            r.NormalizedName,
            prices.ContainsKey(r.NormalizedName) ? r.NormalizedName : null,
            r.CenterY,
            r.Confidence)).ToList();

        var matched = rows.Count(r => r.HasMatchedPrice);
        var summary = rows.Count == 0
            ? "No text rows were read. Try widening or moving the calibration box."
            : $"Read {rows.Count} rows; {matched} matched known prices.";
        return new CalibrationPreview(profile, rows, summary);
    }
}
