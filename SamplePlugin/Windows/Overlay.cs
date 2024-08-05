using System;
using Dalamud.Interface.Colors;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.System.Threading;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;
using Vector4 = System.Numerics.Vector4;

namespace SamplePlugin.Windows;

public class Overlay
    : Window, IDisposable
{
    private Configuration Configuration;

    private IFontHandle Font;

    private bool visible;
    
    private DateTime lastVisibleTime;
    
    private TimeSpan TimeSpan = TimeSpan.FromSeconds(1);
    
    public Overlay(Plugin plugin, Configuration configuration, IFontHandle font) : base("FoodReminder###Overlay")
    {
        Configuration = configuration;
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(200, 60);
        SizeCondition = ImGuiCond.Always;

        Configuration = plugin.Configuration;
        Font = font;
    }

    public void Dispose()
    {
        
    }

    public override void PreDraw()
    {
        // Toggle();
        // Flags must be added or removed before Draw() is being called, or they won't apply
        // ;
        if (Configuration.IsOverlayMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
        Flags |= ImGuiWindowFlags.NoTitleBar;
        Flags |= ImGuiWindowFlags.NoBackground;
    }
    
    public override void Draw()
    {
        var eatFood = "EAT FOOD";
        var currentTime = DateTime.Now;

        if (Configuration.IsFlashingEffectEnabled && currentTime - lastVisibleTime > TimeSpan)
        {
            visible = !visible;
            lastVisibleTime = currentTime;
        }
        
        Font.Push();
        var topLeft = ImGui.GetWindowContentRegionMin() + ImGui.GetWindowPos();
        var bottomRight = ImGui.GetWindowContentRegionMax() + ImGui.GetWindowPos();
        var imDrawListPtr = ImGui.GetWindowDrawList();
        imDrawListPtr.AddRectFilled(topLeft, bottomRight, ImGui.GetColorU32(new Vector4(0,0,0, 0.5f)));
        imDrawListPtr.AddText(
            topLeft,
            visible ? ImGui.GetColorU32(Configuration.PrimaryTextColor) : ImGui.GetColorU32(Configuration.SecondaryTextColor),
            eatFood);

        Font.Pop();

    }
    
    

    
}
