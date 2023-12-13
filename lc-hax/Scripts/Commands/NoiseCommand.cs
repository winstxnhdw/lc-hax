using System;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class NoiseCommand : ICommand {
    Action<float> PlayNoise(Vector3 position) => (_) =>
        RoundManager.Instance.PlayAudibleNoise(position, float.MaxValue, float.MaxValue, 10, false);

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Helper.PrintSystem("Usage: /noise <player>");
            return;
        }

        if (!Helper.Extant(Helper.GetPlayer(args[0]), out PlayerControllerB player)) {
            Helper.PrintSystem("Player not found!");
            return;
        }

        _ = Helper.CreateComponent<TransientBehaviour>()
                  .Init(this.PlayNoise(player.transform.position), 60.0f);
    }
}
