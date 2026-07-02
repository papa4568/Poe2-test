namespace AncientValueOverlay;

public sealed record RewardRow(string Name, decimal UnitDivines, int Quantity)
{
    public int SafeQuantity => Math.Max(1, Quantity);
    public decimal TotalDivines => UnitDivines * SafeQuantity;
    public bool IsUnknown => UnitDivines <= 0m;
}

public static class ValueAnalyzer
{
    public static RewardRow FindBest(IEnumerable<RewardRow> rows)
    {
        return rows
            .Where(row => !row.IsUnknown)
            .OrderByDescending(row => row.TotalDivines)
            .FirstOrDefault()
            ?? new RewardRow("No priced reward", 0m, 1);
    }

    public static decimal TotalValue(IEnumerable<RewardRow> rows)
    {
        return rows.Where(row => !row.IsUnknown).Sum(row => row.TotalDivines);
    }

    public static int UnknownCount(IEnumerable<RewardRow> rows)
    {
        return rows.Count(row => row.IsUnknown);
    }
}
