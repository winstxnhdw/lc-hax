#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch]
class PossessionPatch {
    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    static bool Prefix() => PossessionMod.Instance is not { IsPossessed: true };

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HUDManager), "PingScan_performed")]
    static bool PingScanPrefix() =>
        HaxCamera.Instance?.HaxCameraContainer is not { activeSelf: true } &&
        PossessionMod.Instance is not { IsPossessed: true };

    [HarmonyPatch(typeof(HUDManager), "Update")]
    static void Postfix(HUDManager __instance, ref float ___holdButtonToEndGameEarlyHoldTime) {
        if (PossessionMod.Instance is not { IsPossessed: true }) return;

        ___holdButtonToEndGameEarlyHoldTime = 0.0f;
        __instance.holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
    }
}
