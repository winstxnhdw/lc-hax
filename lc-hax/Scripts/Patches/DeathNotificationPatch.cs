using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayerClientRpc))]
sealed class DeathNotificationPatch {
    static void Postfix(int playerId, int causeOfDeath) {
        if (Helper.GetPlayer(playerId) is not { } player) return;
        if (player.IsSelf()) return;

        Helper.SendNotification(
            title: player.playerUsername,
            body: $"Killed by {(CauseOfDeath)causeOfDeath}",
            isWarning: true
        );
    }
}
