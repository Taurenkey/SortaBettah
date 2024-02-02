using KamiLib.AutomaticUserInterface;

namespace SortaBettah.Models.Enums;

public enum SortOrderMode {
    [EnumLabel("Alphabetical")] 
    Alphabetically,
    
    [EnumLabel("ItemLevel")] 
    ItemLevel,

    [EnumLabel("Rarity")] 
    Rarity,

    [EnumLabel("SellPrice")] 
    SellPrice,

    [EnumLabel("ItemId")] 
    ItemId,
    
    [EnumLabel("ItemType")]
    ItemType,
}