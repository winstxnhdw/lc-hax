#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyPatch {
    [HarmonyPatch(nameof(EnemyAI.SyncPositionToClients))]
    static void Prefix(EnemyAI __instance) => __instance.updatePositionThreshold = 0.0f;

    [HarmonyPatch(nameof(EnemyAI.CancelSpecialAnimationWithPlayer))]
    static void Postfix(EnemyAI __instance) {
        if (__instance.inSpecialAnimationWithPlayer is not PlayerControllerB player) return;
        player.disableLookInput = false;
    }
}
