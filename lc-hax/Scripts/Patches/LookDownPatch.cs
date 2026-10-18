using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.CalculateSmoothLookingInput))]
sealed class SmoothLookDownPatch {
    [HarmonyPostfix]
    [HarmonyInfix(60.0f)]
    static void IncreaseLimit(ref float __result) => __result = 90.0f;
}

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.CalculateNormalLookingInput))]
sealed class NormalLookDownPatch {
    [HarmonyPostfix]
    [HarmonyInfix(80.0f)]
    static void IncreaseLimit(ref float __result) => __result = 90.0f;
}
