using System.Collections.Generic;
using System.Numerics;
using SortaBettah.Models;
using SortaBettah.Models.Enums;
using SortaBettah.Models.General;

namespace SortaBettah.Interfaces;

public interface ISortingRule : IComparer<IInventorySlot> {
    Vector4 Color { get; set; }
    string Id { get; }
    string Name { get; }
    int Index { get; }

    HashSet<string> AllowedItemNames { get; }
    HashSet<UserRegex> AllowedNameRegexes { get; }
    HashSet<uint> AllowedItemTypes { get; }
    HashSet<ItemRarity> AllowedItemRarities { get; }
    RangeFilter ItemLevelFilter { get; }
    RangeFilter VendorPriceFilter { get; }
    ToggleFilter UntradableFilter { get; }
    ToggleFilter UniqueFilter { get; }
    ToggleFilter CollectableFilter { get; }
    ToggleFilter DyeableFilter { get; }
    ToggleFilter RepairableFilter { get; }

    SortOrderDirection Direction { get; set; }
    FillMode FillMode { get; set; }
    SortOrderMode SortMode { get; set; }

    void ShowTooltip();
    bool IsItemSlotAllowed(IInventorySlot slot);
}