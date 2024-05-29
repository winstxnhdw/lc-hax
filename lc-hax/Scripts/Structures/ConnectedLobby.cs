using Steamworks;
using Steamworks.Data;

internal readonly record struct ConnectedLobby
{
    internal required Lobby Lobby { get; init; }
    internal required SteamId SteamId { get; init; }
}