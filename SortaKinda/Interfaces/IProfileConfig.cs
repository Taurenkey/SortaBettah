using KamiLib.AutomaticUserInterface;

namespace SortaBettah.Interfaces;

[Category("ProfileOptions")]
public interface IProfileConfig
{
    [BoolConfig("UseAccountWideSettings")]
    public bool UseAccountWideSettings {  get; set; }
}

