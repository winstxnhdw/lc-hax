using Steamworks;

namespace Hax;

public static class Setting {
    public static bool EnableGodMode { get; set; } = false;
    public static bool EnableBlockCredits { get; set; } = false;
    public static bool EnableBlockRadar { get; set; } = false;
    public static bool EnableUntargetable { get; set; } = false;
    public static bool EnableStunOnLeftClick { get; set; } = false;
    public static bool EnableHitOnLeftClick { get; set; } = false;
    public static bool EnableInvisible { get; set; } = false;
    public static bool EnableNoCooldown { get; set; } = false;
    public static bool EnableAntiKick { get; set; } = false;
    public static bool EnablePhantom { get; set; } = false;
    public static bool EnableFakeDeath { get; set; } = false;
    public static bool DisableFallDamage { get; set; } = false;
    public static int ShovelHitForce { get; set; } = 1;
    public static bool RealisticPossessionEnabled { get; set; } = false;
    public static bool InvertYAxis => IngamePlayerSettings.Instance.settings.invertYAxis;
    public static bool DisconnectedVoluntarily { get; set; } = false;
    public static SteamId? ConnectedLobbyId { get; set; } = null;
}
