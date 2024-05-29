#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class InvisiblePatch
{
    private static Vector3 LastNewPos { get; set; }
    private static bool LastInElevator { get; set; }
    private static bool LastInShipRoom { get; set; }
    private static bool LastExhausted { get; set; }
    private static bool LastIsPlayerGrounded { get; set; }

    [HarmonyPatch("UpdatePlayerPositionServerRpc")]
    private static void Prefix(
        ulong ___actualClientId,
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool inShipRoom,
        ref bool exhausted,
        ref bool isPlayerGrounded
    )
    {
        if (!Setting.EnableInvisible || Helper.LocalPlayer?.actualClientId != ___actualClientId) return;

        LastNewPos = newPos;
        LastInElevator = inElevator;
        LastInShipRoom = inShipRoom;
        LastExhausted = exhausted;
        LastIsPlayerGrounded = isPlayerGrounded;

        newPos = new Vector3(0.0f, -100.0f, 0.0f);
        inElevator = false;
        inShipRoom = false;
        exhausted = false;
        isPlayerGrounded = true;
    }

    [HarmonyPatch("UpdatePlayerPositionClientRpc")]
    private static void Prefix(
        PlayerControllerB __instance,
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool isInShip,
        ref bool exhausted,
        ref bool isPlayerGrounded
    )
    {
        if (!Setting.EnableInvisible || !__instance.IsSelf()) return;

        newPos = LastNewPos;
        inElevator = LastInElevator;
        isInShip = LastInShipRoom;
        exhausted = LastExhausted;
        isPlayerGrounded = LastIsPlayerGrounded;
    }
}