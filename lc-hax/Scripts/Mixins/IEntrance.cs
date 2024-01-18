using GameNetcodeStuff;
using Hax;

public interface IEntrance { }

public static class IEntranceMixin {
    public static void EntranceTeleport(this IEntrance _, bool outside) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        player.TeleportPlayer(RoundManager.FindMainEntranceScript(outside).entrancePoint.position);
        player.isInsideFactory = !outside;
    }
}
