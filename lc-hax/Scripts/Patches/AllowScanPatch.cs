#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(HUDManager))]
class AllowScanPatch {
    [HarmonyPatch("CanPlayerScan")]
    static bool Prefix(ref bool __result) {
        __result = true;
        return false;
    }
}
