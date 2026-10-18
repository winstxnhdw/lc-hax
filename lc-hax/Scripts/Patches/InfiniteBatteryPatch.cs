using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.LateUpdate))]
sealed class InfiniteBatteryPatch {
    static void Postfix(GrabbableObject __instance) {
        if (__instance.insertedBattery is not { } battery) return;

        battery.charge = 1.0f;
        battery.empty = false;
    }
}
