using ImGuiNET;

namespace FoodReminder.Windows;

public class StyleTab(Configuration configuration)
{
    private Configuration Configuration { get; } = configuration;

    public void Draw()
    {
        if (ImGui.BeginTabItem("Style"))
        {
            var primaryTextColor = Configuration.PrimaryTextColor;
            if (ImGui.ColorEdit4("Primary", ref primaryTextColor, ImGuiColorEditFlags.NoInputs))
            {
                Configuration.PrimaryTextColor = primaryTextColor;
                Configuration.Save();
            }

            var secondaryTextColor = Configuration.SecondaryTextColor;
            if (ImGui.ColorEdit4("Secondary", ref secondaryTextColor, ImGuiColorEditFlags.NoInputs))
            {
                Configuration.SecondaryTextColor = secondaryTextColor;
                Configuration.Save();
            }

            var backgroundColor = Configuration.BackgroundColor;
            if (ImGui.ColorEdit4("Background", ref backgroundColor, ImGuiColorEditFlags.NoInputs))
            {
                Configuration.BackgroundColor = backgroundColor;
                Configuration.Save();
            }

            var shouldFlash = Configuration.IsFlashingEffectEnabled;
            if (ImGui.Checkbox("Flashing Effect", ref shouldFlash))
            {
                Configuration.IsFlashingEffectEnabled = shouldFlash;
                Configuration.Save();
            }

            var showIcon = Configuration.ShowIcon;
            if (ImGui.Checkbox("Show Icon", ref showIcon))
            {
                Configuration.ShowIcon = showIcon;
                Configuration.Save();
            }

            var overlayScale = Configuration.OverlayScale;
            if (ImGui.DragFloat("Scale", ref overlayScale, 0.01f, 0.2f, 3.0f))
            {
                Configuration.OverlayScale = overlayScale;
                Configuration.Save();
            }

            ImGui.EndTabItem();
        }
    }
}
