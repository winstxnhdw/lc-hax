namespace Hax;

internal static partial class Helper {
    internal static HUDManager? HUDManager => HUDManager.Instance.Unfake();

    internal static RoundManager? RoundManager => RoundManager.Instance.Unfake();

    internal static StartOfRound? StartOfRound => StartOfRound.Instance.Unfake();

    internal static SoundManager? SoundManager => SoundManager.Instance.Unfake();

    internal static Terminal? Terminal => Helper.HUDManager?.Reflect().GetInternalField<Terminal>("terminalScript").Unfake();
}
