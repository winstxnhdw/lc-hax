namespace Hax;

internal static class Setting {
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
    internal static bool BuildOverlapMode { get; set; } = false;

    internal static bool InvertYAxis => IngamePlayerSettings.Instance.settings.invertYAxis;
}
