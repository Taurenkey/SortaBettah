using SortaBettah.Interfaces;
using SortaBettah.Models.Enums;
using SortaBettah.System;

namespace SortaBettah.Views.Tabs;

public class ArmoryInventoryTab : IInventoryConfigurationTab {
    public string TabName => "Armory Inventory";
    public bool Enabled => true;
    public void DrawInventory() => SortaBettahController.ModuleController.DrawModule(ModuleName.ArmoryInventory);
}