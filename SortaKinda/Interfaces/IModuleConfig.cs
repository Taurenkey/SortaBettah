using System.Collections.Generic;

namespace SortaBettah.Models.Configuration;

public interface IModuleConfig {
    List<InventoryConfig> InventoryConfigs { get; set; }
}