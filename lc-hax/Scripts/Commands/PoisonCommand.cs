using GameNetcodeStuff;
using Hax;

[Command("poison")]
internal class PoisonCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length < 4)
        {
            Chat.Print("Usages:",
                "poison <player> <damage> <duration> <delay=1>",
                "poison --all <damage> <duration> <delay=1>"
            );

            return;
        }

        if (!int.TryParse(args[1], out var damage))
        {
            Chat.Print("Invalid damage!");
            return;
        }

        if (!ulong.TryParse(args[2], out var duration))
        {
            Chat.Print("Invalid duration!");
            return;
        }

        if (!args[3].TryParse(1, out ulong delay))
        {
            Chat.Print("Invalid delay!");
            return;
        }

        if (args[0] is "--all")
            Helper.ActivePlayers.ForEach(player => PoisonPlayer(player, damage, delay, duration));

        else if (Helper.GetActivePlayer(args[0]) is PlayerControllerB player)
            PoisonPlayer(player, damage, delay, duration);

        else
            Chat.Print("Target player is not alive or found!");
    }

    private void PoisonPlayer(PlayerControllerB player, int damage, ulong delay, ulong duration)
    {
        Helper.CreateComponent<TransientBehaviour>()
            .Init(_ => player.DamagePlayerRpc(damage), duration, delay)
            .Unless(() => player.playersManager.inShipPhase);
    }
}