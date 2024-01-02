#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem))]
class ShotgunPatch {
    [HarmonyPatch(nameof(ShotgunItem.ShootGun))]
    static void Prefix() => SoundManager.Instance.timeSinceEarsStartedRinging = 0.0f;

    [HarmonyPatch(nameof(ShotgunItem.Update))]
    static void Postfix(ref float ___misfireTimer, ref bool ___hasHitGroundWithSafetyOff) {
        ___misfireTimer = 30.0f;
        ___hasHitGroundWithSafetyOff = true;
    }
}
