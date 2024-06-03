namespace Hax;

static partial class Helper {
    internal static void RemoveItemFromHud(int slot) {
        if (HUDManager is not HUDManager hudManager) return;
        if (slot < 0 || slot >= hudManager.itemSlotIcons.Length) return;
        hudManager.holdingTwoHandedItem.enabled = false;
        hudManager.itemSlotIcons[slot].enabled = false;
        hudManager.ClearControlTips();
    }
}
