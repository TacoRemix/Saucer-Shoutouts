using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Game.Command;

namespace SaucerShoutouts;

public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Saucer Shoutouts";

    private readonly IPluginLog log;
    private readonly ICommandManager commands;
    private readonly IUiBuilder uiBuilder;
    private readonly WindowSystem windowSystem = new("SaucerShoutouts");
    private readonly PluginWindow window;

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        IPluginLog log,
        ICommandManager commands,
        IUiBuilder uiBuilder)
    {
        this.log = log;
        this.commands = commands;
        this.uiBuilder = uiBuilder;

        window = new PluginWindow();
        windowSystem.AddWindow(window);

        uiBuilder.Draw += () => windowSystem.Draw();
        uiBuilder.OpenConfigUi += ToggleWindow;

        commands.AddHandler("/saucer", new CommandInfo((_, __) => ToggleWindow())
        {
            HelpMessage = "Open Saucer Shoutouts"
        });

        log.Info("Saucer Shoutouts loaded.");
    }

    private void ToggleWindow() => window.IsOpen = !window.IsOpen;

    public void Dispose()
    {
        commands.RemoveHandler("/saucer");
        uiBuilder.OpenConfigUi -= ToggleWindow;
        windowSystem.RemoveAllWindows();
    }
}
