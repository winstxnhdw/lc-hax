using HarmonyLib;
using Steamworks.Data;

[HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter")]
internal class ControlCompanyLobbyPatch {
    private static void Prefix(ref Lobby[] ___currentLobbyList) {
        foreach (var lobby in ___currentLobbyList) {
            string data = lobby.GetData("name");
            if (data.Contains('​')) {
                _ = lobby.SetData("name", $"[Control Company] {data}");
            }
        }
    }
}
