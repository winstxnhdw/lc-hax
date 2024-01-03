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
        while (endOfGame.MoveNext()) {
            yield return endOfGame.Current;
        }

        yield return new WaitForSeconds(5.0f); // Wait a bit to give it a chance to fix itself

        bool leverBroken = true;

        while (leverBroken) {
            yield return new WaitForSeconds(Random.Range(1.0f, 5.0f)); // Make users not send it all at once

            leverBroken = (Helper.StartOfRound?.inShipPhase ?? true) && !(Object.FindObjectOfType<StartMatchLever>()?.triggerScript.interactable ?? true);

            if (leverBroken) Helper.StartOfRound?.PlayerHasRevivedServerRpc();
        }
    }
}
