using System.Text.RegularExpressions;
using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.JoinLobby))]
internal class LobbyDependencyPatch
{
    private static void Postfix(Lobby lobby, SteamId id)
    {
        if (State.DisconnectedVoluntarily && Regex.IsMatch(GUIUtility.systemCopyBuffer, @"^\d{17}$"))
            GUIUtility.systemCopyBuffer = "";

        State.ConnectedLobby = new ConnectedLobby()
        {
            Lobby = lobby,
            SteamId = id
        };

        State.DisconnectedVoluntarily = false;
    }
}