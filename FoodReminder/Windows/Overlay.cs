using System;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using FFXIVClientStructs.FFXIV.Common.Math;

namespace FoodReminder.Windows;

public class Overlay
    : Window, IDisposable
{
    private readonly Configuration configuration;

    private readonly IFontHandle font;

    private DateTime lastVisibleTime;

    private readonly TimeSpan flashTimeSpan = TimeSpan.FromSeconds(1);

    private bool visible;

    private string iconPath;

    public Overlay(Plugin plugin, Configuration configuration, IFontHandle font, string iconPath) : base(
        "FoodReminder###Overlay")
    {
        this.configuration = configuration;
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize;

        Size = new Vector2(680, 280);
        SizeCondition = ImGuiCond.Always;

        this.configuration = plugin.Configuration;
        this.font = font;
        this.iconPath = iconPath;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        if (configuration.IsOverlayMovable)
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

        if (configuration.IsFlashingEffectEnabled && currentTime - lastVisibleTime > flashTimeSpan)
        {
            visible = !visible;
            lastVisibleTime = currentTime;
        }

        var topLeft = ImGui.GetWindowContentRegionMin() + ImGui.GetWindowPos();
        var imDrawListPtr = ImGui.GetWindowDrawList();

        var image = Plugin.TextureProvider.GetFromFile(iconPath).GetWrapOrDefault();

        if (configuration.ShowIcon && image != null)
        {
            ImGui.Image(image.ImGuiHandle,
                        new Vector2(image.Width * configuration.OverlayScale,
                                    image.Height * configuration.OverlayScale));
        }

        var imageEdge = image != null
                            ? new Vector2(topLeft.X + (image.Width * configuration.OverlayScale),
                                          topLeft.Y + (image.Height * configuration.OverlayScale))
                            : new Vector2(topLeft.X + (186 * configuration.OverlayScale),
                                          topLeft.Y + (186 * configuration.OverlayScale));

        imDrawListPtr.AddRectFilled(
            new Vector2(imageEdge.X + (6 * configuration.OverlayScale), topLeft.Y + (24 * configuration.OverlayScale)),
            new Vector2(imageEdge.X + (210 * configuration.OverlayScale),
                        topLeft.Y + (100 * configuration.OverlayScale)),
            ImGui.GetColorU32(configuration.BackgroundColor));
        font.Push();
        ImGui.SetWindowFontScale(configuration.OverlayScale);
        imDrawListPtr.AddText(
            new Vector2(imageEdge.X + (20 * configuration.OverlayScale), topLeft.Y + (40 * configuration.OverlayScale)),
            !configuration.IsFlashingEffectEnabled || visible
                ? ImGui.GetColorU32(configuration.PrimaryTextColor)
                : ImGui.GetColorU32(configuration.SecondaryTextColor),
            eatFood);
        font.Pop();
    }
}
