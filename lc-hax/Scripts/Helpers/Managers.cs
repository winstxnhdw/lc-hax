namespace Hax;

public static partial class Helper {
    public static HUDManager? HUDManager => HUDManager.Instance.Unfake();

    public static RoundManager? RoundManager => RoundManager.Instance.Unfake();

    public static StartOfRound? StartOfRound => StartOfRound.Instance.Unfake();
    public static SoundManager? SoundManager => SoundManager.Instance.Unfake();

    public static Terminal? Terminal => Helper.HUDManager?.Reflect().GetInternalField<Terminal>("terminalScript").Unfake();
}
