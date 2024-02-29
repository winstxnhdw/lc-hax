#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch]
class PossessionPatch {
    [HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
    static bool Prefix(ref bool __result) {
        if (HaxCamera.Instance?.HaxCamContainer?.activeSelf == false) return false;
        if (PossessionMod.Instance is null or { IsPossessed: false }) return true;

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(HUDManager), "Update")]
    static void Prefix(HUDManager __instance, ref float ___holdButtonToEndGameEarlyHoldTime,
        ref bool ___hasLoadedSpectateUI) {
        if (PossessionMod.Instance is null or { IsPossessed: true }) {
            ___holdButtonToEndGameEarlyHoldTime = 0.0f;
            __instance.holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    static bool Prefix() => PossessionMod.Instance is null or { IsPossessed: false };
}
