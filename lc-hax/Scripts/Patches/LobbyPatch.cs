#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(SteamLobbyManager), nameof(SteamLobbyManager.RefreshServerListButton))]
class LobbyPatch {
    static void Prefix(ref bool ___censorOffensiveLobbyNames, ref float ___refreshServerListTimer) {
        ___refreshServerListTimer = 1.0f;
        ___censorOffensiveLobbyNames = false;
    }
}
