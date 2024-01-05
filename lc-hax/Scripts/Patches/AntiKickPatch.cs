#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using Steamworks;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "SendNewPlayerValuesServerRpc")]
class AntiKickPatch {
    static bool Prefix(PlayerControllerB __instance) {
        if (!Setting.EnableAntiKick) return true;

        ulong[] playerSteamIds = new ulong[__instance.playersManager.allPlayerScripts.Length];

        for (int i = 0; i < __instance.playersManager.allPlayerScripts.Length; i++) {
            playerSteamIds[i] = __instance.playersManager.allPlayerScripts[i].playerSteamId;
        }

        playerSteamIds[__instance.playerClientId] = SteamClient.SteamId;

        _ = __instance.Reflect().InvokeInternalMethod("SendNewPlayerValuesClientRpc", playerSteamIds);

        return false;
    }
}
