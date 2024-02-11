#pragma warning disable IDE1006

using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

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
    static bool Prefix(NetworkObjectReference netObjectRef, int presentValue, Vector3 startFallingPos) {
        bool skipFlag = !GiftBoxPatch.LocalPlayerActivated;
        GiftBoxPatch.LocalPlayerActivated = false;
        return skipFlag;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GiftBoxItem.OpenGiftBoxNoPresentClientRpc))]
    static bool Prefix() => false;
}
