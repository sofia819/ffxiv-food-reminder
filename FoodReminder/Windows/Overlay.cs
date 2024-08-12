using System;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace FoodReminder.Windows;

public class Overlay
    : Window, IDisposable
{
    private const int ImageWidth = 64;

    private const int ImageHeight = 64;

    private readonly Configuration configuration;

    private readonly TimeSpan flashTimeSpan = TimeSpan.FromSeconds(1);

    private readonly IFontHandle font;

    private readonly string iconPath;

    private DateTime lastVisibleTime;

    private bool visible;

    public Overlay(Plugin plugin, Configuration configuration, IFontHandle font, string iconPath) : base(
        "FoodReminder###Overlay")
    {
        this.configuration = configuration;
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(1040, 400);
        SizeCondition = ImGuiCond.Once;

        this.configuration = plugin.Configuration;
        this.font = font;
        this.iconPath = iconPath;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        if (configuration.IsOverlayLocked)
        {
            Flags |= ImGuiWindowFlags.NoInputs;
            Flags |= ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags &= ~ImGuiWindowFlags.NoInputs;
            Flags &= ~ImGuiWindowFlags.NoMove;
        }

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

        var fontGlobalScale = ImGui.GetIO().FontGlobalScale;


        var imageEdge = new Vector2(topLeft.X + (ImageWidth * fontGlobalScale * configuration.OverlayScale),
                                    topLeft.Y + (ImageHeight * fontGlobalScale * configuration.OverlayScale));


        font.Push();
        ImGui.SetWindowFontScale(configuration.OverlayScale);
        var textSize = ImGui.CalcTextSize(eatFood);
        imDrawListPtr.AddRectFilled(
            new Vector2(
                imageEdge.X + (16 * fontGlobalScale * configuration.OverlayScale),
                topLeft.Y + (20 * fontGlobalScale * configuration.OverlayScale)
            ),
            new Vector2(
                imageEdge.X + textSize.X + (24 * fontGlobalScale * configuration.OverlayScale),
                imageEdge.Y + textSize.Y
            ), ImGui.GetColorU32(configuration.BackgroundColor)
        );
        imDrawListPtr.AddText(
            new Vector2(imageEdge.X + (20 * fontGlobalScale * configuration.OverlayScale),
                        topLeft.Y + (30 * fontGlobalScale * configuration.OverlayScale)),
            !configuration.IsFlashingEffectEnabled || visible
                ? ImGui.GetColorU32(configuration.PrimaryTextColor)
                : ImGui.GetColorU32(configuration.SecondaryTextColor),
            eatFood);
        font.Pop();

        if (configuration.ShowIcon && image != null)
        {
            ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMin().X +
                                (16 * fontGlobalScale * configuration.OverlayScale));
            ImGui.SetCursorPosY(ImGui.GetWindowContentRegionMin().Y +
                                (16 * fontGlobalScale * configuration.OverlayScale));
            ImGui.Image(image.ImGuiHandle,
                        new Vector2(ImageWidth * fontGlobalScale * configuration.OverlayScale,
                                    ImageHeight * fontGlobalScale * configuration.OverlayScale));
        }
    }
}
