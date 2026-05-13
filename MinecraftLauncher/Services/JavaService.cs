using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.IO.Compression;

namespace MinecraftLauncher.Services;

public class JavaService
{
    private readonly string _javaDir;
    private const string ADOPTIUM_API = "https://api.adoptium.net/v3/binary/latest/21/ga/windows/x64/jre/hotspot/normal/eclipse";

    public event Action<string>? StatusChanged;
    public event Action<double>? ProgressChanged;

    public JavaService(string launcherDir)
    {
        _javaDir = Path.Combine(launcherDir, "runtime", "java-21");
        Directory.CreateDirectory(Path.Combine(launcherDir, "runtime"));
    }

    public string? FindJava()
    {
        // Check our bundled java first
        var bundledJava = Path.Combine(_javaDir, "bin", "javaw.exe");
        if (File.Exists(bundledJava))
            return bundledJava;

        // Check JAVA_HOME
        var javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
        if (!string.IsNullOrEmpty(javaHome))
        {
            var javaw = Path.Combine(javaHome, "bin", "javaw.exe");
            if (File.Exists(javaw)) return javaw;
        }

        // Check PATH
        var pathDirs = Environment.GetEnvironmentVariable("PATH")?.Split(';') ?? Array.Empty<string>();
        foreach (var dir in pathDirs)
        {
            var javaw = Path.Combine(dir, "javaw.exe");
            if (File.Exists(javaw)) return javaw;
        }

        return null;
    }

    public bool IsJavaInstalled() => FindJava() != null;

    public async Task InstallJavaAsync()
    {
        StatusChanged?.Invoke("Downloading Java 21 (Adoptium)...");

        var zipPath = _javaDir + ".zip";
        Directory.CreateDirectory(Path.GetDirectoryName(_javaDir)!);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "PowerLauncher/1.0");

        using var response = await client.GetAsync(ADOPTIUM_API, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1;
        var downloadedBytes = 0L;

        using (var stream = await response.Content.ReadAsStreamAsync())
        using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            var buffer = new byte[81920];
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                downloadedBytes += bytesRead;
                if (totalBytes > 0)
                    ProgressChanged?.Invoke((double)downloadedBytes / totalBytes * 100);
            }
        }

        StatusChanged?.Invoke("Extracting Java...");
        ProgressChanged?.Invoke(0);

        if (Directory.Exists(_javaDir))
            Directory.Delete(_javaDir, true);

        var tempExtract = _javaDir + "_temp";
        if (Directory.Exists(tempExtract))
            Directory.Delete(tempExtract, true);

        ZipFile.ExtractToDirectory(zipPath, tempExtract);

        // Adoptium zips have a nested folder like jdk-21.0.x+y-jre
        var innerDir = Directory.GetDirectories(tempExtract).FirstOrDefault();
        if (innerDir != null)
            Directory.Move(innerDir, _javaDir);
        else
            Directory.Move(tempExtract, _javaDir);

        if (Directory.Exists(tempExtract))
            Directory.Delete(tempExtract, true);

        File.Delete(zipPath);

        StatusChanged?.Invoke("Java 21 installed successfully!");
        ProgressChanged?.Invoke(100);
    }

    public string GetJavaVersion()
    {
        var java = FindJava();
        if (java == null) return "Not installed";

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = java.Replace("javaw.exe", "java.exe"),
                Arguments = "-version",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            if (proc == null) return "Unknown";
            var output = proc.StandardError.ReadToEnd();
            proc.WaitForExit(5000);
            var line = output.Split('\n').FirstOrDefault() ?? "Unknown";
            return line.Trim();
        }
        catch
        {
            return "Unknown";
        }
    }
}
