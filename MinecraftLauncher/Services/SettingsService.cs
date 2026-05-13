using System.IO;
using MinecraftLauncher.Models;
using Newtonsoft.Json;

namespace MinecraftLauncher.Services;

public class SettingsService
{
    private readonly string _settingsPath;
    private readonly string _accountsPath;

    public SettingsService()
    {
        var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PowerLauncher");
        Directory.CreateDirectory(appData);
        _settingsPath = Path.Combine(appData, "settings.json");
        _accountsPath = Path.Combine(appData, "accounts.json");
    }

    public LauncherSettings LoadSettings()
    {
        if (!File.Exists(_settingsPath))
            return CreateDefaultSettings();

        var json = File.ReadAllText(_settingsPath);
        return JsonConvert.DeserializeObject<LauncherSettings>(json) ?? CreateDefaultSettings();
    }

    public void SaveSettings(LauncherSettings settings)
    {
        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(_settingsPath, json);
    }

    public List<LauncherAccount> LoadAccounts()
    {
        if (!File.Exists(_accountsPath))
            return new List<LauncherAccount>();

        var json = File.ReadAllText(_accountsPath);
        return JsonConvert.DeserializeObject<List<LauncherAccount>>(json) ?? new List<LauncherAccount>();
    }

    public void SaveAccounts(List<LauncherAccount> accounts)
    {
        var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
        File.WriteAllText(_accountsPath, json);
    }

    private LauncherSettings CreateDefaultSettings()
    {
        var settings = new LauncherSettings
        {
            GameDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PowerLauncher", "game")
        };
        Directory.CreateDirectory(settings.GameDirectory);
        SaveSettings(settings);
        return settings;
    }
}
