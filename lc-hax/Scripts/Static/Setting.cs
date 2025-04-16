static class Setting {
    internal static bool EnableGodMode { get; set; }
    internal static bool EnableBlockCredits { get; set; }
    internal static bool EnableBlockRadar { get; set; }
    internal static bool EnableUntargetable { get; set; }
    internal static bool EnableStunOnLeftClick { get; set; }
    internal static bool EnableKillOnLeftClick { get; set; }
    internal static bool EnableInvisible { get; set; }
    internal static bool EnableNoCooldown { get; set; }
    internal static bool EnableAntiKick { get; set; }
    internal static bool EnablePhantom { get; set; }
    internal static bool EnableFakeDeath { get; set; }
    internal static bool EnableEavesdrop { get; set; }
    internal static bool EnableUnlimitedJump { get; set; }
    internal static bool InvertYAxis => IngamePlayerSettings.Instance.settings.invertYAxis;
}
