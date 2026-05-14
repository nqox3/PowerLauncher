namespace MinecraftLauncher.Models;

public class GameVersion
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // release, snapshot, fabric, forge, quilt
    public DateTime ReleaseDate { get; set; }
    public bool IsInstalled { get; set; }
    public string LoaderType { get; set; } = "vanilla"; // vanilla, fabric, forge, quilt, neoforge
    public string DisplayName => LoaderType == "vanilla" ? Id : $"{Id} [{LoaderType}]";
}
