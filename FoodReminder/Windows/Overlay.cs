using System;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace FoodReminder.Windows;

public class Overlay : Window, IDisposable
{
    private const string EatFood = "EAT FOOD";
    private const int ImageWidth = 60;
    private const int ImageHeight = 60;

    private readonly Configuration configuration;

    private readonly TimeSpan flashTimeSpan = TimeSpan.FromSeconds(1);

    private readonly IFontHandle font;

    private readonly string iconPath;

    private DateTime lastVisibleTime;

    private bool visible;

    public Overlay(Plugin plugin, Configuration configuration, IFontHandle font, string iconPath)
        : base("FoodReminder###Overlay")
    {
        this.configuration = configuration;
        Flags =
            ImGuiWindowFlags.NoCollapse
            | ImGuiWindowFlags.NoScrollbar
            | ImGuiWindowFlags.NoScrollWithMouse;

        SizeConstraints = new WindowSizeConstraints { MaximumSize = new Vector2(1280, 360) };
        SizeCondition = ImGuiCond.FirstUseEver;

        this.configuration = plugin.Configuration;
        this.font = font;
        this.iconPath = iconPath;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        Flags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBackground;
        if (configuration.IsOverlayLocked)
        {
            Flags |=
                ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBackground;
        }
        else
        {
            Flags &=
                ~ImGuiWindowFlags.NoInputs
                & ~ImGuiWindowFlags.NoMove
                & ~ImGuiWindowFlags.NoBackground;
        }
    }

    public override void Draw()
    {
        var currentTime = DateTime.Now;
        if (currentTime - lastVisibleTime > flashTimeSpan)
        {
            visible = !visible;
            lastVisibleTime = currentTime;
        }

        var topLeft = ImGui.GetWindowContentRegionMin() + ImGui.GetWindowPos();
        var imDrawListPtr = ImGui.GetWindowDrawList();

        var image = Plugin.TextureProvider.GetFromFile(iconPath).GetWrapOrDefault();

        var imageEdge = new Vector2(
            topLeft.X + (ImageWidth * configuration.OverlayScale),
            topLeft.Y + (ImageHeight * configuration.OverlayScale)
        );

        font.Push();
        ImGui.SetWindowFontScale(configuration.OverlayScale * 1 / ImGui.GetIO().FontGlobalScale);
        var textSize = ImGui.CalcTextSize(EatFood);

        if (configuration.ShowIcon && image != null)
        {
            imDrawListPtr.AddRectFilled(
                topLeft,
                new Vector2(
                    imageEdge.X + (textSize.X + (12 * configuration.OverlayScale)),
                    imageEdge.Y + (8 * configuration.OverlayScale)
                ),
                ImGui.GetColorU32(configuration.BackgroundColor),
                10f
            );
            imDrawListPtr.AddText(
                new Vector2(
                    imageEdge.X + (8 * configuration.OverlayScale),
                    topLeft.Y + (10 * configuration.OverlayScale)
                ),
                !configuration.IsFlashingEffectEnabled || visible
                    ? ImGui.GetColorU32(configuration.PrimaryTextColor)
                    : ImGui.GetColorU32(configuration.SecondaryTextColor),
                EatFood
            );
            ImGui.SetCursorPosX(
                ImGui.GetWindowContentRegionMin().X + (4 * configuration.OverlayScale)
            );
            ImGui.SetCursorPosY(
                ImGui.GetWindowContentRegionMin().Y + (4 * configuration.OverlayScale)
            );
            ImGui.Image(
                image.ImGuiHandle,
                new Vector2(
                    ImageWidth * configuration.OverlayScale,
                    ImageHeight * configuration.OverlayScale
                )
            );
        }
        else
        {
            imDrawListPtr.AddRectFilled(
                topLeft,
                new Vector2(
                    topLeft.X + textSize.X + (6 * configuration.OverlayScale),
                    topLeft.Y + textSize.Y + (8 * configuration.OverlayScale)
                ),
                ImGui.GetColorU32(configuration.BackgroundColor),
                10f
            );
            imDrawListPtr.AddText(
                new Vector2(
                    topLeft.X + (4 * configuration.OverlayScale),
                    topLeft.Y + (4 * configuration.OverlayScale)
                ),
                !configuration.IsFlashingEffectEnabled || visible
                    ? ImGui.GetColorU32(configuration.PrimaryTextColor)
                    : ImGui.GetColorU32(configuration.SecondaryTextColor),
                EatFood
            );
        }

        font.Pop();
    }
}
