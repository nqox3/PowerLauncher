using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MinecraftLauncher.Services;

public class UpdateService
{
    private const string GITHUB_API = "https://api.github.com/repos/nqox3/PowerLauncher/releases/latest";
    private const string CURRENT_VERSION = "1.0.0";

    public event Action<string>? StatusChanged;
    public event Action<double>? ProgressChanged;

    private readonly string _launcherDir;
    private readonly HttpClient _client;

    public UpdateService(string launcherDir)
    {
        _launcherDir = launcherDir;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", "PowerLauncher/" + CURRENT_VERSION);
    }

    public string CurrentVersion => CURRENT_VERSION;

    public async Task<UpdateInfo?> CheckForUpdateAsync()
    {
        try
        {
            var response = await _client.GetStringAsync(GITHUB_API);
            var json = JObject.Parse(response);

            var tagName = json["tag_name"]?.ToString()?.TrimStart('v') ?? "";
            if (string.IsNullOrEmpty(tagName)) return null;

            if (IsNewerVersion(tagName, CURRENT_VERSION))
            {
                var assets = json["assets"] as JArray;
                var exeAsset = assets?.FirstOrDefault(a =>
                    a["name"]?.ToString().EndsWith(".exe") == true);

                return new UpdateInfo
                {
                    Version = tagName,
                    DownloadUrl = exeAsset?["browser_download_url"]?.ToString() ?? "",
                    FileName = exeAsset?["name"]?.ToString() ?? "",
                    ReleaseNotes = json["body"]?.ToString() ?? "",
                    PublishedAt = json["published_at"]?.ToString() ?? ""
                };
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task DownloadAndInstallAsync(UpdateInfo update)
    {
        if (string.IsNullOrEmpty(update.DownloadUrl)) return;

        StatusChanged?.Invoke("Downloading update...");
        var tempPath = Path.Combine(_launcherDir, "update_" + update.FileName);

        using var response = await _client.GetAsync(update.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1;
        var downloaded = 0L;

        using (var stream = await response.Content.ReadAsStreamAsync())
        using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
        {
            var buffer = new byte[81920];
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                downloaded += bytesRead;
                if (totalBytes > 0)
                    ProgressChanged?.Invoke((double)downloaded / totalBytes * 100);
            }
        }

        StatusChanged?.Invoke("Launching installer...");

        // Launch the installer and close current app
        Process.Start(new ProcessStartInfo
        {
            FileName = tempPath,
            UseShellExecute = true
        });

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            System.Windows.Application.Current.Shutdown();
        });
    }

    private bool IsNewerVersion(string remote, string local)
    {
        if (Version.TryParse(remote, out var remoteVer) && Version.TryParse(local, out var localVer))
            return remoteVer > localVer;
        return false;
    }
}

public class UpdateInfo
{
    public string Version { get; set; } = "";
    public string DownloadUrl { get; set; } = "";
    public string FileName { get; set; } = "";
    public string ReleaseNotes { get; set; } = "";
    public string PublishedAt { get; set; } = "";
}
