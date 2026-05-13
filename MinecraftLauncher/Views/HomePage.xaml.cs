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

    private async void HomePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        var vm = DataContext as MainViewModel;
        if (vm == null) return;

        // Load player head from CDN
        if (!string.IsNullOrEmpty(vm.Username))
        {
            var head = await SkinService.GetPlayerHeadAsync(vm.Username);
            if (head != null)
                PlayerHeadImage.Source = head;
        }
    }
}
