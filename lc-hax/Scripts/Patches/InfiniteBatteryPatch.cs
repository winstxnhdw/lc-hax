using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.SyncBatteryServerRpc))]
class InfiniteBatteryPatch {
    static bool Prefix(ref int charge) {
        charge = 100;

        return false;
    }
}
