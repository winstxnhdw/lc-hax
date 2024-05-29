using GameNetcodeStuff;
using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
internal class DeathNotificationPatch
{
    private static void Postfix(int playerId, int causeOfDeath)
    {
        if (Helper.GetPlayer(playerId) is not PlayerControllerB player) return;
        if (player.IsSelf()) return;

        Helper.DisplayFlatHudMessage($"{player.playerUsername} was killed by {(CauseOfDeath)causeOfDeath}");
    }
}