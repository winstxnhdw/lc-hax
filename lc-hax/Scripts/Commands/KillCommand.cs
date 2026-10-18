using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("kill")]
sealed class KillCommand : ICommand {
    bool EnableGodMode { get; set; }

    Result KillSelf() {
        Helper.LocalPlayer?.KillPlayer();
        return new Result { Success = true };
    }

    Result KillTargetPlayer(string[] args) {
        if (Helper.GetActivePlayer(args[0]) is not { } targetPlayer) {
            return new Result { Message = "Target player is not alive or found!" };
        }

        targetPlayer.KillPlayer();
        return new Result { Success = true };
    }

    Result KillAllPlayers() {
        Helper.Players?.ForEach(player => player.KillPlayer());
        return new Result { Success = true };
    }

    Result KillAllEnemies() {
        Helper.Enemies.ForEach(Helper.Kill);
        return new Result { Success = true };
    }

    void HandleResult(Result result) {
        if (result.Success) return;
        Chat.Print(result.Message);
    }

    Task ICommand.Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            this.HandleResult(this.KillSelf());
            return Task.CompletedTask;
        }

        this.EnableGodMode = Setting.EnableGodMode;
        Setting.EnableGodMode = false;

        Result result = args[0] switch {
            "--all" => this.KillAllPlayers(),
            "--enemy" => this.KillAllEnemies(),
            _ => this.KillTargetPlayer(args)
        };

        Helper.ShortDelay(() => Setting.EnableGodMode = this.EnableGodMode);
        this.HandleResult(result);
        return Task.CompletedTask;
    }
}
