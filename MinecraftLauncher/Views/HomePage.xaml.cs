using System.Windows;
using System.Windows.Controls;
using MinecraftLauncher.Services;
using MinecraftLauncher.ViewModels;

namespace MinecraftLauncher.Views;

public partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();
        Loaded += HomePage_Loaded;
    }

    private async void HomePage_Loaded(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as MainViewModel;
        if (vm == null) return;

        try
        {
            if (!string.IsNullOrEmpty(vm.Username))
            {
                var head = await SkinService.GetPlayerHeadAsync(vm.Username);
                if (head != null)
                    PlayerHeadImage.Source = head;
            }
        }
        catch { }
    }

    private async void InstallFabric_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as MainViewModel;
        if (vm == null) return;

        var version = vm.SelectedVersion;
        if (string.IsNullOrEmpty(version))
        {
            vm.StatusText = "Select a vanilla version first, then install Fabric for it.";
            return;
        }

        FabricBtn.IsEnabled = false;
        FabricBtn.Content = "Installing...";
        try
        {
            var loader = new LoaderInstallService(vm.Settings.GameDirectory);
            loader.StatusChanged += s => Dispatcher.Invoke(() => vm.StatusText = s);
            await loader.InstallFabricAsync(version);
            vm.LoadVersionsCommand.Execute(null);
        }
        catch (Exception ex)
        {
            vm.StatusText = $"Fabric install failed: {ex.Message}";
        }
        finally
        {
            FabricBtn.IsEnabled = true;
            FabricBtn.Content = "Install Fabric";
        }
    }

    private async void InstallQuilt_Click(object sender, RoutedEventArgs e)
    {
        var vm = DataContext as MainViewModel;
        if (vm == null) return;

        var version = vm.SelectedVersion;
        if (string.IsNullOrEmpty(version))
        {
            vm.StatusText = "Select a vanilla version first, then install Quilt for it.";
            return;
        }

        QuiltBtn.IsEnabled = false;
        QuiltBtn.Content = "Installing...";
        try
        {
            var loader = new LoaderInstallService(vm.Settings.GameDirectory);
            loader.StatusChanged += s => Dispatcher.Invoke(() => vm.StatusText = s);
            await loader.InstallQuiltAsync(version);
            vm.LoadVersionsCommand.Execute(null);
        }
        catch (Exception ex)
        {
            vm.StatusText = $"Quilt install failed: {ex.Message}";
        }
        finally
        {
            QuiltBtn.IsEnabled = true;
            QuiltBtn.Content = "Install Quilt";
        }
    }
}
