#pragma warning disable IDE1006

using HarmonyLib;

[HarmonyPatch(typeof(SteamLobbyManager), nameof(SteamLobbyManager.RefreshServerListButton))]
class NoRefreshDelayPatch {
    static void Prefix(ref float ___refreshServerListTimer) => ___refreshServerListTimer = 1.0f;
}
