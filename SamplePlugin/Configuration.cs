using Dalamud.Configuration;
using System;

namespace SamplePlugin;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool EnableGoldSaucerNotifications { get; set; } = true;
    public bool EnableCosmicExplorationNotifications { get; set; } = true;
    public bool EnableFirmamentFeteNotifications { get; set; } = true;
    public bool ShowMainWindowOnStartup { get; set; } = false;

    // The below exist just to make saving less cumbersome
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
