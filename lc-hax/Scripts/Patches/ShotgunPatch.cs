#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(ShotgunItem))]
class ShotgunPatch {
    [HarmonyPatch(nameof(ShotgunItem.ShootGun))]
    static void Prefix() => SoundManager.Instance.timeSinceEarsStartedRinging = 0f;

    [HarmonyPatch(nameof(ShotgunItem.Update))]
    static void Prefix(ref float ___misfireTimer, ref bool ___hasHitGroundWithSafetyOff) {
        ___misfireTimer = 30f;
        ___hasHitGroundWithSafetyOff = true;
    }
}
