#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch]
class VoteShipLeaveEarlyPatch {

    [HarmonyPatch(typeof(HUDManager), "Update")]
    [HarmonyPrefix]
    static void BlockVoteEarlyHud(ref float ___holdButtonToEndGameEarlyHoldTime, ref UnityEngine.UI.Image ___holdButtonToEndGameEarlyMeter) {
        if (PossessionMod.Instance == null) return;
        if (PossessionMod.Instance.IsPossessed) {
            ___holdButtonToEndGameEarlyHoldTime = 0.0f;
            ___holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    [HarmonyPrefix]
    static bool BlockVoteEarlyRPC() {
        return PossessionMod.Instance == null ? true : !PossessionMod.Instance.IsPossessed;
    }

}
