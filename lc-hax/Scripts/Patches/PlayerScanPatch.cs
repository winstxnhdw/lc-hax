using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
class PlayerScanPatch {
    static bool Prefix(ref bool __result) {
        if(PossessionMod.Instance == null) return true;
        if (PossessionMod.Instance.IsPossessed) {
            __result = false;
            return false;
        }
        return true;
    }
}
