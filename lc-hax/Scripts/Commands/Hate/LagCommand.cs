using System.Collections;
using UnityEngine;
using GameNetcodeStuff;
using Hax;

[Command("lag")]
class LagCommand : ICommand {
    IEnumerator WaitForEnemyOwnershipChange(
        PlayerControllerB player,
        EnemyAI enemy,
        Reflector<FlowermanAI> enemyReflector
    ) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (enemyReflector.GetInternalField<int>("currentOwnershipOnThisClient") != player.PlayerIndex()) {
            if (Helper.StartOfRound is { inShipPhase: true }) yield break;

            enemy.SetOwner(player);
            yield return waitForEndOfFrame;
        }
    }

    IEnumerator PassBrackenComputeToTargetPlayer(
        PlayerControllerB localPlayer,
        PlayerControllerB targetPlayer,
        FlowermanAI bracken
    ) {
        Reflector<FlowermanAI> enemyReflector = bracken.Reflect();

        yield return this.WaitForEnemyOwnershipChange(localPlayer, bracken, enemyReflector);

        bracken.SetMovingTowardsTargetPlayer(targetPlayer);
        bracken.SetBehaviourState(BehaviourState.AGGRAVATED);
        bracken.EnterAngerModeServerRpc(float.MaxValue);

        _ = enemyReflector.InvokeInternalMethod(
            "UpdateEnemyPositionServerRpc",
            targetPlayer.transform.position
        );

        yield return this.WaitForEnemyOwnershipChange(targetPlayer, bracken, enemyReflector);
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: lag <player>");
            return;
        }

        if (Helper.GetEnemy<FlowermanAI>() is not FlowermanAI bracken) {
            Chat.Print("A Bracken must have spawned to use this command!");
            return;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not found!");
            return;
        }

        if (targetPlayer.isInsideFactory) {
            Chat.Print("Target player must be outside of the factory!");
            return;
        }

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => this.PassBrackenComputeToTargetPlayer(localPlayer, targetPlayer, bracken));
    }
}
