#pragma warning disable IDE1006

using HarmonyLib;
using System.Collections.Generic;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyListener {
    [HarmonyPatch(nameof(EnemyAI.OnDestroy))]
    static void Prefix(ref EnemyAI __instance) {
        _ = ActiveEnemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(EnemyAI.Start))]
    static void Postfix(ref EnemyAI __instance) {
        _ = ActiveEnemies.Add(__instance);
    }


    public static HashSet<EnemyAI> ActiveEnemies = [];
}
