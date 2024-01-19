#pragma warning disable IDE1006

using HarmonyLib;
using System.Collections.Generic;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyDependencyPatch {
    public static HashSet<EnemyAI> ActiveEnemies { get; } = [];

    [HarmonyPatch(nameof(EnemyAI.OnDestroy))]
    static void Prefix(EnemyAI __instance) {
        _ = ActiveEnemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(EnemyAI.Start))]
    static void Postfix(EnemyAI __instance) {
        _ = ActiveEnemies.Add(__instance);
    }
}
