#pragma warning disable IDE1006

using HarmonyLib;
using Unity.Netcode;

[HarmonyPatch(typeof(HoarderBugAI))]
class HoarderBugAIFixPatch {
    [HarmonyPatch(nameof(HoarderBugAI.HitEnemy))]
    static void Prefix(HoarderBugAI __instance) {
        if (!__instance.isEnemyDead) return;
        if (!__instance.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) return;

        __instance.DropItemAndCallDropRPC(networkObject, false);
    }
}
