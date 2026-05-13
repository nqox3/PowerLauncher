using System.Windows;
using System.Windows.Controls;

namespace MinecraftLauncher.Views;

public partial class ConsolePage : Page
{
    public ConsolePage()
    {
        InitializeComponent();
    }

    public void AppendLog(string message)
    {
        Dispatcher.Invoke(() =>
        {
            LogOutput.Text += $"\n[{DateTime.Now:HH:mm:ss}] {message}";
            LogScroller.ScrollToEnd();
        });
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        LogOutput.Text = "[PowerLauncher] Console cleared.";
    }

    private void CopyAll_Click(object sender, RoutedEventArgs e)
    {
        Clipboard.SetText(LogOutput.Text);
    }
}
