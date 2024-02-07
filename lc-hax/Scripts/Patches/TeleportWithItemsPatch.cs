#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DropAllHeldItems))]
class TeleportWithItemsPatch {
    static bool Prefix(PlayerControllerB __instance) => __instance.shipTeleporterId is not -1 or 1;
}
