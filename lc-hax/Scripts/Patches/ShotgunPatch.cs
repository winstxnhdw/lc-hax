#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem))]
class ShotgunPatch {
    static bool InterruptDestroyItem { get; set; } = false;

    /// <summary>
    /// Prevents the shotgun from consuming ammo when firing
    /// </summary>
    [HarmonyPatch(typeof(ShotgunItem), "FindAmmoInInventory")]
    static bool Prefix(ref int __result, ref int ___ammoSlotToUse) {
        ___ammoSlotToUse = -2;
        __result = -2;
        return false;
    }

    /// <summary>
    /// Always allow the Shotgun to reload
    /// </summary>
    [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemInteractLeftRight))]
    static void Prefix(ShotgunItem __instance) {
        __instance.shellsLoaded = 1;
        ShotgunPatch.InterruptDestroyItem = true;
    }

    /// <summary>
    /// Resets the interrupt flag
    /// </summary>
    [HarmonyPatch(typeof(ShotgunItem), "StopUsingGun")]
    static void Postfix() => ShotgunPatch.InterruptDestroyItem = false;

    /// <summary>
    /// Prevents the shotgun shell from being destroyed
    /// </summary>
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DestroyItemInSlotAndSync))]
    static bool Prefix() => !ShotgunPatch.InterruptDestroyItem;

    /// <summary>
    /// Prevents the shotgun from misfiring
    /// </summary>
    [HarmonyPatch(nameof(ShotgunItem.Update))]
    static void Postfix(ref float ___misfireTimer, ref bool ___hasHitGroundWithSafetyOff) {
        ___misfireTimer = 30.0f;
        ___hasHitGroundWithSafetyOff = true;
    }
}
