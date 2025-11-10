using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;

namespace SamplePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly string goatImagePath;
    private readonly Plugin plugin;

    // We give this window a hidden ID using ##.
    // The user will see "Saucer Shoutouts" as window title,
    // but for ImGui the ID is "Saucer Shoutouts##With a hidden ID"
    public MainWindow(Plugin plugin, string goatImagePath)
        : base("Saucer Shoutouts##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.goatImagePath = goatImagePath;
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.TextUnformatted("Gold Saucer & Event Tracker");
        ImGui.Separator();

        // Gold Saucer GATE Timer
        if (plugin.Configuration.EnableGoldSaucerNotifications)
        {
            ImGui.TextColored(new Vector4(1.0f, 0.84f, 0.0f, 1.0f), "Gold Saucer GATE");
            var timeUntilNextGate = plugin.EventTracker.GetTimeUntilNextGoldSaucerGate();
            var nextGateTime = plugin.EventTracker.GetNextGoldSaucerGateTime();
            
            ImGui.TextUnformatted($"Next GATE in: {timeUntilNextGate.Minutes:D2}:{timeUntilNextGate.Seconds:D2}");
            ImGui.TextUnformatted($"Next GATE at: {nextGateTime.ToLocalTime():HH:mm:ss}");
            
            if (plugin.EventTracker.LastGoldSaucerGateTime.HasValue)
            {
                ImGui.TextUnformatted($"Last detected: {plugin.EventTracker.LastGoldSaucerGateTime.Value.ToLocalTime():HH:mm:ss}");
            }
            ImGui.Spacing();
        }

        // Cosmic Exploration Red Alerts
        if (plugin.Configuration.EnableCosmicExplorationNotifications)
        {
            ImGui.TextColored(new Vector4(1.0f, 0.2f, 0.2f, 1.0f), "Cosmic Exploration Red Alerts");
            var alertCount = plugin.EventTracker.GetCosmicExplorationRedAlertCount();
            ImGui.TextUnformatted($"Total alerts tracked: {alertCount}");
            
            var lastAlert = plugin.EventTracker.GetLastCosmicExplorationRedAlert();
            if (lastAlert.HasValue)
            {
                var timeSinceLastAlert = DateTime.UtcNow - lastAlert.Value;
                ImGui.TextUnformatted($"Last alert: {timeSinceLastAlert.TotalMinutes:F1} minutes ago");
                ImGui.TextUnformatted($"Time: {lastAlert.Value.ToLocalTime():HH:mm:ss}");
            }
            else
            {
                ImGui.TextUnformatted("No alerts detected yet");
            }
            ImGui.Spacing();
        }

        // Firmament Fete
        if (plugin.Configuration.EnableFirmamentFeteNotifications)
        {
            ImGui.TextColored(new Vector4(0.5f, 0.8f, 1.0f, 1.0f), "Firmament Fête");
            if (plugin.EventTracker.LastFirmamentFeteTime.HasValue)
            {
                var timeSinceFete = DateTime.UtcNow - plugin.EventTracker.LastFirmamentFeteTime.Value;
                ImGui.TextUnformatted($"Last Fête: {timeSinceFete.TotalMinutes:F1} minutes ago");
                ImGui.TextUnformatted($"Time: {plugin.EventTracker.LastFirmamentFeteTime.Value.ToLocalTime():HH:mm:ss}");
            }
            else
            {
                ImGui.TextUnformatted("No Fête detected yet");
                ImGui.TextUnformatted("Fête schedule is server-specific");
            }
            ImGui.Spacing();
        }

        ImGui.Separator();

        if (ImGui.Button("Show Settings"))
        {
            plugin.ToggleConfigUi();
        }

        ImGui.Spacing();

        // Normally a BeginChild() would have to be followed by an unconditional EndChild(),
        // ImRaii takes care of this after the scope ends.
        // This works for all ImGui functions that require specific handling, examples are BeginTable() or Indent().
        using (var child = ImRaii.Child("EventLog", Vector2.Zero, true))
        {
            // Check if this child is drawing
            if (child.Success)
            {
                ImGui.TextUnformatted("Event Detection Info:");
                ImGui.Spacing();
                
                ImGui.TextUnformatted("This plugin monitors chat messages to detect:");
                ImGui.BulletText("Gold Saucer GATE announcements (every 20 mins)");
                ImGui.BulletText("Cosmic Exploration Red Alerts");
                ImGui.BulletText("Firmament Fête announcements");
                
                ImGui.Spacing();
                ImGui.TextUnformatted("Tips:");
                ImGui.BulletText("Talk to gatekeepers in Gold Saucer for GATE info");
                ImGui.BulletText("GATEs occur at :00, :20, and :40 past each hour");
                ImGui.BulletText("Events are tracked when mentioned in chat");

                ImGuiHelpers.ScaledDummy(20.0f);

                // Example for other services that Dalamud provides.
                // ClientState provides a wrapper filled with information about the local player object and client.

                var localPlayer = Plugin.ClientState.LocalPlayer;
                if (localPlayer == null)
                {
                    ImGui.TextUnformatted("Our local player is currently not loaded.");
                    return;
                }

                if (!localPlayer.ClassJob.IsValid)
                {
                    ImGui.TextUnformatted("Our current job is currently not valid.");
                    return;
                }

                // If you want to see the Macro representation of this SeString use `ToMacroString()`
                ImGui.TextUnformatted($"Current job: ({localPlayer.ClassJob.RowId}) \"{localPlayer.ClassJob.Value.Abbreviation}\"");

                // Example for quarrying Lumina directly, getting the name of our current area.
                var territoryId = Plugin.ClientState.TerritoryType;
                if (Plugin.DataManager.GetExcelSheet<TerritoryType>().TryGetRow(territoryId, out var territoryRow))
                {
                    ImGui.TextUnformatted($"Current location: ({territoryId}) \"{territoryRow.PlaceName.Value.Name}\"");
                }
                else
                {
                    ImGui.TextUnformatted("Invalid territory.");
                }
            }
        }
    }
}
