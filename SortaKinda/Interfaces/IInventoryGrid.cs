using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SortaBettah.Interfaces;

public interface IInventoryGrid {
    List<IInventorySlot> Inventory { get; set; }
    InventoryType Type { get; }
}