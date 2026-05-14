using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace MinecraftLauncher.Views;

public partial class ServersPage : Page
{
    private readonly string _serversPath;
    private ObservableCollection<ServerEntry> _servers = new();

    public ServersPage()
    {
        InitializeComponent();
        _serversPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PowerLauncher", "servers.json");
        LoadServers();
        ServerList.ItemsSource = _servers;
    }

    private void LoadServers()
    {
        if (File.Exists(_serversPath))
        {
            var json = File.ReadAllText(_serversPath);
            var list = JsonConvert.DeserializeObject<List<ServerEntry>>(json);
            if (list != null)
                _servers = new ObservableCollection<ServerEntry>(list);
        }
    }

    private void SaveServers()
    {
        var json = JsonConvert.SerializeObject(_servers.ToList(), Formatting.Indented);
        File.WriteAllText(_serversPath, json);
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        var address = ServerInput.Text?.Trim();
        if (string.IsNullOrEmpty(address)) return;

        var name = string.IsNullOrWhiteSpace(ServerName.Text) ? address : ServerName.Text.Trim();
        _servers.Add(new ServerEntry { Name = name, Address = address });
        ServerList.ItemsSource = _servers;
        SaveServers();
        ServerInput.Text = "";
        ServerName.Text = "";
    }

    private void Copy_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Wpf.Ui.Controls.Button;
        var server = btn?.Tag as ServerEntry;
        if (server != null)
            Clipboard.SetText(server.Address);
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        var btn = sender as Wpf.Ui.Controls.Button;
        var server = btn?.Tag as ServerEntry;
        if (server != null)
        {
            _servers.Remove(server);
            SaveServers();
        }
    }
}

public class ServerEntry
{
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
}
