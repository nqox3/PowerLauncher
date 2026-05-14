using System.IO;
using System.Windows;
using System.Windows.Controls;
using MinecraftLauncher.Services;

namespace MinecraftLauncher.Views;

public partial class UpdatePage : Page
{
    private readonly UpdateService _updateService;
    private UpdateInfo? _availableUpdate;

    public UpdatePage()
    {
        InitializeComponent();
        var launcherDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PowerLauncher");
        _updateService = new UpdateService(launcherDir);
        CurrentVersionText.Text = "v" + _updateService.CurrentVersion;
    }

    private async void Check_Click(object sender, RoutedEventArgs e)
    {
        CheckBtn.IsEnabled = false;
        UpdateStatusText.Text = "Checking...";
        ReleaseNotesText.Text = "";
        InstallBtn.Visibility = Visibility.Collapsed;

        _availableUpdate = await _updateService.CheckForUpdateAsync();

        if (_availableUpdate != null)
        {
            UpdateStatusText.Text = $"New version available: v{_availableUpdate.Version}";
            ReleaseNotesText.Text = _availableUpdate.ReleaseNotes;
            InstallBtn.Visibility = Visibility.Visible;
        }
        else
        {
            UpdateStatusText.Text = "You are running the latest version.";
        }

        CheckBtn.IsEnabled = true;
    }

    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        if (_availableUpdate == null) return;

        InstallBtn.IsEnabled = false;
        CheckBtn.IsEnabled = false;
        ProgressBar.Visibility = Visibility.Visible;
        ProgressBar.Value = 0;

        _updateService.StatusChanged += s => Dispatcher.Invoke(() => ProgressText.Text = s);
        _updateService.ProgressChanged += p => Dispatcher.Invoke(() => ProgressBar.Value = p);

        await _updateService.DownloadAndInstallAsync(_availableUpdate);
    }
}
