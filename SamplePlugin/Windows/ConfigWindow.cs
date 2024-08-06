using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace SamplePlugin.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration Configuration;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("FoodReminder###Config")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(200, 300);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
    }

    public override void Draw()
    {
        // can't ref a property, so use a local copy
        var configValue = Configuration.IsEnabled;
        if (ImGui.Checkbox("IsEnabled", ref configValue))
        {
            Configuration.IsEnabled = configValue;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            Configuration.Save();
        }

        var overlayMovable = Configuration.IsOverlayMovable;
        if (ImGui.Checkbox("IsOverlayMovable", ref overlayMovable))
        {
            Configuration.IsOverlayMovable = overlayMovable;
            Configuration.Save();
        }
        
        var shouldFlash = Configuration.IsFlashingEffectEnabled;
        if (ImGui.Checkbox("FlashingEffect", ref shouldFlash))
        {
            Configuration.IsFlashingEffectEnabled = shouldFlash;
            Configuration.Save();
        }
        
        var hideInCombat = Configuration.HideInCombat;
        if (ImGui.Checkbox("HideInCombat", ref hideInCombat))
        {
            Configuration.HideInCombat = hideInCombat;
            Configuration.Save();
        }
        
        var remainingTimeInMinutes = Configuration.RemainingTimeInSeconds / 60;
        if (ImGui.InputInt("Time", ref remainingTimeInMinutes, 1))
        {
            if (remainingTimeInMinutes > 60)
            {
                remainingTimeInMinutes = 0;
            }
            if (remainingTimeInMinutes < 0)
            {
                remainingTimeInMinutes = 60;
            }
            Configuration.RemainingTimeInSeconds = remainingTimeInMinutes * 60;
            Configuration.Save();
        }
        
        ImGui.Text("Content Type");
        
        var showIfLevelSynced = Configuration.ShowIfLevelSynced;
        if (ImGui.Checkbox("Level Synced Only", ref showIfLevelSynced))
        {
            Configuration.ShowIfLevelSynced = showIfLevelSynced;
            Configuration.Save();
        }

        var showInExtreme = Configuration.ShowInExtreme;
        if (ImGui.Checkbox("Extreme", ref showInExtreme))
        {
            Configuration.ShowInExtreme = showInExtreme;
            Configuration.Save();
        }
        
        var showInSavage = Configuration.ShowInSavage;
        if (ImGui.Checkbox("Savage", ref showInSavage))
        {
            Configuration.ShowInSavage = showInSavage;
            Configuration.Save();
        }
        
        var showInUltimate = Configuration.ShowInUltimate;
        if (ImGui.Checkbox("Ultimate", ref showInUltimate))
        {
            Configuration.ShowInUltimate = showInUltimate;
            Configuration.Save();
        }
    }
}
