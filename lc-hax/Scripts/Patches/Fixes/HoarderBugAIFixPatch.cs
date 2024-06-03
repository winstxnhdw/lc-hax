#pragma warning disable IDE1006

#region

using HarmonyLib;
using Unity.Netcode;

#endregion

[HarmonyPatch(typeof(HoarderBugAI))]
class HoarderBugAIFixPatch {
    [HarmonyPatch(nameof(HoarderBugAI.HitEnemy))]
    static void Prefix(HoarderBugAI __instance) {
        if (!__instance.isEnemyDead) return;
        if (!__instance.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) return;

        _ = __instance.Reflect().InvokeInternalMethod("DropItemAndCallDropRPC", networkObject, false);
    }
}
