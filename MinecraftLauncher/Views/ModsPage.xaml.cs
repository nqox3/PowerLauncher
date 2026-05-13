using Microsoft.Win32;
using MinecraftLauncher.ViewModels;
using System.Windows.Controls;

namespace MinecraftLauncher.Views;

public partial class ModsPage : Page
{
    public ModsPage()
    {
        InitializeComponent();
    }

    private async void AddMod_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Minecraft Mods (*.jar)|*.jar|All Files (*.*)|*.*",
            Title = "Select a mod file",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            var vm = DataContext as MainViewModel;
            if (vm == null) return;

            foreach (var file in dialog.FileNames)
            {
                var modService = new Services.ModService(vm.Settings.GameDirectory);
                await modService.AddModFromFile(file);
            }
            vm.RefreshModsCommand.Execute(null);
        }
    }
}
