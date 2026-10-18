using System.Collections.Generic;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch]
sealed class WeightPatch {
    static IEnumerable<MethodBase> TargetMethods() => [
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.GetPlayerVelocity)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.Update)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.LateUpdate)),
        AccessTools.Method(typeof(HUDManager), nameof(HUDManager.UpdateWeightCounter))
    ];

    [HarmonyPostfix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.carryWeight), InnerTargetKind.FieldRead)]
    static void IgnoreWeight(PlayerControllerB __instance, ref float __result) {
        if (!__instance.IsSelf()) return;

        __result = 1.0f;
    }
}
