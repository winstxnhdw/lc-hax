#pragma warning disable IDE1006

using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(MaskedPlayerEnemy))]
class MaskedDependencyPatch {
    [HarmonyPatch(nameof(MaskedPlayerEnemy.OnDestroy))]
    static void Prefix(MaskedPlayerEnemy __instance) {
        _ = Helper.Enemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(MaskedPlayerEnemy.Start))]
    static void Postfix(MaskedPlayerEnemy __instance) {
        _ = Helper.Enemies.Add(__instance);
    }
}
