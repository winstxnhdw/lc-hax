using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;

[Command("poison")]
class PoisonCommand : ICommand {
    static void PoisonPlayer(PlayerControllerB player, int damage, ulong delay, ulong duration) =>
        Helper.CreateComponent<TransientBehaviour>()
              .Init(_ => player.DamagePlayerRpc(damage), duration, delay)
              .Unless(() => player.playersManager.inShipPhase);

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length < 4) {
            Chat.Print("Usages:",
                "poison <player> <damage> <duration> <delay=1>",
                "poison --all <damage> <duration> <delay=1>"
            );

            return;
        }

        if (!int.TryParse(args[1], out int damage)) {
            Chat.Print("Invalid damage!");
            return;
        }

        if (!ulong.TryParse(args[2], out ulong duration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        if (!args[3].TryParse(defaultValue: 1, result: out ulong delay)) {
            Chat.Print("Invalid delay!");
            return;
        }

        if (args[0] is "--all") {
            Helper.ActivePlayers.ForEach(player => PoisonCommand.PoisonPlayer(player, damage, delay, duration));
        }

        else if (Helper.GetActivePlayer(args[0]) is PlayerControllerB player) {
            PoisonCommand.PoisonPlayer(player, damage, delay, duration);
        }

        else {
            Chat.Print("Target player is not alive or found!");
        }
    }
}
