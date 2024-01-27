#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(SteamLobbyManager), nameof(SteamLobbyManager.RefreshServerListButton))]
class LobbyPatch {
    static void Prefix(SteamLobbyManager __instance, ref float ___refreshServerListTimer) {
        ___refreshServerListTimer = 1.0f;
        __instance.censorOffensiveLobbyNames = false;
    }
}
