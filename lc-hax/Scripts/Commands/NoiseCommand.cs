using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class NoiseCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /noise <player>");
            return;
        }

        if (!Helpers.Extant(Helpers.Terminal, out Terminal terminal)) {
            Console.Print("SYSTEM", "Terminal not found!");
            return;
        }

        if (!Helpers.Extant(Reflector.Target(terminal).GetInternalField<RoundManager>("roundManager"), out RoundManager roundManager)) {
            Console.Print("SYSTEM", "RoundManager not found!");
            return;
        }

        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB player)) {
            Console.Print("SYSTEM", "Player not found!");
            return;
        }

        _ = new GameObject().AddComponent<TransientObject>()
                            .Init((_) => roundManager.PlayAudibleNoise(player.transform.position, 1000.0f, 1.0f, 10, false), 60.0f);
    }
}
