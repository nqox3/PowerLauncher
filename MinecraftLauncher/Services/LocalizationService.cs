using System.Globalization;

namespace MinecraftLauncher.Services;

public static class LocalizationService
{
    private static readonly Dictionary<string, Dictionary<string, string>> _strings = new()
    {
        ["en"] = new()
        {
            ["app_title"] = "PowerLauncher",
            ["home"] = "Home",
            ["mod_market"] = "Mod Market",
            ["my_mods"] = "My Mods",
            ["skins"] = "Skins",
            ["java"] = "Java",
            ["console"] = "Console",
            ["settings"] = "Settings",
            ["about"] = "About",
            ["launch"] = "LAUNCH MINECRAFT",
            ["game_version"] = "Game Version",
            ["choose_version"] = "Choose which version to play",
            ["refresh_versions"] = "Refresh Versions",
            ["account"] = "Account",
            ["manage_accounts"] = "Manage player accounts",
            ["add"] = "Add",
            ["remove"] = "Remove",
            ["username"] = "Username",
            ["quick_info"] = "Quick Info",
            ["version"] = "Version",
            ["player"] = "Player",
            ["ram"] = "RAM",
            ["mods"] = "Mods",
            ["ready"] = "Ready",
            ["loading_versions"] = "Loading versions...",
            ["select_version_first"] = "Please select a version first!",
            ["game_launched"] = "Game launched!",
            ["launch_failed"] = "Launch failed",
            ["settings_saved"] = "Settings saved!",
            ["search_mods"] = "Search mods...",
            ["all_loaders"] = "All Loaders",
            ["relevance"] = "Relevance",
            ["downloads"] = "Downloads",
            ["updated"] = "Updated",
            ["newest"] = "Newest",
            ["search"] = "Search",
            ["install"] = "Install",
            ["installing"] = "Installing...",
            ["installed"] = "Installed",
            ["memory"] = "Memory (RAM)",
            ["min_ram"] = "Minimum RAM (MB)",
            ["max_ram"] = "Maximum RAM (MB)",
            ["java_path"] = "Java Path",
            ["auto_detect"] = "Auto-detect",
            ["browse"] = "Browse",
            ["custom_jvm"] = "Custom JVM Arguments",
            ["window"] = "Window",
            ["fullscreen"] = "Fullscreen",
            ["width"] = "Width",
            ["height"] = "Height",
            ["game_directory"] = "Game Directory",
            ["options"] = "Options",
            ["show_snapshots"] = "Show Snapshots",
            ["auto_close"] = "Auto-close launcher on game start",
            ["save_settings"] = "Save Settings",
            ["open_mods_folder"] = "Open Mods Folder",
            ["refresh"] = "Refresh",
            ["add_mod"] = "Add Mod (.jar)",
            ["toggle"] = "Toggle",
            ["delete"] = "Delete",
            ["game_crashed_title"] = "Game Crashed",
            ["game_crashed_msg"] = "Something broke your game!",
            ["game_closed"] = "Game closed normally",
            ["checking_java"] = "Checking Java installation...",
            ["java_not_found"] = "Java not found",
            ["java_found"] = "Java found",
            ["install_java"] = "Install/Reinstall Java 21",
            ["java_installed"] = "Java installed successfully!",
            ["setup_title"] = "Setting up PowerLauncher...",
            ["setup_checking"] = "Checking environment...",
            ["setup_ready"] = "Ready!",
            ["setup_launching"] = "Launching PowerLauncher...",
            ["mod_market_desc"] = "Browse and install mods from Modrinth",
            ["player_skins"] = "Player Skins",
            ["load_skin"] = "Load Skin",
            ["clear"] = "Clear",
            ["copy_all"] = "Copy All",
        },
        ["ru"] = new()
        {
            ["app_title"] = "PowerLauncher",
            ["home"] = "\u0413\u043b\u0430\u0432\u043d\u0430\u044f",
            ["mod_market"] = "\u041c\u0430\u0433\u0430\u0437\u0438\u043d \u043c\u043e\u0434\u043e\u0432",
            ["my_mods"] = "\u041c\u043e\u0438 \u043c\u043e\u0434\u044b",
            ["skins"] = "\u0421\u043a\u0438\u043d\u044b",
            ["java"] = "Java",
            ["console"] = "\u041a\u043e\u043d\u0441\u043e\u043b\u044c",
            ["settings"] = "\u041d\u0430\u0441\u0442\u0440\u043e\u0439\u043a\u0438",
            ["about"] = "\u041e \u043f\u0440\u043e\u0433\u0440\u0430\u043c\u043c\u0435",
            ["launch"] = "\u0417\u0410\u041f\u0423\u0421\u0422\u0418\u0422\u042c MINECRAFT",
            ["game_version"] = "\u0412\u0435\u0440\u0441\u0438\u044f \u0438\u0433\u0440\u044b",
            ["choose_version"] = "\u0412\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0432\u0435\u0440\u0441\u0438\u044e",
            ["refresh_versions"] = "\u041e\u0431\u043d\u043e\u0432\u0438\u0442\u044c",
            ["account"] = "\u0410\u043a\u043a\u0430\u0443\u043d\u0442",
            ["manage_accounts"] = "\u0423\u043f\u0440\u0430\u0432\u043b\u0435\u043d\u0438\u0435 \u0430\u043a\u043a\u0430\u0443\u043d\u0442\u0430\u043c\u0438",
            ["add"] = "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c",
            ["remove"] = "\u0423\u0434\u0430\u043b\u0438\u0442\u044c",
            ["username"] = "\u0418\u043c\u044f \u0438\u0433\u0440\u043e\u043a\u0430",
            ["quick_info"] = "\u0418\u043d\u0444\u043e\u0440\u043c\u0430\u0446\u0438\u044f",
            ["version"] = "\u0412\u0435\u0440\u0441\u0438\u044f",
            ["player"] = "\u0418\u0433\u0440\u043e\u043a",
            ["ram"] = "\u041f\u0430\u043c\u044f\u0442\u044c",
            ["mods"] = "\u041c\u043e\u0434\u044b",
            ["ready"] = "\u0413\u043e\u0442\u043e\u0432",
            ["loading_versions"] = "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0430 \u0432\u0435\u0440\u0441\u0438\u0439...",
            ["select_version_first"] = "\u0421\u043d\u0430\u0447\u0430\u043b\u0430 \u0432\u044b\u0431\u0435\u0440\u0438\u0442\u0435 \u0432\u0435\u0440\u0441\u0438\u044e!",
            ["game_launched"] = "\u0418\u0433\u0440\u0430 \u0437\u0430\u043f\u0443\u0449\u0435\u043d\u0430!",
            ["launch_failed"] = "\u041e\u0448\u0438\u0431\u043a\u0430 \u0437\u0430\u043f\u0443\u0441\u043a\u0430",
            ["settings_saved"] = "\u041d\u0430\u0441\u0442\u0440\u043e\u0439\u043a\u0438 \u0441\u043e\u0445\u0440\u0430\u043d\u0435\u043d\u044b!",
            ["search_mods"] = "\u041f\u043e\u0438\u0441\u043a \u043c\u043e\u0434\u043e\u0432...",
            ["all_loaders"] = "\u0412\u0441\u0435 \u0437\u0430\u0433\u0440\u0443\u0437\u0447\u0438\u043a\u0438",
            ["relevance"] = "\u0420\u0435\u043b\u0435\u0432\u0430\u043d\u0442\u043d\u043e\u0441\u0442\u044c",
            ["downloads"] = "\u0417\u0430\u0433\u0440\u0443\u0437\u043a\u0438",
            ["updated"] = "\u041e\u0431\u043d\u043e\u0432\u043b\u0435\u043d\u043e",
            ["newest"] = "\u041d\u043e\u0432\u044b\u0435",
            ["search"] = "\u041f\u043e\u0438\u0441\u043a",
            ["install"] = "\u0423\u0441\u0442\u0430\u043d\u043e\u0432\u0438\u0442\u044c",
            ["installing"] = "\u0423\u0441\u0442\u0430\u043d\u043e\u0432\u043a\u0430...",
            ["installed"] = "\u0423\u0441\u0442\u0430\u043d\u043e\u0432\u043b\u0435\u043d\u043e",
            ["memory"] = "\u041f\u0430\u043c\u044f\u0442\u044c (RAM)",
            ["min_ram"] = "\u041c\u0438\u043d\u0438\u043c\u0443\u043c RAM (\u041c\u0411)",
            ["max_ram"] = "\u041c\u0430\u043a\u0441\u0438\u043c\u0443\u043c RAM (\u041c\u0411)",
            ["java_path"] = "\u041f\u0443\u0442\u044c \u043a Java",
            ["auto_detect"] = "\u0410\u0432\u0442\u043e\u043e\u043f\u0440\u0435\u0434\u0435\u043b\u0435\u043d\u0438\u0435",
            ["browse"] = "\u041e\u0431\u0437\u043e\u0440",
            ["custom_jvm"] = "\u0414\u043e\u043f. \u0430\u0440\u0433\u0443\u043c\u0435\u043d\u0442\u044b JVM",
            ["window"] = "\u041e\u043a\u043d\u043e",
            ["fullscreen"] = "\u041f\u043e\u043b\u043d\u044b\u0439 \u044d\u043a\u0440\u0430\u043d",
            ["width"] = "\u0428\u0438\u0440\u0438\u043d\u0430",
            ["height"] = "\u0412\u044b\u0441\u043e\u0442\u0430",
            ["game_directory"] = "\u041f\u0430\u043f\u043a\u0430 \u0438\u0433\u0440\u044b",
            ["options"] = "\u041e\u043f\u0446\u0438\u0438",
            ["show_snapshots"] = "\u041f\u043e\u043a\u0430\u0437\u044b\u0432\u0430\u0442\u044c \u0441\u043d\u0430\u043f\u0448\u043e\u0442\u044b",
            ["auto_close"] = "\u0417\u0430\u043a\u0440\u044b\u0432\u0430\u0442\u044c \u043b\u0430\u0443\u043d\u0447\u0435\u0440 \u043f\u0440\u0438 \u0437\u0430\u043f\u0443\u0441\u043a\u0435",
            ["save_settings"] = "\u0421\u043e\u0445\u0440\u0430\u043d\u0438\u0442\u044c",
            ["open_mods_folder"] = "\u041e\u0442\u043a\u0440\u044b\u0442\u044c \u043f\u0430\u043f\u043a\u0443 \u043c\u043e\u0434\u043e\u0432",
            ["refresh"] = "\u041e\u0431\u043d\u043e\u0432\u0438\u0442\u044c",
            ["add_mod"] = "\u0414\u043e\u0431\u0430\u0432\u0438\u0442\u044c \u043c\u043e\u0434 (.jar)",
            ["toggle"] = "\u041f\u0435\u0440\u0435\u043a\u043b\u044e\u0447\u0438\u0442\u044c",
            ["delete"] = "\u0423\u0434\u0430\u043b\u0438\u0442\u044c",
            ["game_crashed_title"] = "\u0418\u0433\u0440\u0430 \u0443\u043f\u0430\u043b\u0430",
            ["game_crashed_msg"] = "\u0427\u0442\u043e-\u0442\u043e \u0441\u043b\u043e\u043c\u0430\u043b\u043e \u0432\u0430\u0448\u0443 \u0438\u0433\u0440\u0443!",
            ["game_closed"] = "\u0418\u0433\u0440\u0430 \u0437\u0430\u043a\u0440\u044b\u0442\u0430",
            ["checking_java"] = "\u041f\u0440\u043e\u0432\u0435\u0440\u043a\u0430 Java...",
            ["java_not_found"] = "Java \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d\u0430",
            ["java_found"] = "Java \u043d\u0430\u0439\u0434\u0435\u043d\u0430",
            ["install_java"] = "\u0423\u0441\u0442\u0430\u043d\u043e\u0432\u0438\u0442\u044c Java 21",
            ["java_installed"] = "Java \u0443\u0441\u0442\u0430\u043d\u043e\u0432\u043b\u0435\u043d\u0430!",
            ["setup_title"] = "\u041d\u0430\u0441\u0442\u0440\u043e\u0439\u043a\u0430 PowerLauncher...",
            ["setup_checking"] = "\u041f\u0440\u043e\u0432\u0435\u0440\u043a\u0430 \u043e\u043a\u0440\u0443\u0436\u0435\u043d\u0438\u044f...",
            ["setup_ready"] = "\u0413\u043e\u0442\u043e\u0432\u043e!",
            ["setup_launching"] = "\u0417\u0430\u043f\u0443\u0441\u043a PowerLauncher...",
            ["mod_market_desc"] = "\u041f\u043e\u0438\u0441\u043a \u0438 \u0443\u0441\u0442\u0430\u043d\u043e\u0432\u043a\u0430 \u043c\u043e\u0434\u043e\u0432 \u0438\u0437 Modrinth",
            ["player_skins"] = "\u0421\u043a\u0438\u043d\u044b \u0438\u0433\u0440\u043e\u043a\u043e\u0432",
            ["load_skin"] = "\u0417\u0430\u0433\u0440\u0443\u0437\u0438\u0442\u044c",
            ["clear"] = "\u041e\u0447\u0438\u0441\u0442\u0438\u0442\u044c",
            ["copy_all"] = "\u041a\u043e\u043f\u0438\u0440\u043e\u0432\u0430\u0442\u044c \u0432\u0441\u0435",
        }
    };

    private static string _currentLang = "en";

    public static string CurrentLanguage => _currentLang;

    public static void Initialize()
    {
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        _currentLang = _strings.ContainsKey(culture) ? culture : "en";
    }

    public static string Get(string key)
    {
        if (_strings.TryGetValue(_currentLang, out var lang) && lang.TryGetValue(key, out var val))
            return val;
        if (_strings["en"].TryGetValue(key, out var fallback))
            return fallback;
        return key;
    }

    public static void SetLanguage(string langCode)
    {
        if (_strings.ContainsKey(langCode))
            _currentLang = langCode;
    }
}
