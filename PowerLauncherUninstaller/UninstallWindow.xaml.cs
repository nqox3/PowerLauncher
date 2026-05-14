using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace PowerLauncherUninstaller;

public partial class UninstallWindow : Window
{
    private const string APP_NAME = "PowerLauncher";

    public UninstallWindow()
    {
        InitializeComponent();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private async void Uninstall_Click(object sender, RoutedEventArgs e)
    {
        ConfirmPanel.Visibility = Visibility.Collapsed;
        ProgressPanel.Visibility = Visibility.Visible;

        await Task.Run(() =>
        {
            // Remove install directory
            var installDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);
            if (installDir != null)
            {
                // Kill the main app if running
                foreach (var proc in Process.GetProcessesByName("MinecraftLauncher"))
                {
                    try { proc.Kill(); } catch { }
                }
                Thread.Sleep(500);
            }

            // Remove shortcuts
            Dispatcher.Invoke(() => StatusText.Text = "Removing shortcuts...");
            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var startMenu = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs");
            TryDelete(Path.Combine(desktop, APP_NAME + ".lnk"));
            TryDelete(Path.Combine(startMenu, APP_NAME + ".lnk"));

            // Remove registry
            Dispatcher.Invoke(() => StatusText.Text = "Removing registry entries...");
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\" + APP_NAME, false);
                var runKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                runKey?.DeleteValue(APP_NAME, false);
                runKey?.Close();
            }
            catch { }

            // Remove app data if checked
            var removeData = false;
            Dispatcher.Invoke(() => removeData = RemoveData.IsChecked == true);

            if (removeData)
            {
                Dispatcher.Invoke(() => StatusText.Text = "Removing game data...");
                var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APP_NAME);
                TryDeleteDir(appData);
            }

            // Schedule self-deletion via cmd
            if (installDir != null)
            {
                Dispatcher.Invoke(() => StatusText.Text = "Cleaning up...");
                var bat = Path.Combine(Path.GetTempPath(), "uninstall_powerlauncher.bat");
                var script = $@"@echo off
timeout /t 2 /nobreak >nul
rmdir /s /q ""{installDir}""
del ""%~f0""";
                File.WriteAllText(bat, script);
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{bat}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
        });

        ProgressPanel.Visibility = Visibility.Collapsed;
        DonePanel.Visibility = Visibility.Visible;
    }

    private void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); } catch { }
    }

    private void TryDeleteDir(string path)
    {
        try { if (Directory.Exists(path)) Directory.Delete(path, true); } catch { }
    }
}
