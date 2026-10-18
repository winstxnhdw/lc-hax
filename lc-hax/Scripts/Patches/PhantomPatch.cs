using HarmonyLib;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.CanPlayerScan))]
sealed class PhantomScanPatch {
    static bool Prefix(ref bool __result) {
        if (!Setting.EnablePhantom) return true;

        __result = false;
        return false;
    }
}
