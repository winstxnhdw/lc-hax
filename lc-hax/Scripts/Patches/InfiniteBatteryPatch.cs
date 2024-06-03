#pragma warning disable IDE1006

#region

using HarmonyLib;

#endregion

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.LateUpdate))]
class InfiniteBatteryPatch {
    static void Postfix(GrabbableObject __instance) {
        if (__instance.insertedBattery is not Battery battery) return;

        battery.charge = 1.0f;
        battery.empty = false;
    }
}
