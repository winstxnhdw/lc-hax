#pragma warning disable IDE1006

using HarmonyLib;

namespace Hax;

[HarmonyPatch(typeof(SteamLobbyManager), nameof(SteamLobbyManager.RefreshServerListButton))]
class NoRefreshDelayPatch {
    static void Prefix(ref float ___refreshServerListTimer) => ___refreshServerListTimer = 1.0f;
}
