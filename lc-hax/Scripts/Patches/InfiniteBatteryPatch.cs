#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.Update))]
class InfiniteBatteryPatch {
    static void Postfix(ref Battery ___insertedBattery) {
        if (!___insertedBattery.IsNotNull(out Battery battery)) return;
        battery.charge = 1.0f;
        battery.empty = false;
    }
}
