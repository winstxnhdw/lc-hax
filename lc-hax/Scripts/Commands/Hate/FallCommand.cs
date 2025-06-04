using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("fall")]
class FallCommand : ICommand {
    static IEnumerator WaitForEnemyOwnershipChange(PlayerControllerB player, EnemyAI enemy) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (enemy.currentOwnershipOnThisClient != player.PlayerIndex()) {
            if (Helper.StartOfRound is { inShipPhase: true }) yield break;

            enemy.ChangeEnemyOwnerServerRpc(player.actualClientId);
            yield return waitForEndOfFrame;
        }
    }

    static IEnumerator MoveTargetPlayerFloor(PlayerControllerB localPlayer, PlayerControllerB targetPlayer, EnemyAI enemy) {
        yield return FallCommand.WaitForEnemyOwnershipChange(localPlayer, enemy);

        enemy.UpdateEnemyPositionServerRpc(
            enemy.serverPosition == Vector3.positiveInfinity
                ? targetPlayer.transform.position
                : Vector3.positiveInfinity
        );

        yield return FallCommand.WaitForEnemyOwnershipChange(targetPlayer, enemy);
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: fall <player>");
            return;
        }

        if (Helper.Enemies.First() is not EnemyAI enemy) {
            Chat.Print("An enemy must have spawned to use this command!");
            return;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print("Target player is not found!");
            return;
        }

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => FallCommand.MoveTargetPlayerFloor(localPlayer, targetPlayer, enemy));
    }
}
