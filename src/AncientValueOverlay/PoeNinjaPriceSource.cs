namespace AncientValueOverlay;

public sealed class PoeNinjaPriceSource
{
    public string Status => "Disabled in simple starter build";

    public IReadOnlyList<RewardRow> GetDemoRows()
    {
        return
        [
            new RewardRow("Divine Orb", 1.00m, 1),
            new RewardRow("Exalted Orb", 0.05m, 3),
            new RewardRow("Unknown Reward", 0m, 1)
        ];
    }
}
