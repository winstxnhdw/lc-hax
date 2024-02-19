#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
class PossessionPatch {
    static bool Prefix(ref bool __result) {
        if (PossessionMod.Instance is null or { IsPossessed: false }) return true;

        __result = false;
        return false;
    }
}
