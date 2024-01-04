using GameNetcodeStuff;

namespace Hax;

public static class Setting {
    public static bool EnableGodMode { get; set; } = false;
    public static bool EnableDemigodMode { get; set; } = false;
    public static bool EnableBlockCredits { get; set; } = false;
    public static bool EnableBlockRadar { get; set; } = false;
    public static bool EnableUntargetable { get; set; } = false;
    public static bool EnableStunOnLeftClick { get; set; } = false;
    public static bool EnableInvisible { get; set; } = false;
    public static bool EnableNoCooldown { get; set; } = false;
    public static bool EnableAntiKick { get; set; } = false;
    public static bool EnablePhantom { get; set; } = false;
    public static bool DisableFallDamage { get; set; } = false;
    public static PlayerControllerB? PlayerToFollow { get; set; } = null;
    public static int ShovelHitForce { get; set; } = 1;
    public static bool InvertYAxis => IngamePlayerSettings.Instance.settings.invertYAxis;
    public static bool RealisticPossessionEnabled { get; set; } = false;
}
