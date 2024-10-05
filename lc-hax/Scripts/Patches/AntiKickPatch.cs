#pragma warning disable IDE1006

using HarmonyLib;
using Steamworks;
using GameNetcodeStuff;

[HarmonyPatch(typeof(PlayerControllerB))]
class AntiKickPatch {
    [HarmonyPatch("SendNewPlayerValuesServerRpc")]
    static bool Prefix(PlayerControllerB __instance) {
        if (!Setting.EnableAntiKick) return true;

        Helper.GameNetworkManager?.currentLobby?.Members.ForEach((i, member) => {
            if (Helper.LocalPlayer?.playerSteamId == member.Id.Value) return;

            PlayerControllerB player = __instance.playersManager.allPlayerScripts[i];
            player.playerSteamId = member.Id.Value;
            player.playerUsername = member.Name;
            player.usernameBillboardText.text = member.Name;
            __instance.playersManager.mapScreen.radarTargets[i].name = player.playerUsername;
            __instance.quickMenuManager.AddUserToPlayerList(player.playerSteamId, player.playerUsername, i);
        });

        return false;
    }

    [HarmonyPatch("SendNewPlayerValuesClientRpc")]
    static void Postfix(PlayerControllerB __instance) {
        if (!Setting.EnableAntiKick) return;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;

        localPlayer.playerSteamId = SteamClient.SteamId;
        localPlayer.playerUsername = SteamClient.Name;
        localPlayer.usernameBillboardText.text = SteamClient.Name;
        __instance.playersManager.mapScreen.radarTargets[localPlayer.PlayerIndex()].name = localPlayer.playerUsername;
        __instance.quickMenuManager.AddUserToPlayerList(localPlayer.playerSteamId, localPlayer.playerUsername, localPlayer.PlayerIndex());
    }
}
