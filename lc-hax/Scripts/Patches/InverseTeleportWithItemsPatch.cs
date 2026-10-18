using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(ShipTeleporter), nameof(ShipTeleporter.TeleportPlayerOutWithInverseTeleporter))]
sealed class InverseTeleportWithItemsPatch {
    [HarmonyPrefix]
    [HarmonyInfix(typeof(PlayerControllerB), nameof(PlayerControllerB.DropAllHeldItems))]
    static bool KeepItems() => false;
}
