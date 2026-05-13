using System.Windows.Controls;
using MinecraftLauncher.Services;

namespace MinecraftLauncher.Views;

public partial class SkinsPage : Page
{
    public SkinsPage()
    {
        InitializeComponent();
    }

    private async void LoadSkin_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var username = SkinUsername.Text?.Trim();
        if (string.IsNullOrEmpty(username)) return;

        SkinStatusText.Text = "Loading...";
        var image = await SkinService.GetPlayerHeadAsync(username);
        if (image != null)
        {
            PlayerHead.Source = image;
            SkinStatusText.Text = username;
        }
        else
        {
            SkinStatusText.Text = "Could not load skin";
        }
    }
}
