using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaBettah.Interfaces;
using SortaBettah.Models.Configuration;
using SortaBettah.Models.Enums;
using SortaBettah.Views.SortControllerViews;

namespace SortaBettah.System.Modules;

public class MainInventoryModule : ModuleBase {
    private QuadInventoryView? view;
    public override ModuleName ModuleName => ModuleName.MainInventory;
    protected override List<IInventoryGrid> Inventories { get; set; } = null!;
    protected override IModuleConfig ModuleConfig { get; set; } = new MainInventoryConfig();

    public override void Draw() {
        view?.Draw();
    }
    
    protected override void LoadViews() {
        Inventories = new List<IInventoryGrid>();
        foreach (var config in ModuleConfig.InventoryConfigs) {
            Inventories.Add(new InventoryGrid(config.Type, config));
        }

        view = new QuadInventoryView(Inventories, Vector2.Zero);
    } 

    protected override void Sort(params InventoryType[] inventoryTypes) {
        if (Inventories.SelectMany(inventory => inventory.Inventory).Any(slot => slot.Rule.Id is not SortController.DefaultId)) {
            SortaBettahController.SortingThreadController.AddSortingTask(InventoryType.Inventory1, Inventories.ToArray());
        }
    }
}