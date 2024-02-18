#pragma warning disable IDE1006

using System.Linq;
using HarmonyLib;
using Steamworks.Data;

[HarmonyPatch(typeof(SteamLobbyManager))]
class LobbyPatch {
    [HarmonyPatch(nameof(SteamLobbyManager.RefreshServerListButton))]
    static void Prefix(SteamLobbyManager __instance, ref float ___refreshServerListTimer) {
        ___refreshServerListTimer = 1.0f;
        __instance.censorOffensiveLobbyNames = false;
    }

    [HarmonyPatch("loadLobbyListAndFilter")]
    static void Prefix(ref Lobby[] ___currentLobbyList) {
        ___currentLobbyList.Where(lobby => lobby.GetData("name").Contains('\u200b')).ForEach(lobby =>
            lobby.SetData("name", $"[CC] {lobby.GetData("name")}")
        );
    }
}
