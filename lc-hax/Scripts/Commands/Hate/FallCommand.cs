using System.Collections;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("fall")]
internal class FallCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0)
        {
            Chat.Print("Usage: fall <player>");
            return;
        }

        if (Helper.Enemies.First() is not EnemyAI enemy)
        {
            Chat.Print("An enemy must have spawned to use this command!");
            return;
        }

        if (Helper.GetPlayer(args[0]) is not PlayerControllerB targetPlayer)
        {
            Chat.Print("Target player is not found!");
            return;
        }

        Helper.CreateComponent<AsyncBehaviour>()
            .Init(() => MoveTargetPlayerFloor(localPlayer, targetPlayer, enemy));
    }

    private IEnumerator WaitForEnemyOwnershipChange(PlayerControllerB player, EnemyAI enemy,
        Reflector<EnemyAI> enemyReflector)
    {
        WaitForEndOfFrame waitForEndOfFrame = new();

        while (enemyReflector.GetInternalField<int>("currentOwnershipOnThisClient") != player.GetPlayerID())
        {
            if (Helper.StartOfRound is { inShipPhase: true }) yield break;

            enemy.SetOwner(player);
            yield return waitForEndOfFrame;
        }
    }

    private IEnumerator MoveTargetPlayerFloor(PlayerControllerB localPlayer, PlayerControllerB targetPlayer,
        EnemyAI enemy)
    {
        var enemyReflector = enemy.Reflect();

        yield return WaitForEnemyOwnershipChange(localPlayer, enemy, enemyReflector);

        _ = enemyReflector.InvokeInternalMethod(
            "UpdateEnemyPositionServerRpc",
            enemy.serverPosition == Vector3.positiveInfinity
                ? targetPlayer.transform.position
                : Vector3.positiveInfinity
        );

        yield return WaitForEnemyOwnershipChange(targetPlayer, enemy, enemyReflector);
    }
}