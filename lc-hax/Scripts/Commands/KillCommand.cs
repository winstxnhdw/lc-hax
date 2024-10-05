using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("kill")]
class KillCommand : ICommand {
    bool EnableGodMode { get; set; }

    Result KillSelf() {
        Helper.LocalPlayer?.KillPlayer();
        return new Result(true);
    }

    Result KillTargetPlayer(string[] args) {
        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer) {
            return new Result { Message = "Target player is not alive or found!" };
        }

        targetPlayer.KillPlayer();
        return new Result(true);
    }

    Result KillAllPlayers() {
        Helper.Players?.ForEach(player => player.KillPlayer());
        return new Result(true);
    }

    Result KillAllEnemies() {
        Helper.Enemies.ForEach(Helper.Kill);
        return new Result(true);
    }

    void HandleResult(Result result) {
        if (result.Success) return;
        Chat.Print(result.Message);
    }

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            this.HandleResult(this.KillSelf());
            return;
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
    }
}
