using GameNetcodeStuff;

public static class Settings {
    public static bool EnableGodMode { get; set; } = false;
    public static int ShovelHitForce { get; set; } = 1;
    public static bool EnableStunOnLeftClick { get; set; } = false;
    public static bool DisableFallDamage { get; set; } = false;
    public static PlayerControllerB? PlayerToFollow { get; set; } = null;
}
