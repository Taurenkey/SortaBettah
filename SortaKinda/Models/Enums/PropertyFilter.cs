﻿using KamiLib.AutomaticUserInterface;

namespace SortaBettah.Models.Enums;

public enum PropertyFilter {
    [EnumLabel("Untradable")]
    Untradable,
    
    [EnumLabel("Dyeable")]
    Dyeable,
    
    [EnumLabel("Unique")]
    Unique,
    
    [EnumLabel("Collectable")]
    Collectable,
    
    [EnumLabel("Repairable")]
    Repairable,
}