using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MinecraftLauncher.Models;
using MinecraftLauncher.Services;
using System.Collections.ObjectModel;

namespace MinecraftLauncher.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SettingsService _settingsService;
    private readonly GameLaunchService _gameLaunchService;
    private readonly ModService _modService;
    private readonly string _launcherDir;

    public event Action? OnGameStarted;
    public event Action? OnGameExited;
    public event Action<string>? OnGameCrashed;
    public event Action<string>? OnGameLog;

    [ObservableProperty] private string _statusText = "Ready";
    [ObservableProperty] private double _progress;
    [ObservableProperty] private bool _isLaunching;
    [ObservableProperty] private string _selectedVersion = string.Empty;
    [ObservableProperty] private string _username = "Player";
    [ObservableProperty] private LauncherSettings _settings;
    [ObservableProperty] private ObservableCollection<GameVersion> _versions = new();
    [ObservableProperty] private GameVersion? _selectedVersionItem;
    [ObservableProperty] private ObservableCollection<ModInfo> _mods = new();
    [ObservableProperty] private ObservableCollection<LauncherAccount> _accounts = new();
    [ObservableProperty] private LauncherAccount? _selectedAccount;
    [ObservableProperty] private int _modCount;

    public MainViewModel()
    {
        _launcherDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PowerLauncher");
        Directory.CreateDirectory(_launcherDir);

        _settingsService = new SettingsService();
        Settings = _settingsService.LoadSettings();

        if (string.IsNullOrEmpty(Settings.GameDirectory))
            Settings.GameDirectory = Path.Combine(_launcherDir, "game");

        Directory.CreateDirectory(Settings.GameDirectory);

        _gameLaunchService = new GameLaunchService(Settings.GameDirectory, _launcherDir);
        _modService = new ModService(Settings.GameDirectory);

        _gameLaunchService.StatusChanged += status => StatusText = status;
        _gameLaunchService.ProgressChanged += p => Progress = p;
        _gameLaunchService.GameStarted += () => OnGameStarted?.Invoke();
        _gameLaunchService.GameExited += code => OnGameExited?.Invoke();
        _gameLaunchService.GameCrashed += info => OnGameCrashed?.Invoke(info);
        _gameLaunchService.GameLogReceived += log => OnGameLog?.Invoke(log);

        LoadAccounts();
        LoadMods();
    }

    private void LoadAccounts()
    {
        var accounts = _settingsService.LoadAccounts();
        Accounts = new ObservableCollection<LauncherAccount>(accounts);
        SelectedAccount = Accounts.FirstOrDefault();
        if (SelectedAccount != null)
            Username = SelectedAccount.Username;
    }

    private void LoadMods()
    {
        var mods = _modService.GetInstalledMods();
        Mods = new ObservableCollection<ModInfo>(mods);
        ModCount = Mods.Count;
    }

    [RelayCommand]
    private async Task LoadVersionsAsync()
    {
        try
        {
            StatusText = "Loading versions...";
            var gameVersions = await _gameLaunchService.GetVersionsAsync(Settings.ShowSnapshots);
            Versions = new ObservableCollection<GameVersion>(gameVersions);
            if (Versions.Any())
            {
                SelectedVersionItem = Versions.First();
                SelectedVersion = SelectedVersionItem.Id;
            }
            StatusText = $"Loaded {Versions.Count} versions";
        }
        catch (Exception ex)
        {
            StatusText = $"Error loading versions: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task LaunchGameAsync()
    {
        if (string.IsNullOrEmpty(SelectedVersion))
        {
            StatusText = "Please select a version first!";
            return;
        }

        try
        {
            IsLaunching = true;
            Settings.SelectedVersion = SelectedVersion;

            var account = SelectedAccount ?? new LauncherAccount
            {
                Username = Username,
                Type = AccountType.Offline
            };

            await _gameLaunchService.LaunchGameAsync(Settings, account);
            StatusText = "Game launched!";
        }
        catch (Exception ex)
        {
            StatusText = $"Launch failed: {ex.Message}";
        }
        finally
        {
            IsLaunching = false;
        }
    }

    [RelayCommand]
    private void AddOfflineAccount()
    {
        if (string.IsNullOrWhiteSpace(Username)) return;

        var account = new LauncherAccount
        {
            Username = Username,
            Type = AccountType.Offline,
            UUID = Guid.NewGuid().ToString("N")
        };

        Accounts.Add(account);
        SelectedAccount = account;
        _settingsService.SaveAccounts(Accounts.ToList());
        StatusText = $"Account '{Username}' added";
    }

    [RelayCommand]
    private void RemoveAccount()
    {
        if (SelectedAccount == null) return;
        var name = SelectedAccount.Username;
        Accounts.Remove(SelectedAccount);
        SelectedAccount = Accounts.FirstOrDefault();
        _settingsService.SaveAccounts(Accounts.ToList());
        StatusText = $"Account '{name}' removed";
    }

    [RelayCommand]
    private void ToggleMod(ModInfo? mod)
    {
        if (mod == null) return;
        _modService.ToggleMod(mod);
        LoadMods();
    }

    [RelayCommand]
    private void DeleteMod(ModInfo? mod)
    {
        if (mod == null) return;
        _modService.DeleteMod(mod);
        LoadMods();
        StatusText = $"Mod '{mod.Name}' deleted";
    }

    [RelayCommand]
    private void OpenModsFolder()
    {
        _modService.OpenModsFolder();
    }

    [RelayCommand]
    private void RefreshMods()
    {
        LoadMods();
        StatusText = $"Found {Mods.Count} mods";
    }

    [RelayCommand]
    private void SaveSettings()
    {
        _settingsService.SaveSettings(Settings);
        StatusText = "Settings saved!";
    }
}
