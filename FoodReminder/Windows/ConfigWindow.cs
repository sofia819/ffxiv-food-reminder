using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;

namespace FoodReminder.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly MainTab mainTab;

    private readonly ContentTab contentTab;

    private readonly StyleTab styleTab;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin)
        : base("FoodReminder###FoodReminderConfig")
    {
        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(200, 230);
        SizeCondition = ImGuiCond.Always;

        mainTab = new MainTab(plugin.Configuration);
        contentTab = new ContentTab(plugin.Configuration);
        styleTab = new StyleTab(plugin.Configuration);
    }

    public void Dispose() { }

    public override void PreDraw() { }

    public override void Draw()
    {
        ImGui.BeginTabBar("Settings");

        mainTab.Draw();
        contentTab.Draw();
        styleTab.Draw();

        ImGui.EndTabBar();
    }
}
