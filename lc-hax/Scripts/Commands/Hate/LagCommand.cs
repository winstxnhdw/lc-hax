using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("lag")]
internal class LagCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0)
        {
            Chat.Print("Usage: lag <player>");
            return;
        }

        if (Helper.GetEnemy<FlowermanAI>() is not FlowermanAI bracken)
        {
            Chat.Print("A Bracken must have spawned to use this command!");
            return;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB targetPlayer)
        {
            Chat.Print("Target player is not found!");
            return;
        }

        if (targetPlayer.isInsideFactory)
        {
            Chat.Print("Target player must be outside of the factory!");
            return;
        }

        Helper.CreateComponent<AsyncBehaviour>()
            .Init(() => PassBrackenComputeToTargetPlayer(localPlayer, targetPlayer, bracken));
    }

    private IEnumerator WaitForEnemyOwnershipChange(
        PlayerControllerB player,
        EnemyAI enemy,
        Reflector<FlowermanAI> enemyReflector
    )
    {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (enemyReflector.GetInternalField<int>("currentOwnershipOnThisClient") != player.GetPlayerID())
        {
            if (Helper.StartOfRound is { inShipPhase: true }) yield break;

            enemy.SetOwner(player);
            yield return waitForEndOfFrame;
        }
    }

    private IEnumerator PassBrackenComputeToTargetPlayer(
        PlayerControllerB localPlayer,
        PlayerControllerB targetPlayer,
        FlowermanAI bracken
    )
    {
        var enemyReflector = bracken.Reflect();

        yield return WaitForEnemyOwnershipChange(localPlayer, bracken, enemyReflector);

        bracken.SetMovingTowardsTargetPlayer(targetPlayer);
        bracken.SetBehaviourState(BehaviourState.AGGRAVATED);
        bracken.EnterAngerModeServerRpc(float.MaxValue);

        _ = enemyReflector.InvokeInternalMethod(
            "UpdateEnemyPositionServerRpc",
            targetPlayer.transform.position
        );

        yield return WaitForEnemyOwnershipChange(targetPlayer, bracken, enemyReflector);
    }
}