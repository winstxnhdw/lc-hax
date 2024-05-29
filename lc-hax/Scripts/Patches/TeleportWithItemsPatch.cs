#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DropAllHeldItems))]
internal class TeleportWithItemsPatch
{
    private static bool Prefix(PlayerControllerB __instance)
    {
        return __instance.shipTeleporterId is not -1 or 1;
    }
}