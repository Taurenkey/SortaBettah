using System.Collections.Generic;
using SortaBettah.Models;

namespace SortaBettah.Interfaces;

public interface ISortController {
    List<SortingRule> Rules { get; }

    void SortAllInventories();
    void SaveConfig();
}