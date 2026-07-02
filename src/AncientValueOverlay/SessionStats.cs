namespace AncientValueOverlay;

public sealed class SessionStats
{
    private readonly List<PanelAdvice> _panels = [];

    public IReadOnlyList<PanelAdvice> Panels => _panels;
    public int PanelCount => _panels.Count;
    public decimal TotalSeenDivines => _panels.Sum(p => p.TotalDivines);
    public RewardRow? BestSeen => _panels.Select(p => p.BestRow).Where(r => r is not null).OrderByDescending(r => r!.TotalDivines).FirstOrDefault();

    public void Record(PanelAdvice advice)
    {
        if (advice.Rows.Count == 0) return;
        _panels.Add(advice);
    }

    public void Reset() => _panels.Clear();
}
