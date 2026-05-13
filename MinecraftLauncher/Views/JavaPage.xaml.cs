using System.IO;
using System.Windows;
using System.Windows.Controls;
using MinecraftLauncher.Services;

namespace MinecraftLauncher.Views;

public partial class JavaPage : Page
{
    private readonly JavaService _javaService;

    public JavaPage()
    {
        InitializeComponent();
        var launcherDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PowerLauncher");
        _javaService = new JavaService(launcherDir);
        Loaded += (_, _) => RefreshStatus();
    }

    private void RefreshStatus()
    {
        var java = _javaService.FindJava();
        if (java != null)
        {
            JavaVersionText.Text = $"✅ {_javaService.GetJavaVersion()}";
            JavaPathText.Text = $"Path: {java}";
        }
        else
        {
            JavaVersionText.Text = "❌ Java not found";
            JavaPathText.Text = "Click 'Install' to download Java 21 automatically.";
        }
    }

    private void Refresh_Click(object sender, RoutedEventArgs e) => RefreshStatus();

    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        InstallBtn.IsEnabled = false;
        ProgressBar.Visibility = Visibility.Visible;
        ProgressBar.IsIndeterminate = false;
        ProgressBar.Value = 0;

        _javaService.StatusChanged += s => Dispatcher.Invoke(() => StatusText.Text = s);
        _javaService.ProgressChanged += p => Dispatcher.Invoke(() => ProgressBar.Value = p);

        try
        {
            await _javaService.InstallJavaAsync();
            RefreshStatus();
            StatusText.Text = "Java installed successfully!";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
        finally
        {
            InstallBtn.IsEnabled = true;
            ProgressBar.Visibility = Visibility.Collapsed;
        }
    }
}
