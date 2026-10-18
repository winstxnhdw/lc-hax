using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.MeetsScanNodeRequirements))]
sealed class ScanThroughWallsPatch {
    [HarmonyPostfix]
    [HarmonyInfix(typeof(ScanNodeProperties), nameof(ScanNodeProperties.requiresLineOfSight), InnerTargetKind.FieldRead)]
    static void IgnoreLineOfSight(ref bool __result) => __result = false;
}
