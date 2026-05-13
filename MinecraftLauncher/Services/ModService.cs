using System.IO;
using MinecraftLauncher.Models;

namespace MinecraftLauncher.Services;

public class ModService
{
    private readonly string _modsDirectory;

    public ModService(string gameDirectory)
    {
        _modsDirectory = Path.Combine(gameDirectory, "mods");
        Directory.CreateDirectory(_modsDirectory);
    }

    public List<ModInfo> GetInstalledMods()
    {
        var mods = new List<ModInfo>();
        if (!Directory.Exists(_modsDirectory)) return mods;

        var files = Directory.GetFiles(_modsDirectory, "*.jar")
            .Concat(Directory.GetFiles(_modsDirectory, "*.jar.disabled"));

        foreach (var file in files)
        {
            var info = new FileInfo(file);
            mods.Add(new ModInfo
            {
                Name = Path.GetFileNameWithoutExtension(info.Name).Replace(".disabled", ""),
                FileName = info.Name,
                FilePath = info.FullName,
                FileSize = info.Length,
                IsEnabled = !info.Name.EndsWith(".disabled")
            });
        }

        return mods;
    }

    public void ToggleMod(ModInfo mod)
    {
        if (mod.IsEnabled)
        {
            var newPath = mod.FilePath + ".disabled";
            File.Move(mod.FilePath, newPath);
            mod.FilePath = newPath;
            mod.IsEnabled = false;
        }
        else
        {
            var newPath = mod.FilePath.Replace(".disabled", "");
            File.Move(mod.FilePath, newPath);
            mod.FilePath = newPath;
            mod.IsEnabled = true;
        }
    }

    public void DeleteMod(ModInfo mod)
    {
        if (File.Exists(mod.FilePath))
            File.Delete(mod.FilePath);
    }

    public async Task AddModFromFile(string sourceFilePath)
    {
        var fileName = Path.GetFileName(sourceFilePath);
        var destPath = Path.Combine(_modsDirectory, fileName);
        await Task.Run(() => File.Copy(sourceFilePath, destPath, true));
    }

    public void OpenModsFolder()
    {
        Directory.CreateDirectory(_modsDirectory);
        System.Diagnostics.Process.Start("explorer.exe", _modsDirectory);
    }
}
