using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using KamiLib.Game;
using Lumina.Excel.GeneratedSheets;
using SortaBettah.Interfaces;
using SortaBettah.Models.Configuration;
using SortaBettah.System;

namespace SortaBettah.Models.Inventory;

public unsafe class InventorySlot : IInventorySlot {
    public InventorySlot(InventoryType type, SlotConfig config, int index) {
        Type = type;
        Config = config;
        Slot = index;
    }

    private InventoryType Type { get; }
    public SlotConfig Config { get; init; }
    public bool HasItem => InventoryItem->ItemID is not 0;
    public Item? ExdItem => LuminaCache<Item>.Instance.GetRow(InventoryItem->ItemID);
    public InventoryItem* InventoryItem => InventoryController.GetItemForSlot(Type, Slot);
    public ItemOrderModuleSorterItemEntry* ItemOrderEntry => InventoryController.GetItemOrderData(Type, Slot);

    public ISortingRule Rule {
        get {
            var sortControllerRule = SortaBettahController.SortController.GetRule(Config.RuleId);

            if (sortControllerRule.Id != Config.RuleId) {
                TryApplyRule(sortControllerRule.Id);
            }
            return sortControllerRule;
        }
    }

    public int Slot { get; init; }

    public void OnLeftClick() => TryApplyRule(SortaBettahController.SortController.SelectedRule.Id);

    public void OnRightClick() => TryApplyRule(SortController.DefaultId);

    public void OnDragCollision() => TryApplyRule(SortaBettahController.SortController.SelectedRule.Id);

    public void OnHover() => Rule.ShowTooltip();

    private void TryApplyRule(string id) {
        if (Config.RuleId != id) {
            Config.RuleId = id;
            Config.Dirty = true;
        }
    }
}