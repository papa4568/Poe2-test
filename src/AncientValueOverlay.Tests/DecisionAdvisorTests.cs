using AncientValueOverlay;
using Xunit;

namespace AncientValueOverlay.Tests;

public sealed class DecisionAdvisorTests
{
    [Fact]
    public void FindBest_PicksHighestTotalValue()
    {
        var rows = new[]
        {
            new RewardRow("Chaos Orb", 0.01m, 10),
            new RewardRow("Divine Orb", 1.0m, 1),
            new RewardRow("Exalted Orb", 0.05m, 3)
        };

        var best = ValueAnalyzer.FindBest(rows);

        Assert.Equal("Divine Orb", best.Name);
        Assert.Equal(1.0m, best.TotalDivines);
    }

    [Fact]
    public void TotalValue_ExcludesUnknownRows()
    {
        var rows = new[]
        {
            new RewardRow("Known", 0.25m, 2),
            new RewardRow("Unknown", 0m, 5)
        };

        Assert.Equal(0.50m, ValueAnalyzer.TotalValue(rows));
        Assert.Equal(1, ValueAnalyzer.UnknownCount(rows));
    }

    [Fact]
    public void BuildSummary_IncludesBestTotalAndUnknownCount()
    {
        var rows = new[]
        {
            new RewardRow("Divine Orb", 1.0m, 1),
            new RewardRow("Chaos Orb", 0.01m, 10),
            new RewardRow("Unknown", 0m, 1)
        };

        var summary = new DecisionAdvisor().BuildSummary(rows);

        Assert.Contains("Best: Divine Orb", summary);
        Assert.Contains("total 1.1 divine", summary);
        Assert.Contains("unknown 1", summary);
    }

    [Fact]
    public void SessionStats_AccumulatesAndResets()
    {
        var stats = new SessionStats();

        stats.Record(new[]
        {
            new RewardRow("Exalted Orb", 0.05m, 3),
            new RewardRow("Divine Orb", 1.0m, 1)
        });

        Assert.Equal(2, stats.RowCount);
        Assert.Equal(1.15m, stats.TotalSeenDivines);
        Assert.Equal("Divine Orb", stats.BestSeen.Name);

        stats.Reset();

        Assert.Equal(0, stats.RowCount);
        Assert.Equal(0m, stats.TotalSeenDivines);
        Assert.Equal("No priced reward", stats.BestSeen.Name);
    }
}
