﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using SortaBettah.Interfaces;
using SortaBettah.Models.Enums;
using SortaBettah.Models.General;
using SortaBettah.System;
using SortaBettah.Views.SortControllerViews;

namespace SortaBettah.Models;

public unsafe class SortingRule : ISortingRule {
    private readonly SortingRuleTooltipView view;
    private readonly List<SortingFilter> filterRules;

    public SortingRule() {
        view = new SortingRuleTooltipView(this);
        filterRules = new List<SortingFilter> {
            new() {
                Active = () => AllowedNameRegexes.Any(),
                IsSlotAllowed = slot => {
                    foreach (var allowedRegex in AllowedNameRegexes) {
                        if (slot is { ExdItem: { Name.RawString: var itemName, RowId: not 0 } }) {
                            if (allowedRegex.Match(itemName)) return true;
                        }
                    }

                    return false;
                },
            },
            new() {
                Active = () => AllowedItemTypes.Any(),
                IsSlotAllowed = slot => AllowedItemTypes.Any(allowed => slot.ExdItem?.ItemUICategory.Row == allowed),
            },
            new() {
                Active = () => AllowedItemRarities.Any(),
                IsSlotAllowed = slot => AllowedItemRarities.Any(allowed => slot.ExdItem?.Rarity == (byte) allowed),
            },
            new() {
                Active = () => ItemLevelFilter.Enable,
                IsSlotAllowed = slot => ItemLevelFilter.IsItemSlotAllowed(slot.ExdItem?.LevelItem.Row),
            },
            new() {
                Active = () => VendorPriceFilter.Enable,
                IsSlotAllowed = slot => VendorPriceFilter.IsItemSlotAllowed(slot.ExdItem?.PriceLow),
            },
            new() {
                Active = () => UntradableFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => UntradableFilter.IsItemSlotAllowed(slot),
            },
            new() {
                Active = () => UniqueFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => UniqueFilter.IsItemSlotAllowed(slot),
            },
            new() {
                Active = () => CollectableFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => CollectableFilter.IsItemSlotAllowed(slot),
            },
            new() {
                Active = () => DyeableFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => DyeableFilter.IsItemSlotAllowed(slot),
            },
            new() {
                Active = () => RepairableFilter.State is not ToggleFilterState.Ignored,
                IsSlotAllowed = slot => RepairableFilter.IsItemSlotAllowed(slot),
            }
        };
    }

    public Vector4 Color { get; set; }
    public string Id { get; set; } = SortController.DefaultId;
    public string Name { get; set; } = "New Rule";
    public int Index { get; set; }
    public HashSet<string> AllowedItemNames { get; set; } = new();
    public HashSet<UserRegex> AllowedNameRegexes { get; set; } = new();
    public HashSet<uint> AllowedItemTypes { get; set; } = new();
    public HashSet<ItemRarity> AllowedItemRarities { get; set; } = new();
    public RangeFilter ItemLevelFilter { get; set; } = new("Item Level Filter", 0, 1000);
    public RangeFilter VendorPriceFilter { get; set; } = new("Vendor Price Filter", 0, 1_000_000);
    public ToggleFilter UntradableFilter { get; set; } = new(PropertyFilter.Untradable);
    public ToggleFilter UniqueFilter { get; set; } = new(PropertyFilter.Unique);
    public ToggleFilter CollectableFilter { get; set; } = new(PropertyFilter.Collectable);
    public ToggleFilter DyeableFilter { get; set; } = new(PropertyFilter.Dyeable);
    public ToggleFilter RepairableFilter { get; set; } = new(PropertyFilter.Repairable);
    public SortOrderDirection Direction { get; set; } = SortOrderDirection.Ascending;
    public FillMode FillMode { get; set; } = FillMode.Top;
    public SortOrderMode SortMode { get; set; } = SortOrderMode.Alphabetically;
    public bool InclusiveAnd = false;

    public void ShowTooltip() {
        view.Draw();
    }

    public int Compare(IInventorySlot? x, IInventorySlot? y) {
        if (x is null) return 0;
        if (y is null) return 0;
        if (x.ExdItem is null) return 0;
        if (y.ExdItem is null) return 0;
        if (IsFilterMatch(x.ExdItem, y.ExdItem)) return 0;
        if (CompareSlots(x, y)) return 1;
        return -1;
    }

