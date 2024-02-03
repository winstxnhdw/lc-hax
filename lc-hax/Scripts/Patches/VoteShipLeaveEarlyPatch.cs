#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch]
class VoteShipLeaveEarlyPatch {

    [HarmonyPatch(typeof(HUDManager), "Update")]
    [HarmonyPostfix]
    static void BlockVoteEarlyHud(ref float ___holdButtonToEndGameEarlyHoldTime, ref UnityEngine.UI.Image ___holdButtonToEndGameEarlyMeter) {
        if (PossessionMod.Instance.IsPossessed) {
            ___holdButtonToEndGameEarlyHoldTime = 0.0f;
            if (___holdButtonToEndGameEarlyMeter != null)
                ___holdButtonToEndGameEarlyMeter.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    [HarmonyPostfix]
    static bool BlockVoteEarlyRPC() {
        return !PossessionMod.Instance.IsPossessed;
    }

}
