using GameNetcodeStuff;
using Hax;

[Command("kill")]
internal class KillCommand : ICommand
{
    private bool EnableGodMode { get; set; }

    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            HandleResult(KillSelf());
            return;
        }

        EnableGodMode = Setting.EnableGodMode;
        Setting.EnableGodMode = false;

        var result = args[0] switch
        {
            "--all" => KillAllPlayers(),
            "--enemy" => KillAllEnemies(),
            _ => KillTargetPlayer(args)
        };

        Helper.ShortDelay(() => Setting.EnableGodMode = EnableGodMode);
        HandleResult(result);
    }

    private Result KillSelf()
    {
        var enableGodMode = Setting.EnableGodMode;
        Setting.EnableGodMode = false;
        Helper.LocalPlayer?.KillPlayer();

        Helper.Delay(0.5f, () =>
            Setting.EnableGodMode = enableGodMode
        );

        return new Result(true);
    }

    private Result KillTargetPlayer(StringArray args)
    {
        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB targetPlayer)
            return new Result(message: "Target player is not alive or found!");

        targetPlayer.KillPlayer();
        return new Result(true);
    }

    private Result KillAllPlayers()
    {
        Helper.Players?.ForEach(player => player.KillPlayer());
        return new Result(true);
    }

    private Result KillAllEnemies()
    {
        Helper.Enemies.ForEach(Helper.Kill);
        return new Result(true);
    }

    private void HandleResult(Result result)
    {
        if (result.Success) return;
        Chat.Print(result.Message);
    }
}