    public bool IsItemSlotAllowed(IInventorySlot slot) => InclusiveAnd ? 
        filterRules.Any(rule => rule.Active() && rule.IsSlotAllowed(slot)) : 
        filterRules.All(rule => !rule.Active() || rule.Active() && rule.IsSlotAllowed(slot));

    public bool CompareSlots(IInventorySlot a, IInventorySlot b) {
        var firstItem = a.ExdItem;
        var secondItem = b.ExdItem;

        switch (a.HasItem, b.HasItem) {
            // If both items are null, don't swap
            case (false, false): return false;

            // first slot empty, second slot full, if Ascending we want to left justify, move the items left, if Descending right justify, leave the empty slot on the left.
            case (false, true): return FillMode is FillMode.Top;

            // first slot full, second slot empty, if Ascending we want to left justify, and we have that already, if Descending right justify, move the item right
            case (true, false): return FillMode is FillMode.Bottom;

            case (true, true) when firstItem is not null && secondItem is not null:
                
                var shouldSwap = false;
                
                // They are the same item
                if (firstItem.RowId == secondItem.RowId) {
                    // if left is not HQ, and right is HQ, swap
                    if (!a.InventoryItem->Flags.HasFlag(InventoryItem.ItemFlags.HQ) && b.InventoryItem->Flags.HasFlag(InventoryItem.ItemFlags.HQ)) {
                        shouldSwap = true;
                    }
                    // else if left has lower quantity then right, swap
                    else if (a.InventoryItem->Quantity < b.InventoryItem->Quantity) {
                        shouldSwap = true;
                    }
                }
                // else if they match according to the default filter, fallback to alphabetical
                else if (IsFilterMatch(firstItem, secondItem)) {
                    shouldSwap = ShouldSwap(firstItem, secondItem, SortOrderMode.Alphabetically);
                }
                // else they are not the same item, and the filter result doesn't match
                else {
                    shouldSwap = ShouldSwap(firstItem, secondItem, SortMode);
                }
                
                return Direction is SortOrderDirection.Descending ? !shouldSwap : shouldSwap;

            // Something went horribly wrong... best not touch it and walk away.
            default: return false;
        }
    }
    
    private bool IsFilterMatch(Item firstItem, Item secondItem) => SortMode switch {
        SortOrderMode.ItemId => firstItem.RowId == secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.Row == secondItem.LevelItem.Row,
        SortOrderMode.Alphabetically => string.Equals(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase),
        SortOrderMode.SellPrice => firstItem.PriceLow == secondItem.PriceLow,
        SortOrderMode.Rarity => firstItem.Rarity == secondItem.Rarity,
        SortOrderMode.ItemType => firstItem.ItemUICategory.Row == secondItem.ItemUICategory.Row,
        _ => false
    };

    private static bool ShouldSwap(Item firstItem, Item secondItem, SortOrderMode sortMode) => sortMode switch {
        SortOrderMode.ItemId => firstItem.RowId > secondItem.RowId,
        SortOrderMode.ItemLevel => firstItem.LevelItem.Row > secondItem.LevelItem.Row,
        SortOrderMode.Alphabetically => string.Compare(firstItem.Name.RawString, secondItem.Name.RawString, StringComparison.OrdinalIgnoreCase) > 0,
        SortOrderMode.SellPrice => firstItem.PriceLow > secondItem.PriceLow,
        SortOrderMode.Rarity => firstItem.Rarity > secondItem.Rarity,
        SortOrderMode.ItemType => ShouldSwapItemUiCategory(firstItem, secondItem),
        _ => false
    };

    private static bool ShouldSwapItemUiCategory(Item firstItem, Item secondItem) {
        // If same category, don't swap, other system handles fallback to alphabetical in this case
        if (firstItem.ItemUICategory.Row == secondItem.ItemUICategory.Row) return false;

        if (firstItem is { ItemUICategory.Value: { } first } && secondItem is { ItemUICategory.Value: { } second }) {
            if (first.OrderMajor == second.OrderMajor) {
                return first.OrderMinor > second.OrderMinor;
            }

            return first.OrderMajor > second.OrderMajor;
        }

        return false;
    }
}