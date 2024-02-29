#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch]
class HudPatch {
    [HarmonyPatch(typeof(HUDManager), "PingScan_performed")]
    [HarmonyPrefix]
    static bool PingScanPrefix() => HaxCamera.Instance?.HaxCamContainer?.activeSelf != true &&
                            PossessionMod.Instance?.IsPossessed != true;

    [HarmonyPatch(typeof(HUDManager), "Update")]
    static void Postfix(HUDManager __instance, ref float ___holdButtonToEndGameEarlyHoldTime,
        ref bool ___hasLoadedSpectateUI) {
        if (PossessionMod.Instance is null or { IsPossessed: true }) {
            ___holdButtonToEndGameEarlyHoldTime = 0.0f;
            __instance.holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    static bool Prefix() => PossessionMod.Instance is null or { IsPossessed: false };
}
