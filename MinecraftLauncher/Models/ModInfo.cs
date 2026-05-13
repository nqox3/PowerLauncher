namespace MinecraftLauncher.Models;

public class ModInfo
{
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public long FileSize { get; set; }
    public ModLoader Loader { get; set; } = ModLoader.Forge;
}

public enum ModLoader
{
    Forge,
    Fabric,
    Quilt,
    NeoForge
}
