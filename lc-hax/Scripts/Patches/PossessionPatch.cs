#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch]
class PossessionPatch {
    [HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
    static bool Prefix(ref bool __result) {
        if (PossessionMod.Instance is null or { IsPossessed: false }) return true;

        __result = false;
        return false;
    }

    static bool ReloadUI = false;

    [HarmonyPatch(typeof(HUDManager), "Update")]
    static void Prefix(HUDManager __instance, ref float ___holdButtonToEndGameEarlyHoldTime, ref bool ___hasLoadedSpectateUI) {
        if (PossessionMod.Instance is null or { IsPossessed: true }) {
            ___holdButtonToEndGameEarlyHoldTime = 0.0f;
            __instance.holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
            __instance.RemoveSpectateUI();
            ___hasLoadedSpectateUI = true;
            __instance.HUDAnimator.SetTrigger("hideHud");
            __instance.scanInfoAnimator.SetBool("display", false);
            ReloadUI = true;
        }
        else if (ReloadUI) {
            ___hasLoadedSpectateUI = false;
            __instance.scanInfoAnimator.SetBool("display", true);
            ReloadUI = false;
        }
        else {
            __instance.HUDAnimator.SetTrigger("revealHud");
        }
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    static bool Prefix() => PossessionMod.Instance is null or { IsPossessed: false };
}
