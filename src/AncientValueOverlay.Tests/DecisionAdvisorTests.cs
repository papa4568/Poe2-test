using AncientValueOverlay;
using Xunit;

namespace AncientValueOverlay.Tests;

public sealed class DecisionAdvisorTests
{
    [Fact]
    public void Analyze_PicksHighestTotalValue()
    {
        var rows = new[]
        {
            Row("chaos orb", 0.01m, 10),
            Row("divine orb", 1.0m, 1),
            Row("exalted orb", 0.05m, 3),
        };

        var advice = new DecisionAdvisor().Analyze(rows);

        Assert.Equal("divine orb", advice.BestRow!.MatchedKey);
        Assert.Equal(1.25m, advice.TotalDivines);
        Assert.Equal(0, advice.UnknownRows);
    }

    [Fact]
    public void Analyze_CountsUnknownRows()
    {
        var rows = new[]
        {
            Row("known", 0.1m, 1),
            new RewardRow(new RewardRead("???", "unknown", 2), null, null, false),
        };

        var advice = new DecisionAdvisor().Analyze(rows);

        Assert.Equal(1, advice.UnknownRows);
        Assert.Equal("known", advice.BestRow!.MatchedKey);
    }

    private static RewardRow Row(string name, decimal divine, int quantity) =>
        new(new RewardRead(name, name, 1, quantity), name, new PriceEntry(divine, divine * 100m), true);
}
