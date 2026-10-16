using System.Collections;
using HarmonyLib;
using UnityEngine;


[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.EndOfGame))]
sealed class WaitForShipFixPatch {
    static void Postfix(StartOfRound __instance) => __instance.StartCoroutine(WaitForShipFixPatch.RecoverShip(__instance));

    static bool IsWaitingForPlayers(StartOfRound startOfRound) =>
        !startOfRound.inShipPhase && !startOfRound.shipHasLanded &&
        !startOfRound.shipIsLeaving && !startOfRound.beganLoadingNewLevel &&
        !startOfRound.travellingToNewLevel && !startOfRound.firingPlayersCutsceneRunning;

    static IEnumerator RecoverShip(StartOfRound startOfRound) {
        yield return new WaitUntil(() => !startOfRound.shipIsLeaving);
        yield return new WaitForSeconds(5.0f);
        int attemptsRemaining = startOfRound.connectedPlayersAmount + 1;

        while (attemptsRemaining-- > 0 && WaitForShipFixPatch.IsWaitingForPlayers(startOfRound)) {
            yield return new WaitForSeconds(Random.Range(2.0f, 5.0f));
            if (!WaitForShipFixPatch.IsWaitingForPlayers(startOfRound)) yield break;

            startOfRound.PlayerHasRevivedServerRpc();
        }
    }
}
