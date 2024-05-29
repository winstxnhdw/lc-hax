#pragma warning disable IDE1006

using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

[HarmonyPatch(typeof(GiftBoxItem))]
internal class GiftBoxPatch
{
    private static bool LocalPlayerActivated { get; set; } = false;

    [HarmonyPatch(nameof(GiftBoxItem.ItemActivate))]
    private static bool Prefix(GiftBoxItem __instance)
    {
        if (!Setting.EnableUnlimitedGiftBox) return true;
        LocalPlayerActivated = true;
        __instance.OpenGiftBoxServerRpc();
        return false;
    }

    [HarmonyPatch(nameof(GiftBoxItem.OpenGiftBoxClientRpc))]
    private static bool Prefix(GiftBoxItem __instance, NetworkObjectReference netObjectRef, int presentValue,
        Vector3 startFallingPos)
    {
        if (!Setting.EnableUnlimitedGiftBox) return true;

        var skipFlag = !LocalPlayerActivated;
        LocalPlayerActivated = false;
        return skipFlag;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GiftBoxItem.OpenGiftBoxNoPresentClientRpc))]
    private static bool Prefix()
    {
        return !Setting.EnableUnlimitedGiftBox;
    }
}