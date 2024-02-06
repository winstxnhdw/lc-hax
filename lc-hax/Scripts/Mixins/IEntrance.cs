using GameNetcodeStuff;
using Hax;

internal interface IEntrance { }

internal static class IEntranceMixin {
    internal static void EntranceTeleport(this IEntrance _, bool outside) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;

        player.TeleportPlayer(RoundManager.FindMainEntranceScript(outside).entrancePoint.position);
        player.isInsideFactory = !outside;
    }
}
