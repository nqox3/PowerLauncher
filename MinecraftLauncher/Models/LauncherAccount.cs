namespace MinecraftLauncher.Models;

public class LauncherAccount
{
    public string Username { get; set; } = string.Empty;
    public string UUID { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public AccountType Type { get; set; } = AccountType.Offline;
    public DateTime LastUsed { get; set; } = DateTime.Now;
}

public enum AccountType
{
    Offline,
    Microsoft
}
