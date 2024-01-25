#pragma warning disable IDE1006

using HarmonyLib;
using Hax;
using System.Collections.Generic;

[HarmonyPatch(typeof(MaskedPlayerEnemy))]
class MaskedDependencyPatch {

    [HarmonyPatch(nameof(MaskedPlayerEnemy.OnDestroy))]
    static void Prefix(MaskedPlayerEnemy __instance) {
        _ = Helper.ActiveEnemies.Remove(__instance);
    }

    [HarmonyPatch(nameof(MaskedPlayerEnemy.Start))]
    static void Postfix(MaskedPlayerEnemy __instance) {
        _ = Helper.ActiveEnemies.Add(__instance);
    }
}
