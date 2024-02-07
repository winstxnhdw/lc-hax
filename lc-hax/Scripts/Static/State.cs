using Steamworks;

namespace Hax;

internal static class State {
    internal static int ShovelHitForce { get; set; } = 1;
    internal static bool DisconnectedVoluntarily { get; set; } = false;
    internal static SteamId? ConnectedLobbyId { get; set; } = null;
}
