using HarmonyLib;

[HarmonyPatch(typeof(SteamLobbyManager), nameof(SteamLobbyManager.RefreshServerListButton))]
sealed class LobbyCensorshipPatch {
    static void Prefix(SteamLobbyManager __instance) => __instance.censorOffensiveLobbyNames = false;
}
