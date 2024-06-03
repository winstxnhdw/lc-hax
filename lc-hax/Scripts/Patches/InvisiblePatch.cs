#pragma warning disable IDE1006

#region

using GameNetcodeStuff;
using HarmonyLib;
using Hax;
using UnityEngine;

#endregion

[HarmonyPatch(typeof(PlayerControllerB))]
class InvisiblePatch {
    static Vector3 LastNewPos { get; set; }
    static bool LastInElevator { get; set; }
    static bool LastInShipRoom { get; set; }
    static bool LastExhausted { get; set; }
    static bool LastIsPlayerGrounded { get; set; }

    [HarmonyPatch("UpdatePlayerPositionServerRpc")]
    static void Prefix(
        ulong ___actualClientId,
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool inShipRoom,
        ref bool exhausted,
        ref bool isPlayerGrounded
    ) {
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
    static void Prefix(
        PlayerControllerB __instance,
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool isInShip,
        ref bool exhausted,
        ref bool isPlayerGrounded
    ) {
        if (!Setting.EnableInvisible || !__instance.IsSelf()) return;

        newPos = LastNewPos;
        inElevator = LastInElevator;
        isInShip = LastInShipRoom;
        exhausted = LastExhausted;
        isPlayerGrounded = LastIsPlayerGrounded;
    }
}
