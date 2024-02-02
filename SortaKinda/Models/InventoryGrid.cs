﻿using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaBettah.Interfaces;
using SortaBettah.Models.Configuration;
using SortaBettah.Models.Inventory;

namespace SortaBettah.System;

public class InventoryGrid : IInventoryGrid {
    public InventoryGrid(InventoryType type, InventoryConfig config) {
        Type = type;
        Config = config;
        Inventory = new List<IInventorySlot>();

        foreach (var index in Enumerable.Range(0, InventoryController.GetInventoryPageSize(Type))) {
            Inventory.Add(new InventorySlot(Type, config.SlotConfigs[index], index));
        }
    }

    public InventoryConfig Config { get; init; }
    public InventoryType Type { get; }
    public List<IInventorySlot> Inventory { get; set; }
}