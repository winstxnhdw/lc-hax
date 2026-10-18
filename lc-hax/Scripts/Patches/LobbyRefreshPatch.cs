using HarmonyLib;

[HarmonyPatch(typeof(SteamLobbyManager), nameof(SteamLobbyManager.RefreshServerListButton))]
sealed class LobbyRefreshPatch {
    static void Prefix(ref float ___refreshServerListTimer) => ___refreshServerListTimer = 1.0f;
}
