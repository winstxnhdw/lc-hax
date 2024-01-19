#pragma warning disable IDE1006

using HarmonyLib;
using System.Collections.Generic;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyListener {
    [HarmonyPatch(nameof(EnemyAI.OnDestroy))]
    static void Prefix(ref EnemyAI __instance) {
        if (ActiveEnemies.Contains(__instance)) _ = ActiveEnemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(EnemyAI.Start))]
    static void Postfix(ref EnemyAI __instance) {
        if (!ActiveEnemies.Contains(__instance)) ActiveEnemies.Add(__instance);
    }


    public static List<EnemyAI> ActiveEnemies = [];
}
