﻿using System.Numerics;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Interfaces;

namespace SortaBettah.Views.Tabs;

public class TutorialAboutTab : ITabItem {
    public string TabName => "About";
    public bool Enabled => true;
    public void Draw() {
        ImGuiHelpers.ScaledDummy(10.0f);
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(ImGui.GetStyle().ItemSpacing.X, 10.0f * ImGuiHelpers.GlobalScale));
        
        ImGui.TextWrapped(AboutText);
        
        ImGui.PopStyleVar();
    }

    private const string AboutText = "Welcome to SortaBettah! A highly customizable inventory management tool.\n" +
                                     "This plugin was designed for you to define precisely what items you want to always be in specific sections of your inventory.\n\n" +
                                     "SortaBettah has no relation to the built in 'isort' function. It does not interact with 'isort' in any way.\n" +
                                     "SortaBettah will override any other sorting systems you attempt to use.\n\n" +
                                     "Automatic sort triggers are available in the general settings tab, these triggers allow SortaBettah to automatically re-sort your inventory as you are playing.\n\n" +
                                     "There may be times where SortaBettah might not catch a change, worry not as triggers should be frequent enough to sort things out on the next change.";
}