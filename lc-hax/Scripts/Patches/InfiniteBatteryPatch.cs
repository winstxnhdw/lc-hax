#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.LateUpdate))]
class InfiniteBatteryPatch {
    static void Postfix(ref Battery? ___insertedBattery) {
        if (___insertedBattery is null) return;

        ___insertedBattery.charge = 1.0f;
        ___insertedBattery.empty = false;
    }
}
