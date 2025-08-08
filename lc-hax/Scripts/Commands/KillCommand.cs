using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("kill")]
sealed class KillCommand : ICommand {
    bool EnableGodMode { get; set; }

    static Result KillSelf() {
        Helper.LocalPlayer?.KillPlayer();
        return new Result { Success = true };
    }

    static Result KillTargetPlayer(string[] args) {
        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            return new Result { Message = "Target player is not alive or found!" };
        }

        targetPlayer.KillPlayer();
        return new Result { Success = true };
    }

    static Result KillAllPlayers() {
        Helper.Players?.ForEach(player => player.KillPlayer());
        return new Result { Success = true };
    }

    static Result KillAllEnemies() {
        Helper.Enemies.ForEach(Helper.Kill);
        return new Result { Success = true };
    }

    static void HandleResult(Result result) {
        if (result.Success) return;
        Chat.Print(result.Message);
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            KillCommand.HandleResult(KillSelf());
            return;
        }

        this.EnableGodMode = Setting.EnableGodMode;
        Setting.EnableGodMode = false;

        Result result = args[0] switch {
            "--all" => KillCommand.KillAllPlayers(),
            "--enemy" => KillCommand.KillAllEnemies(),
            _ => KillCommand.KillTargetPlayer(args)
        };

        Helper.ShortDelay(() => Setting.EnableGodMode = this.EnableGodMode);
        KillCommand.HandleResult(result);
    }
}
