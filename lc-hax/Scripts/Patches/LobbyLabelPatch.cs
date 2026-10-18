using HarmonyLib;
using Steamworks.Data;
using ZLinq;

[HarmonyPatch(typeof(SteamLobbyManager), nameof(SteamLobbyManager.loadLobbyListAndFilter))]
sealed class LobbyLabelPatch {
    static void Prefix(ref Lobby[] ___currentLobbyList) {
        ___currentLobbyList
            .AsValueEnumerable()
            .Where(lobby => lobby.GetData("name")
            .Contains('\u200b'))
            .ForEach(lobby => lobby.SetData("name", $"[CC] {lobby.GetData("name")}"));
    }
}
