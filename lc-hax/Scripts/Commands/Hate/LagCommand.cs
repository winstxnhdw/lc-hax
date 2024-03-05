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

            enemy.ChangeEnemyOwnerServerRpc(player.actualClientId);
            yield return waitForEndOfFrame;
        }
    }

    IEnumerator PassBrackenComputeToTargetPlayer(
        PlayerControllerB localPlayer,
        PlayerControllerB targetPlayer,
        FlowermanAI enemy
    ) {
        Reflector<FlowermanAI> enemyReflector = enemy.Reflect();

        yield return this.WaitForEnemyOwnershipChange(localPlayer, enemy, enemyReflector);

        Vector3 outsideFactory = new(0.0f, -50.0f, 0.0f);

        _ = enemyReflector.InvokeInternalMethod(
            "UpdateEnemyPositionServerRpc",
            enemy.serverPosition == outsideFactory
                ? targetPlayer.transform.position
                : outsideFactory
        );

        yield return this.WaitForEnemyOwnershipChange(targetPlayer, enemy, enemyReflector);
    }

    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: /lag <player>");
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

        Helper.CreateComponent<AsyncBehaviour>()
              .Init(() => this.PassBrackenComputeToTargetPlayer(localPlayer, targetPlayer, bracken));
    }
}
