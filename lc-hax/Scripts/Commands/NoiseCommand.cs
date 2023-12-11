using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class NoiseCommand : ICommand {
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
                   .Init((_) => RoundManager.Instance.PlayAudibleNoise(player.transform.position, 10000.0f, 10000.0f, 10, false), 60.0f);
    }
}
