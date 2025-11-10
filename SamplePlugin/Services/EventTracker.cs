using System;
using System.Collections.Generic;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;

namespace SamplePlugin.Services;

public class EventTracker : IDisposable
{
    private readonly IChatGui chatGui;
    private readonly IClientState clientState;
    private readonly Configuration configuration;
    private readonly IPluginLog log;
    
    public DateTime? LastGoldSaucerGateTime { get; private set; }
    public List<DateTime> CosmicExplorationRedAlerts { get; private set; } = new();
    public DateTime? LastFirmamentFeteTime { get; private set; }
    
    // Gold Saucer GATE schedule: Every 20 minutes at :00, :20, and :40
    private const int GateIntervalMinutes = 20;
    
    public EventTracker(IChatGui chatGui, IClientState clientState, Configuration configuration, IPluginLog log)
    {
        this.chatGui = chatGui;
        this.clientState = clientState;
        this.configuration = configuration;
        this.log = log;
        
        // Subscribe to chat messages to detect events
        this.chatGui.ChatMessage += OnChatMessage;
        
        log.Information("EventTracker initialized");
    }
    
    public void Dispose()
    {
        chatGui.ChatMessage -= OnChatMessage;
    }
    
    private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        try
        {
            var messageText = message.TextValue;
            
            // Detect Gold Saucer GATE announcements
            if (configuration.EnableGoldSaucerNotifications && DetectGoldSaucerGate(messageText))
            {
                LastGoldSaucerGateTime = DateTime.UtcNow;
                log.Information($"Gold Saucer GATE detected at {LastGoldSaucerGateTime}");
            }
            
            // Detect Cosmic Exploration Red Alerts
            if (configuration.EnableCosmicExplorationNotifications && DetectCosmicExplorationRedAlert(messageText))
            {
                var alertTime = DateTime.UtcNow;
                CosmicExplorationRedAlerts.Add(alertTime);
                log.Information($"Cosmic Exploration Red Alert detected at {alertTime}");
                
                // Keep only the last 10 alerts
                if (CosmicExplorationRedAlerts.Count > 10)
                {
                    CosmicExplorationRedAlerts.RemoveAt(0);
                }
            }
            
            // Detect Firmament Fete
            if (configuration.EnableFirmamentFeteNotifications && DetectFirmamentFete(messageText))
            {
                LastFirmamentFeteTime = DateTime.UtcNow;
                log.Information($"Firmament Fete detected at {LastFirmamentFeteTime}");
            }
        }
        catch (Exception ex)
        {
            log.Error(ex, "Error processing chat message");
        }
    }
    
    private bool DetectGoldSaucerGate(string message)
    {
        // Common phrases from Gold Saucer gatekeepers about GATE timing
        // These NPCs announce when the next GATE is happening
        var gateKeywords = new[]
        {
            "Gold Saucer Active Time Event",
            "G.A.T.E.",
            "GATE",
            "next event will begin",
            "event will begin in",
            "event is about to begin",
            "the next attraction",
            "attraction will begin"
        };
        
        foreach (var keyword in gateKeywords)
        {
            if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    private bool DetectCosmicExplorationRedAlert(string message)
    {
        // Cosmic Exploration (Island Sanctuary) red alert keywords
        var keywords = new[]
        {
            "red alert",
            "Red Alert",
            "emergency",
            "cosmic exploration",
            "Cosmic Exploration"
        };
        
        foreach (var keyword in keywords)
        {
            if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    private bool DetectFirmamentFete(string message)
    {
        // Firmament Fete keywords
        var keywords = new[]
        {
            "Fête",
            "Fete",
            "Firmament",
            "Skybuilders"
        };
        
        foreach (var keyword in keywords)
        {
            if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        
        return false;
    }
    
    public DateTime GetNextGoldSaucerGateTime()
    {
        var now = DateTime.UtcNow;
        var currentMinute = now.Minute;
        
        // Find the next GATE time (:00, :20, or :40)
        int nextMinute;
        int hourOffset = 0;
        
        if (currentMinute >= 40)
        {
            // Next GATE is at the top of the next hour
            nextMinute = 0;
            hourOffset = 1;
        }
        else if (currentMinute >= 20)
        {
            nextMinute = 40;
        }
        else if (currentMinute >= 0)
        {
            nextMinute = 20;
        }
        else
        {
            // Fallback (should never reach here)
            nextMinute = 0;
            hourOffset = 1;
        }
        
        var nextGateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, nextMinute, 0, DateTimeKind.Utc)
            .AddHours(hourOffset);
        
        return nextGateTime;
    }
    
    public TimeSpan GetTimeUntilNextGoldSaucerGate()
    {
        var nextGateTime = GetNextGoldSaucerGateTime();
        return nextGateTime - DateTime.UtcNow;
    }
    
    public int GetCosmicExplorationRedAlertCount()
    {
        return CosmicExplorationRedAlerts.Count;
    }
    
    public DateTime? GetLastCosmicExplorationRedAlert()
    {
        return CosmicExplorationRedAlerts.Count > 0 
            ? CosmicExplorationRedAlerts[^1] 
            : null;
    }
}
