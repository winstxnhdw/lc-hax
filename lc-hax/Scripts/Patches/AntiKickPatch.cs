using GameNetcodeStuff;
using HarmonyLib;
using Steamworks;
using Steamworks.Data;

[HarmonyPatch(typeof(PlayerControllerB))]
sealed class AntiKickPatch {
    [HarmonyPatch(nameof(PlayerControllerB.SendNewPlayerValuesServerRpc))]
    static bool Prefix(PlayerControllerB __instance) {
        if (!AntiKickPatch.IsLocalClient(__instance)) return true;
        if (Helper.GameNetworkManager?.currentLobby is not { } lobby) return true;

        AntiKickPatch.SetIdentity(__instance, SteamClient.SteamId, SteamClient.Name);
        AntiKickPatch.SetIdentity(__instance.playersManager.allPlayerScripts[0], lobby.Owner.Id, lobby.Owner.Name);

        return false;
    }

    [HarmonyPatch(nameof(PlayerControllerB.SendNewPlayerValuesClientRpc))]
    static void Postfix() {
        if (Helper.LocalPlayer is not { } localPlayer) return;
        if (!AntiKickPatch.IsLocalClient(localPlayer)) return;

        AntiKickPatch.SetIdentity(localPlayer, SteamClient.SteamId, SteamClient.Name);
    }

    static bool IsLocalClient(PlayerControllerB player) =>
        Setting.EnableAntiKick &&
        player is { IsServer: false, IsOwner: true } &&
        player == Helper.LocalPlayer &&
        Helper.GameNetworkManager is { disableSteam: false } &&
        SteamClient.IsValid &&
        SteamClient.SteamId.Value != 0;

    static void SetIdentity(PlayerControllerB player, ulong steamId, string name) {
        player.playerSteamId = steamId;
        player.playerUsername = name;
        player.usernameBillboardText.text = name;
        player.playersManager.Unfake()?.mapScreen.Unfake()?.ChangeNameOfTargetTransform(player.transform, name);
        player.quickMenuManager.AddUserToPlayerList(steamId, name, player.PlayerIndex());
    }
}
