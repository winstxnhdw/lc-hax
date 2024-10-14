using System.Threading;
using System.Threading.Tasks;
using GameNetcodeStuff;
using UnityEngine;

[Command("noise")]
class NoiseCommand : ICommand {
    async Task PlayNoiseContinuously(PlayerControllerB player, float duration, CancellationToken cancellationToken) {
        float startTime = Time.time;

        while (Time.time - startTime < duration && !cancellationToken.IsCancellationRequested) {
            if (player.playersManager.inShipPhase) break;

            Helper.RoundManager?.PlayAudibleNoise(player.transform.position, float.MaxValue, float.MaxValue, 10, false);
            await Task.Yield();
        }
    }

    public async Task Execute(string[] args, CancellationToken cancellationToken) {
        if (args.Length is 0) {
            Chat.Print("Usage: noise <player> <duration=30>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (!args[1].TryParse(defaultValue: 30, result: out ulong duration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        await this.PlayNoiseContinuously(player, duration, cancellationToken);
    }
}
