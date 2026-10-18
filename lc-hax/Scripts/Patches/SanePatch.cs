using System.Collections.Generic;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch]
sealed class SanePatch {
    static IEnumerable<MethodBase> TargetMethods() => [
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.SetPlayerSanityLevel)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.IncreaseFearLevelOverTime)),
        AccessTools.Method(typeof(PlayerControllerB), nameof(PlayerControllerB.JumpToFearLevel))
    ];

    static bool Prefix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) return true;

        __instance.playersManager.gameStats.allPlayerStats[__instance.playerClientId].turnAmount = 0;
        __instance.playersManager.fearLevel = 0.0f;
        __instance.playersManager.fearLevelIncreasing = false;
        __instance.insanityLevel = 0.0f;
        __instance.insanitySpeedMultiplier = 0.0f;

        return false;
    }
}
