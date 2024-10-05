#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(EnemyAI))]
class EnemyDependencyPatch {
    [HarmonyPatch(nameof(EnemyAI.OnDestroy))]
    static void Prefix(EnemyAI __instance) => Helper.Enemies.Remove(__instance);

    [HarmonyPatch(nameof(EnemyAI.Start))]
    static void Postfix(EnemyAI __instance) => Helper.Enemies.Add(__instance);
}

[HarmonyPatch(typeof(MaskedPlayerEnemy))]
class MaskedDependencyPatch {
    [HarmonyPatch(nameof(MaskedPlayerEnemy.OnDestroy))]
    static void Prefix(MaskedPlayerEnemy __instance) => Helper.Enemies.Remove(__instance);

    [HarmonyPatch(nameof(MaskedPlayerEnemy.Start))]
    static void Postfix(MaskedPlayerEnemy __instance) => Helper.Enemies.Add(__instance);
}
