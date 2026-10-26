using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("fall")]
sealed class FallCommand : ICommand {
    IEnumerator WaitForEnemyOwnershipChange(PlayerControllerB player, EnemyAI enemy) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (enemy.currentOwnershipOnThisClient != player.PlayerIndex()) {
            if (Helper.StartOfRound is { inShipPhase: true }) yield break;

            enemy.ChangeEnemyOwnerServerRpc(player.actualClientId);
            yield return waitForEndOfFrame;
        }
    }

    IEnumerator MoveTargetPlayerFloor(PlayerControllerB localPlayer, PlayerControllerB targetPlayer, EnemyAI enemy) {
        yield return this.WaitForEnemyOwnershipChange(localPlayer, enemy);

        enemy.UpdateEnemyPositionRpc(
            enemy.serverPosition == Vector3.positiveInfinity
                ? targetPlayer.transform.position
                : Vector3.positiveInfinity
        );

        yield return this.WaitForEnemyOwnershipChange(targetPlayer, enemy);
    }

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not { } localPlayer) return Task.CompletedTask;
        if (args.Length is 0) {
            Chat.Print("Usage: fall <player>");
            return Task.CompletedTask;
        }

        if (Helper.Enemies.First() is not { } enemy) {
            Chat.Print("An enemy must have spawned to use this command!");
            return Task.CompletedTask;
        }

        if (Helper.GetPlayer(args[0]) is not { } targetPlayer) {
            Chat.Print("Target player is not found!");
            return Task.CompletedTask;
        }

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => this.MoveTargetPlayerFloor(localPlayer, targetPlayer, enemy));
        return Task.CompletedTask;
    }
}
