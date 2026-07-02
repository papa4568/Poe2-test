namespace AncientValueOverlay;

public sealed class PriceCacheStore
{
    public string CacheDirectory { get; }

    public PriceCacheStore(string? cacheDirectory = null)
    {
        CacheDirectory = cacheDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AncientValueOverlay",
            "cache");
    }

    public void EnsureCreated()
    {
        Directory.CreateDirectory(CacheDirectory);
    }
}
