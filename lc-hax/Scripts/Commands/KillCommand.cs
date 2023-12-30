using System;
using GameNetcodeStuff;
using UnityObject = UnityEngine.Object;

namespace Hax;

public class KillCommand : ICommand {
    void ForEachEnemy(Action<EnemyAI> action) =>
        UnityObject.FindObjectsOfType<EnemyAI>().ForEach(action);

    Result KillSelf() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer) || localPlayer.isPlayerDead) {
            return new Result(message: "Player not found!");
        }

        localPlayer.KillPlayer();
        return new Result(true);
    }

    Result KillTargetPlayer(string[] args) {
        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        targetPlayer.KillPlayer();
        return new Result(true);
    }

    Result KillAllPlayers() {
        Helper.Players?.ForEach(player => player.KillPlayer());
        return new Result(true);
    }

    Result KillAllLocalEnemies() {
        this.ForEachEnemy(enemy => enemy.gameObject.SetActive(false));
        return new Result(true);
    }

    Result KillAllEnemies() {
        this.ForEachEnemy(enemy => {
            enemy.HitEnemyServerRpc(1000, -1, false);

            if (!enemy.isEnemyDead) {
                enemy.KillEnemyServerRpc(true);
            }
        });

        return new Result(true);
    }

    void HandleResult(Result result) {
        if (result.Success) return;
        Console.Print(result.Message);
    }

    public void Execute(string[] args) {
        if (args.Length is 0) {
            this.HandleResult(this.KillSelf());
            return;
        }

        Result result = args[0] switch {
            "--all" => this.KillAllPlayers(),
            "--enemy" => this.KillAllEnemies(),
            "--localenemy" => this.KillAllLocalEnemies(),
            _ => this.KillTargetPlayer(args)
        };

        this.HandleResult(result);
    }
}
