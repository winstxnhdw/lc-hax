#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GiftBoxItem), nameof(GiftBoxItem.ItemActivate))]
class GiftBoxPatch {
    static bool Prefix(ref GiftBoxItem __instance) {
        __instance.OpenGiftBoxServerRpc();
        return false;
    }
}
