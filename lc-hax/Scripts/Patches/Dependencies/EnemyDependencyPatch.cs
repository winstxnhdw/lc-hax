#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(EnemyAI))]
internal class EnemyDependencyPatch
{
    [HarmonyPatch(nameof(EnemyAI.OnDestroy))]
    private static void Prefix(EnemyAI __instance)
    {
        Helper.Enemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(EnemyAI.Start))]
    private static void Postfix(EnemyAI __instance)
    {
        Helper.Enemies.Add(__instance);
    }
}

[HarmonyPatch(typeof(MaskedPlayerEnemy))]
internal class MaskedDependencyPatch
{
    [HarmonyPatch(nameof(MaskedPlayerEnemy.OnDestroy))]
    private static void Prefix(MaskedPlayerEnemy __instance)
    {
        Helper.Enemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(MaskedPlayerEnemy.Start))]
    private static void Postfix(MaskedPlayerEnemy __instance)
    {
        Helper.Enemies.Add(__instance);
    }
}