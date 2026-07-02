namespace AncientValueOverlay;

public interface IPriceSource
{
    Task<PriceFetchResult> FetchAsync(AppConfig config, CancellationToken ct = default);
}

public interface IRewardReader
{
    Task<IReadOnlyList<RewardRead>> ReadAsync(CalibrationProfile profile, CancellationToken ct = default);
}

public interface IValuePresenter
{
    void Show(PanelAdvice advice, PriceSourceHealth health);
    void Hide();
}

public interface IConfigStore
{
    Task<AppConfig> LoadAsync(CancellationToken ct = default);
    Task SaveAsync(AppConfig config, CancellationToken ct = default);
}
