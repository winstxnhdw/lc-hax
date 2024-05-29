#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem))]
internal class ShotgunPatch
{
    private static bool InterruptDestroyItem { get; set; } = false;

    /// <summary>
    ///     Prevents the shotgun from consuming ammo when firing
    /// </summary>
    [HarmonyPatch(typeof(ShotgunItem), "FindAmmoInInventory")]
    private static bool Prefix(ref int __result, ref int ___ammoSlotToUse)
    {
        ___ammoSlotToUse = -2;
        __result = -2;
        return false;
    }

    /// <summary>
    ///     Always allow the Shotgun to reload
    /// </summary>
    [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemInteractLeftRight))]
    private static void Prefix(ShotgunItem __instance)
    {
        __instance.shellsLoaded = 1;
        InterruptDestroyItem = true;
    }

    /// <summary>
    ///     Resets the interrupt flag
    /// </summary>
    [HarmonyPatch(typeof(ShotgunItem), "StopUsingGun")]
    private static void Postfix()
    {
        InterruptDestroyItem = false;
    }

    /// <summary>
    ///     Prevents the shotgun shell from being destroyed
    /// </summary>
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DestroyItemInSlotAndSync))]
    private static bool Prefix()
    {
        return !InterruptDestroyItem;
    }

    /// <summary>
    ///     Prevents the shotgun from misfiring
    /// </summary>
    [HarmonyPatch(nameof(ShotgunItem.Update))]
    private static void Postfix(ref float ___misfireTimer, ref bool ___hasHitGroundWithSafetyOff)
    {
        ___misfireTimer = 30.0f;
        ___hasHitGroundWithSafetyOff = true;
    }
}