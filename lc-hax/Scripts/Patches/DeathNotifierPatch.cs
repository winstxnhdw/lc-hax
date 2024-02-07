using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
class DeathNotifierPatch {
    static void Postfix(int playerId, int causeOfDeath) {
        if (!Setting.EnablePlayerDeathNotifications) return;
        if (Helper.GetPlayer((ulong)playerId) is not PlayerControllerB player) return;
        if (player.IsSelf()) return;
        Helper.DisplayNotification($"{player.playerUsername} ",
            $"{player.playerUsername} was killed by {(CauseOfDeath)causeOfDeath}", true);
    }
}
