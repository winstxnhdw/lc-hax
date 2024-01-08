using GameNetcodeStuff;
using Hax;

[Command("/fatality")]
public class FatalityCommand : ICommand {
    T? GetEnemy<T>() where T : EnemyAI {
        if (Helper.FindObject<T>() is not T enemy) return null;
        if (Helper.LocalPlayer is not PlayerControllerB localPlayer) return null;

        enemy.ChangeEnemyOwnerServerRpc(localPlayer.actualClientId);
        return enemy;
    }

    Result HandleGiant(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<ForestGiantAI>() is not ForestGiantAI forestGiant) {
            return new Result(message: "Enemy has not yet spawned!");
        }

        forestGiant.GrabPlayerServerRpc((int)targetPlayer.playerClientId);
        return new Result(true);
    }

    Result HandleJester(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<JesterAI>() is not JesterAI jester) {
            return new Result(message: "Enemy has not yet spawned!");
        }

        jester.KillPlayerServerRpc((int)targetPlayer.playerClientId);
        return new Result(true);
    }

    Result HandleMask(PlayerControllerB targetPlayer) {
        if (this.GetEnemy<MaskedPlayerEnemy>() is not MaskedPlayerEnemy spider) {
            return new Result(message: "Enemy has not yet spawned!");
        }

        spider.KillPlayerAnimationServerRpc((int)targetPlayer.playerClientId);
        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length < 2) {
            Chat.Print("Usage: /fatality <player> <enemy>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            Chat.Print($"Unable to find player: {args[0]}!");
            return;
        }

        Result result = args[1].ToLower() switch {
            "giant" => this.HandleGiant(targetPlayer),
            "jester" => this.HandleJester(targetPlayer),
            "mask" => this.HandleMask(targetPlayer),
            _ => new Result(message: "Enemy has either not yet been implemented or does not exist!")
        };

        if (!result.Success) {
            Chat.Print(result.Message);
        }
    }
}
