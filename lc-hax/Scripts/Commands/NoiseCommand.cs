using System;
using GameNetcodeStuff;
using UnityEngine;
using Hax;

[Command("/noise")]
public class NoiseCommand : ICommand {
    Action<float> PlayNoise(Vector3 position) => (_) =>
        Helper.RoundManager?.PlayAudibleNoise(position, float.MaxValue, float.MaxValue, 10, false);

    void PlayNoiseContinuously(Vector3 position, float duration) =>
        Helper.CreateComponent<TransientBehaviour>()
              .Init(this.PlayNoise(position), duration);

    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /noise <player> <duration=30>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Player not found!");
            return;
        }

        if (args.Length is 1) {
            this.PlayNoiseContinuously(player.transform.position, 30.0f);
            return;
        }

        if (!ulong.TryParse(args[1], out ulong duration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        this.PlayNoiseContinuously(player.transform.position, duration);
    }
}
