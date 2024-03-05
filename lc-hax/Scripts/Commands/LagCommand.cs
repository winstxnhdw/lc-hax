using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("lag")]
class LagCommand : ICommand {
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

        Vector3 outsideFactory = new(0.0f, -50.0f, 0.0f);
        bracken.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        bracken.transform.position = bracken.transform.position == outsideFactory
            ? targetPlayer.transform.position
            : outsideFactory;

        Helper.ShortDelay(() =>
            bracken.ChangeEnemyOwnerServerRpc(targetPlayer.actualClientId)
        );
    }
}
