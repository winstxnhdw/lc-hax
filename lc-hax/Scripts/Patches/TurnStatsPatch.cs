using GameNetcodeStuff;
using HarmonyLib;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.SetPlayerSanityLevel))]
sealed class TurnStatsPatch {
    static void Postfix(PlayerControllerB __instance) {
        if (!__instance.IsSelf()) return;

        __instance.playersManager.gameStats.allPlayerStats[__instance.playerClientId].turnAmount = 0;
    }
}
