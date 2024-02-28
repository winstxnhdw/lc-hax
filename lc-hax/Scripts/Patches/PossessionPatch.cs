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

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    static bool Prefix() => PossessionMod.Instance is null or { IsPossessed: false };
}
