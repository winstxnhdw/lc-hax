namespace Hax;

public interface IEntrance { }

public static class IEntranceMixin {
    public static void EntranceTeleport(this IEntrance _, bool outside) {
        Helper.LocalPlayer?.TeleportPlayer(RoundManager.FindMainEntranceScript(outside).entrancePoint.position);
    }
}
