using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(ShipTeleporter), "beamUpPlayer")]
sealed class ShipTeleportWithItemsPatch {
    [HarmonyPrefix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.DropAllHeldItemsAndSync), OuterBody = InfixOuterBody.Auto)]
    static bool KeepItems() => false;
}
