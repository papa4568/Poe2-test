using Newtonsoft.Json;

namespace AncientValueOverlay;

public sealed class PriceCacheStore
{
    private readonly string _cacheDir;

    public PriceCacheStore(string? cacheDir = null)
    {
        _cacheDir = cacheDir ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AncientValueOverlay",
            "cache");
    }

    public async Task SaveAsync(string league, IReadOnlyDictionary<string, PriceEntry> prices, CancellationToken ct = default)
    {
        Directory.CreateDirectory(_cacheDir);
        var snapshot = new PriceCacheSnapshot(DateTimeOffset.UtcNow, league, prices);
        var json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);
        var path = CachePath(league);
        var temp = path + ".tmp";
        await File.WriteAllTextAsync(temp, json, ct);
        File.Move(temp, path, overwrite: true);
    }

    public async Task<PriceCacheSnapshot?> LoadAsync(string league, CancellationToken ct = default)
    {
        var path = CachePath(league);
        if (!File.Exists(path)) return null;
        try
        {
            var json = await File.ReadAllTextAsync(path, ct);
            return JsonConvert.DeserializeObject<PriceCacheSnapshot>(json);
        }
        catch
        {
            return null;
        }
    }

    private string CachePath(string league)
    {
        var safe = string.Concat(league.Select(c => char.IsLetterOrDigit(c) ? c : '_'));
        return Path.Combine(_cacheDir, $"prices_{safe}.json");
    }
}

public sealed record PriceCacheSnapshot(DateTimeOffset SavedUtc, string League, IReadOnlyDictionary<string, PriceEntry> Prices);
