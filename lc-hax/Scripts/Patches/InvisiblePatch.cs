#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB))]
sealed class InvisiblePatch {
    [HarmonyPatch("UpdatePlayerPositionRpc")]
    static void Prefix(
        ulong ___actualClientId,
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool inShipRoom,
        ref bool exhausted,
        ref bool isPlayerGrounded
    ) {
        if (!Setting.EnableInvisible) return;
        if (Helper.LocalPlayer?.actualClientId != ___actualClientId) return;

        newPos = new Vector3(0.0f, -100.0f, 0.0f);
        inElevator = false;
        inShipRoom = false;
        exhausted = false;
        isPlayerGrounded = true;
    }
}
