using SortaBettah.Interfaces;
using SortaBettah.Models.Enums;
using SortaBettah.System;

namespace SortaBettah.Views.Tabs;

public class MainInventoryTab : IInventoryConfigurationTab {
    public string TabName => "Main Inventory";
    public bool Enabled => true;
    public void DrawInventory() => SortaBettahController.ModuleController.DrawModule(ModuleName.MainInventory);
}