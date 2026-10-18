using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(ShipTeleporter), nameof(ShipTeleporter.TeleportPlayerOutWithInverseTeleporter))]
sealed class InverseTeleportWithItemsPatch {
    [HarmonyPrefix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.DropAllHeldItems))]
    static bool KeepItems() => false;
}

[HarmonyPatch(typeof(ShipTeleporter), "beamUpPlayer")]
sealed class ShipTeleportWithItemsPatch {
    [HarmonyPrefix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.DropAllHeldItemsAndSync), OuterBody = InfixOuterBody.Auto)]
    static bool KeepItems() => false;
}
