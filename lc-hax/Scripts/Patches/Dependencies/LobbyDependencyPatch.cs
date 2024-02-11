using HarmonyLib;
using Hax;
using Steamworks;

[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.StartClient))]
class LobbyDependencyPatch {
    static void Postfix(SteamId id) {
        State.ConnectedLobbyId = id;
        State.DisconnectedVoluntarily = false;
    }
}
