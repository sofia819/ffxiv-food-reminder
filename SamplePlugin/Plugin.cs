using System;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using SamplePlugin.Windows;

namespace SamplePlugin;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;

    private const string CommandName = "/food";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private Overlay Overlay { get; init; }

    private IFramework Framework { get; }
    public IChatGui ChatGui { get;  }
    
    public IDataManager DataManager { get;  }
    
    public IClientState ClientState { get;  }

    public IPlayerCharacter PlayerCharacter { get; set; }
    
    public Plugin(IFramework framework, IChatGui chatGui,  IDataManager dataManager, IClientState clientState, IDalamudPluginInterface pluginInterface)
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        
        var newGameFontHandle = PluginInterface.UiBuilder.FontAtlas.NewGameFontHandle(new GameFontStyle(GameFontFamily.Axis, 46));
        
        ConfigWindow = new ConfigWindow(this);
        Overlay=new Overlay(this, Configuration, newGameFontHandle);
        Framework = framework;
        ChatGui = chatGui;
        DataManager = dataManager;
        ClientState = clientState; 

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(Overlay);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens Food Reminder config"
        });
        
        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

       
        Framework.Update += CheckFood;
        
    }

    private void CheckFood(IFramework framework)
    {
        // Check if enabled
        if (!Configuration.IsEnabled)
        {
            ToggleOverlayOff();
            return;
        }
        
        // Make sure there is a player
        PlayerCharacter = ClientState.LocalPlayer;
        if (PlayerCharacter == null)
        {
            ToggleOverlayOff();
            return;
        }
        
        // TODO: check content type = EX, savage, ulti and level synced
        
        // Whether to show in combat
        if (Configuration.HideInCombat && (PlayerCharacter.StatusFlags & StatusFlags.InCombat) != 0)
        {
            ToggleOverlayOff();
            return;
        }
        
        // Check if well-fed
        var playerCharacterStatusList = PlayerCharacter.StatusList;
        bool hasFood = false;
        foreach (var status in playerCharacterStatusList)
        {
            if (status is { StatusId: 48, RemainingTime: > 1780 })
            {
                hasFood = true;
            }
            
        }
        if ((!hasFood && !Overlay.IsOpen) || (hasFood && Overlay.IsOpen))
        {
            Overlay.Toggle();
        }
    }

    private void ToggleOverlayOff()
    {
        if (Overlay.IsOpen)
        {
            Overlay.Toggle();
        }
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        Overlay.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // in response to the slash command, just toggle the display status of our config UI
        ToggleConfigUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
}
