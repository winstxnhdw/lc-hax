#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyPatch {
    [HarmonyPatch(nameof(EnemyAI.SyncPositionToClients))]
    static void Prefix(ref float ___updatePositionThreshold) => ___updatePositionThreshold = 0.0f;

    [HarmonyPatch(nameof(EnemyAI.CancelSpecialAnimationWithPlayer))]
    static void Prefix(ref PlayerControllerB? ___inSpecialAnimationWithPlayer) {
        if (___inSpecialAnimationWithPlayer is null) return;
        ___inSpecialAnimationWithPlayer.disableLookInput = false;
    }
}
