#pragma warning disable IDE1006

#region

using GameNetcodeStuff;
using HarmonyLib;

#endregion

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DropAllHeldItems))]
class TeleportWithItemsPatch {
    static bool Prefix(PlayerControllerB __instance) => __instance.shipTeleporterId is not -1 or 1;
}
