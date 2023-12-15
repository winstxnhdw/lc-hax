using System;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class NoiseCommand : ICommand {
    Action<float> PlayNoise(Vector3 position) => (_) =>
        Helper.RoundManager?.PlayAudibleNoise(position, float.MaxValue, float.MaxValue, 10, false);

    public void Execute(string[] args) {
        if (args.Length is 0) {
            Console.Print("Usage: /noise <player>");
            return;
        }

        if (!Helper.GetPlayer(args[0]).IsNotNull(out PlayerControllerB player)) {
            Console.Print("Player not found!");
            return;
        }

        _ = Helper.CreateComponent<TransientBehaviour>()
                  .Init(this.PlayNoise(player.transform.position), 30.0f);
    }
}
