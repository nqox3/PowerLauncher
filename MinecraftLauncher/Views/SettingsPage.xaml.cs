using Microsoft.Win32;
using System.Windows.Controls;

namespace MinecraftLauncher.Views;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void BrowseJava_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Java Executable (javaw.exe;java.exe)|javaw.exe;java.exe|All Files (*.*)|*.*",
            Title = "Select Java Runtime"
        };

        if (dialog.ShowDialog() == true)
        {
            var vm = DataContext as ViewModels.MainViewModel;
            if (vm != null)
                vm.Settings.JavaPath = dialog.FileName;
        }
    }

    private void BrowseGameDir_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Minecraft Game Directory"
        };

        if (dialog.ShowDialog() == true)
        {
            var vm = DataContext as ViewModels.MainViewModel;
            if (vm != null)
                vm.Settings.GameDirectory = dialog.FolderName;
        }
    }
}
