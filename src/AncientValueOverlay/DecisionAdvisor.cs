namespace AncientValueOverlay;

public sealed class DecisionAdvisor
{
    public string BuildSummary(IEnumerable<RewardRow> rows)
    {
        var list = rows.ToList();
        if (list.Count == 0) return "No rewards entered.";

        var best = ValueAnalyzer.FindBest(list);
        var total = ValueAnalyzer.TotalValue(list);
        var unknown = ValueAnalyzer.UnknownCount(list);

        return $"Best: {best.Name} · {best.TotalDivines:0.##} divine · total {total:0.##} divine · unknown {unknown}";
    }
}
