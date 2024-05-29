using System;
using GameNetcodeStuff;
using Hax;
using UnityEngine;

[Command("noise")]
internal class NoiseCommand : ICommand
{
    public void Execute(StringArray args)
    {
        if (args.Length is 0)
        {
            Chat.Print("Usage: noise <player> <duration=30>");
            return;
        }

        if (Helper.GetActivePlayer(args[0]) is not PlayerControllerB player)
        {
            Chat.Print("Target player is not alive or found!");
            return;
        }

        if (!args[1].TryParse(30, out ulong duration))
        {
            Chat.Print("Invalid duration!");
            return;
        }

        PlayNoiseContinuously(player.transform.position, duration);
    }

    private Action<float> PlayNoise(Vector3 position)
    {
        return (_) =>
            Helper.RoundManager?.PlayAudibleNoise(position, float.MaxValue, float.MaxValue, 10, false);
    }

    private void PlayNoiseContinuously(Vector3 position, float duration)
    {
        Helper.CreateComponent<TransientBehaviour>("Noise")
            .Init(PlayNoise(position), duration)
            .Unless(() => Helper.LocalPlayer?.playersManager is { inShipPhase: true });
    }
}