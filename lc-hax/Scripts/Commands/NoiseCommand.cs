using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class NoiseCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /noise <player>");
            return;
        }

        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB player)) {
            Console.Print("SYSTEM", "Player not found!");
            return;
        }

        _ = new GameObject().AddComponent<TransientObject>()
                            .Init((_) => RoundManager.Instance.PlayAudibleNoise(player.transform.position, 10000.0f, 10000.0f, 10, false), 60.0f);
    }
}
