#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GiftBoxItem), nameof(GiftBoxItem.ItemActivate))]
class GiftBoxPatch {
    static void Prefix(ref bool used, ref bool ___hasUsedGift) {
        used = false;
        ___hasUsedGift = false;
    }
}
