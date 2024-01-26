#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DropAllHeldItems))]
class TeleportWithItemsPatch {
    static bool Prefix(int ___shipTeleporterId) => ___shipTeleporterId is not -1 and not 1;
}
