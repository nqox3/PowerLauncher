using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace PowerLauncherInstaller;

public partial class InstallerWindow : Wpf.Ui.Controls.FluentWindow
{
    private int _currentStep = 1;
    private string _installPath;
    private const string DOWNLOAD_URL = "https://github.com/nqox3/PowerLauncher/releases/latest/download/PowerLauncher.zip";
    private const string APP_NAME = "PowerLauncher";
    private const string EXE_NAME = "MinecraftLauncher.exe";

    private readonly Label[] _stepLabels;
    private readonly UIElement[] _pages;
    private static readonly string[] StepNames = ["Welcome", "License", "Options", "Install", "Done"];

    public InstallerWindow()
    {
        _installPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Programs", APP_NAME);
        InitializeComponent();
        InstallPathBox.Text = _installPath;

        // Build step labels in code to avoid WPF UI TextBlock conflict
        _stepLabels = new Label[5];
        for (int i = 0; i < StepNames.Length; i++)
        {
            if (i > 0)
                StepsPanel.Children.Add(new Label { Content = ">", Padding = new Thickness(0), Margin = new Thickness(0, 0, 16, 0), Opacity = 0.3 });
            var lbl = new Label { Content = StepNames[i], Padding = new Thickness(0), Margin = new Thickness(0, 0, 16, 0), Opacity = 0.4 };
            _stepLabels[i] = lbl;
            StepsPanel.Children.Add(lbl);
        }
        _stepLabels[0].FontWeight = FontWeights.Bold;
        _stepLabels[0].Opacity = 1.0;

        _pages = [Page1, Page2, Page3, Page4, Page5];
        UpdateUI();
    }

    private void ShowStep(int step)
    {
        _currentStep = step;
        for (int i = 0; i < _pages.Length; i++)
            _pages[i].Visibility = i == step - 1 ? Visibility.Visible : Visibility.Collapsed;

        for (int i = 0; i < _stepLabels.Length; i++)
        {
            _stepLabels[i].FontWeight = i == step - 1 ? FontWeights.Bold : FontWeights.Normal;
            _stepLabels[i].Opacity = i == step - 1 ? 1.0 : (i < step - 1 ? 0.7 : 0.4);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        BackBtn.Visibility = _currentStep > 1 && _currentStep < 4 ? Visibility.Visible : Visibility.Collapsed;
        CancelBtn.Visibility = _currentStep < 4 ? Visibility.Visible : Visibility.Collapsed;

        if (_currentStep == 5)
        {
            NextBtn.Content = "Finish";
            BackBtn.Visibility = Visibility.Collapsed;
            CancelBtn.Visibility = Visibility.Collapsed;
            NextBtn.Visibility = Visibility.Visible;
        }
        else if (_currentStep == 3)
        {
            NextBtn.Content = "Install";
            NextBtn.Visibility = Visibility.Visible;
        }
        else if (_currentStep == 4)
        {
            NextBtn.Visibility = Visibility.Collapsed;
            BackBtn.Visibility = Visibility.Collapsed;
        }
        else
        {
            NextBtn.Content = "Next";
            NextBtn.Visibility = Visibility.Visible;
        }
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        if (_currentStep == 2 && AcceptLicense.IsChecked != true)
        {
            System.Windows.MessageBox.Show("Please accept the license agreement.",
                "License", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }
        if (_currentStep == 3) { ShowStep(4); StartInstall(); return; }
        if (_currentStep == 5)
        {
            if (LaunchAfter.IsChecked == true)
            {
                var exe = Path.Combine(_installPath, EXE_NAME);
                if (File.Exists(exe))
                    Process.Start(new ProcessStartInfo { FileName = exe, UseShellExecute = true });
            }
            Close(); return;
        }
        if (_currentStep < 5) ShowStep(_currentStep + 1);
    }

    private void Back_Click(object sender, RoutedEventArgs e) { if (_currentStep > 1) ShowStep(_currentStep - 1); }
    private void Cancel_Click(object sender, RoutedEventArgs e) { Close(); }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog { Title = "Select installation folder" };
        if (dialog.ShowDialog() == true)
        {
            _installPath = Path.Combine(dialog.FolderName, APP_NAME);
            InstallPathBox.Text = _installPath;
        }
    }

    private async void StartInstall()
    {
        try
        {
            Directory.CreateDirectory(_installPath);
            ProgressStatus.Content = "Downloading PowerLauncher...";

            var zipPath = Path.Combine(Path.GetTempPath(), "PowerLauncher_install.zip");
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "PowerLauncher-Installer/1.0");

            using var response = await client.GetAsync(DOWNLOAD_URL, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            var downloaded = 0L;

            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var fs = new FileStream(zipPath, FileMode.Create))
            {
                var buffer = new byte[81920];
                int read;
                while ((read = await stream.ReadAsync(buffer)) > 0)
                {
                    await fs.WriteAsync(buffer.AsMemory(0, read));
                    downloaded += read;
                    if (totalBytes > 0)
                    {
                        var pct = (double)downloaded / totalBytes * 70;
                        ProgressBar.Value = pct;
                        ProgressPercent.Content = $"{pct:F0}%";
                    }
                }
            }

            ProgressStatus.Content = "Extracting...";
            ProgressBar.Value = 75;
            ZipFile.ExtractToDirectory(zipPath, _installPath, true);
            File.Delete(zipPath);

            ProgressStatus.Content = "Creating shortcuts...";
            ProgressBar.Value = 85;
            var exePath = Path.Combine(_installPath, EXE_NAME);

            if (DesktopShortcut.IsChecked == true)
                CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), APP_NAME + ".lnk"), exePath);
            if (StartMenuShortcut.IsChecked == true)
                CreateShortcut(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", APP_NAME + ".lnk"), exePath);
            if (LaunchOnStartup.IsChecked == true)
            {
                var k = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                k?.SetValue(APP_NAME, $"\"{exePath}\""); k?.Close();
            }

            ProgressStatus.Content = "Registering...";
            ProgressBar.Value = 95;
            var uninstallExe = Path.Combine(_installPath, "Uninstall.exe");
            var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\" + APP_NAME);
            key?.SetValue("DisplayName", APP_NAME);
            key?.SetValue("DisplayVersion", "1.0.0");
            key?.SetValue("InstallLocation", _installPath);
            key?.SetValue("UninstallString", $"\"{uninstallExe}\"");
            key?.SetValue("DisplayIcon", exePath);
            key?.SetValue("NoModify", 1); key?.SetValue("NoRepair", 1);
            key?.Close();

            ProgressBar.Value = 100;
            ProgressPercent.Content = "100%";
            await Task.Delay(500);
            ShowStep(5);
        }
        catch (Exception ex)
        {
            ProgressTitle.Content = "Failed";
            ProgressStatus.Content = ex.Message;
            ProgressBar.Value = 0;
        }
    }

    private void CreateShortcut(string linkPath, string targetPath)
    {
        try
        {
            var ps = $"$ws = New-Object -ComObject WScript.Shell; $s = $ws.CreateShortcut('{linkPath.Replace("'", "''")}'); $s.TargetPath = '{targetPath.Replace("'", "''")}'; $s.WorkingDirectory = '{Path.GetDirectoryName(targetPath)?.Replace("'", "''")}'; $s.Save()";
            Process.Start(new ProcessStartInfo { FileName = "powershell", Arguments = $"-NoProfile -Command \"{ps}\"", CreateNoWindow = true, UseShellExecute = false })?.WaitForExit(5000);
        }
        catch { }
    }
}
