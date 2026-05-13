using System.IO;
using System.Net.Http;
using MinecraftLauncher.Models;
using Newtonsoft.Json.Linq;

namespace MinecraftLauncher.Services;

public class ModMarketService
{
    private readonly HttpClient _client;
    private readonly string _modsDir;

    private const string MODRINTH_SEARCH = "https://api.modrinth.com/v2/search";
    private const string MODRINTH_VERSION = "https://api.modrinth.com/v2/project/{0}/version";

    public ModMarketService(string gameDirectory)
    {
        _modsDir = Path.Combine(gameDirectory, "mods");
        Directory.CreateDirectory(_modsDir);
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", "PowerLauncher/1.0");
    }

    public async Task<List<MarketMod>> SearchModrinthAsync(
        string query,
        ModLoaderTag loader = ModLoaderTag.Any,
        string gameVersion = "",
        string sortBy = "relevance",
        int limit = 20,
        int offset = 0)
    {
        var facets = new List<string>();
        facets.Add("[\"project_type:mod\"]");

        if (loader != ModLoaderTag.Any)
            facets.Add($"[\"categories:{loader.ToString().ToLower()}\"]");

        if (!string.IsNullOrEmpty(gameVersion))
            facets.Add($"[\"versions:{gameVersion}\"]");

        var facetStr = $"[{string.Join(",", facets)}]";
        var url = $"{MODRINTH_SEARCH}?query={Uri.EscapeDataString(query)}&facets={Uri.EscapeDataString(facetStr)}&index={sortBy}&limit={limit}&offset={offset}";

        var response = await _client.GetStringAsync(url);
        var json = JObject.Parse(response);
        var hits = json["hits"] as JArray ?? new JArray();

        var results = new List<MarketMod>();
        foreach (var hit in hits)
        {
            results.Add(new MarketMod
            {
                Id = hit["project_id"]?.ToString() ?? "",
                Name = hit["title"]?.ToString() ?? "",
                Description = hit["description"]?.ToString() ?? "",
                Author = hit["author"]?.ToString() ?? "",
                IconUrl = hit["icon_url"]?.ToString() ?? "",
                Downloads = hit["downloads"]?.Value<int>() ?? 0,
                Source = ModSource.Modrinth,
                Loader = ParseLoaderFromCategories(hit["categories"] as JArray),
                Category = hit["categories"]?.FirstOrDefault()?.ToString() ?? ""
            });
        }

        return results;
    }

    public async Task<string?> GetModrinthDownloadUrlAsync(string projectId, string gameVersion = "", ModLoaderTag loader = ModLoaderTag.Any)
    {
        var url = string.Format(MODRINTH_VERSION, projectId);
        var response = await _client.GetStringAsync(url);
        var versions = JArray.Parse(response);

        foreach (var ver in versions)
        {
            var gameVersions = ver["game_versions"] as JArray;
            var loaders = ver["loaders"] as JArray;

            if (!string.IsNullOrEmpty(gameVersion) && gameVersions != null)
            {
                if (!gameVersions.Any(v => v.ToString() == gameVersion))
                    continue;
            }

            if (loader != ModLoaderTag.Any && loaders != null)
            {
                if (!loaders.Any(l => l.ToString().Equals(loader.ToString(), StringComparison.OrdinalIgnoreCase)))
                    continue;
            }

            var files = ver["files"] as JArray;
            var primary = files?.FirstOrDefault(f => f["primary"]?.Value<bool>() == true)
                          ?? files?.FirstOrDefault();
            if (primary != null)
                return primary["url"]?.ToString();
        }

        // Fallback: first version's first file
        var firstVer = versions.FirstOrDefault();
        var firstFiles = firstVer?["files"] as JArray;
        return firstFiles?.FirstOrDefault()?["url"]?.ToString();
    }

    public async Task DownloadModAsync(string downloadUrl, string fileName, Action<double>? progress = null)
    {
        using var response = await _client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1;
        var destPath = Path.Combine(_modsDir, fileName);

        using var stream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(destPath, FileMode.Create, FileAccess.Write);

        var buffer = new byte[81920];
        var downloaded = 0L;
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await fileStream.WriteAsync(buffer, 0, bytesRead);
            downloaded += bytesRead;
            if (totalBytes > 0)
                progress?.Invoke((double)downloaded / totalBytes * 100);
        }
    }

    private ModLoaderTag ParseLoaderFromCategories(JArray? categories)
    {
        if (categories == null) return ModLoaderTag.Any;
        foreach (var cat in categories)
        {
            var s = cat.ToString().ToLower();
            if (s == "fabric") return ModLoaderTag.Fabric;
            if (s == "forge") return ModLoaderTag.Forge;
            if (s == "quilt") return ModLoaderTag.Quilt;
            if (s == "neoforge") return ModLoaderTag.NeoForge;
        }
        return ModLoaderTag.Any;
    }
}
