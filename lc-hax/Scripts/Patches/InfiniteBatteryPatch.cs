#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.SyncBatteryServerRpc))]
class InfiniteBatteryPatch {
    static bool Prefix(GrabbableObject __instance, ref int charge) {
        if (!__instance.itemProperties.requiresBattery) return true;

        __instance.insertedBattery.empty = false;
        __instance.insertedBattery.charge = 1.0f;
        charge = 100;

        return false;
    }
}
