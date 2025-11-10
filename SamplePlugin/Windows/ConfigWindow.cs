using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace SamplePlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    // We give this window a constant ID using ###.
    // This allows for labels to be dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("Saucer Shoutouts Configuration###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(350, 200);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        // Flags must be added or removed before Draw() is being called, or they won't apply
        if (configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        ImGui.TextUnformatted("Event Tracking Settings");
        ImGui.Separator();
        ImGui.Spacing();

        var enableGoldSaucer = configuration.EnableGoldSaucerNotifications;
        if (ImGui.Checkbox("Enable Gold Saucer GATE tracking", ref enableGoldSaucer))
        {
            configuration.EnableGoldSaucerNotifications = enableGoldSaucer;
            configuration.Save();
        }

        var enableCosmicExploration = configuration.EnableCosmicExplorationNotifications;
        if (ImGui.Checkbox("Enable Cosmic Exploration alerts", ref enableCosmicExploration))
        {
            configuration.EnableCosmicExplorationNotifications = enableCosmicExploration;
            configuration.Save();
        }

        var enableFirmamentFete = configuration.EnableFirmamentFeteNotifications;
        if (ImGui.Checkbox("Enable Firmament Fête tracking", ref enableFirmamentFete))
        {
            configuration.EnableFirmamentFeteNotifications = enableFirmamentFete;
            configuration.Save();
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();

        var showOnStartup = configuration.ShowMainWindowOnStartup;
        if (ImGui.Checkbox("Show window on startup", ref showOnStartup))
        {
            configuration.ShowMainWindowOnStartup = showOnStartup;
            configuration.Save();
        }

        var movable = configuration.IsConfigWindowMovable;
        if (ImGui.Checkbox("Movable config window", ref movable))
        {
            configuration.IsConfigWindowMovable = movable;
            configuration.Save();
        }
    }
}
