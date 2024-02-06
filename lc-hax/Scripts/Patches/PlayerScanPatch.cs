#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
class PlayerScanPatch {
    static bool Prefix(ref bool __result) {
        if (PossessionMod.Instance?.IsPossessed is false) return true;

        __result = false;
        return false;
    }
}
