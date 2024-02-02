using ImGuiNET;
using KamiLib.Interfaces;
using SortaBettah.System;
using System.IO;

namespace SortaBettah.Views.Tabs;

public class ProfileConfigurationTab : ITabItem
{
    public string TabName => "Profile Settings";

    public bool Enabled => true;

    public void Draw()
    {
        SortaBettahController.DrawProfileConfig();

        if (ImGui.Button("Copy this character's config from SortaKinda to global SortaBettah"))
        {
            var dir = GetSortaKindaCharacterDirectory(Service.ClientState.LocalContentId);
            if (dir != null)
            {
                foreach (var file in dir.GetFiles())
                {
                    Service.Log.Debug($"Copying {file.FullName} to {Path.Combine(Service.PluginInterface.ConfigDirectory.FullName, file.Name)}");
                    file.CopyTo(Path.Combine(Service.PluginInterface.ConfigDirectory.FullName, file.Name), true);
                }

                SortaBettahController.ModuleController.Load();
                SortaBettahController.SortController.Load();
            }
        }
    }

    private static DirectoryInfo GetSortaKindaCharacterDirectory(ulong contentId)
    {
        var directoryInfo = new DirectoryInfo(Path.Combine(Service.PluginInterface.ConfigDirectory.FullName.Replace("SortaBettah", "SortaKinda"), contentId.ToString()));
        if (directoryInfo.Exists)
        {
            return directoryInfo;
        }
        return null;
    }
}

