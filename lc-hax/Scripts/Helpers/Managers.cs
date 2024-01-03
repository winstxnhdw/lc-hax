namespace Hax;

public static partial class Helper {
    public static HUDManager? HUDManager => HUDManager.Instance.Unfake();

    public static RoundManager? RoundManager => RoundManager.Instance.Unfake();

    public static StartOfRound? StartOfRound => StartOfRound.Instance.Unfake();
}
