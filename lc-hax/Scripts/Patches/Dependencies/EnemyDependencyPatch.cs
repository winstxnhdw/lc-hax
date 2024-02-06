#pragma warning disable IDE1006

using HarmonyLib;
using Hax;
using Unity.Netcode;

[HarmonyPatch(typeof(EnemyAI))]
internal class EnemyDependencyPatch {

    [HarmonyPatch(nameof(EnemyAI.OnDestroy))]
    static void Prefix(EnemyAI __instance) => _ = Helper.Enemies.Remove(__instance);

    [HarmonyPatch(nameof(EnemyAI.Start))]
    static void Postfix(EnemyAI __instance) => _ = Helper.Enemies.Add(__instance);
}

[HarmonyPatch(typeof(MaskedPlayerEnemy))]
internal class MaskedDependencyPatch {

    [HarmonyPatch(nameof(MaskedPlayerEnemy.OnDestroy))]
    static void Prefix(MaskedPlayerEnemy __instance) => _ = Helper.Enemies.Remove(__instance);

    [HarmonyPatch(nameof(MaskedPlayerEnemy.Start))]
    static void Postfix(MaskedPlayerEnemy __instance) => _ = Helper.Enemies.Add(__instance);
}


[HarmonyPatch(typeof(RoundManager))]
internal class RoundManagerDependencyPatch {

    [HarmonyPatch(nameof(RoundManager.SpawnEnemyGameObject))]
    static void Postfix(NetworkObjectReference __result) {
        if (__result.TryGet(out NetworkObject networkObject)) {
            if (!networkObject.TryGetComponent(out EnemyAI AI)) return;
            if (AI != null) {
                _ = Helper.Enemies.Add(AI);
            }
        }
    }
}
