using ImGuiNET;

namespace FoodReminder.Windows;

public class ContentTab(Configuration Configuration)
{
    private Configuration Configuration { get; } = Configuration;

    public void Draw()
    {
        if (ImGui.BeginTabItem("Content"))
        {
            var showIfLevelSynced = Configuration.ShowIfLevelSynced;
            if (ImGui.Checkbox("Level Synced Only", ref showIfLevelSynced))
            {
                Configuration.ShowIfLevelSynced = showIfLevelSynced;
                Configuration.Save();
            }

            var enableAll = Configuration.EnableAll;
            if (ImGui.Checkbox("All Duty", ref enableAll))
            {
                Configuration.EnableAll = enableAll;
                Configuration.Save();
            }

            if (!Configuration.EnableAll)
            {
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

            ImGui.EndTabItem();
        }
    }
}
