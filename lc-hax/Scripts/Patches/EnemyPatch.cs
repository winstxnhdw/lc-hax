#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyPatch {
    [HarmonyPatch(nameof(EnemyAI.SyncPositionToClients))]
    static void Prefix(ref float ___updatePositionThreshold) => ___updatePositionThreshold = 0.0f;

    // Fixes game bug where you can't aim anymore if an enemy dies while attempting to kill you.
    [HarmonyPatch(nameof(EnemyAI.CancelSpecialAnimationWithPlayer))]
    static void Prefix(ref PlayerControllerB ___inSpecialAnimationWithPlayer) {
        if (Helper.LocalPlayer == ___inSpecialAnimationWithPlayer) {
            Helper.LocalPlayer.disableLookInput = false;
        }
    }
}
