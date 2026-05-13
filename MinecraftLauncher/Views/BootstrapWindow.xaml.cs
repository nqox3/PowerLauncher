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
            // Step 1: Create directories
            StatusText.Text = "Creating launcher directories...";
            Directory.CreateDirectory(_launcherDir);
            Directory.CreateDirectory(Path.Combine(_launcherDir, "game"));
            Directory.CreateDirectory(Path.Combine(_launcherDir, "game", "mods"));
            Directory.CreateDirectory(Path.Combine(_launcherDir, "runtime"));
            await Task.Delay(500);

            // Step 2: Check Java
            StatusText.Text = "Checking Java installation...";
            var javaService = new JavaService(_launcherDir);
            await Task.Delay(300);

            if (!javaService.IsJavaInstalled())
            {
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
            }
            else
            {
                StatusText.Text = $"Java found: {javaService.GetJavaVersion()}";
                await Task.Delay(800);
            }

            // Step 3: Done
            TitleText.Text = "Ready!";
            StatusText.Text = "Launching PowerLauncher...";
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Value = 100;
            ProgressText.Text = "";
            await Task.Delay(600);

            // Open main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
        catch (Exception ex)
        {
            TitleText.Text = "Setup Error";
            StatusText.Text = ex.Message;
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Value = 0;
            ProgressText.Text = "Click to retry or close the window";
            MouseLeftButtonDown += (_, _) =>
            {
                BootstrapWindow_Loaded(sender, e);
            };
        }
    }
}
