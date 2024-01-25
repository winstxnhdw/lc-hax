#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyDependencyPatch {

    [HarmonyPatch(nameof(EnemyAI.OnDestroy))]
    static void Prefix(EnemyAI __instance) {
        _ = Helper.Enemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(EnemyAI.Start))]
    static void Postfix(EnemyAI __instance) {
        _ = Helper.Enemies.Add(__instance);
    }

}
