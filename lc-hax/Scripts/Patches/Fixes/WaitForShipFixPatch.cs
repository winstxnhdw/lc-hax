using System.Collections;
using HarmonyLib;
using Hax;
using UnityEngine;

// Sometimes, especially in bigger lobbies, the ship stays forever deadlocked on "Wait for ship to land" because the server failed to receive a
// PlayerHasRevivedServerRpc from one of the players at end of round. This patch sends another RPC a few seconds after round end, which increments
// the playersRevived property, satisfying the server's "playersRevived >= GameNetworkManager.Instance.connectedPlayers" WaitUntil condition.

[HarmonyPatch(typeof(StartOfRound), "EndOfGame")]
internal class WaitForShipFixPatch
{
    private static IEnumerator Postfix(IEnumerator endOfGame)
    {
        if (Helper.StartOfRound is not StartOfRound startOfRound) yield break;
        if (Helper.FindObject<StartMatchLever>() is not StartMatchLever startMatchLever) yield break;

        while (endOfGame.MoveNext()) yield return endOfGame.Current;

        yield return new WaitUntil(() => !startOfRound.shipIsLeaving);
        yield return new WaitForSeconds(5.0f); // Wait a bit to give it a chance to fix itself
        var isLeverBroken = true;

        while (isLeverBroken)
        {
            yield return
                new WaitForSeconds(Random.Range(2.0f, 5.0f)); // Make lc-hax users not send it all at the same time

            if (startOfRound.travellingToNewLevel) continue;

            if (startOfRound.inShipPhase && !startMatchLever.triggerScript.interactable)
                startOfRound.PlayerHasRevivedServerRpc();
        }
    }
}