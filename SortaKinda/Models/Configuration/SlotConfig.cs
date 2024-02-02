using System;
using SortaBettah.System;

namespace SortaBettah.Models.Configuration;

public class SlotConfig {
    [NonSerialized] public bool Dirty;

    public string RuleId { get; set; } = SortController.DefaultId;
}