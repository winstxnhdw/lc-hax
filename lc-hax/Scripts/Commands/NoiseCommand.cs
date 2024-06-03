#region

using System;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

#endregion

[Command("noise")]
class NoiseCommand : ICommand {
    public void Execute(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: noise <player> <duration=30>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player) {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (!args[1].TryParse(30, out ulong duration)) {
            Chat.Print("Invalid duration!");
            return;
        }

        this.PlayNoiseContinuously(player.transform.position, duration);
    }

    Action<float> PlayNoise(Vector3 position) =>
        (_) =>
            Helper.RoundManager?.PlayAudibleNoise(position, float.MaxValue, float.MaxValue, 10, false);

    void PlayNoiseContinuously(Vector3 position, float duration) =>
        Helper.CreateComponent<TransientBehaviour>("Noise")
            .Init(this.PlayNoise(position), duration)
            .Unless(() => Helper.LocalPlayer?.playersManager is { inShipPhase: true });
}
