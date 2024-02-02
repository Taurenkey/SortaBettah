﻿using System.Diagnostics.CodeAnalysis;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;

namespace SortaBettah.Interfaces;

public unsafe interface IInventorySlot {
    [MemberNotNullWhen(true, "ExdItem")] bool HasItem { get; }

    Item? ExdItem { get; }
    InventoryItem* InventoryItem { get; }
    ItemOrderModuleSorterItemEntry* ItemOrderEntry { get; }
    int Slot { get; }
    ISortingRule Rule { get; }

    void OnLeftClick();
    void OnRightClick();
    void OnDragCollision();
    void OnHover();
}