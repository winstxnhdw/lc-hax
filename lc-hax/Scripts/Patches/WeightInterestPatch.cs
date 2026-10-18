using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), $"{nameof(IVisibleThreat)}.{nameof(IVisibleThreat.GetInterestLevel)}")]
sealed class WeightInterestPatch {
    [HarmonyPostfix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.carryWeight), InnerTargetKind.FieldRead)]
    static void IgnoreWeight(PlayerControllerB __instance, ref float __result) {
        if (!__instance.IsSelf()) return;

        __result = 1.0f;
    }
}
