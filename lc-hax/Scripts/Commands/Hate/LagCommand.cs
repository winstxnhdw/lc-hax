using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("lag")]
sealed class LagCommand : ICommand {
    static IEnumerator WaitForEnemyOwnershipChange(PlayerControllerB player, EnemyAI enemy) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (enemy.currentOwnershipOnThisClient != player.PlayerIndex()) {
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
        yield return LagCommand.WaitForEnemyOwnershipChange(localPlayer, bracken);

        bracken.SetMovingTowardsTargetPlayer(targetPlayer);
        bracken.SetBehaviourState(BehaviourState.AGGRAVATED);
        bracken.EnterAngerModeServerRpc(float.MaxValue);
        bracken.UpdateEnemyPositionRpc(targetPlayer.transform.position);

        yield return LagCommand.WaitForEnemyOwnershipChange(targetPlayer, bracken);
    }

    public Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer)
            return Task.CompletedTask;
        if (args.Length is 0) {
            Chat.Print("Usage: lag <player>");
            return Task.CompletedTask;
        }

        if (Helper.GetEnemy<FlowermanAI>() is not FlowermanAI bracken) {
            Chat.Print("A Bracken must have spawned to use this command!");
            return Task.CompletedTask;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not found!");
            return Task.CompletedTask;
        }

        if (targetPlayer.isInsideFactory) {
            Chat.Print("Target player must be outside of the factory!");
            return Task.CompletedTask;
        }

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => LagCommand.PassBrackenComputeToTargetPlayer(localPlayer, targetPlayer, bracken));
        return Task.CompletedTask;
    }
}
