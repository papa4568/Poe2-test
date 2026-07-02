namespace AncientValueOverlay;

public sealed class SessionStats
{
    private readonly List<RewardRow> _seenRows = [];

    public int RowCount => _seenRows.Count;
    public decimal TotalSeenDivines => ValueAnalyzer.TotalValue(_seenRows);
    public RewardRow BestSeen => ValueAnalyzer.FindBest(_seenRows);

    public void Record(IEnumerable<RewardRow> rows)
    {
        _seenRows.AddRange(rows);
    }

    public void Reset()
    {
        _seenRows.Clear();
    }
}
