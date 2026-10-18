using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(ShotgunItem))]
sealed class ShotgunPatch {
    /// <summary>
    /// Prevents the shotgun from consuming ammo when firing
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

    /// <summary>
    /// Prevents the shotgun from misfiring
    /// </summary>
    [HarmonyPatch(nameof(ShotgunItem.Update))]
    static void Postfix(ref float ___misfireTimer, ref bool ___hasHitGroundWithSafetyOff) {
        ___misfireTimer = 30.0f;
        ___hasHitGroundWithSafetyOff = true;
    }
}

[HarmonyPatch(typeof(ShotgunItem), "reloadGunAnimation")]
sealed class ShotgunReloadPatch {
    [HarmonyPrefix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.DestroyItemInSlotAndSync), OuterBody = InfixOuterBody.Auto)]
    static bool KeepAmmo() => false;
}
