using KamiLib.Interfaces;
using SortaBettah.System;

namespace SortaBettah.Views.Tabs;

public class GeneralConfigurationTab : ITabItem {
    public string TabName => "General Settings";
    public bool Enabled => true;
    public void Draw() => SortaBettahController.DrawConfig();
}