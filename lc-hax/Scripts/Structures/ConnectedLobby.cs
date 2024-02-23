using Steamworks;
using Steamworks.Data;

readonly record struct ConnectedLobby {
    internal required Lobby Lobby { get; init; }
    internal required SteamId SteamId { get; init; }
}
