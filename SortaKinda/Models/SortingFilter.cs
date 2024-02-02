using System;
using SortaBettah.Interfaces;

namespace SortaBettah.Models;

public class SortingFilter {
    public required Func<bool> Active { get; init; }
    public required Func<IInventorySlot, bool> IsSlotAllowed { get; init; }
}