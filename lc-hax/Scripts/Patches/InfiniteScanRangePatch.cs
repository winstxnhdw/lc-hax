using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.AssignNewNodes))]
sealed class InfiniteScanRangePatch {
    [HarmonyPostfix]
    [HarmonyInfix(20.0f)]
    static void IncreaseRadius(ref float __result) => __result = 50.0f;

    [HarmonyPostfix]
    [HarmonyInfix(80.0f, Positions = new[] { 1 })]
    static void IncreaseDistance(ref float __result) => __result = float.MaxValue;
}

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.MeetsScanNodeRequirements))]
sealed class ScanNodeRangePatch {
    [HarmonyPostfix]
    [HarmonyInfix(typeof(ScanNodeProperties), nameof(ScanNodeProperties.maxRange), InnerTargetKind.FieldRead)]
    static void IncreaseMaximumRange(ref int __result) => __result = int.MaxValue;

    [HarmonyPostfix]
    [HarmonyInfix(typeof(ScanNodeProperties), nameof(ScanNodeProperties.minRange), InnerTargetKind.FieldRead)]
    static void RemoveMinimumRange(ref int __result) => __result = -1;
}
