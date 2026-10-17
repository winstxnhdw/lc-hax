using System.Text.RegularExpressions;
using GameNetcodeStuff;
using HarmonyLib;
using Steamworks.Data;
using UnityEngine;

[HarmonyPatch]
sealed class LobbyDependencyPatch {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.JoinLobby))]
    static void JoinLobbyPostfix() {
        if (State.DisconnectedVoluntarily && Regex.IsMatch(GUIUtility.systemCopyBuffer, @"^\d{17}$")) {
            GUIUtility.systemCopyBuffer = "";
        }

        State.DisconnectedVoluntarily = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    static void PlayerConnectPostfix(PlayerControllerB __instance) {
        if (ReferenceEquals(__instance, Helper.LocalPlayer)) LobbyDependencyPatch.RecordConnectedLobby();
    }

    internal static void RecordConnectedLobby() {
        State.DisconnectedVoluntarily = false;
        State.ConnectedLobby = Helper.GameNetworkManager is { disableSteam: false, isHostingGame: false, currentLobby: Lobby lobby }
            ? new ConnectedLobby { Lobby = lobby, SteamId = lobby.Id }
            : null;
    }
}
