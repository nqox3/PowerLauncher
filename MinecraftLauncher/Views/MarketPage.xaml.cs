using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MinecraftLauncher.Models;
using MinecraftLauncher.Services;
using MinecraftLauncher.ViewModels;

namespace MinecraftLauncher.Views;

public partial class MarketPage : Page
{
    private ModMarketService? _marketService;

    public MarketPage()
    {
        InitializeComponent();
        Loaded += (_, _) => InitService();
    }

    private void InitService()
    {
        var vm = DataContext as MainViewModel;
        if (vm != null)
            _marketService = new ModMarketService(vm.Settings.GameDirectory);
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            Search_Click(sender, new RoutedEventArgs());
    }

    private async void Search_Click(object sender, RoutedEventArgs e)
    {
        if (_marketService == null) InitService();
        if (_marketService == null) return;

        var query = SearchBox.Text?.Trim();
        if (string.IsNullOrEmpty(query)) return;

        var loader = (ModLoaderTag)(LoaderFilter.SelectedIndex);
        var sortOptions = new[] { "relevance", "downloads", "updated", "newest" };
        var sort = sortOptions[SortBy.SelectedIndex];

        MarketStatus.Text = "Searching...";
        ModsList.ItemsSource = null;

        try
        {
            var results = await _marketService.SearchModrinthAsync(query, loader, "", sort);
            ModsList.ItemsSource = results;
            MarketStatus.Text = $"Found {results.Count} mods";
        }
        catch (Exception ex)
        {
            MarketStatus.Text = $"Error: {ex.Message}";
        }
    }

    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        if (_marketService == null) return;
        var button = sender as Wpf.Ui.Controls.Button;
        var mod = button?.Tag as MarketMod;
        if (mod == null) return;

        button!.IsEnabled = false;
        button.Content = "Installing...";
        DownloadProgress.Visibility = Visibility.Visible;
        DownloadProgress.Value = 0;
        MarketStatus.Text = $"Downloading {mod.Name}...";

        try
        {
            var url = await _marketService.GetModrinthDownloadUrlAsync(mod.Id);
            if (url == null)
            {
                MarketStatus.Text = "No download available for this mod";
                return;
            }

            var fileName = $"{mod.Name.Replace(" ", "_")}-{mod.Id}.jar";
            await _marketService.DownloadModAsync(url, fileName, p =>
                Dispatcher.Invoke(() => DownloadProgress.Value = p));

            MarketStatus.Text = $"✅ {mod.Name} installed!";
            button.Content = "Installed ✓";

            // Refresh mods list
            var vm = DataContext as MainViewModel;
            vm?.RefreshModsCommand.Execute(null);
        }
        catch (Exception ex)
        {
            MarketStatus.Text = $"Failed: {ex.Message}";
            button.Content = "Retry";
            button.IsEnabled = true;
        }
        finally
        {
            DownloadProgress.Visibility = Visibility.Collapsed;
        }
    }
}
