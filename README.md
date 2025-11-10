# Saucer Shoutouts

FFXIV Dalamud plugin for shoutouts/timers for:
- Gold Saucer minigames (GATEs)
- Firmament Fêtes
- Cosmic Exploration red alerts

## Prerequisites
- Windows
- XIVLauncher (Dalamud)
- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0

## Build (first time)
```powershell
dotnet --info
dotnet new install Dalamud.Templates # optional reference
```

## Build and load as a dev plugin
```powershell
# Build
dotnet build -c Debug

# Create dev plugin folder
$dev = "$env:AppData\XIVLauncher\devPlugins\SaucerShoutouts"
mkdir $dev -Force

# Copy output (DLL + manifest)
Copy-Item "SaucerShoutouts\bin\Debug\net8.0-windows\SaucerShoutouts.dll" $dev -Force
Copy-Item "SaucerShoutouts\DalamudPluginManifest.json" $dev -Force
```

Then:
1) Launch FFXIV via XIVLauncher.
2) In Dalamud settings, enable "Load dev plugins."
3) Use `/xlplugins` to verify it loaded.
4) Use `/saucer` to open the test window.

## Dev loop
- Edit code → `dotnet build` → copy updated DLL/manifest to devPlugins → `/xlreload` in-game.

## Links
- Dalamud docs: https://goatcorp.github.io/Dalamud/
- Template repo (reference): https://github.com/goatcorp/DalamudPluginTemplate
- Sample plugin: https://github.com/goatcorp/SamplePlugin
