using System.Linq;
using GameNetcodeStuff;
using UnityEngine;

namespace Hax;

public class KillCommand : ICommand {
    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /kill <player>");
            return;
        }

        if (args[0] is "all") {
            Helpers.Players
                   .ToList()
                   .ForEach(player => player.DamagePlayerFromOtherClientServerRpc(1000, Vector3.zero, -1));

            Console.Print("SYSTEM", "Attempting to kill all players!");
        }

        else {
            if (Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
                Console.Print("SYSTEM", "Player not found!");
                return;
            }

            targetPlayer.DamagePlayerFromOtherClientServerRpc(1000, Vector3.zero, -1);
            Console.Print("SYSTEM", $"Attempting to kill {targetPlayer.playerUsername}!");
        }
    }
}
