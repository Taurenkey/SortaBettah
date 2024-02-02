using KamiLib.AutomaticUserInterface;

namespace SortaBettah.Models.Enums;

public enum ToggleFilterState {
    [EnumLabel("Ignored")]
    Ignored,
    
    [EnumLabel("Allow")]
    Allow,
    
    [EnumLabel("Disallow")]
    Disallow,
}