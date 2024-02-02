using SortaBettah.Interfaces;

namespace SortaBettah.Models.Configuration;

public class ProfileConfig : IProfileConfig
{
    public bool UseAccountWideSettings { get; set; } = false;
}


