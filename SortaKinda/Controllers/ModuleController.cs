﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Inventory;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaBettah.Interfaces;
using SortaBettah.Models.Enums;
using SortaBettah.System.Modules;

namespace SortaBettah.System;

public class ModuleController : IDisposable {
    private readonly IEnumerable<IModule> modules = new List<IModule> {
        new MainInventoryModule(),
        new ArmoryInventoryModule()
    };

    public void Dispose() {
        Unload();

        foreach (var module in modules.OfType<IDisposable>()) {
            module.Dispose();
        }
    }

    public void Load() {
        foreach (var module in modules) {
            module.LoadModule();
        }
    }

    public void Unload() {
        foreach (var module in modules) {
            module.UnloadModule();
        }
    }

    public void Update() {
        foreach (var module in modules) {
            module.UpdateModule();
        }
    }

    public void Sort() {
        foreach (var module in modules) {
            module.SortModule();
        }
    }

    public void InventoryChanged(IReadOnlyCollection<InventoryEventArgs> events) {
        if (!Service.ClientState.IsLoggedIn) return;
        
        foreach (var module in modules) {
            var inventoryTypes = new HashSet<InventoryType>();

            foreach (var itemEvent in events) {
                if (!IsEventAllowed(itemEvent)) continue;

                AddChangedInventories(itemEvent, inventoryTypes);

                inventoryTypes.RemoveWhere(type => !module.InventoryTypes.Contains(type));
            }

            if (inventoryTypes.Any()) {
                module.InventoryChanged(inventoryTypes.ToArray());
            }
        }
    }

    private static void AddChangedInventories(InventoryEventArgs itemEvent, ICollection<InventoryType> inventoryTypes) {
        switch (itemEvent) {
            case null:
                break;

            case InventoryItemAddedArgs itemAdded:
                inventoryTypes.Add((InventoryType) itemAdded.Inventory);
                break;
            
            case InventoryItemRemovedArgs itemRemoved:
                inventoryTypes.Add((InventoryType) itemRemoved.Inventory);
                break;

            case InventoryItemChangedArgs itemChanged:
                inventoryTypes.Add((InventoryType) itemChanged.Inventory);
                break;

            case InventoryItemMergedArgs itemMerged:
                inventoryTypes.Add((InventoryType) itemMerged.SourceInventory);
                inventoryTypes.Add((InventoryType) itemMerged.TargetInventory);
                break;

            case InventoryItemMovedArgs itemMoved:
                inventoryTypes.Add((InventoryType) itemMoved.SourceInventory);
                inventoryTypes.Add((InventoryType) itemMoved.TargetInventory);
                break;

            case InventoryItemSplitArgs itemSplit:
                inventoryTypes.Add((InventoryType) itemSplit.SourceInventory);
                inventoryTypes.Add((InventoryType) itemSplit.TargetInventory);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(itemEvent), itemEvent, null);
        }
    }

    public void DrawModule(ModuleName module) {
        modules.FirstOrDefault(drawableModule => drawableModule.ModuleName == module)?.Draw();
    }

    private static bool IsEventAllowed(InventoryEventArgs arg) => arg.Type switch {
        GameInventoryEvent.Added when SortaBettahController.SystemConfig.SortOnItemAdded => true,
        GameInventoryEvent.Removed when SortaBettahController.SystemConfig.SortOnItemRemoved => true,
        GameInventoryEvent.Changed when SortaBettahController.SystemConfig.SortOnItemChanged => true,
        GameInventoryEvent.Moved when SortaBettahController.SystemConfig.SortOnItemMoved => true,
        GameInventoryEvent.Split when SortaBettahController.SystemConfig.SortOnItemSplit => true,
        GameInventoryEvent.Merged when SortaBettahController.SystemConfig.SortOnItemMerged => true,
        _ => false
    };
}