#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GiftBoxItem))]
class GiftBoxPatch {
    static bool LocalPlayerActivated { get; set; } = false;

    [HarmonyPatch(nameof(GiftBoxItem.ItemActivate))]
    static bool Prefix(GiftBoxItem __instance) {
        GiftBoxPatch.LocalPlayerActivated = true;
        __instance.OpenGiftBoxServerRpc();
        return false;
    }

    [HarmonyPatch(nameof(GiftBoxItem.OpenGiftBoxClientRpc))]
    static bool Prefix() {
        bool skipFlag = !GiftBoxPatch.LocalPlayerActivated;
        GiftBoxPatch.LocalPlayerActivated = false;
        return skipFlag;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GiftBoxItem.OpenGiftBoxNoPresentClientRpc))]
    static bool NoPresentPrefix() => false;
}
