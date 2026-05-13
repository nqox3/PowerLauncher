using System.Windows;
using MinecraftLauncher.ViewModels;
using Wpf.Ui.Controls;

namespace MinecraftLauncher;

public partial class MainWindow : FluentWindow
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        ViewModel = new MainViewModel();
        DataContext = ViewModel;
        InitializeComponent();

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        NavigationView.Navigated += NavigationView_Navigated;
        NavigationView.Navigate(typeof(Views.HomePage));

        // Game lifecycle events
        ViewModel.OnGameStarted += () => Dispatcher.Invoke(() => Hide());

        ViewModel.OnGameExited += () => Dispatcher.Invoke(() =>
        {
            Show();
            Activate();
            ViewModel.StatusText = "Game closed normally";
        });

        ViewModel.OnGameCrashed += (crashInfo) => Dispatcher.Invoke(() =>
        {
            Show();
            Activate();
            var msg = "Something broke your game!\n\n" +
                      (crashInfo.Length > 500 ? crashInfo[..500] + "..." : crashInfo);
            System.Windows.MessageBox.Show(msg, "💥 Game Crashed",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            ViewModel.StatusText = "Game crashed! Check console for details.";
        });
    }

    private void NavigationView_Navigated(NavigationView sender, NavigatedEventArgs args)
    {
        if (args.Page is FrameworkElement page)
        {
            page.DataContext = ViewModel;
        }
    }
}
