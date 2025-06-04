using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("poison")]
class PoisonCommand : ICommand {
    static async Task PoisonPlayer(PlayerControllerB player, int damage, ulong delay, ulong duration, CancellationToken cancellationToken) {
        float startTime = Time.time;

        while (Time.time - startTime < duration) {
            if (player.playersManager.inShipPhase) break;

            player.DamagePlayerRpc(damage);
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
        }
    }

    public async Task Execute(Arguments args, CancellationToken cancellationToken) {
        if (args.Length < 3) {
            Chat.Print("Usages:",
                "poison <player> <damage> <duration> <delay=1>",
                "poison --all <damage> <duration> <delay=1>"
            );

            return;
        }

        if (!int.TryParse(args[1], out int damage)) {
            Chat.Print($"Invalid {nameof(damage)}!");
            return;
        }

        if (!ulong.TryParse(args[2], out ulong duration)) {
            Chat.Print($"Poison {nameof(duration)} must be a positive number!");
            return;
        }

        if (!args[3].TryParse(defaultValue: 1, result: out ulong delay)) {
            Chat.Print($"Poison {nameof(delay)} must be a positive number!");
            return;
        }

        if (args[0] is "--all") {
            await Task.WhenAll(
                Helper.ActivePlayers.Select(player => PoisonCommand.PoisonPlayer(player, damage, delay, duration, cancellationToken))
            );
        }

        else if (Helper.GetActivePlayer(args[0]) is PlayerControllerB player) {
            await PoisonCommand.PoisonPlayer(player, damage, delay, duration, cancellationToken);
        }

        else {
            Chat.Print("Target player is not alive or found!");
        }
    }
}
