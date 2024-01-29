using HarmonyLib;
using Steamworks;
using Hax;

[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.StartClient))]
class LobbyDependencyPatch {
    static void Postfix(SteamId id) {
        Setting.ConnectedLobbyId = id;
        Setting.DisconnectedVoluntarily = false;
    }
}
