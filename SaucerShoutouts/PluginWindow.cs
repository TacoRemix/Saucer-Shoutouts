using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace SaucerShoutouts;

public sealed class PluginWindow : Window
{
    public PluginWindow() : base("Saucer Shoutouts")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(320, 120),
            MaximumSize = new Vector2(1000, 800),
        };
        RespectCloseHotkey = true;
        IsOpen = true; // show on first load
    }

    public override void Draw()
    {
        ImGui.TextUnformatted("Hello from Saucer Shoutouts!");
        ImGui.Separator();
        ImGui.TextWrapped("Use this as a smoke test. Once confirmed working, we'll add timers for Gold Saucer, Fêtes, and Cosmic Exploration red alerts.");
        ImGui.Spacing();
        ImGui.TextDisabled("Open/close with /saucer or via the plugin settings.");
    }
}
