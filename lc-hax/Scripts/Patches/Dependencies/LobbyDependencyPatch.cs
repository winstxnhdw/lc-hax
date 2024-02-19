using HarmonyLib;
using Steamworks;
using Hax;
using Steamworks.Data;

[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.JoinLobby))]
class LobbyDependencyPatch {
    static void Postfix(Lobby lobby, SteamId id) {
        State.ConnectedLobby = new ConnectedLobby(lobby, id);
        State.DisconnectedVoluntarily = false;
    }
}
