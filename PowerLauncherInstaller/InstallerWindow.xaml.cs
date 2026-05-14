using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Wpf.Ui.Controls;

namespace PowerLauncherInstaller;

public partial class InstallerWindow : FluentWindow
{
    private string _installPath;
    private const string DOWNLOAD_URL = "https://github.com/nqox3/PowerLauncher/releases/latest/download/PowerLauncher.zip";
    private const string APP_NAME = "PowerLauncher";
    private const string EXE_NAME = "MinecraftLauncher.exe";

    public string InstallPath => _installPath;

    public InstallerWindow()
    {
        _installPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Programs", APP_NAME);
        DataContext = this;
        InitializeComponent();
        InstallPathBox.Text = _installPath;
    }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog { Title = "Select installation folder" };
        if (dialog.ShowDialog() == true)
        {
            _installPath = Path.Combine(dialog.FolderName, APP_NAME);
            InstallPathBox.Text = _installPath;
        }
    }

    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        WelcomePage.Visibility = Visibility.Collapsed;
        ProgressPage.Visibility = Visibility.Visible;

        try
        {
            Directory.CreateDirectory(_installPath);
            ProgressStatus.Text = "Downloading PowerLauncher...";
            await Task.Delay(200);

            var zipPath = Path.Combine(Path.GetTempPath(), "PowerLauncher_install.zip");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "PowerLauncher-Installer/1.0");

            using var response = await client.GetAsync(DOWNLOAD_URL, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            var downloaded = 0L;

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var fileStream = new FileStream(zipPath, FileMode.Create))
            {
                var buffer = new byte[81920];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                    downloaded += bytesRead;
                    if (totalBytes > 0)
                    {
                        var pct = (double)downloaded / totalBytes * 70;
                        ProgressBar.Value = pct;
                        ProgressPercent.Text = $"{pct:F0}%";
                    }
                }
            }

            ProgressStatus.Text = "Extracting files...";
            ProgressBar.Value = 75;
            ProgressPercent.Text = "75%";

            ZipFile.ExtractToDirectory(zipPath, _installPath, true);
            File.Delete(zipPath);

            ProgressBar.Value = 85;
            ProgressPercent.Text = "85%";

            ProgressStatus.Text = "Creating shortcuts...";
            var exePath = Path.Combine(_installPath, EXE_NAME);

            if (DesktopShortcut.IsChecked == true)
            {
                var desktopLink = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    APP_NAME + ".lnk");
                CreateShortcutViaPs(desktopLink, exePath);
            }

            if (StartMenuShortcut.IsChecked == true)
            {
                var startDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
                    "Programs");
                var startLink = Path.Combine(startDir, APP_NAME + ".lnk");
                CreateShortcutViaPs(startLink, exePath);
            }

            ProgressStatus.Text = "Registering application...";
            RegisterUninstall(exePath);

            ProgressBar.Value = 100;
            ProgressPercent.Text = "100%";
            await Task.Delay(500);

            ProgressPage.Visibility = Visibility.Collapsed;
            DonePage.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            ProgressTitle.Text = "Installation Failed";
            ProgressStatus.Text = ex.Message;
            ProgressBar.Value = 0;
            ProgressPercent.Text = "";
        }
    }

    private void Finish_Click(object sender, RoutedEventArgs e)
    {
        if (LaunchAfter.IsChecked == true)
        {
            var exePath = Path.Combine(_installPath, EXE_NAME);
            if (File.Exists(exePath))
                Process.Start(new ProcessStartInfo { FileName = exePath, UseShellExecute = true });
        }
        Close();
    }

    private void CreateShortcutViaPs(string linkPath, string targetPath)
    {
        try
        {
            var script = $@"$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut('{linkPath}'); $s.TargetPath = '{targetPath}'; $s.WorkingDirectory = '{Path.GetDirectoryName(targetPath)}'; $s.Save()";
            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-NoProfile -Command \"{script}\"",
                CreateNoWindow = true,
                UseShellExecute = false
            })?.WaitForExit(5000);
        }
        catch { }
    }

    private void RegisterUninstall(string exePath)
    {
        try
        {
            var key = Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Uninstall\" + APP_NAME);
            if (key == null) return;
            key.SetValue("DisplayName", APP_NAME);
            key.SetValue("DisplayVersion", "1.0.0");
            key.SetValue("Publisher", "PowerLauncher Team");
            key.SetValue("InstallLocation", _installPath);
            key.SetValue("UninstallString", $"\"{exePath}\" --uninstall");
            key.SetValue("DisplayIcon", exePath);
            key.SetValue("NoModify", 1);
            key.SetValue("NoRepair", 1);
            key.Close();
        }
        catch { }
    }
}
