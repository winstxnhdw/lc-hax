using System.Linq;
using GameNetcodeStuff;

namespace Hax;

public class KillCommand : ICommand {
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
        Helper.Players.ToList().ForEach(player => player.KillPlayer());
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
