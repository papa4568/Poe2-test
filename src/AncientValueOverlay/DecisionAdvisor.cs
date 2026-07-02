namespace AncientValueOverlay;

public sealed class DecisionAdvisor
{
    public PanelAdvice Analyze(IEnumerable<RewardRow> rows)
    {
        var list = rows.OrderBy(r => r.Read.CenterY).ToList();
        var ranked = list.Where(r => r.HasPrice).OrderByDescending(r => r.TotalDivines).ToList();
        var best = ranked.ElementAtOrDefault(0);
        var second = ranked.ElementAtOrDefault(1);
        var unknown = list.Count(r => !r.HasPrice);
        return new PanelAdvice(list, best, second, list.Sum(r => r.TotalDivines), unknown);
    }

    public string BuildSummary(PanelAdvice advice)
    {
        if (advice.Rows.Count == 0) return "No rows read.";
        if (advice.BestRow is null) return $"No priced rows yet · unknown {advice.UnknownRows}";

        var bestName = advice.BestRow.MatchedKey ?? advice.BestRow.Read.NormalizedName;
        var gap = advice.SecondBestRow is null ? "only priced row" : $"+{advice.BestGapDivines:0.##}d over next";
        return $"Best: {bestName} · {advice.BestRow.TotalDivines:0.##}d · {gap} · total {advice.TotalDivines:0.##}d · unknown {advice.UnknownRows}";
    }
}
