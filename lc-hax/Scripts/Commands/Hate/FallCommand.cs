using System.Collections;
using UnityEngine;
using GameNetcodeStuff;
using Hax;

[Command("fall")]
class FallCommand : ICommand {
    IEnumerator WaitForEnemyOwnershipChange(PlayerControllerB player, EnemyAI enemy, Reflector<EnemyAI> enemyReflector) {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (enemyReflector.GetInternalField<int>("currentOwnershipOnThisClient") != player.PlayerIndex()) {
            if (Helper.StartOfRound is { inShipPhase: true }) yield break;

            enemy.ChangeEnemyOwnerServerRpc(player.actualClientId);
            yield return waitForEndOfFrame;
        }
    }

    IEnumerator MoveTargetPlayerFloor(PlayerControllerB localPlayer, PlayerControllerB targetPlayer, EnemyAI enemy) {
        Reflector<EnemyAI> enemyReflector = enemy.Reflect();

        yield return this.WaitForEnemyOwnershipChange(localPlayer, enemy, enemyReflector);

        _ = enemyReflector.InvokeInternalMethod(
            "UpdateEnemyPositionServerRpc",
            enemy.serverPosition == Vector3.positiveInfinity
                ? targetPlayer.transform.position
                : Vector3.positiveInfinity
        );

        yield return this.WaitForEnemyOwnershipChange(targetPlayer, enemy, enemyReflector);
    }

    public void Execute(StringArray args) {
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
              .Init(() => this.MoveTargetPlayerFloor(localPlayer, targetPlayer, enemy));
    }
}
