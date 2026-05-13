namespace MinecraftLauncher.Models;

public class GameVersion
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // release, snapshot, old_beta, old_alpha
    public DateTime ReleaseDate { get; set; }
    public bool IsInstalled { get; set; }
}
