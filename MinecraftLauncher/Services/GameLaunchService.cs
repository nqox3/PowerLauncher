using System.Diagnostics;
using System.IO;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using MinecraftLauncher.Models;

namespace MinecraftLauncher.Services;

public class GameLaunchService
{
    public event Action<string>? StatusChanged;
    public event Action<double>? ProgressChanged;
    public event Action? GameStarted;
    public event Action<int>? GameExited;
    public event Action<string>? GameCrashed;
    public event Action<string>? GameLogReceived;

    private MinecraftPath _minecraftPath;
    private string _gameDirectory;
    private readonly JavaService _javaService;
    private Process? _gameProcess;

    public bool IsGameRunning => _gameProcess != null && !_gameProcess.HasExited;

    public GameLaunchService(string gameDirectory, string launcherDir)
    {
        _gameDirectory = gameDirectory;
        _minecraftPath = new MinecraftPath(gameDirectory);
        _javaService = new JavaService(launcherDir);
    }

    public void UpdateGameDirectory(string newDirectory)
    {
        _gameDirectory = newDirectory;
        _minecraftPath = new MinecraftPath(newDirectory);
    }

    public async Task<List<GameVersion>> GetVersionsAsync(bool includeSnapshots = false)
    {
        StatusChanged?.Invoke("Fetching versions...");
        var launcher = new CmlLib.Core.MinecraftLauncher(_minecraftPath);
        var versions = await launcher.GetAllVersionsAsync();

        var result = new List<GameVersion>();
        foreach (var v in versions)
        {
            if (!includeSnapshots && v.Type != "release")
                continue;

            result.Add(new GameVersion
            {
                Id = v.Name,
                Type = v.Type ?? "release",
                IsInstalled = false
            });
        }

        return result;
    }

    public async Task LaunchGameAsync(LauncherSettings settings, LauncherAccount account)
    {
        StatusChanged?.Invoke("Preparing to launch...");

        // Auto-detect or install Java
        var javaPath = settings.JavaPath;
        if (string.IsNullOrEmpty(javaPath))
        {
            javaPath = _javaService.FindJava();
            if (javaPath == null)
            {
                StatusChanged?.Invoke("Java not found, installing...");
                _javaService.StatusChanged += s => StatusChanged?.Invoke(s);
                _javaService.ProgressChanged += p => ProgressChanged?.Invoke(p);
                await _javaService.InstallJavaAsync();
                javaPath = _javaService.FindJava();
            }
        }

        var launcher = new CmlLib.Core.MinecraftLauncher(_minecraftPath);
        launcher.FileProgressChanged += (sender, args) =>
        {
            StatusChanged?.Invoke($"Downloading: {args.Name} ({args.ProgressedTasks}/{args.TotalTasks})");
            if (args.TotalTasks > 0)
                ProgressChanged?.Invoke((double)args.ProgressedTasks / args.TotalTasks * 100);
        };
        launcher.ByteProgressChanged += (sender, args) =>
        {
            if (args.TotalBytes > 0)
                ProgressChanged?.Invoke((double)args.ProgressedBytes / args.TotalBytes * 100);
        };

        var session = account.Type == AccountType.Offline
            ? MSession.CreateOfflineSession(account.Username)
            : new MSession(account.Username, account.AccessToken, account.UUID);

        StatusChanged?.Invoke($"Installing version {settings.SelectedVersion}...");
        await launcher.InstallAsync(settings.SelectedVersion);

        StatusChanged?.Invoke("Building process...");
        var process = await launcher.BuildProcessAsync(settings.SelectedVersion, new MLaunchOption
        {
            Session = session,
            MaximumRamMb = settings.MaxRamMb,
            MinimumRamMb = settings.MinRamMb,
            FullScreen = settings.FullScreen,
            ScreenWidth = settings.WindowWidth,
            ScreenHeight = settings.WindowHeight,
            JavaPath = javaPath,
        });

        // Configure process for output capture
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.EnableRaisingEvents = true;

        process.OutputDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                GameLogReceived?.Invoke(e.Data);
        };
        process.ErrorDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                GameLogReceived?.Invoke($"[ERR] {e.Data}");
        };

        process.Exited += (s, e) =>
        {
            var exitCode = process.ExitCode;
            _gameProcess = null;

            if (exitCode != 0)
            {
                // Crash detected
                var crashLog = GetLatestCrashLog();
                GameCrashed?.Invoke(crashLog ?? $"Game exited with code {exitCode}");
            }
            else
            {
                GameExited?.Invoke(exitCode);
            }
        };

        StatusChanged?.Invoke("Launching Minecraft...");
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        _gameProcess = process;

        GameStarted?.Invoke();
    }

    private string? GetLatestCrashLog()
    {
        var crashDir = Path.Combine(_gameDirectory, "crash-reports");
        if (!Directory.Exists(crashDir)) return null;

        var latest = Directory.GetFiles(crashDir, "*.txt")
            .OrderByDescending(File.GetLastWriteTime)
            .FirstOrDefault();

        if (latest == null) return null;

        try
        {
            var content = File.ReadAllText(latest);
            // Return first 2000 chars of crash report
            return content.Length > 2000 ? content[..2000] + "\n..." : content;
        }
        catch
        {
            return null;
        }
    }
}
