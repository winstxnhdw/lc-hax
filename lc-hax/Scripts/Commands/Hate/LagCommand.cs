using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("lag")]
class LagCommand : ICommand {
    static IEnumerator WaitForEnemyOwnershipChange(
        PlayerControllerB player,
        EnemyAI enemy,
        Reflector<FlowermanAI> enemyReflector
    ) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (enemyReflector.GetInternalField<int>("currentOwnershipOnThisClient") != player.PlayerIndex()) {
            if (Helper.StartOfRound is { inShipPhase: true }) yield break;

            enemy.ChangeEnemyOwnerServerRpc(player.actualClientId);
            yield return waitForEndOfFrame;
        }
    }

    static IEnumerator PassBrackenComputeToTargetPlayer(
        PlayerControllerB localPlayer,
        PlayerControllerB targetPlayer,
        FlowermanAI bracken
    ) {
        Reflector<FlowermanAI> enemyReflector = bracken.Reflect();

        yield return LagCommand.WaitForEnemyOwnershipChange(localPlayer, bracken, enemyReflector);

        bracken.SetMovingTowardsTargetPlayer(targetPlayer);
        bracken.SetBehaviourState(BehaviourState.AGGRAVATED);
        bracken.EnterAngerModeServerRpc(float.MaxValue);

        _ = enemyReflector.InvokeInternalMethod(
            "UpdateEnemyPositionServerRpc",
            targetPlayer.transform.position
        );

        yield return LagCommand.WaitForEnemyOwnershipChange(targetPlayer, bracken, enemyReflector);
    }

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
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
              .Init(() => LagCommand.PassBrackenComputeToTargetPlayer(localPlayer, targetPlayer, bracken));
    }
}
