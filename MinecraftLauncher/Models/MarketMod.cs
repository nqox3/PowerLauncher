namespace MinecraftLauncher.Models;

public class MarketMod
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string GameVersion { get; set; } = string.Empty;
    public int Downloads { get; set; }
    public ModSource Source { get; set; }
    public ModLoaderTag Loader { get; set; }
    public string Category { get; set; } = string.Empty;
}

public enum ModSource
{
    Modrinth,
    CurseForge
}

public enum ModLoaderTag
{
    Any,
    Fabric,
    Forge,
    Quilt,
    NeoForge
}
