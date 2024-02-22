using System.Text.RegularExpressions;
using UnityEngine;
using Steamworks;
using Steamworks.Data;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.JoinLobby))]
class LobbyDependencyPatch {
    static void Postfix(Lobby lobby, SteamId id) {
        if (State.DisconnectedVoluntarily && Regex.IsMatch(GUIUtility.systemCopyBuffer, @"^\d{17}$")) {
            GUIUtility.systemCopyBuffer = "";
        }

        State.ConnectedLobby = new ConnectedLobby() {
            Lobby = lobby,
            SteamId = id
        };

        State.DisconnectedVoluntarily = false;
    }
}
