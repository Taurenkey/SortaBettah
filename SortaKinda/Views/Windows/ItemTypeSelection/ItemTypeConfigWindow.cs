﻿using System.Linq;
using System.Numerics;
using Dalamud.Interface.Style;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib;
using KamiLib.Game;
using Lumina.Excel.GeneratedSheets;
using SortaBettah.Interfaces;

namespace SortaBettah.Views.Windows;

public class ItemTypeConfigWindow : Window {
    private readonly ISortingRule sortingRule;

    public ItemTypeConfigWindow(ISortingRule rule) : base($"SortaBettah All Item Types - {rule.Name}###ItemTypeConfig{rule.Id}") {
        sortingRule = rule;

        Position = ImGui.GetMainViewport().Size / 2.0f - new Vector2(1024.0f, 720.0f) / 2.0f;
        PositionCondition = ImGuiCond.Appearing;

        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(1024, 720),
            MaximumSize = new Vector2(9999, 9999)
        };

        IsOpen = true;
    }

    public override void PreDraw() {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, StyleModelV1.DalamudStandard.WindowPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, StyleModelV1.DalamudStandard.FramePadding);
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, StyleModelV1.DalamudStandard.CellPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, StyleModelV1.DalamudStandard.ItemSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, StyleModelV1.DalamudStandard.ItemInnerSpacing);
        ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, StyleModelV1.DalamudStandard.IndentSpacing);
    }
    
    public override void Draw() {
        ImGui.Columns(4);

        foreach (var result in LuminaCache<ItemUICategory>.Instance.OrderBy(item => item.OrderMajor).ThenBy(item => item.OrderMinor)) {
            if (result is { RowId: 0, Name.RawString: "" }) continue;

            var enabled = sortingRule.AllowedItemTypes.Contains(result.RowId);
            if (ImGui.Checkbox($"##ItemUiCategory{result.RowId}", ref enabled)) {
                if (enabled) sortingRule.AllowedItemTypes.Add(result.RowId);
                if (!enabled) sortingRule.AllowedItemTypes.Remove(result.RowId);
            }

            if (Service.TextureProvider.GetFromGameIcon(new((uint) result.Icon)) is { } icon) {
                ImGui.SameLine();
                ImGui.SetCursorPos(ImGui.GetCursorPos() with { Y = ImGui.GetCursorPos().Y + 2.0f });
                ImGui.Image(icon.RentAsync().Result.ImGuiHandle, new Vector2(20.0f, 20.0f));
            }

            ImGui.SameLine();
            ImGui.TextUnformatted(result.Name.RawString);

            ImGui.NextColumn();
        }

        ImGui.Columns(1);
    }

    public override void PostDraw() {
        ImGui.PopStyleVar(6);
    }
    
    public override void OnClose() => KamiCommon.WindowManager.RemoveWindow(this);
}