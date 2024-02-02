using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using SortaBettah.Models.Enums;

namespace SortaBettah.Interfaces;

public interface IModule : IDisposable {
    ModuleName ModuleName { get; }
    IEnumerable<InventoryType> InventoryTypes { get; }

    void LoadModule();
    void UnloadModule();
    void UpdateModule();
    void SortModule();
    void Draw();
    void InventoryChanged(params InventoryType[] changedInventories);
}