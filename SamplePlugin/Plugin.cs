using System;
using System.Linq;
using System.Text.RegularExpressions;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets2;
using SamplePlugin.Windows;
using ContentFinderCondition = Lumina.Excel.GeneratedSheets.ContentFinderCondition;

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
    
    public IDutyState DutyState { get; init; }
    
    public Plugin(IFramework framework, IChatGui chatGui,  IDataManager dataManager, IClientState clientState, IDutyState dutyState)
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        
        var newGameFontHandle = PluginInterface.UiBuilder.FontAtlas.NewGameFontHandle(new GameFontStyle(GameFontFamily.Axis, 46));
        
        ConfigWindow = new ConfigWindow(this);
        Overlay=new Overlay(this, Configuration, newGameFontHandle);
        Framework = framework;
        ChatGui = chatGui;
        DataManager = dataManager;
        ClientState = clientState;
        DutyState = dutyState;

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

    private unsafe void CheckFood(IFramework framework)
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
        
        var currentContent = DataManager.GetExcelSheet<ContentFinderCondition>()!.GetRow(GameMain.Instance()->CurrentContentFinderConditionId);
        // Only show if level synced
        if (Configuration.ShowIfLevelSynced && currentContent.ClassJobLevelSync != PlayerCharacter.Level)  {
            ToggleOverlayOff();
            return;
        }
        
        // Check Content Type By Name
        var contentName = currentContent.Name.RawString;
        var isMatch = Regex.IsMatch(contentName, "(Minstrel's Ballad|\\(Extreme\\)|\\(Savage\\)|\\(Ultimate\\))");
        if (!isMatch) return;
        if ((contentName.Contains("(Extreme)") || contentName.Contains("The Minstrel's Ballad")) && !Configuration.ShowInSavage) return;
        if (contentName.Contains("(Savage)") && !Configuration.ShowInSavage) return;
        if (contentName.Contains("(Ultimate)") && !Configuration.ShowInSavage) return;
        
        // Whether to show in combat
        if (Configuration.HideInCombat && (PlayerCharacter.StatusFlags & StatusFlags.InCombat) != 0)
        {
            ToggleOverlayOff();
            return;
        }
        
        // Make sure duty is ready
        if (!DutyState.IsDutyStarted) return;
        
        // Check if well-fed
        var playerCharacterStatusList = PlayerCharacter.StatusList;
        bool hasFood = false;
        foreach (var status in playerCharacterStatusList)
        {
            if (status.StatusId == 48 && status.RemainingTime > Configuration.RemainingTimeInSeconds)
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
