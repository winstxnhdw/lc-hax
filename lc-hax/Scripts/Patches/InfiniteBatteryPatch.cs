#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.LateUpdate))]
internal class InfiniteBatteryPatch
{
    private static void Postfix(GrabbableObject __instance)
    {
        if (__instance.insertedBattery is not Battery battery) return;

        battery.charge = 1.0f;
        battery.empty = false;
    }
}