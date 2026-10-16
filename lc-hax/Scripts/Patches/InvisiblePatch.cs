using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(PlayerControllerB))]
sealed class InvisiblePatch {
    [HarmonyPatch(nameof(PlayerControllerB.UpdatePlayerPositionRpc))]
    static void Prefix(
        ulong ___actualClientId,
        ref Vector3 newPos,
        ref bool inElevator,
        ref bool isInShip,
        ref bool exhausted,
        ref bool isPlayerGrounded
    ) {
        if (!Setting.EnableInvisible) return;
        if (Helper.LocalPlayer?.actualClientId != ___actualClientId) return;

        newPos = new Vector3(0.0f, -100.0f, 0.0f);
        inElevator = false;
        isInShip = false;
        exhausted = false;
        isPlayerGrounded = true;
    }
}
