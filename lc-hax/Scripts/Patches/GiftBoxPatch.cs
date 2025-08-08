#pragma warning disable IDE1006
#pragma warning disable IDE0060

using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

[HarmonyPatch(typeof(GiftBoxItem))]
sealed class GiftBoxPatch {
    static bool LocalPlayerActivated { get; set; }

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

    [HarmonyPatch(nameof(GiftBoxItem.OpenGiftBoxNoPresentClientRpc))]
    static bool Prefix() => false;
}
