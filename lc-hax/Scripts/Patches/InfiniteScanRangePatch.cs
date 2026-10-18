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
sealed class ScanNodeRequirementsPatch {
    static bool Prefix(ScanNodeProperties? node, ref bool __result) {
        if (node is null) return true;

        __result = true;
        return false;
    }
}
