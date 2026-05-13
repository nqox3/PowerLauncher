# PowerLauncher

A powerful Minecraft launcher for Windows built with WPF UI and .NET 8.

## Features

- Mod Market - browse and install mods from Modrinth (Fabric, Forge, Quilt, NeoForge)
- Auto Java Install - downloads Java 21 automatically if not found
- All Versions - launch any Minecraft release version
- Mod Manager - enable, disable, delete mods with one click
- Crash Detection - shows crash reports when the game crashes
- Offline Accounts - play with any username, manage multiple accounts
- Multi-Language - syncs with system language (English, Russian)
- Dark Modern UI - WPF UI with Fluent Design

## Requirements

- Windows 10/11 x64
- No additional dependencies (self-contained build)

## Build from Source

```
dotnet restore
dotnet build
dotnet run --project MinecraftLauncher
```

## Publish

```
dotnet publish MinecraftLauncher/MinecraftLauncher.csproj -c Release -r win-x64 --self-contained true
```

## Create Installer

Requires Inno Setup 6. After publishing:

```
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer/setup.iss
```

## File Structure

```
%AppData%/PowerLauncher/
+-- settings.json
+-- accounts.json
+-- game/
|   +-- versions/
|   +-- assets/
|   +-- libraries/
|   +-- mods/
+-- runtime/
    +-- java-21/
```

## Tech Stack

- .NET 8 + WPF UI 3.0.5
- CmlLib.Core 4.0.6
- CommunityToolkit.Mvvm
- Newtonsoft.Json

## License

MIT
