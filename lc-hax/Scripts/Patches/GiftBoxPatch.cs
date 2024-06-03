#pragma warning disable IDE1006

#region

using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

#endregion

[HarmonyPatch(typeof(GiftBoxItem))]
class GiftBoxPatch {
    static bool LocalPlayerActivated { get; set; } = false;

    [HarmonyPatch(nameof(GiftBoxItem.ItemActivate))]
    static bool Prefix(GiftBoxItem __instance) {
        if (!Setting.EnableUnlimitedGiftBox) return true;
        LocalPlayerActivated = true;
        __instance.OpenGiftBoxServerRpc();
        return false;
    }

    [HarmonyPatch(nameof(GiftBoxItem.OpenGiftBoxClientRpc))]
    static bool Prefix(GiftBoxItem __instance, NetworkObjectReference netObjectRef, int presentValue,
        Vector3 startFallingPos) {
        if (!Setting.EnableUnlimitedGiftBox) return true;

        bool skipFlag = !LocalPlayerActivated;
        LocalPlayerActivated = false;
        return skipFlag;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GiftBoxItem.OpenGiftBoxNoPresentClientRpc))]
    static bool Prefix() => !Setting.EnableUnlimitedGiftBox;
}
