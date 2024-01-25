using GameNetcodeStuff;
using UnityEngine;
using Hax;

[Command("/kill")]
public class KillCommand : ICommand {
    Result KillSelf() {
        bool EnableGodMode = Setting.EnableGodMode;
        Setting.EnableGodMode = false;
        Helper.LocalPlayer?.KillPlayer();
        Setting.EnableGodMode = EnableGodMode;

        return new Result(true);
    }

    Result KillTargetPlayer(StringArray args) {
        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            return new Result(message: "Player not found!");
        }

        targetPlayer.KillPlayer();
        return new Result(true);
    }

    Result KillAllPlayers() {
        Helper.Players?.ForEach(player => player.KillPlayer());
        return new Result(true);
    }

    Result KillAllEnemies() {
        Helper.ActiveEnemies.ForEach(enemy => {
            if (Helper.LocalPlayer is PlayerControllerB localPlayer && enemy is NutcrackerEnemyAI nutcracker) {
                nutcracker.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
                nutcracker.DropGunServerRpc(Vector3.zero);
            }

            enemy.KillEnemyServerRpc(true);
        });

        return new Result(true);
    }

    void HandleResult(Result result) {
        if (result.Success) return;
        Chat.Print(result.Message);
    }

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            this.HandleResult(this.KillSelf());
            return;
        }

        Result result = args[0] switch {
            "--all" => this.KillAllPlayers(),
            "--enemy" => this.KillAllEnemies(),
            _ => this.KillTargetPlayer(args)
        };

        this.HandleResult(result);
    }
}
