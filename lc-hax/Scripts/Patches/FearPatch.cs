using System.Collections.Generic;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch]
sealed class FearPatch {
    static IEnumerable<MethodBase> TargetMethods() => [
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.IncreaseFearLevelOverTime)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.JumpToFearLevel))
    ];

    static bool Prefix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) return true;

        __instance.playersManager.fearLevel = 0.0f;
        __instance.playersManager.fearLevelIncreasing = false;
        return false;
    }
}

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetPlayerSanityLevel))]
sealed class FearResetPatch {
    static void Postfix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) return;

        __instance.playersManager.fearLevel = 0.0f;
        __instance.playersManager.fearLevelIncreasing = false;
    }
}
