
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("fall")]
class FallCommand : ICommand {
    public void Execute(StringArray args) {
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return;
        if (args.Length is 0) {
            Chat.Print("Usage: /fall <player>");
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

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        enemy.serverPosition = enemy.serverPosition == Vector3.positiveInfinity
            ? targetPlayer.transform.position
            : Vector3.positiveInfinity;

        Helper.ShortDelay(() =>
            enemy.ChangeEnemyOwnerServerRpc(targetPlayer.actualClientId)
        );
    }
}
