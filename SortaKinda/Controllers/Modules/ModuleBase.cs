using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.FileIO;
using SortaBettah.Interfaces;
using SortaBettah.Models.Configuration;
using SortaBettah.Models.Enums;

namespace SortaBettah.System;

public abstract class ModuleBase : IModule {
    private bool IsLoaded { get; set; }
    private float lastScale;
    protected abstract List<IInventoryGrid> Inventories { get; set; }
    public IEnumerable<InventoryType> InventoryTypes => Inventories.Select(inventory => inventory.Type);

    protected abstract IModuleConfig ModuleConfig { get; set; }

    private IModuleConfig DefaultConfig => ModuleName switch {
        ModuleName.MainInventory => new MainInventoryConfig(),
        ModuleName.ArmoryInventory => new ArmoryConfig(),
        _ => throw new ArgumentOutOfRangeException()
    };

    public abstract ModuleName ModuleName { get; }

    public abstract void Draw();

    protected abstract void LoadViews();

    public virtual void Dispose() { }

    public void LoadModule() {
        Service.Log.Debug($"[{ModuleName}] Loading Module");

        ModuleConfig = DefaultConfig;
        ModuleConfig = LoadConfig();
        Load();
        LoadViews();
        IsLoaded = true;

        SaveConfig();
    }

    public void UnloadModule() {
        IsLoaded = false;
    }

    public void UpdateModule() {
        if (!IsLoaded) return;

        var needsSaving = false;
        foreach (var inventory in ModuleConfig.InventoryConfigs) {
            foreach (var slot in inventory.SlotConfigs) {
                if (slot.Dirty) {
                    needsSaving = true;
                    slot.Dirty = false;
                }
            }
        }

        if (needsSaving) SaveConfig();

        if (!ImGuiHelpers.GlobalScale.Equals(lastScale)) {
            LoadViews();
        }
        lastScale = ImGuiHelpers.GlobalScale;
    }

    public void SortModule() {
        if (!IsLoaded) return;

        Sort(InventoryTypes.ToArray());
    }

    protected virtual void Load() { }

    public void InventoryChanged(params InventoryType[] changedInventories) => Sort(changedInventories);

    protected abstract void Sort(params InventoryType[] inventories);

    private IModuleConfig LoadConfig() => SortaBettahController.ProfileConfig.UseAccountWideSettings ? FileController.LoadFile<IModuleConfig>($"{ModuleName}.config.json", ModuleConfig) : CharacterFileController.LoadFile<IModuleConfig>($"{ModuleName}.config.json", ModuleConfig);

    private void SaveConfig()
    {
        if (SortaBettahController.ProfileConfig.UseAccountWideSettings) FileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
        else CharacterFileController.SaveFile($"{ModuleName}.config.json", ModuleConfig.GetType(), ModuleConfig);
    }
}