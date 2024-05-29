#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch]
internal class HudPatch
{
    [HarmonyPatch(typeof(HUDManager), "PingScan_performed")]
    [HarmonyPrefix]
    private static bool PingScanPrefix()
    {
        return HaxCamera.Instance?.enabled != true &&
               PossessionMod.Instance?.IsPossessed != true;
    }

    [HarmonyPatch(typeof(HUDManager), "Update")]
    private static void Postfix(HUDManager __instance, ref float ___holdButtonToEndGameEarlyHoldTime,
        ref bool ___hasLoadedSpectateUI)
    {
        if (PossessionMod.Instance is null or { IsPossessed: true })
        {
            ___holdButtonToEndGameEarlyHoldTime = 0.0f;
            __instance.holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    private static bool Prefix()
    {
        return PossessionMod.Instance is null or { IsPossessed: false };
    }
}