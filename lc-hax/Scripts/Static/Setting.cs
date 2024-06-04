static class Setting {
    internal static bool EnableGodMode { get; set; } = false;
    internal static bool EnableBlockCredits { get; set; } = false;
    internal static bool EnableBlockRadar { get; set; } = false;
    internal static bool EnableUntargetable { get; set; } = false;
    internal static bool EnableStunOnLeftClick { get; set; } = false;
    internal static bool EnableKillOnLeftClick { get; set; } = false;
    internal static bool EnableInvisible { get; set; } = false;
    internal static bool EnableNoCooldown { get; set; } = false;
    internal static bool EnableAntiKick { get; set; } = false;
    internal static bool EnablePhantom { get; set; } = false;
    internal static bool EnableFakeDeath { get; set; } = false;
    internal static bool EnableEavesdrop { get; set; } = false;
    internal static bool EnableUnlimitedJump { get; set; } = true;
    internal static bool EnableUnlimitedGiftBox { get; set; } = false;

    internal static bool InvertYAxis => IngamePlayerSettings.Instance.settings.invertYAxis;
    internal static bool EnableNoSinking { get; set; } = true;
    internal static bool DisableSpiderWebSlowness { get; set; } = true;

    // speeds
    internal static float Default_MovementSpeed { get; set; } = 0.5f;
    internal static float Default_climbSpeed { get; set; } = 4f;
    internal static float Default_JumpForce { get; set; } = 5f;

    // flags to override the default speeds
    internal static bool OverrideMovementSpeed { get; set; } = false;
    internal static bool OverrideClimbSpeed { get; set; } = false;
    internal static bool OverrideJumpForce { get; set; } = false;


    internal static float New_MovementSpeed { get; set; } = Default_MovementSpeed;
    internal static float New_ClimbSpeed { get; set; } = Default_climbSpeed;
    internal static float New_JumpForce { get; set; } = Default_JumpForce;


    internal static bool isEditorMode = false;
}
