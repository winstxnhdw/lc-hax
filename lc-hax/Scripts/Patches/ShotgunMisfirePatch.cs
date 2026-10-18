using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.Update))]
sealed class ShotgunMisfirePatch {
    static void Postfix(ref float ___misfireTimer, ref bool ___hasHitGroundWithSafetyOff) {
        ___misfireTimer = 30.0f;
        ___hasHitGroundWithSafetyOff = true;
    }
}
