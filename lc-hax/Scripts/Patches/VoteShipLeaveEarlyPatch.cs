#pragma warning disable IDE1006

using HarmonyLib;
using UnityEngine.UI;

[HarmonyPatch]
class VoteShipLeaveEarlyPatch {
    [HarmonyPatch(typeof(HUDManager), "Update")]
    static void Prefix(HUDManager __instance, ref float ___holdButtonToEndGameEarlyHoldTime) {
        if (PossessionMod.Instance?.IsPossessed is false) return;

        ___holdButtonToEndGameEarlyHoldTime = 0.0f;
        __instance.holdButtonToEndGameEarlyMeter?.gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    static bool Prefix() => PossessionMod.Instance?.IsPossessed is false;
}
