#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyListener {
    [HarmonyPatch(nameof(EnemyAI.OnDestroy))]
    static void Prefix(ref EnemyAI __instance) {
        ActiveEnemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(EnemyAI.Start))]
    static void Postfix(ref EnemyAI __instance) {
        ActiveEnemies.Add(__instance);
    }


    public static HashSet<EnemyAI> ActiveEnemies = [];
}
