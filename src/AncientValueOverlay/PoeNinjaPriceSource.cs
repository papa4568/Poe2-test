using Newtonsoft.Json.Linq;

namespace AncientValueOverlay;

public sealed class PoeNinjaPriceSource : IPriceSource
{
    private static readonly string[] ExchangeTypes = ["Currency", "Runes", "Expedition", "Verisium", "UncutGems"];
    private readonly HttpClient _http;
    private readonly PriceCacheStore _cache;

    public PoeNinjaPriceSource(HttpClient http, PriceCacheStore cache)
    {
        _http = http;
        _cache = cache;
    }

    public async Task<PriceFetchResult> FetchAsync(AppConfig config, CancellationToken ct = default)
    {
        try
        {
            var tasks = ExchangeTypes.Select(t => FetchTypeAsync(config.LeagueName, t, ct)).ToArray();
            var parts = await Task.WhenAll(tasks);
            var dict = parts.SelectMany(x => x).ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
            if (dict.Count > 0)
            {
                if (config.SavePriceCache) await _cache.SaveAsync(config.LeagueName, dict, ct);
                return new PriceFetchResult(dict, new PriceSourceHealth(PriceSourceStatus.Live, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, dict.Count, $"Loaded {dict.Count} live prices."));
            }
        }
        catch
        {
        }

        if (config.LoadCacheOnStartupFailure)
        {
            var cached = await _cache.LoadAsync(config.LeagueName, ct);
            if (cached is { Prices.Count: > 0 })
                return new PriceFetchResult(cached.Prices, new PriceSourceHealth(PriceSourceStatus.CacheOnly, null, cached.SavedUtc, cached.Prices.Count, $"Using cached prices from {cached.SavedUtc.LocalDateTime:g}."));
        }

        return new PriceFetchResult(new Dictionary<string, PriceEntry>(), new PriceSourceHealth(PriceSourceStatus.Failed, null, null, 0, "Price source failed and no cache was available."));
    }

    private async Task<Dictionary<string, PriceEntry>> FetchTypeAsync(string league, string type, CancellationToken ct)
    {
        var url = $"https://poe.ninja/poe2/api/economy/exchange/current/overview?league={Uri.EscapeDataString(league)}&type={Uri.EscapeDataString(type)}";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.TryAddWithoutValidation("User-Agent", "AncientValueOverlay/0.1");
        using var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode) return [];
        var json = await resp.Content.ReadAsStringAsync(ct);
        return ParseExchangeOverview(json);
    }

    internal static Dictionary<string, PriceEntry> ParseExchangeOverview(string json)
    {
        var result = new Dictionary<string, PriceEntry>(StringComparer.OrdinalIgnoreCase);
        var obj = JObject.Parse(json);
        var idToName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (obj["items"] is JArray items)
            foreach (var item in items)
            {
                var id = item["id"]?.Value<string>();
                var name = item["name"]?.Value<string>();
                if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(name)) idToName[id] = NormalizeName(name);
            }

        var core = obj["core"];
        var primary = core?["primary"]?.Value<string>() ?? "divine";
        var rates = core?["rates"];
        var divinePerPrimary = primary == "divine" ? 1m : rates?["divine"]?.Value<decimal>() ?? 0m;
        var exaltedPerPrimary = primary == "exalted" ? 1m : rates?["exalted"]?.Value<decimal>() ?? 1m;

        if (obj["lines"] is not JArray lines) return result;
        foreach (var line in lines)
        {
            var id = line["id"]?.Value<string>();
            if (id is null || !idToName.TryGetValue(id, out var key)) continue;
            var token = line["primaryValue"];
            if (token is null || token.Type == JTokenType.Null)
            {
                result[key] = new PriceEntry(0m, 0m, false);
                continue;
            }
            var primaryValue = token.Value<decimal>();
            result[key] = new PriceEntry(primaryValue * divinePerPrimary, Math.Round(primaryValue * exaltedPerPrimary, 1));
        }
        return result;
    }

    internal static string NormalizeName(string input)
    {
        var chars = input.ToLowerInvariant().Select(c => char.IsLetterOrDigit(c) ? c : ' ').ToArray();
        return string.Join(' ', new string(chars).Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
