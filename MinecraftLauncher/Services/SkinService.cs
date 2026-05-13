using System.IO;
using System.Net.Http;
using System.Windows.Media.Imaging;

namespace MinecraftLauncher.Services;

public static class SkinService
{
    // Crafatar CDN for player heads/skins
    private const string CRAFATAR_HEAD = "https://crafatar.com/avatars/{0}?size=64&overlay";
    private const string CRAFATAR_BODY = "https://crafatar.com/renders/body/{0}?overlay";
    private const string MC_HEADS = "https://mc-heads.net/avatar/{0}/64";

    public static async Task<BitmapImage?> GetPlayerHeadAsync(string username)
    {
        try
        {
            var url = string.Format(MC_HEADS, username);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "PowerLauncher/1.0");
            var data = await client.GetByteArrayAsync(url);

            var image = new BitmapImage();
            using var ms = new MemoryStream(data);
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch
        {
            return null;
        }
    }
}
