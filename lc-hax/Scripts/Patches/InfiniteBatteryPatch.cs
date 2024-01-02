#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.LateUpdate))]
class InfiniteBatteryPatch {
    static void Postfix(ref Battery ___insertedBattery) {
        if (!___insertedBattery.IsNotNull(out Battery battery)) return;
        battery.charge = 1.0f;
        battery.empty = false;
    }
}
