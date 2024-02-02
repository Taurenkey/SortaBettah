﻿using System.Collections.Generic;
using KamiLib.Interfaces;
using KamiLib.UserInterface;
using SortaBettah.Interfaces;
using SortaBettah.Views.Tabs;

namespace SortaBettah.Views.SortControllerViews;

public class SortingRuleView {
    private readonly TabBar tabBar;

    public SortingRuleView(ISortingRule rule) {
        tabBar = new TabBar {
            Id = "SortingRuleTabBar",
            TabItems = new List<ITabItem> {
                new ItemTypeFilterTab(rule),
                new ItemNameFilterTab(rule),
                new OtherFiltersTab(rule),
                new ToggleFiltersTab(rule),
                new SortOrderTab(rule),
            }
        };
    }

    public void Draw() => tabBar.Draw();
}