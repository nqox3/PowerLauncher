using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MinecraftLauncher.Services;

public class LoaderInstallService
{
    private readonly string _gameDirectory;
    private readonly HttpClient _client;

    private const string FABRIC_META = "https://meta.fabricmc.net/v2";
    private const string FORGE_VERSIONS = "https://files.minecraftforge.net/net/minecraftforge/forge/promotions_slim.json";

    public event Action<string>? StatusChanged;

    public LoaderInstallService(string gameDirectory)
    {
        _gameDirectory = gameDirectory;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", "PowerLauncher/1.0");
    }

    public async Task<List<string>> GetFabricGameVersionsAsync()
    {
        var json = await _client.GetStringAsync($"{FABRIC_META}/versions/game");
        var arr = JArray.Parse(json);
        return arr.Where(v => v["stable"]?.Value<bool>() == true)
                  .Select(v => v["version"]?.ToString() ?? "")
                  .Where(v => !string.IsNullOrEmpty(v))
                  .ToList();
    }

    public async Task<string?> GetLatestFabricLoaderAsync()
    {
        var json = await _client.GetStringAsync($"{FABRIC_META}/versions/loader");
        var arr = JArray.Parse(json);
        return arr.FirstOrDefault()?["version"]?.ToString();
    }

    public async Task InstallFabricAsync(string gameVersion)
    {
        StatusChanged?.Invoke($"Getting Fabric loader for {gameVersion}...");

        var loaderVersion = await GetLatestFabricLoaderAsync();
        if (loaderVersion == null) throw new Exception("Could not find Fabric loader version");

        var profileUrl = $"{FABRIC_META}/versions/loader/{gameVersion}/{loaderVersion}/profile/json";
        StatusChanged?.Invoke($"Downloading Fabric {loaderVersion} profile...");

        var profileJson = await _client.GetStringAsync(profileUrl);
        var profile = JObject.Parse(profileJson);
        var versionId = profile["id"]?.ToString() ?? $"fabric-loader-{loaderVersion}-{gameVersion}";

        // Save to versions folder
        var versionDir = Path.Combine(_gameDirectory, "versions", versionId);
        Directory.CreateDirectory(versionDir);
        var jsonPath = Path.Combine(versionDir, $"{versionId}.json");
        await File.WriteAllTextAsync(jsonPath, profileJson);

        StatusChanged?.Invoke($"Fabric {loaderVersion} for {gameVersion} installed!");
    }

    public async Task InstallQuiltAsync(string gameVersion)
    {
        StatusChanged?.Invoke($"Getting Quilt loader for {gameVersion}...");

        // Quilt uses similar meta API
        var metaUrl = $"https://meta.quiltmc.org/v3/versions/loader/{gameVersion}";
        var json = await _client.GetStringAsync(metaUrl);
        var arr = JArray.Parse(json);
        var latest = arr.FirstOrDefault();
        if (latest == null) throw new Exception("No Quilt loader found for this version");

        var loaderVersion = latest["loader"]?["version"]?.ToString() ?? "";
        var profileUrl = $"https://meta.quiltmc.org/v3/versions/loader/{gameVersion}/{loaderVersion}/profile/json";

        StatusChanged?.Invoke($"Downloading Quilt {loaderVersion} profile...");
        var profileJson = await _client.GetStringAsync(profileUrl);
        var profile = JObject.Parse(profileJson);
        var versionId = profile["id"]?.ToString() ?? $"quilt-loader-{loaderVersion}-{gameVersion}";

        var versionDir = Path.Combine(_gameDirectory, "versions", versionId);
        Directory.CreateDirectory(versionDir);
        await File.WriteAllTextAsync(Path.Combine(versionDir, $"{versionId}.json"), profileJson);

        StatusChanged?.Invoke($"Quilt {loaderVersion} for {gameVersion} installed!");
    }
}
