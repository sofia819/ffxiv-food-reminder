using ImGuiNET;

namespace FoodReminder.Windows;

public class MainTab(Configuration configuration)
{
    private Configuration Configuration { get; } = configuration;

    public void Draw()
    {
        if (ImGui.BeginTabItem("Main"))
        {
            // can't ref a property, so use a local copy
            var configValue = Configuration.IsEnabled;
            if (ImGui.Checkbox("Enable", ref configValue))
            {
                Configuration.IsEnabled = configValue;
                // can save immediately on change, if you don't want to provide a "Save and Close" button
                Configuration.Save();
            }

            var isOverlayLocked = Configuration.IsOverlayLocked;
            if (ImGui.Checkbox("Lock Overlay", ref isOverlayLocked))
            {
                Configuration.IsOverlayLocked = isOverlayLocked;
                Configuration.Save();
            }

            var hideInCombat = Configuration.HideInCombat;
            if (ImGui.Checkbox("Hide In Combat", ref hideInCombat))
            {
                Configuration.HideInCombat = hideInCombat;
                Configuration.Save();
            }

            var remainingTimeInMinutes = Configuration.RemainingTimeInSeconds / 60;
            if (ImGui.InputInt("Minute(s)", ref remainingTimeInMinutes, 1))
            {
                if (remainingTimeInMinutes > 60) remainingTimeInMinutes = 0;
                if (remainingTimeInMinutes < 0) remainingTimeInMinutes = 60;
                Configuration.RemainingTimeInSeconds = remainingTimeInMinutes * 60;
                Configuration.Save();
            }

            ImGui.EndTabItem();
        }
    }
}
