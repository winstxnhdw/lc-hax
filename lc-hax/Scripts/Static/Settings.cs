using GameNetcodeStuff;
using Hax;

public static class Settings {
    public static bool EnableGodMode { get; set; } = false;
    public static bool EnableDemigodMode { get; set; } = false;
    public static bool EnableBlockCredits { get; set; } = false;
    public static bool EnableUntargetable { get; set; } = false;
    public static int ShovelHitForce { get; set; } = 1;
    public static bool EnableStunOnLeftClick { get; set; } = false;
    public static bool DisableFallDamage { get; set; } = false;
    public static PlayerControllerB? PlayerToFollow { get; set; } = null;
    public static bool PhantomEnabled { get; set; } = false;
    public static EnemyAI? PossessedEnemy { get; set; } = null;
    public static PossessionMod? PossessionMod { get; set; } = null;
    public static bool InvertYAxis => IngamePlayerSettings.Instance.settings.invertYAxis;
    public static bool RealisticPossessionEnabled { get; set; } = false;
}
