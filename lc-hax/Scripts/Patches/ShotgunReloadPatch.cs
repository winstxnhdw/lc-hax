using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem))]
sealed class ShotgunFreeReloadPatch {
    /// <summary>
    /// Allows reloading without an inventory shell. Keep paired with KeepAmmo below.
    /// </summary>
    [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.FindAmmoInInventory))]
    static bool Prefix(ref int __result) {
        __result = -2;
        return false;
    }

    /// <summary>
    /// Always allow the Shotgun to reload
    /// </summary>
    [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemInteractLeftRight))]
    static void Prefix(ShotgunItem __instance) => __instance.shellsLoaded = 1;
}

[HarmonyPatch(typeof(ShotgunItem), "reloadGunAnimation")]
sealed class ShotgunReloadPatch {
    [HarmonyPrefix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.DestroyItemInSlotAndSync), OuterBody = InfixOuterBody.Auto)]
    static bool KeepAmmo() => false;
}
