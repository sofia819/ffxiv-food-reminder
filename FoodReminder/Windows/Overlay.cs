using System;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace FoodReminder.Windows;

public class Overlay
    : Window, IDisposable
{
    private readonly Configuration Configuration;

    private readonly IFontHandle Font;

    private DateTime lastVisibleTime;

    private readonly TimeSpan TimeSpan = TimeSpan.FromSeconds(1);

    private bool visible;

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

    public void Dispose() { }

    public override void PreDraw()
    {
        if (Configuration.IsOverlayMovable)
            Flags &= ~ImGuiWindowFlags.NoMove;
        else
            Flags |= ImGuiWindowFlags.NoMove;
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
        imDrawListPtr.AddRectFilled(topLeft, bottomRight, ImGui.GetColorU32(Configuration.BackgroundColor));
        imDrawListPtr.AddText(
            new Vector2(topLeft.X + 4, topLeft.Y),
            !Configuration.IsFlashingEffectEnabled || visible
                ? ImGui.GetColorU32(Configuration.PrimaryTextColor)
                : ImGui.GetColorU32(Configuration.SecondaryTextColor),
            eatFood);

        Font.Pop();
    }
}
