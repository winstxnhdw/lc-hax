using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class KillCommand : ICommand {
    void KillPlayer(PlayerControllerB player) =>
        player.DamagePlayerFromOtherClientServerRpc(1000, Vector3.zero, -1);

    Result KillSelf() {
        if (!Helper.LocalPlayer.IsNotNull(out PlayerControllerB localPlayer) || localPlayer.isPlayerDead) {
            return new Result(message: "Player not found!");
        }

        this.KillPlayer(localPlayer);
        return new Result(true);
    }

    Result KillTargetPlayer(string[] args) {
        if (!Helper.GetActivePlayer(args[0]).IsNotNull(out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        this.KillPlayer(targetPlayer);
        return new Result(true);
    }

    Result KillAllPlayers() {
        Helper.Players.ToList().ForEach(this.KillPlayer);
        return new Result(true);
    }

    public void Execute(string[] args) {
        Result result = args.Length is 0
                      ? this.KillSelf()
                      : args[0] is "--all"
                      ? this.KillAllPlayers()
                      : this.KillTargetPlayer(args);

        if (!result.Success) {
            Console.Print(result.Message);
        }
    }
}
