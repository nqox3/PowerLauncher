using System.IO;
using System.Windows;
using MinecraftLauncher.Services;
using Wpf.Ui.Controls;

namespace MinecraftLauncher.Views;

public partial class BootstrapWindow : FluentWindow
{
    private readonly string _launcherDir;

    public BootstrapWindow()
    {
        InitializeComponent();
        _launcherDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PowerLauncher");

        Loaded += BootstrapWindow_Loaded;
    }

    private async void BootstrapWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Directory.CreateDirectory(_launcherDir);
            Directory.CreateDirectory(Path.Combine(_launcherDir, "game"));
            Directory.CreateDirectory(Path.Combine(_launcherDir, "game", "mods"));
            Directory.CreateDirectory(Path.Combine(_launcherDir, "runtime"));

            var javaService = new JavaService(_launcherDir);

            // Skip bootstrapper if everything is ready
            if (javaService.IsJavaInstalled())
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
                return;
            }

            // First time: install Java
            TitleText.Text = "Installing Java...";
            StatusText.Text = "Downloading Java 21 (this may take a minute)...";
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Value = 0;

            javaService.StatusChanged += s =>
                Dispatcher.Invoke(() => StatusText.Text = s);
            javaService.ProgressChanged += p =>
                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = p;
                    ProgressText.Text = $"{p:F0}%";
                });

            await javaService.InstallJavaAsync();
            await Task.Delay(500);

            TitleText.Text = "Ready!";
            StatusText.Text = "Launching PowerLauncher...";
            ProgressBar.Value = 100;
            ProgressText.Text = "";
            await Task.Delay(400);

            var main = new MainWindow();
            main.Show();
            Close();
        }
        catch (Exception ex)
        {
            TitleText.Text = "Setup Error";
            StatusText.Text = ex.Message;
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Value = 0;
            ProgressText.Text = "Click to retry";
            MouseLeftButtonDown += (_, _) => BootstrapWindow_Loaded(sender, e);
        }
    }
}
