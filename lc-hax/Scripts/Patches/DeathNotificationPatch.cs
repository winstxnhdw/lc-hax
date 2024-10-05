using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
class DeathNotificationPatch {
    static void Postfix(int playerId, int causeOfDeath) {
        if (Helper.GetPlayer(playerId) is not PlayerControllerB player) return;
        if (player.IsSelf()) return;

        Helper.SendNotification(
            title: player.playerUsername,
            body: $"Killed by {(CauseOfDeath)causeOfDeath}",
            isWarning: true
        );
    }
}
