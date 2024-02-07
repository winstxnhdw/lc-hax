using System.Collections;
using UnityEngine;
using HarmonyLib;
using Hax;

// Sometimes, especially in bigger lobbies, the ship stays forever deadlocked on "Wait for ship to land" because the server failed to receive a
// PlayerHasRevivedServerRpc from one of the players at end of round. This patch sends another RPC a few seconds after round end, which increments
// the playersRevived property, satisfying the server's "playersRevived >= GameNetworkManager.Instance.connectedPlayers" WaitUntil condition.

[HarmonyPatch(typeof(StartOfRound), "EndOfGame")]
class WaitForShipPatch {
    static IEnumerator Postfix(IEnumerator endOfGame) {
        if (Helper.StartOfRound is not StartOfRound startOfRound) yield break;

        while (endOfGame.MoveNext()) {
            yield return endOfGame.Current;
        }

        yield return new WaitUntil(() => startOfRound.shipIsLeaving is false);
        yield return new WaitForSeconds(5.0f); // Wait a bit to give it a chance to fix itself
        bool isLeverBroken = true;

        while (isLeverBroken) {
            yield return new WaitForSeconds(Random.Range(2.0f, 5.0f)); // Make lc-hax users not send it all at the same time

            if (Helper.StartOfRound?.travellingToNewLevel is true) {
                continue;
            }

            isLeverBroken =
                startOfRound.inShipPhase is true &&
                Helper.FindObject<StartMatchLever>()?.triggerScript.interactable is false;

            if (isLeverBroken) {
                startOfRound.PlayerHasRevivedServerRpc();
            }
        }
    }
}
