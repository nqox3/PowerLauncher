namespace MinecraftLauncher.Models;

public class LauncherSettings
{
    public int MinRamMb { get; set; } = 512;
    public int MaxRamMb { get; set; } = 4096;
    public string JavaPath { get; set; } = string.Empty;
    public string GameDirectory { get; set; } = string.Empty;
    public bool FullScreen { get; set; } = false;
    public int WindowWidth { get; set; } = 854;
    public int WindowHeight { get; set; } = 480;
    public string SelectedVersion { get; set; } = string.Empty;
    public string LastAccount { get; set; } = string.Empty;
    public bool ShowSnapshots { get; set; } = false;
    public bool AutoCloseOnLaunch { get; set; } = false;
    public string CustomJvmArgs { get; set; } = string.Empty;
}
