using Steamworks;
using Steamworks.Data;

readonly struct ConnectedLobby(Lobby lobby, SteamId steamId) {
    internal Lobby Lobby { get; } = lobby;
    internal SteamId SteamId { get; } = steamId;
}
