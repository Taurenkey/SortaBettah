using System;
using System.Collections.Generic;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Plugin.Services;
using KamiLib.AutomaticUserInterface;
using KamiLib.Command;
using KamiLib.FileIO;
using KamiLib.Game;
using KamiLib.System;
using SortaBettah.Models.Configuration;

namespace SortaBettah.System;

public class SortaBettahController : IDisposable {
    public static ModuleController ModuleController = null!;
    public static SortController SortController = null!;
    public static SystemConfig SystemConfig = null!;
    public static ProfileConfig ProfileConfig = null!;
    public static SortingThreadController SortingThreadController = null!;

    private uint lastJob = uint.MaxValue;
    private DateTime lastSortCommand = DateTime.MinValue;

    public SortaBettahController() {
        SortingThreadController = new SortingThreadController();
        SystemConfig = new SystemConfig();
        ProfileConfig =  new ProfileConfig();
        SortController = new SortController();
        ModuleController = new ModuleController();

        if (Service.ClientState is { IsLoggedIn: true, LocalPlayer: not null, LocalContentId: not 0 }) {
            OnLogin();
        }
        
        CommandController.RegisterCommands(this);

        Service.ClientState.Login += OnLogin;
        Service.ClientState.Logout += OnLogout;
        Service.Framework.Update += OnUpdate;
        Service.ClientState.TerritoryChanged += OnZoneChange;
        Service.GameInventory.InventoryChanged += OnInventoryChanged;
    }

    public void Dispose() {
        Service.ClientState.Login -= OnLogin;
        Service.ClientState.Logout -= OnLogout;
        Service.Framework.Update -= OnUpdate;
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        Service.GameInventory.InventoryChanged -= OnInventoryChanged;

        ModuleController.Dispose();
        SortingThreadController.Dispose();
        CommandController.UnregisterCommands(this);
    }

    private void OnLogin() {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState is not { LocalPlayer: { Name.TextValue: var playerName, HomeWorld.GameData.InternalName.RawString: var worldName } }) return;

        ProfileConfig = LoadProfileConfig() ?? new();

        SystemConfig = new SystemConfig();
        SystemConfig = LoadConfig();
        SystemConfig.CharacterName = playerName;
        SystemConfig.HomeWorld = worldName;
        SaveConfig();

        SortController.Load();
        ModuleController.Load();

        if (SystemConfig.SortOnLogin) ModuleController.Sort();
    }

    private void OnLogout() {
        ModuleController.Unload();

        lastJob = uint.MaxValue;
    }

    private void OnUpdate(IFramework framework) {
        if (!Service.ClientState.IsLoggedIn) return;
        if (Service.ClientState is not { LocalPlayer.ClassJob.Id: var classJobId }) return;
        
        // Don't update modules if the Retainer transfer window is open
        if (Service.GameGui.GetAddonByName("RetainerItemTransferProgress") != nint.Zero) return;
        
        ModuleController.Update();

        // Prevent sorting on load, we have a different option for that
        if (lastJob is uint.MaxValue) lastJob = classJobId;

        if (SystemConfig.SortOnJobChange && lastJob != classJobId) {
            ModuleController.Sort();
            lastJob = classJobId;
        }
        
        SortingThreadController.Update();
    }

    private void OnZoneChange(ushort e) {
        if (!Service.ClientState.IsLoggedIn) return;

        if (SystemConfig.SortOnZoneChange) ModuleController.Sort();
    }

    private void OnInventoryChanged(IReadOnlyCollection<InventoryEventArgs> events) => ModuleController.InventoryChanged(events);

    [SingleTierCommandHandler("SortAll", "sort")]
    // ReSharper disable once UnusedMember.Local
    private void SortCommand() {
        var timeSinceLastSort = DateTime.UtcNow - lastSortCommand;
        
        if (timeSinceLastSort.TotalSeconds > 10) {
            ModuleController.Sort();
            lastSortCommand = DateTime.UtcNow;
        }
        else {
            Chat.PrintError($"Attempted to sort too soon after last sort. Try again in {10 - timeSinceLastSort.Seconds} seconds.");
        }
    }
    
    public static void DrawConfig() => DrawableAttribute.DrawAttributes(SystemConfig, SaveConfig);

    public static void DrawProfileConfig() => DrawableAttribute.DrawAttributes(ProfileConfig, SaveProfileConfig);
    private static SystemConfig LoadConfig() => ProfileConfig.UseAccountWideSettings ? FileController.LoadFile<SystemConfig>("System.config.json", SystemConfig) : CharacterFileController.LoadFile<SystemConfig>("System.config.json", SystemConfig);

    private static ProfileConfig LoadProfileConfig() => FileController.LoadFile<ProfileConfig>("Profile.config.json", ProfileConfig);

    private static void SaveConfig()
    {
        if (ProfileConfig.UseAccountWideSettings) FileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig); 
        else CharacterFileController.SaveFile("System.config.json", SystemConfig.GetType(), SystemConfig);
    }

    private static void SaveProfileConfig()
    {
        FileController.SaveFile("Profile.config.json", ProfileConfig.GetType(), ProfileConfig);
        ModuleController.Load();
        SortController.Load();
    }
}